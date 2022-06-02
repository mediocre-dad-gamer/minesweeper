using Spectre.Console;
using Spectre.Console.Cli;
using Spectre.Console.Rendering;

namespace Minesweeper.Application;

public class MinesweeperCommand : Command<MinesweeperSettings>
{
    public override int Execute(CommandContext context, MinesweeperSettings settings)
    {
        AnsiConsole.Clear();
        var debug = settings.Debug.GetValueOrDefault();
        var height = settings.VerticalHeight;
        var width = settings.HorizontalWidth;
        var mines = Convert.ToDecimal(settings.PercentMines);

        var table = new Table().Centered().NoBorder();

        var minefield = BuildMineField(width, height, mines);
        var defaultCellValue = new List<Panel>();
        CreateMinefield(table, minefield, defaultCellValue);

        if (debug)
        {
            RevealMines(table, minefield);
        }

        while (true)
        {
            AnsiConsole.Write(table);
            AnsiConsole.WriteLine("Type a coordinate (A1) to click a square. Type \"Flag <coordinate>\" to flag a square.");

            var command = AnsiConsole.Ask<string>(string.Empty);
            if (command.Equals("quit", StringComparison.CurrentCultureIgnoreCase))
            {
                AnsiConsole.Clear();
                return 0;
            }
            var clickResults = minefield.ClickSquare(command);
            if (clickResults == null)
            {
                Console.WriteLine("Command wasn't valid. Try again");
                Thread.Sleep(200);
                AnsiConsole.Clear();
                continue;
            }

            if (debug)
            {
                foreach (var clickResult in clickResults)
                {
                    Console.WriteLine(clickResult);
                }
            }

            IRenderable content;
            foreach (var clickResult in clickResults)
            {
                if (clickResult.WasMine)
                {
                    RevealMines(table, minefield);
                    AnsiConsole.Clear();
                    AnsiConsole.Write(table);
                    AnsiConsole.WriteLine("OH NOOOOOOOO!");
                    return 0;
                }
                else if (clickResult.HasNeighborMines)
                {
                    content = new Text($"{clickResult.NumberOfNeighborMines}");
                }
                else if (clickResult.WasFlag)
                {
                    content = YellowCanvas();
                }
                else if (clickResult.Deflag)
                {
                    content = WhiteCanvas();
                }
                else if (clickResult.WasWinningClick)
                {
                    content = EmptyCanvas();
                }
                else
                {
                    content = EmptyCanvas();
                }

                table.UpdateCell(clickResult.YCoordinate, clickResult.XCoordinate, new Panel(content).Expand());

            }
            if (clickResults.Any(cr => cr.WasWinningClick))
            {
                AnsiConsole.Clear();
                RevealMines(table, minefield);
                AnsiConsole.Write(table);
                AnsiConsole.WriteLine("Winner winner, Chicken Dinner!");
                return 0;
            }
            table.Collapse();
            if (!debug)
            {
                AnsiConsole.Clear();
            }
        }
    }

    private void RevealMines(Table table, MineField minefield)
    {
        foreach (var kvp in minefield.GetMineIndeces())
        {
            table.UpdateCell(kvp.Key, kvp.Value, new Panel(RedCanvas()));
        }
    }

    private void CreateMinefield(Table table, MineField minefield, List<Panel> defaultCellValue)
    {
        table.AddColumn("", column =>
        {
            column.Alignment(Justify.Center);
        });

        for (var i = 0; i < minefield.ColumnCount; i++)
        {
            defaultCellValue.Add(new Panel(WhiteCanvas()));
            table.AddColumn(Constants.ALPHABET[i], column =>
            {
                column.Alignment(Justify.Center);
            });
        }

        for (var i = 1; i < minefield.RowCount + 1; i++)
        {
            var index = i.ToString();

            if (i < 10)
            {
                index = $"0{index}";
            }

            var localValues = new List<IRenderable>
            {
                new Panel(index)
            };

            localValues.AddRange(defaultCellValue);
            table.AddRow(localValues);
        }
    }

    private MineField BuildMineField(int width, int height, decimal percentOfMines)
    {
        var minefield = new MineField(width, height, percentOfMines);

        return minefield;
    }

    private Canvas SquareCanvas(Color color)
    {
        return new Canvas(1, 1).SetPixel(0, 0, color);
    }

    private Canvas WhiteCanvas()
    {
        return SquareCanvas(Color.White);
    }

    private Canvas RedCanvas()
    {
        return SquareCanvas(Color.Red);
    }

    private Canvas YellowCanvas()
    {
        return SquareCanvas(Color.Yellow);
    }

    private Canvas EmptyCanvas()
    {
        return SquareCanvas(Color.Default);
    }
}