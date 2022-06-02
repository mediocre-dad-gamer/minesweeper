using Minesweeper.Application;
using Spectre.Console.Cli;

var app = new CommandApp();

app.Configure(config =>
{
    config.AddBranch<MinesweeperSettings>("play", add =>
    {
        add.AddCommand<MinesweeperCommand>("classic");
    });
});

return app.Run(args);