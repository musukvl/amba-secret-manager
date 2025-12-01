using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using Amba.SecretManager.SecretStorage;
using Xunit;
using Xunit.Abstractions;

namespace Amba.SecretManagerTest;

public class StorageServiceTest : IDisposable
{
    private readonly string _testRepositoryPath;
    private readonly string _testStoragePath;
    private readonly ITestOutputHelper _output;
    
    public StorageServiceTest(ITestOutputHelper output)
    {
        _output = output;
        
        // Create the base directory structure
        _testRepositoryPath = CreateSolutionSecrets();
        _testStoragePath = Path.Combine(Path.GetTempPath(), $"AmbaTestStorage_{Guid.NewGuid()}");
        
        // Output the path for debugging
        _output.WriteLine($"Test repository path: {_testRepositoryPath}");
        _output.WriteLine($"Test storage path: {_testStoragePath}");
    }

    private string CreateSolutionSecrets()
    {
        var testRepositoryPath = Path.Combine(Path.GetTempPath(), $"AmbaTestRepo_{Guid.NewGuid()}");
        // Output the path to console as well for CI environments
        Console.WriteLine($"Created test repository at: {testRepositoryPath}");
        // Create the repository root
        Directory.CreateDirectory(testRepositoryPath);
        // Create a simulated repository structure with .env files and .secrets folders

        // Root level .env file
        CreateFile(Path.Combine(testRepositoryPath, ".env"), "APP_ENV=development\nDEBUG=true");
        CreateFile(Path.Combine(testRepositoryPath, "test.sln"), "cscporjfile");
        // Backend directory with .env
        var backendDir = CreateDirectory(Path.Combine(testRepositoryPath, "backend"));
        CreateFile(Path.Combine(backendDir, ".env"), "DB_CONNECTION=mysql\nDB_HOST=localhost\nDB_PORT=3306");
        CreateFile(Path.Combine(backendDir, "Program.cs"), "app");
        CreateFile(Path.Combine(backendDir, "Backend.csproj"), "csproj");
        
        var notificationServiceDir = CreateDirectory(Path.Combine(backendDir, "Services"));
        var notificationSecrets = CreateDirectory(Path.Combine(notificationServiceDir, ".secrets"));
        CreateFile(Path.Combine(notificationSecrets, "smtp.json"), "{}");
        

        // Backend .secrets directory
        var backendSecrets = CreateDirectory(Path.Combine(backendDir, ".secrets"));
        CreateFile(Path.Combine(backendSecrets, "api_keys.json"), "{\"google_api_key\": \"GOOG12345\", \"aws_key\": \"AWSKEY67890\"}");
        CreateFile(Path.Combine(backendSecrets, "db_creds.json"), "{\"username\": \"dbuser\", \"password\": \"dbpass123\"}");

        // Frontend directory with .env
        var frontendDir = CreateDirectory(Path.Combine(testRepositoryPath, "frontend"));
        CreateFile(Path.Combine(frontendDir, ".env"), "API_URL=http://localhost:5000\nENABLE_ANALYTICS=false");
        CreateFile(Path.Combine(frontendDir, "packages.json"), "{}");

 
       
        return testRepositoryPath;
    }

    [Fact]
    public void SaveSecrets_ShouldCopyEnvFilesAndSecretsDirectories()
    {
        // Arrange
        var service = new SecretStorageService(_testStoragePath);
        var profileName = "test-profile";

        // Act
        service.SaveSecrets(profileName, _testRepositoryPath);

        // Assert
        var profilePath = Path.Combine(_testStoragePath, profileName);
        Assert.True(Directory.Exists(profilePath));
        
        // Check root .env file
        Assert.True(File.Exists(Path.Combine(profilePath, ".env")));
        
        // Check backend .env file
        Assert.True(File.Exists(Path.Combine(profilePath, "backend", ".env")));
        
        // Check backend .secrets directory
        Assert.True(File.Exists(Path.Combine(profilePath, "backend", ".secrets", "api_keys.json")));
        Assert.True(File.Exists(Path.Combine(profilePath, "backend", ".secrets", "db_creds.json")));
        
        // Check notification service .secrets
        Assert.True(File.Exists(Path.Combine(profilePath, "backend", "Services", ".secrets", "smtp.json")));
        
        // Check frontend .env file
        Assert.True(File.Exists(Path.Combine(profilePath, "frontend", ".env")));
        
        // Verify non-secret files are not copied
        Assert.False(File.Exists(Path.Combine(profilePath, "test.sln")));
        Assert.False(File.Exists(Path.Combine(profilePath, "backend", "Program.cs")));
        Assert.False(File.Exists(Path.Combine(profilePath, "backend", "Backend.csproj")));
        
        _output.WriteLine($"SaveSecrets test passed. Profile path: {profilePath}");
    }

    [Fact]
    public void SaveSecrets_ShouldClearExistingProfile()
    {
        // Arrange
        var service = new SecretStorageService(_testStoragePath);
        var profileName = "test-profile-clear";
        
        // Create profile with some content
        var profilePath = Path.Combine(_testStoragePath, profileName);
        Directory.CreateDirectory(profilePath);
        CreateFile(Path.Combine(profilePath, "old-file.txt"), "old content");

        // Act
        service.SaveSecrets(profileName, _testRepositoryPath);

        // Assert
        Assert.True(Directory.Exists(profilePath));
        Assert.False(File.Exists(Path.Combine(profilePath, "old-file.txt")), "Old file should be deleted");
        Assert.True(File.Exists(Path.Combine(profilePath, ".env")), "New files should be present");
        
        _output.WriteLine("SaveSecrets clear test passed");
    }

    [Fact]
    public void LoadSecrets_ShouldCopyAllFilesFromProfile()
    {
        // Arrange
        var service = new SecretStorageService(_testStoragePath);
        var profileName = "test-profile-load";
        var destinationPath = Path.Combine(Path.GetTempPath(), $"AmbaTestDestination_{Guid.NewGuid()}");
        Directory.CreateDirectory(destinationPath);
        
        // First save secrets
        service.SaveSecrets(profileName, _testRepositoryPath);

        // Act
        service.LoadSecrets(profileName, destinationPath, false);

        // Assert
        Assert.True(File.Exists(Path.Combine(destinationPath, ".env")));
        Assert.True(File.Exists(Path.Combine(destinationPath, "backend", ".env")));
        Assert.True(File.Exists(Path.Combine(destinationPath, "backend", ".secrets", "api_keys.json")));
        Assert.True(File.Exists(Path.Combine(destinationPath, "frontend", ".env")));
        
        _output.WriteLine($"LoadSecrets test passed. Destination: {destinationPath}");
        
        // Cleanup
        if (Directory.Exists(destinationPath))
        {
            Directory.Delete(destinationPath, true);
        }
    }

    [Fact]
    public void LoadSecrets_WithOverwrite_ShouldRemoveExistingSecretsDirectories()
    {
        // Arrange
        var service = new SecretStorageService(_testStoragePath);
        var profileName = "test-profile-overwrite";
        var destinationPath = Path.Combine(Path.GetTempPath(), $"AmbaTestDestination_{Guid.NewGuid()}");
        Directory.CreateDirectory(destinationPath);
        
        // Create existing .secrets directory with old content
        var existingSecretsDir = Path.Combine(destinationPath, "backend", ".secrets");
        Directory.CreateDirectory(existingSecretsDir);
        CreateFile(Path.Combine(existingSecretsDir, "old-secret.json"), "old content");
        
        // Save secrets to profile
        service.SaveSecrets(profileName, _testRepositoryPath);

        // Act
        service.LoadSecrets(profileName, destinationPath, true);

        // Assert
        Assert.True(Directory.Exists(Path.Combine(destinationPath, "backend", ".secrets")));
        Assert.False(File.Exists(Path.Combine(destinationPath, "backend", ".secrets", "old-secret.json")), 
            "Old secret should be removed with overwrite");
        Assert.True(File.Exists(Path.Combine(destinationPath, "backend", ".secrets", "api_keys.json")), 
            "New secrets should be present");
        
        _output.WriteLine("LoadSecrets with overwrite test passed");
        
        // Cleanup
        if (Directory.Exists(destinationPath))
        {
            Directory.Delete(destinationPath, true);
        }
    }

    [Fact]
    public void LoadSecrets_WithoutOverwrite_ShouldPreserveOtherFiles()
    {
        // Arrange
        var service = new SecretStorageService(_testStoragePath);
        var profileName = "test-profile-no-overwrite";
        var destinationPath = Path.Combine(Path.GetTempPath(), $"AmbaTestDestination_{Guid.NewGuid()}");
        Directory.CreateDirectory(destinationPath);
        
        // Create existing file that should not be affected
        CreateFile(Path.Combine(destinationPath, "existing-file.txt"), "existing content");
        
        // Save and load secrets
        service.SaveSecrets(profileName, _testRepositoryPath);
        service.LoadSecrets(profileName, destinationPath, false);

        // Assert
        Assert.True(File.Exists(Path.Combine(destinationPath, "existing-file.txt")), 
            "Existing files should not be affected");
        Assert.True(File.Exists(Path.Combine(destinationPath, ".env")), 
            "New secrets should be loaded");
        
        _output.WriteLine("LoadSecrets without overwrite test passed");
        
        // Cleanup
        if (Directory.Exists(destinationPath))
        {
            Directory.Delete(destinationPath, true);
        }
    }
    
    private string CreateDirectory(string path)
    {
        Directory.CreateDirectory(path);
        return path;
    }
    
    private void CreateFile(string path, string content)
    {
        var dir = Path.GetDirectoryName(path);
        if (dir != null && !Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }
        File.WriteAllText(path, content, Encoding.UTF8);
    }
    
    public void Dispose()
    {
        // Clean up the test environment after each test
        if (Directory.Exists(_testRepositoryPath))
        {
            Directory.Delete(_testRepositoryPath, true);
        }
        if (Directory.Exists(_testStoragePath))
        {
            Directory.Delete(_testStoragePath, true);
        }
    }
}