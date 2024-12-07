# LitConnect - Local Deployment Guide

## Overview
LitConnect is an ASP.NET Core web application that allows users to create and join book clubs, share reading lists, and participate in discussions. This guide will help you set up and run the application locally.

## Prerequisites
- [.NET SDK 8.0](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Visual Studio 2022](https://visualstudio.microsoft.com/vs/) or [JetBrains Rider](https://www.jetbrains.com/rider/) (recommended IDEs)
- [SQL Server](https://www.microsoft.com/sql-server/sql-server-downloads) (Express or Developer Edition)
- Version Control: Choose one of:
  - [Git](https://git-scm.com/downloads) for command line
  - [GitHub Desktop](https://desktop.github.com/) for GUI interface
  - [Visual Studio's Git Integration](https://visualstudio.microsoft.com/vs/github/) built into IDE

## Getting Started

### 1. Clone the Repository

Choose your preferred method:

**Using Git Command Line:**
```bash
git clone https://github.com/PavelStoyanov06/LitConnect.git
cd LitConnect
```

**Using GitHub Desktop:**
1. Open GitHub Desktop
2. Go to File > Clone Repository
3. Enter: https://github.com/PavelStoyanov06/LitConnect.git
4. Choose local path
5. Click "Clone"

**Using Visual Studio:**
1. Open Visual Studio
2. Go to View > Git Changes
3. Click "Clone Repository"
4. Enter: https://github.com/PavelStoyanov06/LitConnect.git
5. Choose local path
6. Click "Clone"

### 2. Update Database Connection String
Navigate to `LitConnect.Web/appsettings.json` and update the connection string:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=.;Database=LitConnect;User Id=sa;Password=YourPassword;TrustServerCertificate=True;"
  }
}
```

### 3. Database Setup

The application is configured to automatically create and initialize the database on startup through the `EnsureCreated()` method in `Program.cs`. You typically don't need to run migrations manually.

However, if you encounter database issues or need to manually update the database structure, you can use migrations:

**Using Package Manager Console in Visual Studio:**
1. Open Package Manager Console (Tools > NuGet Package Manager > Package Manager Console)
2. Set Default Project to "LitConnect.Data" (Important!)
3. Only if needed, run:
```powershell
Update-Database
```

**Using .NET CLI:**
```bash
cd LitConnect.Data    # Important! Migrations are in the Data project
dotnet ef database update
```

If you need to reset the database:

**Package Manager Console (in LitConnect.Data):**
```powershell
Drop-Database
Update-Database
```

**CLI (from LitConnect.Data directory):**
```bash
dotnet ef database drop
dotnet ef database update
```

Note: In most cases, you won't need to run these commands as the database is automatically created and seeded on application startup. Only use these commands if you're experiencing specific database issues or need to reset the database state.

### 4. Build and Run

**Using Visual Studio:**
1. Open `LitConnect.sln`
2. Set `LitConnect.Web` as the startup project
3. Press F5 or click the "Run" button

**Using Command Line:**
```bash
dotnet build
cd LitConnect.Web
dotnet run
```

**Using Visual Studio Code:**
1. Open the project folder
2. Press F5 to start debugging
3. Select .NET 8 environment if prompted

### 5. Access the Application
- HTTPS: https://localhost:7091
- HTTP: http://localhost:5036

## Default Administrator Account
- Email: admin@litconnect.com
- Password: Admin123!

## Solution Structure
- `LitConnect.Web`: Main web application project
- `LitConnect.Data`: Data access layer and Entity Framework context
- `LitConnect.Data.Models`: Entity models
- `LitConnect.Services`: Business logic and service implementations
- `LitConnect.Common`: Shared utilities and constants
- `LitConnect.Services.Tests`: Unit tests
- `LitConnect.Web.Tests`: Integration tests
- `LitConnect.Web.ViewModels`: View models and DTOs

## Running Tests

**Using Visual Studio:**
1. Open Test Explorer (Test > Test Explorer)
2. Click "Run All" or run specific tests

**Using Package Manager Console:**
```powershell
dotnet test
```

**Using CLI:**
```bash
dotnet test
```

## Common Issues and Solutions

### Connection String Issues
Make sure:
- SQL Server is running
- The password matches your SA account
- SQL Server authentication is enabled
- TCP/IP is enabled in SQL Server Configuration Manager

### HTTPS Development Certificate
If you encounter SSL/TLS certificate issues:
```bash
dotnet dev-certs https --clean
dotnet dev-certs https --trust
```

### Git Issues

**Using Command Line:**
```bash
git pull --rebase
git stash    # Save local changes
git stash pop # Apply saved changes
```

**Using GitHub Desktop:**
1. Fetch origin
2. Pull origin
3. Use "Stash changes" if needed

**Using Visual Studio:**
1. Use the Git Changes window
2. Right-click solutions for Git options
3. Use the Git menu for common operations

## Environment Configuration
The application uses the following environment configurations:
- Development
- Production

To switch environments:
```bash
set ASPNETCORE_ENVIRONMENT=Development  # Windows
export ASPNETCORE_ENVIRONMENT=Development  # Linux/macOS
```

## Support
For issues or questions, please create an issue in the repository's issue tracker at: https://github.com/PavelStoyanov06/LitConnect/issues
