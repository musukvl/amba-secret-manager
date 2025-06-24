namespace Amba.SecretManager.SecretStorage;

public class SecretStorageService
{
    private readonly string _storageRootDirectory;

    public SecretStorageService(string storageRootDirectory)
    {
        _storageRootDirectory = storageRootDirectory;
    }


    // returns the path in the storage directory
    private string GetStoragePath(string profile, string path)
    {
        return Path.Combine(_storageRootDirectory, profile, path);
    }

    public void SaveSecrets(string profile, string sourcePath)
    {
        if (!Directory.Exists(sourcePath))
        {
            throw new DirectoryNotFoundException($"Source path '{sourcePath}' does not exist.");
        }

        var storagePath = GetStoragePath(profile, string.Empty);
        if (!Directory.Exists(storagePath))
        {
            Directory.CreateDirectory(storagePath);
        }

        // Copy all .env files from the source path to the storage path
        foreach (var file in Directory.GetFiles(sourcePath, "*.env", SearchOption.AllDirectories))
        {
            var relativePath = Path.GetRelativePath(sourcePath, file);
            var destFile = Path.Combine(storagePath, relativePath);
            var destDir = Path.GetDirectoryName(destFile);
            if (destDir != null && !Directory.Exists(destDir))
            {
                Directory.CreateDirectory(destDir);
            }
            File.Copy(file, destFile, true);
        }
        // copy all .secrets directories from the source path to the storage path
        foreach (var dir in Directory.GetDirectories(sourcePath, ".secrets", SearchOption.AllDirectories))
        {
            var relativePath = Path.GetRelativePath(sourcePath, dir);
            var destDir = Path.Combine(storagePath, relativePath);
            if (!Directory.Exists(destDir))
            {
                Directory.CreateDirectory(destDir);
            }
            // Copy all files in the .secrets directory
            foreach (var file in Directory.GetFiles(dir))
            {
                var destFile = Path.Combine(destDir, Path.GetFileName(file));
                File.Copy(file, destFile, true);
            }
        }
    }
    
        // Copy all files from the source path to the storage path
    public string LoadSecrets(string profile, string destinationPath)
    {
        var storagePath = GetStoragePath(profile, string.Empty);
        if (!Directory.Exists(storagePath))
        {
            throw new DirectoryNotFoundException($"Storage path '{storagePath}' does not exist.");
        }

        var destinationDir = Path.GetDirectoryName(destinationPath);
        if (destinationDir != null && !Directory.Exists(destinationDir))
        {
            Directory.CreateDirectory(destinationDir);
        }

        // Copy all files from the storage path to the destination path
        foreach (var file in Directory.GetFiles(storagePath, "*", SearchOption.AllDirectories))
        {
            var relativePath = Path.GetRelativePath(storagePath, file);
            var destFile = Path.Combine(destinationPath, relativePath);
            var destDir = Path.GetDirectoryName(destFile);
            if (destDir != null && !Directory.Exists(destDir))
            {
                Directory.CreateDirectory(destDir);
            }
            File.Copy(file, destFile, true);
        }
 
        return destinationPath;
    }     
    
}