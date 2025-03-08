using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Models;

public partial class KahootDbContext : DbContext
{
    public KahootDbContext()
    {
    }

    public KahootDbContext(DbContextOptions<KahootDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Game> Games { get; set; }

    public virtual DbSet<Player> Players { get; set; }

    public virtual DbSet<PlayerAnswer> PlayerAnswers { get; set; }

    public virtual DbSet<Question> Questions { get; set; }

    public virtual DbSet<QuestionOption> QuestionOptions { get; set; }

    public virtual DbSet<RoundResult> RoundResults { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=exampledb;Username=example;Password=example");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasPostgresExtension("pgcrypto");

        modelBuilder.Entity<Game>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("game_pkey");

            entity.ToTable("game", "kahoot");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("id");
            entity.Property(e => e.CreatedBy).HasColumnName("created_by");
            entity.Property(e => e.Name).HasColumnName("name");
        });

        modelBuilder.Entity<Player>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("player_pkey");

            entity.ToTable("player", "kahoot");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("id");
            entity.Property(e => e.GameId).HasColumnName("game_id");
            entity.Property(e => e.Nickname).HasColumnName("nickname");

            entity.HasOne(d => d.Game).WithMany(p => p.Players)
                .HasForeignKey(d => d.GameId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("player_game_id_fkey");
        });

        modelBuilder.Entity<PlayerAnswer>(entity =>
        {
            entity.HasKey(e => new { e.PlayerId, e.QuestionId }).HasName("player_answer_pkey");

            entity.ToTable("player_answer", "kahoot");

            entity.Property(e => e.PlayerId).HasColumnName("player_id");
            entity.Property(e => e.QuestionId).HasColumnName("question_id");
            entity.Property(e => e.AnswerTimestamp).HasColumnName("answer_timestamp");
            entity.Property(e => e.SelectedOptionId).HasColumnName("selected_option_id");

            entity.HasOne(d => d.Player).WithMany(p => p.PlayerAnswers)
                .HasForeignKey(d => d.PlayerId)
                .HasConstraintName("player_answer_player_id_fkey");

            entity.HasOne(d => d.Question).WithMany(p => p.PlayerAnswers)
                .HasForeignKey(d => d.QuestionId)
                .HasConstraintName("player_answer_question_id_fkey");

            entity.HasOne(d => d.SelectedOption).WithMany(p => p.PlayerAnswers)
                .HasForeignKey(d => d.SelectedOptionId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("player_answer_selected_option_id_fkey");
        });

        modelBuilder.Entity<Question>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("question_pkey");

            entity.ToTable("question", "kahoot");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("id");
            entity.Property(e => e.Answered)
                .HasDefaultValue(false)
                .HasColumnName("answered");
            entity.Property(e => e.GameId).HasColumnName("game_id");
            entity.Property(e => e.QuestionText).HasColumnName("question_text");

            entity.HasOne(d => d.Game).WithMany(p => p.Questions)
                .HasForeignKey(d => d.GameId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("question_game_id_fkey");
        });

        modelBuilder.Entity<QuestionOption>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("question_option_pkey");

            entity.ToTable("question_option", "kahoot");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("id");
            entity.Property(e => e.IsCorrect).HasColumnName("is_correct");
            entity.Property(e => e.OptionText).HasColumnName("option_text");
            entity.Property(e => e.QuestionId).HasColumnName("question_id");

            entity.HasOne(d => d.Question).WithMany(p => p.QuestionOptions)
                .HasForeignKey(d => d.QuestionId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("question_option_question_id_fkey");
        });

        modelBuilder.Entity<RoundResult>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("round_result_pkey");

            entity.ToTable("round_result", "kahoot");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("id");
            entity.Property(e => e.GameId).HasColumnName("game_id");
            entity.Property(e => e.PlayerId).HasColumnName("player_id");
            entity.Property(e => e.RoundNumber).HasColumnName("round_number");
            entity.Property(e => e.Score).HasColumnName("score");

            entity.HasOne(d => d.Game).WithMany(p => p.RoundResults)
                .HasForeignKey(d => d.GameId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("round_result_game_id_fkey");

            entity.HasOne(d => d.Player).WithMany(p => p.RoundResults)
                .HasForeignKey(d => d.PlayerId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("round_result_player_id_fkey");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
