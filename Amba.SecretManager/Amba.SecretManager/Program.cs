// Program.cs - Minimal Spectre.Console.Cli app with `plan` and `apply` commands
// --------------------------------------------------------------
// Build: dotnet build
// Run:   dotnet run -- plan [directory] [--json]
//        dotnet run -- apply [--auto-approve]
//
// NuGet packages (see .csproj):
//   <PackageReference Include="Spectre.Console" Version="0.50.0" />
//   <PackageReference Include="Spectre.Console.Cli" Version="0.50.0" />
// --------------------------------------------------------------

using Spectre.Console.Cli;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;

var app = new CommandApp();

app.Configure(config =>
{
    config.SetApplicationName("tfclone");

    config.AddCommand<LoadCommand>("load")
          .WithDescription("Generate and show an execution plan");

    config.AddCommand<SaveCommand>("save")
          .WithDescription("Apply the changes required to reach desired state");
});

return await app.RunAsync(args);
