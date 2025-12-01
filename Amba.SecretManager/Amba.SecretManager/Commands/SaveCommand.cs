using System.ComponentModel;
using Amba.SecretManager.SecretStorage;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Amba.SecretManager.Commands;

class SaveSettings : CommandSettings
{
    [Description("Profile name to save secrets to.")]
    [CommandArgument(0, "<profile>")]
    public string Profile { get; init; } = string.Empty;

    [Description("Skip approval prompt.")]
    [CommandOption("--auto-approve")]
    public bool AutoApprove { get; init; }
}


sealed class SaveCommand : AsyncCommand<SaveSettings>
{
    public override Task<int> ExecuteAsync(CommandContext context, SaveSettings settings, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(settings.Profile))
        {
            AnsiConsole.MarkupLine("[red]Error: Profile name is required.[/]");
            return Task.FromResult(1);
        }

        var currentDirectory = Environment.CurrentDirectory;
        var homeDirectory = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        var storageDirectory = Path.Combine(homeDirectory, ".secrets");

        AnsiConsole.MarkupLine($"[yellow]Saving secrets from[/] [aqua]{currentDirectory}[/] [yellow]to profile[/] [aqua]{settings.Profile}[/]...");

        if (!settings.AutoApprove)
        {
            if (!AnsiConsole.Confirm("Do you want to save these secrets?"))
            {
                AnsiConsole.MarkupLine("[yellow]Save cancelled.[/]");
                return Task.FromResult(1);
            }
        }

        try
        {
            AnsiConsole.Status()
                .Start("Saving secrets…", ctx =>
                {
                    var service = new SecretStorageService(storageDirectory);
                    service.SaveSecrets(settings.Profile, currentDirectory);
                });

            AnsiConsole.MarkupLine($"[bold green]Secrets saved successfully to profile '{settings.Profile}'![/]");
            return Task.FromResult(0);
        }
        catch (Exception ex)
        {
            AnsiConsole.MarkupLine($"[red]Error saving secrets: {ex.Message}[/]");
            return Task.FromResult(1);
        }
    }
}