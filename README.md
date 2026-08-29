# MarketplaceSystem

Online marketplace platform.

## Overview

Clean Architecture with the following layers:
- **API Layer**: REST API, ASP.NET Core 8
- **Application Layer**: Business logic, CQRS with MediatR
- **Domain Layer**: Entities, Enums, Interfaces
- **Infrastructure Layer**: Database, External Services
- **Common Layer**: Shared utilities, constants, helpers

## Tech Stack

### Backend
- .NET 8
- Entity Framework Core 9 (SQL Server)
- MediatR (CQRS Pattern)
- AutoMapper
- FluentValidation
- JWT Authentication
- SignalR (Real-time)
- Swagger/OpenAPI

## Project Structure

```
MarketplaceSystem/
├── MarketplaceSystem.API/           # Controllers, Hubs
├── MarketplaceSystem.Application/   # Features, Commands, Queries
├── MarketplaceSystem.Domain/        # Entities, Interfaces
├── MarketplaceSystem.Infrastructure/# DbContext, Services, Migrations
├── MarketplaceSystem.Common/        # Utilities, Constants
├── MarketplaceSystem.UnitTest/      # Unit Tests
└── MarketplaceSystem.sln
```

## Features

### Authentication
- Register/Login with JWT
- Refresh token
- Role-based authorization (User, Admin)

### Product Management
- CRUD products with multiple images
- Product classification (variants)
- Category attributes
- Search & filter

### Chat System
- Real-time messaging (SignalR)
- Conversation management

### Admin Dashboard
- User management
- Product approval
- Statistics

## External Services

- **TokenServices**: JWT generation
- **MailServices**: Email sending
- **StorageServices**: File storage

## SignalR Hubs

- **ChatHub**: Real-time chat
- **NotificationHub**: Real-time notifications

## Setup

### Requirements
- .NET 8 SDK
- SQL Server
- Docker (optional)

### Configuration

Before running, update `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "PrimaryDbConnection": "your_connection_string"
  },
  "AppSettings": {
    "JwtConfig": {
      "Secret": "your_jwt_secret_key_min_32_chars"
    }
  },
  "EmailSettings": {
    "From": "your_email@gmail.com",
    "Password": "your_app_password"
  }
}
```

### Docker
```bash
docker-compose up -d
```

### Local
```bash
dotnet restore
dotnet run --project MarketplaceSystem.API
```

### Migration
```bash
# Add migration
dotnet ef migrations add Migration_Name --project MarketplaceSystem.Infrastructure --startup-project MarketplaceSystem.API

# Update database
dotnet ef database update --project MarketplaceSystem.Infrastructure --startup-project MarketplaceSystem.API
```

## Configuration

`appsettings.json`:
- **ConnectionStrings**: Database connection
- **JwtConfig**: Secret, Issuer, Audience, Expiration
- **EmailSettings**: SMTP config
- **AllowedCors**: CORS origins

## Docker Services

- **marketplace-api**: Backend API (port 8080)
- **sqlserver**: SQL Server 2022 (port 1433)

## License

This project is licensed under the [MIT License](LICENSE).
