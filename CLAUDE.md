# Amba Secret Manager

## Project Overview
.NET 10 CLI tool to manage local secrets (.env files and .secrets directories) across projects. Saves/loads secret files to a centralized user folder using named profiles.

## Terminology
- **Secret files** — .env files and all files inside .secrets directories
- **Profile** — a named set of secret files and folders

## Tech Stack
- C# / .NET 10
- Spectre.Console (CLI framework)
- xUnit (testing)

## Project Structure
```
Amba.SecretManager/
├── Amba.SecretManager/          # Main application
│   ├── Program.cs               # Entry point, CLI setup
│   ├── Commands/
│   │   ├── SaveCommand.cs       # Save secrets to user storage
│   │   └── LoadCommand.cs       # Load secrets back to project
│   └── SecretStorage/
│       └── StorageService.cs    # Core secret file operations
├── Amba.SecretManagerTest/      # xUnit tests
│   └── StorageServiceTest.cs
└── Amba.SecretManager.sln
```

## Commands
```bash
# Build
dotnet build Amba.SecretManager/Amba.SecretManager.sln

# Run tests
dotnet test Amba.SecretManager/Amba.SecretManager.sln

# Run the tool
dotnet run --project Amba.SecretManager/Amba.SecretManager
```

## Guidelines
- Use latest .NET version
- Follow existing code patterns with Spectre.Console.Cli for commands
- Always verify changes build: `dotnet build Amba.SecretManager/Amba.SecretManager.sln`
- Follow instructions in AGENTS.md
