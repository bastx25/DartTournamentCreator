# 🎯 Dart Tournament Creator

A modern, self-hostable tournament management system for darts.

Dart Tournament Creator is a full-stack web application designed to simplify the creation, organization, and management of dart tournaments. It provides tournament organizers with a centralized platform for managing players, groups, rounds, matches, boards, locations, and tournament configurations.

The project is built with a modern React/TypeScript frontend and an ASP.NET Core backend with SQL Server persistence.

## ✨ Features

### 🏆 Tournament Management

* Create and manage dart tournaments
* Configure tournament-specific settings
* Add and manage tournament players
* Organize players into groups
* Manage tournament rounds
* Automatically create matches based on tournament configuration
* Track tournament progress from a central dashboard
* Manage active and ongoing tournaments
* Support for multiple tournaments and tournament configurations

### 📱 QR Code Match & Board Assignment

Dart Tournament Creator supports **QR-code based match and board management**.

Tournament organizers can **print QR codes for dart boards** and place them directly at the physical boards. Players or organizers can then scan the QR code to quickly access and manage the matches assigned to that board.

This provides a simple bridge between the digital tournament system and the physical tournament venue.

With QR codes, you can:

* 🖨️ Print QR codes for individual dart boards
* 🎯 Assign matches to specific boards
* 📱 Scan a board's QR code to access its assigned matches
* ⚡ Quickly enter or manage matches directly from the board
* 🏟️ Avoid manually searching for boards or matches
* 🔄 Move between matches and boards with minimal administration

This is especially useful for tournaments with many dart boards, where players should be able to quickly identify **which match belongs to which board**.

**Print the QR code → Place it at the board → Scan it → Play the assigned match.**


### 🎯 Match Management

* Create and manage matches
* Assign players to matches
* Track match participants
* Organize matches into tournament rounds
* Automatic match generation through the built-in Match Maker service
* Manage match state and tournament progression
* Associate matches with dart boards
* Manage individual match participants independently

### 👥 Player Management

* Centralized player management
* Create and edit player profiles
* Reuse players across multiple tournaments
* Add players directly to tournaments
* Organize players into groups
* Track tournament-specific player assignments

### 👨‍👩‍👧 Group & Round Management

* Create tournament groups
* Assign players to groups
* Manage group participants
* Organize tournaments into multiple rounds
* Define the structure of a tournament through groups and rounds
* Support for automated match generation based on tournament structure

### 🎯 Dart Board Management

* Create and manage dart boards
* Assign boards to matches
* Manage board availability and assignments
* Dedicated board access functionality through the API

This makes the system suitable for venues where multiple boards are used simultaneously.

### 📍 Location Management

* Create and manage tournament locations
* Associate tournament infrastructure with locations
* Manage locations independently from tournaments

This allows the same location setup to be reused for different events.

### ⚙️ Tournament Configuration

Tournament configuration is handled independently from the tournament itself, allowing organizers to define reusable and structured tournament settings.

Configuration support includes:

* Tournament-specific configuration
* Match generation settings
* Group configuration
* Round configuration
* Player configuration
* Board configuration

### 🧠 Automatic Match Generation

The backend contains a dedicated `MatchMakerService` responsible for generating tournament matches.

Instead of manually creating every match, the tournament structure can be used to generate matches programmatically.

This provides a foundation for automated tournament workflows and reduces manual administration.

### 🌐 REST API

The backend exposes a RESTful API built with ASP.NET Core.

Dedicated API controllers are available for:

* Players
* Tournaments
* Matches
* Match participants
* Groups
* Rounds
* Boards
* Board access
* Locations

The API also provides Swagger/OpenAPI integration for development and API exploration.

### 📖 Swagger / OpenAPI

The backend includes Swagger/OpenAPI support, making it easy to inspect and test the available API endpoints during development.

API documentation can be generated directly from the ASP.NET Core application.

### 🔐 Authentication & Authorization Ready

The backend includes ASP.NET Core authentication infrastructure and JWT Bearer support.

The project is therefore structured to support authenticated API access and secure deployment scenarios.

### 💾 Persistent Database Storage

Tournament data is persisted using:

* Entity Framework Core
* Microsoft SQL Server
* Code-first database migrations

The backend contains a dedicated `DartDbContext` and repository layer for database access.

### 🏗️ Repository-Based Architecture

The backend follows a structured repository/service architecture.

Dedicated repositories are available for major domain objects such as:

* Players
* Tournaments
* Tournament players
* Matches
* Match participants
* Groups
* Rounds
* Boards
* Locations
* Tournament configuration

Business logic is separated into services such as the Match Maker and Board services.

This makes the application easier to maintain and extend.

### 🖥️ Modern Web Interface

The frontend is built using:

* React
* TypeScript
* Vite
* React Router
* Tailwind CSS
* Lucide React

The frontend is organized into reusable components, pages, hooks, DTOs, services, enums, and utilities.

### 📊 Tournament Dashboard

The frontend includes dedicated dashboard functionality for monitoring and working with tournaments.

The application contains separate views for:

* Home
* Dashboard
* Tournaments
* Players
* Locations
* Boards
* Settings

### 🧩 Modular Frontend Architecture

The frontend is structured into independent modules:

```text
src/
├── components/
├── dtos/
├── enums/
├── hooks/
├── pages/
├── services/
├── utils/
├── App.tsx
└── main.tsx
```

This makes individual features easier to develop and maintain.

### ⚡ React Hooks for Data Management

The application uses dedicated React hooks for common operations, including:

* Loading tournaments
* Loading active tournaments
* Loading players
* Creating tournaments
* Creating locations

This keeps API interaction separate from the presentation layer.

### 🎨 Modern UI

The application uses Tailwind CSS for styling and Lucide React for interface icons.

The result is a lightweight, modern web interface without requiring a large UI framework.

### 🌍 Self-Hostable

Dart Tournament Creator is designed as a self-hostable application.

You can run the complete stack on your own infrastructure instead of relying on a third-party tournament platform.

This is particularly useful for:

* Dart clubs
* Dart leagues
* Tournament organizers
* Sports venues
* Private events
* Local competitions
* Organizations requiring control over their own tournament data

Because the backend uses SQL Server and the frontend communicates with the API through HTTP, the application can be deployed on infrastructure under your control.

> **Note:** The current repository provides the application source code and does not currently include a turnkey Docker Compose deployment. Containerized deployment can be added independently.

### 🔒 Data Ownership

When self-hosted, tournament and player data can remain within your own infrastructure.

You control:

* The application server
* The database
* Network access
* Backups
* Authentication configuration
* Deployment environment

No external SaaS tournament provider is required by the application architecture.

### 🔌 API-First Architecture

The application is separated into two main parts:

```text
┌──────────────────────────┐
│      React Frontend      │
│     TypeScript + Vite    │
└────────────┬─────────────┘
             │ HTTP / REST
             ▼
┌──────────────────────────┐
│      ASP.NET Core API    │
│        .NET 10           │
└────────────┬─────────────┘
             │ Entity Framework Core
             ▼
┌──────────────────────────┐
│       SQL Server         │
└──────────────────────────┘
```

This separation makes it possible to develop, deploy, and scale the frontend and backend independently.

---

## 🛠️ Technology Stack

### Frontend

| Technology   | Purpose                              |
| ------------ | ------------------------------------ |
| React        | User interface                       |
| TypeScript   | Type-safe frontend development       |
| Vite         | Development server and build tooling |
| React Router | Client-side routing                  |
| Tailwind CSS | UI styling                           |
| Lucide React | Icons                                |
| Axios        | HTTP/API communication               |
| Zod          | Data validation                      |

The frontend package configuration currently targets React 19, TypeScript 6, Vite 8, Tailwind CSS 4 and React Router 7.

### Backend

| Technology            | Purpose                 |
| --------------------- | ----------------------- |
| .NET 10               | Backend runtime         |
| ASP.NET Core          | REST API                |
| Entity Framework Core | ORM/database access     |
| SQL Server            | Persistent data storage |
| JWT Bearer            | Authentication          |
| Swagger / OpenAPI     | API documentation       |
| Newtonsoft.Json       | JSON serialization      |

The backend currently targets `.NET 10` and uses Entity Framework Core with SQL Server.

---

## 🏗️ Project Structure

```text
DartTournamentCreator/
│
├── DTC.Api/
│   ├── Controllers/
│   ├── Data/
│   ├── Dtos/
│   ├── Enums/
│   ├── Interfaces/
│   ├── Mappers/
│   ├── Migrations/
│   ├── Models/
│   ├── Repositories/
│   ├── Services/
│   ├── SubModels/
│   ├── Program.cs
│   └── DTC.Api.csproj
│
├── dtc.frontend/
│   ├── src/
│   │   ├── components/
│   │   ├── dtos/
│   │   ├── enums/
│   │   ├── hooks/
│   │   ├── pages/
│   │   ├── services/
│   │   └── utils/
│   ├── package.json
│   └── vite.config.ts
│
└── DTC.slnx
```

The repository currently separates the ASP.NET Core API and React frontend into dedicated projects.

---

## 📦 Core Domain Model

The application is built around a clear tournament domain model.

```text
Tournament
    │
    ├── Tournament Configuration
    │
    ├── Players
    │
    ├── Groups
    │    └── Group Players
    │
    ├── Rounds
    │    └── Matches
    │         ├── Match Participants
    │         └── Dart Board
    │
    └── Location
```

The backend contains dedicated models and DTOs for tournaments, players, groups, rounds, matches, boards, locations and tournament configuration.

---

## 🚀 Getting Started

### Prerequisites

Make sure the following are installed:

* .NET 10 SDK
* Node.js
* npm
* Microsoft SQL Server
* Git

### Clone the Repository

```bash
git clone https://github.com/bastx25/DartTournamentCreator.git
cd DartTournamentCreator
```

### Backend

Navigate to the API project:

```bash
cd DTC.Api
```

Configure the SQL Server connection in:

```text
appsettings.json
```

The application expects a `DefaultConnection` connection string for the Entity Framework Core database context.

Run the API:

```bash
dotnet restore
dotnet run
```

During development, Swagger is enabled automatically by the ASP.NET Core application.

### Frontend

Open another terminal:

```bash
cd dtc.frontend
```

Install dependencies:

```bash
npm install
```

Start the development server:

```bash
npm run dev
```

For a production build:

```bash
npm run build
```

---

## 🔧 Configuration

The backend configuration is handled through ASP.NET Core configuration files.

Typical configuration includes:

* Database connection
* Application environment
* API configuration
* Authentication configuration

For production deployments, sensitive configuration values should be supplied through environment variables or an appropriate secret-management solution rather than committed to source control.

---

## 🏠 Self-Hosting

Dart Tournament Creator is intended to be deployable on infrastructure you control.

A typical production setup can look like:

```text
                         Internet / LAN
                               │
                               ▼
                    ┌────────────────────┐
                    │ Reverse Proxy      │
                    │ Nginx / IIS / etc. │
                    └─────────┬──────────┘
                              │
                ┌─────────────┴─────────────┐
                │                           │
                ▼                           ▼
        ┌───────────────┐          ┌────────────────┐
        │ React / Vite  │          │ ASP.NET Core   │
        │ Frontend      │ ───────► │ REST API       │
        └───────────────┘          └───────┬────────┘
                                           │
                                           ▼
                                  ┌─────────────────┐
                                  │   SQL Server    │
                                  └─────────────────┘
```

This makes it suitable for deployment on:

* A local server
* A VPS
* A dedicated server
* A club/venue network
* A private cloud
* An internal company infrastructure

---

## 🔮 Extensibility

The architecture is designed to make future features easier to add.

Potential extensions include:

* Live tournament displays
* Public tournament views
* QR-code based tournament access
* Live match boards
* Player statistics
* Tournament history
* Advanced seeding
* Knockout brackets
* Double elimination
* Online player registration
* Real-time updates via WebSockets
* Docker deployment
* Mobile/PWA support
* Tournament export/import
* Advanced reporting

The existing separation between frontend, API, repositories and services provides a solid foundation for these additions.

---

## 🤝 Contributing

Contributions are welcome.

If you would like to improve Dart Tournament Creator:

1. Fork the repository
2. Create a feature branch

```bash
git checkout -b feature/my-feature
```

3. Make your changes
4. Test the application
5. Commit your changes

```bash
git commit -m "Add my feature"
```

6. Push the branch

```bash
git push origin feature/my-feature
```

7. Open a Pull Request

---

## 📋 Current Status

Dart Tournament Creator is under active development.

The current repository already contains:

* A React/TypeScript frontend
* An ASP.NET Core API
* SQL Server persistence
* Entity Framework Core
* Tournament management
* Player management
* Group management
* Round management
* Match management
* Board management
* Location management
* Tournament configuration
* Automatic match generation
* Swagger/OpenAPI support
* Authentication infrastructure
* Repository/service architecture

The GitHub repository currently contains more than 200 commits and is structured as a complete frontend/backend solution.

---

## 📄 License

See the repository for the current license information.

---

## ⭐ Why Dart Tournament Creator?

Dart Tournament Creator aims to provide tournament organizers with a modern alternative to spreadsheets and closed tournament platforms.

**Self-host it. Own your data. Run your tournaments your way.**

Whether you're running a small club tournament or building a larger tournament infrastructure, Dart Tournament Creator provides the foundation for managing the complete tournament lifecycle in one application.
