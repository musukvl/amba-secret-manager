# Amba Secret Manager

A .NET CLI tool to manage local secrets across projects. It saves `.env` files and `.secrets/` directories from your project to a centralized user folder (`~/.secret-profiles/`) using named profiles, so you can quickly switch between secret sets or restore them after a clean clone.

## Installation

### From NuGet

```bash
dotnet tool install -g Amba.SecretManager
```

### From source

```bash
git clone https://github.com/amba/secret-manager.git
cd secret-manager
./scripts/install-local.sh 1.0.0
```

## Usage

### Save secrets

Saves all `.env` files and `.secrets/` directories from the current project into a profile.

```bash
# Save using the current directory name as profile name
sm save

# Save with an explicit profile name
sm save production
```

**Example:** running `sm save` inside `~/dev/myapp` saves secrets to `~/.secret-profiles/myapp/`.

### Load secrets

Restores secret files from a profile back into the current directory.

```bash
# Load using the current directory name as profile name
sm load

# Load a specific profile
sm load production
```

**Example:** running `sm load staging` inside `~/dev/myapp` copies files from `~/.secret-profiles/staging/` into `~/dev/myapp/`, preserving folder structure.

### List profiles

Shows all saved profiles.

```bash
sm list
```

## What counts as a secret file?

- All `*.env` files (`.env`, `production.env`, `local.env`, etc.) anywhere in the project tree
- All files and subdirectories inside `.secrets/` directories, recursively

Non-secret files (source code, configs, etc.) are never touched.

## How it works

```
~/dev/myapp/                          ~/.secret-profiles/myapp/
├── .env                  sm save     ├── .env
├── backend/              -------->   ├── backend/
│   ├── .env                          │   ├── .env
│   ├── .secrets/                     │   └── .secrets/
│   │   ├── api_keys.json             │       ├── api_keys.json
│   │   └── creds.json                │       └── creds.json
│   └── Program.cs                    └── frontend/
├── frontend/             sm load         └── .env
│   ├── .env              <--------
│   └── index.html
└── README.md
```

- **Save** does a clean replace — the entire profile directory is deleted and recreated on each save.
- **Load** overwrites existing secret files in the destination; other files are left untouched.
- Profile name defaults to the current directory name if not provided.

## Building from source

```bash
# Build and run tests
./scripts/build.sh

# Create NuGet package
./scripts/pack.sh 1.0.0

# Publish to NuGet.org
NUGET_API_KEY=<your-key> ./scripts/publish-nuget.sh 1.0.0
```

## Requirements

- [.NET 10 SDK](https://dotnet.microsoft.com/download) or later

## License

[BSD 2-Clause](LICENSE)
