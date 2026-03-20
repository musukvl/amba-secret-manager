using System.Text;
using Amba.SecretManager.SecretStorage;
using Xunit;

namespace Amba.SecretManagerTest;

public class StorageServiceTest : IDisposable
{
    private readonly string _testRepoPath;
    private readonly string _storagePath;
    private readonly SecretStorageService _service;

    public StorageServiceTest()
    {
        var id = Guid.NewGuid().ToString("N")[..8];
        _testRepoPath = Path.Combine(Path.GetTempPath(), $"AmbaTestRepo_{id}");
        _storagePath = Path.Combine(Path.GetTempPath(), $"AmbaTestStorage_{id}");

        Directory.CreateDirectory(_testRepoPath);
        Directory.CreateDirectory(_storagePath);

        _service = new SecretStorageService(_storagePath);

        CreateTestRepository();
    }

    private void CreateTestRepository()
    {
        // Root .env
        WriteFile(Path.Combine(_testRepoPath, ".env"), "APP_ENV=development\nDEBUG=true");

        // Non-secret files (should not be copied)
        WriteFile(Path.Combine(_testRepoPath, "test.sln"), "solution");

        // Backend with .env and .secrets
        var backend = CreateDir(Path.Combine(_testRepoPath, "backend"));
        WriteFile(Path.Combine(backend, ".env"), "DB_HOST=localhost");
        WriteFile(Path.Combine(backend, "Program.cs"), "app");

        var backendSecrets = CreateDir(Path.Combine(backend, ".secrets"));
        WriteFile(Path.Combine(backendSecrets, "api_keys.json"), "{\"key\":\"val\"}");
        WriteFile(Path.Combine(backendSecrets, "db_creds.json"), "{\"user\":\"admin\"}");

        // Nested .secrets subdirectory
        var nestedSecrets = CreateDir(Path.Combine(backendSecrets, "nested"));
        WriteFile(Path.Combine(nestedSecrets, "deep.json"), "{\"deep\":true}");

        // Backend/Services/.secrets
        var services = CreateDir(Path.Combine(backend, "Services"));
        var serviceSecrets = CreateDir(Path.Combine(services, ".secrets"));
        WriteFile(Path.Combine(serviceSecrets, "smtp.json"), "{}");

        // Frontend with .env
        var frontend = CreateDir(Path.Combine(_testRepoPath, "frontend"));
        WriteFile(Path.Combine(frontend, ".env"), "API_URL=http://localhost:5000");
        WriteFile(Path.Combine(frontend, "packages.json"), "{}");
    }

    [Fact]
    public void SaveSecrets_CopiesEnvFiles()
    {
        _service.SaveSecrets("myapp", _testRepoPath);

        var profilePath = Path.Combine(_storagePath, "myapp");
        Assert.True(File.Exists(Path.Combine(profilePath, ".env")));
        Assert.True(File.Exists(Path.Combine(profilePath, "backend", ".env")));
        Assert.True(File.Exists(Path.Combine(profilePath, "frontend", ".env")));
    }

    [Fact]
    public void SaveSecrets_CopiesSecretDirectoriesRecursively()
    {
        _service.SaveSecrets("myapp", _testRepoPath);

        var profilePath = Path.Combine(_storagePath, "myapp");
        Assert.True(File.Exists(Path.Combine(profilePath, "backend", ".secrets", "api_keys.json")));
        Assert.True(File.Exists(Path.Combine(profilePath, "backend", ".secrets", "db_creds.json")));
        Assert.True(File.Exists(Path.Combine(profilePath, "backend", ".secrets", "nested", "deep.json")));
        Assert.True(File.Exists(Path.Combine(profilePath, "backend", "Services", ".secrets", "smtp.json")));
    }

    [Fact]
    public void SaveSecrets_DoesNotCopyNonSecretFiles()
    {
        _service.SaveSecrets("myapp", _testRepoPath);

        var profilePath = Path.Combine(_storagePath, "myapp");
        Assert.False(File.Exists(Path.Combine(profilePath, "test.sln")));
        Assert.False(File.Exists(Path.Combine(profilePath, "backend", "Program.cs")));
        Assert.False(File.Exists(Path.Combine(profilePath, "frontend", "packages.json")));
    }

    [Fact]
    public void SaveSecrets_CleanReplacesExistingProfile()
    {
        _service.SaveSecrets("myapp", _testRepoPath);

        // Add a stale file into the profile
        var staleFile = Path.Combine(_storagePath, "myapp", "stale.env");
        File.WriteAllText(staleFile, "old");

        // Save again — should delete the stale file
        _service.SaveSecrets("myapp", _testRepoPath);

        Assert.False(File.Exists(staleFile));
    }

    [Fact]
    public void LoadSecrets_RestoresFiles()
    {
        _service.SaveSecrets("myapp", _testRepoPath);

        var dest = Path.Combine(Path.GetTempPath(), $"AmbaLoadTest_{Guid.NewGuid():N}");
        Directory.CreateDirectory(dest);

        try
        {
            _service.LoadSecrets("myapp", dest);

            Assert.Equal("APP_ENV=development\nDEBUG=true", File.ReadAllText(Path.Combine(dest, ".env")));
            Assert.Equal("DB_HOST=localhost", File.ReadAllText(Path.Combine(dest, "backend", ".env")));
            Assert.True(File.Exists(Path.Combine(dest, "backend", ".secrets", "api_keys.json")));
            Assert.True(File.Exists(Path.Combine(dest, "backend", ".secrets", "nested", "deep.json")));
        }
        finally
        {
            Directory.Delete(dest, true);
        }
    }

    [Fact]
    public void LoadSecrets_ThrowsWhenProfileNotFound()
    {
        var ex = Assert.Throws<DirectoryNotFoundException>(() =>
            _service.LoadSecrets("nonexistent", _testRepoPath));

        Assert.Equal("Profile not found", ex.Message);
    }

    [Fact]
    public void ProfileExists_ReturnsTrueForExistingProfile()
    {
        _service.SaveSecrets("myapp", _testRepoPath);
        Assert.True(_service.ProfileExists("myapp"));
    }

    [Fact]
    public void ProfileExists_ReturnsFalseForMissingProfile()
    {
        Assert.False(_service.ProfileExists("nope"));
    }

    [Fact]
    public void ListProfiles_ReturnsAllProfilesSorted()
    {
        _service.SaveSecrets("beta", _testRepoPath);
        _service.SaveSecrets("alpha", _testRepoPath);

        var profiles = _service.ListProfiles().ToList();

        Assert.Equal(["alpha", "beta"], profiles);
    }

    [Fact]
    public void ListProfiles_ReturnsEmptyWhenNoProfiles()
    {
        var emptyStorage = Path.Combine(Path.GetTempPath(), $"AmbaEmpty_{Guid.NewGuid():N}");
        var svc = new SecretStorageService(emptyStorage);

        Assert.Empty(svc.ListProfiles());
    }

    private static string CreateDir(string path)
    {
        Directory.CreateDirectory(path);
        return path;
    }

    private static void WriteFile(string path, string content) =>
        File.WriteAllText(path, content, Encoding.UTF8);

    public void Dispose()
    {
        if (Directory.Exists(_testRepoPath))
            Directory.Delete(_testRepoPath, true);
        if (Directory.Exists(_storagePath))
            Directory.Delete(_storagePath, true);
    }
}
