using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using Xunit;
using Xunit.Abstractions;

namespace Amba.SecretManagerTest;

public class StorageServiceTest : IDisposable
{
    private readonly string _testRepositoryPath;
    private readonly ITestOutputHelper _output;
    
    public StorageServiceTest(ITestOutputHelper output)
    {
        _output = output;
        
        // Create the base directory structure
        _testRepositoryPath = CreateSolutionSecrets();
        
        // Output the path for debugging
        _output.WriteLine($"Test repository path: {_testRepositoryPath}");
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
    public void Test()
    {
        Assert.NotNull(_testRepositoryPath);
        _output.WriteLine($"Using test repository: {_testRepositoryPath}");
    }
    
    private string CreateDirectory(string path)
    {
        Directory.CreateDirectory(path);
        return path;
    }
    
    private void CreateFile(string path, string content)
    {
        File.WriteAllText(path, content, Encoding.UTF8);
    }
    
    public void Dispose()
    {
        // Clean up the test environment after each test
        if (Directory.Exists(_testRepositoryPath))
        {
           // Directory.Delete(_testRepositoryPath, true);
        }
    }
}