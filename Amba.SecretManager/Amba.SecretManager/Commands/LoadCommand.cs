using System.ComponentModel;
using Amba.SecretManager.SecretStorage;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Amba.SecretManager.Commands;

// ---------------- Settings ----------------
class LoadSettings : CommandSettings
{
    [Description("Profile name to load secrets from.")]
    [CommandArgument(0, "<profile>")]
    public string Profile { get; init; } = string.Empty;

    [Description("Remove all content from .secrets folders before copying new content.")]
    [CommandOption("--overwrite")]
    public bool Overwrite { get; init; }
}

sealed class LoadCommand : AsyncCommand<LoadSettings>
{
    public override Task<int> ExecuteAsync(CommandContext context, LoadSettings settings, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(settings.Profile))
        {
            AnsiConsole.MarkupLine("[red]Error: Profile name is required.[/]");
            return Task.FromResult(1);
        }

        var currentDirectory = Environment.CurrentDirectory;
        var homeDirectory = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        var storageDirectory = Path.Combine(homeDirectory, ".secrets");

        AnsiConsole.MarkupLine($"[yellow]Loading secrets from profile[/] [aqua]{settings.Profile}[/] [yellow]to[/] [aqua]{currentDirectory}[/]...");

        if (settings.Overwrite)
        {
            AnsiConsole.MarkupLine("[yellow]Warning: --overwrite option will remove all existing .secrets folders content.[/]");
        }

        try
        {
            AnsiConsole.Status()
                .Start("Loading secrets…", ctx =>
                {
                    var service = new SecretStorageService(storageDirectory);
                    service.LoadSecrets(settings.Profile, currentDirectory, settings.Overwrite);
                });

            AnsiConsole.MarkupLine($"[bold green]Secrets loaded successfully from profile '{settings.Profile}'![/]");
            return Task.FromResult(0);
        }
        catch (Exception ex)
        {
            AnsiConsole.MarkupLine($"[red]Error loading secrets: {ex.Message}[/]");
            return Task.FromResult(1);
        }
    }
}