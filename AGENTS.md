# Agents Guidelines

## Project Overview
The project is a dotnet CLI tool to manage secrets.
The main idea is to save secret files like .env into a user catalog.

# Terminology
- **Secret files** — all `*.env` files (e.g. `.env`, `production.env`, `local.env`) and all files/subdirectories inside `.secrets/` directories (recursively)
- **Profile** — a named set of secret files stored under `~/.secret-profiles/<profile-name>/`
- **Profile name** — defaults to the current directory name; can be overridden by passing a name argument

# Main Functions

## Save profile — `sm save [profile-name]`
- Traverse all subfolders of the current directory, searching for `*.env` files and `.secrets/` directories
- `.secrets/` directories are copied recursively (all nested subdirectories and files)
- Clean replace: if the profile already exists in `~/.secret-profiles/<profile>/`, delete it entirely before writing
- Copy secrets into `~/.secret-profiles/<profile>/`, preserving the relative folder structure
- No confirmation prompt — just save
- Profile name defaults to the current folder name if not provided
- Example: `sm save` in `~/dev/myapp` → saves to `~/.secret-profiles/myapp/`
- Example: `sm save prod` in `~/dev/myapp` → saves to `~/.secret-profiles/prod/`

## Load profile — `sm load [profile-name]`
- Copy all files from `~/.secret-profiles/<profile>/` back into the current directory, preserving folder structure
- All existing secret files in the destination are overwritten
- Profile name defaults to the current folder name if not provided
- If the profile does not exist, exit with error: "Profile not found"
- Example: `sm load` in `~/dev/myapp` → loads from `~/.secret-profiles/myapp/`
- Example: `sm load prod` in `~/dev/myapp` → loads from `~/.secret-profiles/prod/`

## List profiles — `sm list`
- List all profile names stored in `~/.secret-profiles/` (each subdirectory is a profile)
- If no profiles exist, show a message indicating none are saved


## General Rules
- Read and understand existing code before making changes
- Keep changes minimal and focused on the task at hand
- Do not add unnecessary abstractions, helpers, or utilities
- Do not add comments, docstrings, or type annotations to code you didn't change

## Code Style
- Follow existing conventions in the codebase
- Use file-scoped namespaces
- Use primary constructors where appropriate
- Prefer pattern matching and modern C# features

## Testing
- Every new feature or bug fix must include corresponding tests
- Run `dotnet test Amba.SecretManager/Amba.SecretManager.sln` before considering work complete
- Tests use xUnit with temporary directory fixtures (see StorageServiceTest.cs for patterns)
- Test classes implement IDisposable for cleanup of temp directories

## Build Verification
- Always run `dotnet build Amba.SecretManager/Amba.SecretManager.sln` after making changes
- Fix all build errors and warnings before submitting

## Git
- Write concise commit messages focused on "why" not "what"
- One logical change per commit
