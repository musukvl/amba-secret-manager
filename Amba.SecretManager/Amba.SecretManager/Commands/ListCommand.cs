using Spectre.Console;
using Spectre.Console.Cli;
using Amba.SecretManager.SecretStorage;

namespace Amba.SecretManager.Commands;

sealed class ListCommand : Command
{
    public override int Execute(CommandContext context, CancellationToken cancellationToken)
    {
        var service = new SecretStorageService();
        var profiles = service.ListProfiles().ToList();

        if (profiles.Count == 0)
        {
            AnsiConsole.MarkupLine("[yellow]No profiles saved.[/]");
            return 0;
        }

        foreach (var profile in profiles)
            AnsiConsole.MarkupLine($"  {profile}");

        return 0;
    }
}
