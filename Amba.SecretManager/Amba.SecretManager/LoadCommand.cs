using System.ComponentModel;
using Spectre.Console;
using Spectre.Console.Cli;

// ---------------- Settings ----------------
class LoadSettings : CommandSettings
{
    [Description("Path to the working directory (default: current directory).")]
    [CommandArgument(0, "[directory]")]
    public string? Directory { get; init; }

    [Description("Print plan in JSON format.")]
    [CommandOption("--json")]
    public bool Json { get; init; }
}

sealed class LoadCommand : AsyncCommand<LoadSettings>
{
    public override Task<int> ExecuteAsync(CommandContext context, LoadSettings settings)
    {
        var dir = settings.Directory ?? Environment.CurrentDirectory;
        AnsiConsole.MarkupLine($"[yellow]Planning changes in[/] [aqua]{dir}[/]...");

        // Simulate work
        AnsiConsole.Status()
            .Start("Evaluating infrastructure…", ctx =>
            {
                Thread.Sleep(800);
            });

        // Fake diff output
        AnsiConsole.Write(new Rule("[bold]Execution plan[/]").Centered());
        AnsiConsole.MarkupLine("[green]+ create[/] resource \"example\" \"app\"");
        AnsiConsole.MarkupLine("[red]- destroy[/] resource \"example\" \"db\"");

        AnsiConsole.MarkupLine("\n[bold green]Plan: 1 to add, 0 to change, 1 to destroy[/]");
        return Task.FromResult(0);
    }
}