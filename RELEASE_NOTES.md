# Release Notes

## v1.0.1 — 2026-03-21

Initial public release of Amba Secret Manager as a .NET global tool.

### Features
- **`sm save [profile-name]`** — save `.env` files and `.secrets/` directories from the current project into a named profile at `~/.secret-profiles/`
- **`sm load [profile-name]`** — restore secret files from a profile back into the current directory
- **`sm list`** — list all saved profiles
- Profile name defaults to the current directory name when not specified
- Save performs a clean replace — the entire profile is deleted and recreated each time
- `.secrets/` directories are copied recursively, preserving nested subdirectories

### Installation
```bash
dotnet tool install -g Amba.SecretManager
```
