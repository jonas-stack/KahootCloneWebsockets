# Kahoot Clone - WebSocket-Based Quiz Game

This project is a **Kahoot-like quiz application** developed using **React TypeScript** for the frontend and **.NET 8 (C#)** for the backend.  
It spans **weeks 9 and 10** of the Fullstack 2025 curriculum and is a practical implementation of real-time WebSocket-based communication.

The project is built using:
- **Frontend:** React TypeScript, Vite, Firebase Hosting (planned deployment)
- **Backend:** C# .NET 8, Entity Framework, PostgreSQL, WebSockets
- **Database:** PostgreSQL (local for development, Fly.io for production)
- **Deployment:** Docker, GitHub Actions, Fly.io

---

## **📌 Project Overview**
This project implements the **core features of a quiz application**, including:
✅ Players can join a **lobby**.  
✅ An admin can **start a game** and move players into a **game room**.  
✅ The admin can **broadcast questions** to all clients in the game.  
✅ Players **submit answers**, which are processed in real-time.  
✅ After a **time delay**, the server broadcasts the **round results**.  
✅ The game progresses through multiple rounds until it ends.  
✅ **Persistent data is stored in PostgreSQL**, while WebSocket state is managed in memory.   

---

## **📌 Getting Started**
### **1️⃣ Clone the repository**
```bash
git clone https://github.com/uldahlalex/fs25_kahoot.git
cd fs25_kahoot
