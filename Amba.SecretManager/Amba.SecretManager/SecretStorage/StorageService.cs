namespace Amba.SecretManager.SecretStorage;

public class SecretStorageService(string storageRootDirectory)
{
    private static readonly string DefaultStorageRoot =
        Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".secret-profiles");

    public SecretStorageService() : this(DefaultStorageRoot) { }

    private string GetProfilePath(string profile) =>
        Path.Combine(storageRootDirectory, profile);

    public void SaveSecrets(string profile, string sourcePath)
    {
        if (!Directory.Exists(sourcePath))
            throw new DirectoryNotFoundException($"Source path '{sourcePath}' does not exist.");

        var profilePath = GetProfilePath(profile);

        // Clean replace: delete entire profile before writing
        if (Directory.Exists(profilePath))
            Directory.Delete(profilePath, true);

        Directory.CreateDirectory(profilePath);

        // Copy all *.env files
        foreach (var file in Directory.GetFiles(sourcePath, "*.env", SearchOption.AllDirectories))
        {
            var relativePath = Path.GetRelativePath(sourcePath, file);
            var destFile = Path.Combine(profilePath, relativePath);
            Directory.CreateDirectory(Path.GetDirectoryName(destFile)!);
            File.Copy(file, destFile, true);
        }

        // Copy all .secrets directories recursively
        foreach (var dir in Directory.GetDirectories(sourcePath, ".secrets", SearchOption.AllDirectories))
        {
            var relativePath = Path.GetRelativePath(sourcePath, dir);
            var destDir = Path.Combine(profilePath, relativePath);

            foreach (var file in Directory.GetFiles(dir, "*", SearchOption.AllDirectories))
            {
                var fileRelative = Path.GetRelativePath(sourcePath, file);
                var destFile = Path.Combine(profilePath, fileRelative);
                Directory.CreateDirectory(Path.GetDirectoryName(destFile)!);
                File.Copy(file, destFile, true);
            }
        }
    }

    public void LoadSecrets(string profile, string destinationPath)
    {
        var profilePath = GetProfilePath(profile);
        if (!Directory.Exists(profilePath))
            throw new DirectoryNotFoundException("Profile not found");

        foreach (var file in Directory.GetFiles(profilePath, "*", SearchOption.AllDirectories))
        {
            var relativePath = Path.GetRelativePath(profilePath, file);
            var destFile = Path.Combine(destinationPath, relativePath);
            Directory.CreateDirectory(Path.GetDirectoryName(destFile)!);
            File.Copy(file, destFile, true);
        }
    }

    public bool ProfileExists(string profile) =>
        Directory.Exists(GetProfilePath(profile));

    public IEnumerable<string> ListProfiles()
    {
        if (!Directory.Exists(storageRootDirectory))
            return [];

        return Directory.GetDirectories(storageRootDirectory)
            .Select(Path.GetFileName)
            .Where(name => name is not null)
            .Cast<string>()
            .Order();
    }
}
