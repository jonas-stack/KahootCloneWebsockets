﻿using System.Collections.Concurrent;
using System.Text.Json;
using Api.EventHandlers.EventMessageDtos;
using Fleck;
using WebSocketBoilerplate;

namespace Api.WebSockets;

public class DictionaryConnectionManager(ILogger<DictionaryConnectionManager> logger) : IConnectionManager
{
    public ConcurrentDictionary<string, HashSet<string>> TopicMembers { get; set; } = new();
    public ConcurrentDictionary<string, HashSet<string>> MemberTopics { get; set; } = new();
    public ConcurrentDictionary<string /* Client ID */, IWebSocketConnection> ConnectionIdToSocket { get; } = new();
    public ConcurrentDictionary<string /* Socket ID */, string /* Client ID */> SocketToConnectionId { get; } = new();
    
    private readonly ConcurrentDictionary<string /* Client ID */, string /* Nickname */> _connectionNicknames = new();

    public Task<ConcurrentDictionary<string, HashSet<string>>> GetAllTopicsWithMembers()
    {
        return Task.FromResult(TopicMembers);
    }

    public Task<ConcurrentDictionary<string, HashSet<string>>> GetAllMembersWithTopics()
    {
        return Task.FromResult(MemberTopics);
    }

    public Task<Dictionary<string, string>> GetAllConnectionIdsWithSocketId()
    {
        return Task.FromResult(ConnectionIdToSocket.ToDictionary(k => k.Key, v => v.Value.ConnectionInfo.Id.ToString())
        );
    }

    public Task<Dictionary<string, string>> GetAllSocketIdsWithConnectionId()
    {
        return Task.FromResult(SocketToConnectionId.ToDictionary(k => k.Key, v => v.Value));
    }

    public async Task AddToTopic(string topic, string memberId, TimeSpan? expiry = null)
    {
        TopicMembers.AddOrUpdate(
            topic,
            _ => new HashSet<string> { memberId },
            (_, existing) =>
            {
                lock (existing)
                {
                    existing.Add(memberId);
                    return existing;
                }
            });

        MemberTopics.AddOrUpdate(
            memberId,
            _ => new HashSet<string> { topic },
            (_, existing) =>
            {
                lock (existing)
                {
                    existing.Add(topic);
                    return existing;
                }
            });

        Console.WriteLine($"Client '{memberId}' joined topic '{topic}'.");

        // If you have logging configured, you can also log here.
        logger.LogDebug("Client '{MemberId}' joined topic '{Topic}'.", memberId, topic);

        await LogCurrentState();
    }


    public Task RemoveFromTopic(string topic, string memberId)
    {
        if (TopicMembers.TryGetValue(topic, out var members))
            lock (members)
            {
                members.Remove(memberId);
            }

        if (MemberTopics.TryGetValue(memberId, out var topics))
            lock (topics)
            {
                topics.Remove(topic);
            }

        return Task.CompletedTask;
    }

    public Task<List<string>> GetMembersFromTopicId(string topic)
    {
        return Task.FromResult(
            TopicMembers.TryGetValue(topic, out var members)
                ? members.ToList()
                : new List<string>());
    }

    public Task<List<string>> GetTopicsFromMemberId(string memberId)
    {
        return Task.FromResult(
            MemberTopics.TryGetValue(memberId, out var topics)
                ? topics.ToList()
                : new List<string>());
    }

    public Task<string> GetClientIdFromSocketId(string socketId)
    {
        var success = SocketToConnectionId.TryGetValue(socketId, out var connectionId);
        if (success)
            return Task.FromResult(connectionId!);
        throw new Exception("Could not find client ID for socket ID " + socketId);
    }


    public async Task OnOpen(IWebSocketConnection socket, string clientId)
    {
        logger.LogDebug($"OnOpen called with clientId: {clientId} and socketId: {socket.ConnectionInfo.Id}");

        if (ConnectionIdToSocket.TryRemove(clientId, out var oldSocket))
        {
            var oldSocketId = oldSocket.ConnectionInfo.Id.ToString();
            SocketToConnectionId.TryRemove(oldSocketId, out _);
            logger.LogInformation($"Removed old connection {oldSocketId} for client {clientId}");
        }

        ConnectionIdToSocket[clientId] = socket;
        SocketToConnectionId[socket.ConnectionInfo.Id.ToString()] = clientId;

        logger.LogInformation($"Added new connection {socket.ConnectionInfo.Id} for client {clientId}");
        await LogCurrentState();
    }

    public async Task OnClose(IWebSocketConnection socket, string clientId)
    {
        var socketId = socket.ConnectionInfo.Id.ToString();

        if (ConnectionIdToSocket.TryGetValue(clientId, out var currentSocket) &&
            currentSocket.ConnectionInfo.Id.ToString() == socketId)
        {
            ConnectionIdToSocket.TryRemove(clientId, out _);
            logger.LogInformation($"Removed connection for client {clientId}");
        }

        SocketToConnectionId.TryRemove(socketId, out _);

        if (MemberTopics.TryGetValue(clientId, out var topics))
            foreach (var topic in topics)
            {
                await RemoveFromTopic(topic, clientId);
                await BroadcastToTopic(topic, new MemberHasLeftDto { MemberId = clientId });
            }

        MemberTopics.TryRemove(clientId, out _);
    }

    public async Task BroadcastToTopic<T>(string topic, T message) where T : CustomBaseDto
    {
        if (!TopicMembers.TryGetValue(topic, out var members) || members.Count == 0)
        {
            logger.LogWarning("⚠️ No active clients in Topic {Topic}. Message not sent.", topic);
            return;
        }

        logger.LogInformation("\n=============================\n" +
                              "📢 Broadcast Message\n" +
                              "👤 Sender: {SenderId}\n" +
                              "📥 Received by: {Clients}\n" +
                              "📍 Topic: {Topic}\n" +
                              "=============================", 
            message.SenderId, string.Join(", ", members), topic);

        foreach (var recipientId in members)
        {
            message.RecipientId = recipientId;
            await SendToClient(recipientId, message);
        }
    }
    
    private async Task SendToClient<T>(string recipientId, T message) where T : CustomBaseDto
    {
        if (!ConnectionIdToSocket.TryGetValue(recipientId, out var socket))
        {
            logger.LogWarning("❌ No socket found for recipient: {RecipientId}", recipientId);
            return;
        }

        if (!socket.IsAvailable)
        {
            logger.LogWarning("❌ Socket unavailable for {RecipientId}. Removing from topic.", recipientId);
            await RemoveFromTopic(message.Topic, recipientId);
            return;
        }

        logger.LogInformation("📩 Sending message from {SenderId} to {RecipientId}", message.SenderId, recipientId);
        socket.SendDto(message);
    }

    
    private async Task BroadcastToMember<T>(string topic, string memberId, T message) where T : BaseDto
    {
        if (!ConnectionIdToSocket.TryGetValue(memberId, out var socket))
        {
            logger.LogWarning($"No socket found for member: {memberId}");
            await RemoveFromTopic(topic, memberId);
            return;
        }

        if (!socket.IsAvailable)
        {
            logger.LogWarning($"Socket not available for {memberId}");
            await RemoveFromTopic(topic, memberId);
            return;
        }


        socket.SendDto(message);
    }

    public async Task LogCurrentState()
    {
        logger.LogDebug(JsonSerializer.Serialize(new
        {
            ConnectionIdToSocket = await GetAllConnectionIdsWithSocketId(),
            SocketToCnnectionId = await GetAllSocketIdsWithConnectionId(),
            TopicsWithMembers = await GetAllTopicsWithMembers(),
            MembersWithTopics = await GetAllMembersWithTopics()
        }, new JsonSerializerOptions
        {
            WriteIndented = true
        }));
    }
}
