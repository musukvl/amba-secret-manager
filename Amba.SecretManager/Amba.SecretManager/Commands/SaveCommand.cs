using System.ComponentModel;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Amba.SecretManager.Commands;

class SaveSettings : CommandSettings
{
    [Description("Skip approval prompt.")]
    [CommandOption("--auto-approve")]
    public bool AutoApprove { get; init; }
}


sealed class SaveCommand : AsyncCommand<SaveSettings>
{
    public override Task<int> ExecuteAsync(CommandContext context, SaveSettings settings)
    {
        if (!settings.AutoApprove)
        {
            if (!AnsiConsole.Confirm("Do you want to perform these actions?"))
            {
                AnsiConsole.MarkupLine("[yellow]Apply cancelled.[/]");
                return Task.FromResult(1);
            }
        }

        // Simulate apply
        AnsiConsole.Status()
            .Start("Saving changes…", ctx =>
            {
                ctx.Status("Creating resources…");
                Thread.Sleep(1200);
                ctx.Status("Destroying resources…");
                Thread.Sleep(700);
            });

        AnsiConsole.MarkupLine("[bold green]Apply complete! Resources: 1 added, 0 changed, 1 destroyed.[/]");
        return Task.FromResult(0);
    }
}