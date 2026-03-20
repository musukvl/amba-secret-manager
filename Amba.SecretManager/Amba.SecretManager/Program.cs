using Spectre.Console.Cli;
using Amba.SecretManager.Commands;

var app = new CommandApp();

app.Configure(config =>
{
    config.SetApplicationName("sm");

    config.AddCommand<SaveCommand>("save")
          .WithDescription("Save secret files to a profile");

    config.AddCommand<LoadCommand>("load")
          .WithDescription("Load secret files from a profile");

    config.AddCommand<ListCommand>("list")
          .WithDescription("List all saved profiles");
});

return app.Run(args);
