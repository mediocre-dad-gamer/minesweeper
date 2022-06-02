using System.ComponentModel;
using Spectre.Console.Cli;

namespace Minesweeper.Application;

public class MinesweeperSettings : CommandSettings
{
    [CommandOption("-w|--horizontal <HORIZONTALWIDTH>")]
    [DefaultValue(10)]
    public int HorizontalWidth { get; set; }

    [CommandOption("-v|--vertical <VERTICALHEIGHT>")]
    [DefaultValue(10)]
    public int VerticalHeight { get; set; }

    [CommandOption("-p|--percent-mines <PERCENTMINES>")]
    [DefaultValue(25)]
    public int PercentMines { get; set; }

    [CommandOption("-d|--debug <DEBUG>", IsHidden = true)]
    public bool? Debug { get; set; }
}