using System.ComponentModel;
using Spectre.Console;
using Spectre.Console.Cli;
using Amba.SecretManager.SecretStorage;

namespace Amba.SecretManager.Commands;

class SaveSettings : CommandSettings
{
    [Description("Profile name (defaults to current directory name).")]
    [CommandArgument(0, "[profile-name]")]
    public string? ProfileName { get; init; }
}

sealed class SaveCommand : Command<SaveSettings>
{
    public override int Execute(CommandContext context, SaveSettings settings, CancellationToken cancellationToken)
    {
        var sourcePath = Environment.CurrentDirectory;
        var profileName = settings.ProfileName ?? new DirectoryInfo(sourcePath).Name;

        var service = new SecretStorageService();
        service.SaveSecrets(profileName, sourcePath);

        AnsiConsole.MarkupLine($"[green]Saved secrets to profile '{profileName}'.[/]");
        return 0;
    }
}
