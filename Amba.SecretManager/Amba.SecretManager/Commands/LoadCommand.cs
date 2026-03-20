using System.ComponentModel;
using Spectre.Console;
using Spectre.Console.Cli;
using Amba.SecretManager.SecretStorage;

namespace Amba.SecretManager.Commands;

class LoadSettings : CommandSettings
{
    [Description("Profile name (defaults to current directory name).")]
    [CommandArgument(0, "[profile-name]")]
    public string? ProfileName { get; init; }
}

sealed class LoadCommand : Command<LoadSettings>
{
    public override int Execute(CommandContext context, LoadSettings settings, CancellationToken cancellationToken)
    {
        var destinationPath = Environment.CurrentDirectory;
        var profileName = settings.ProfileName ?? new DirectoryInfo(destinationPath).Name;

        var service = new SecretStorageService();

        if (!service.ProfileExists(profileName))
        {
            AnsiConsole.MarkupLine($"[red]Profile not found[/]");
            return 1;
        }

        service.LoadSecrets(profileName, destinationPath);

        AnsiConsole.MarkupLine($"[green]Loaded secrets from profile '{profileName}'.[/]");
        return 0;
    }
}
