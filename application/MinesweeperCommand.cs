using Spectre.Console;
using Spectre.Console.Cli;
using Spectre.Console.Rendering;

namespace MineSweeper.Application;

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
        foreach (var mineCoordinate in minefield.MineIndexes)
        {
            foreach (var mineY in mineCoordinate.Value)
            {
                table.UpdateCell(mineCoordinate.Key, mineY, new Panel(RedCanvas()));
            }
        }
    }

    private void CreateMinefield(Table table, MineField minefield, List<Panel> defaultCellValue)
    {
        table.AddColumn("", column =>
        {
            column.Alignment(Justify.Center);
        });

        for (var i = 0; i < minefield.Columns.Count; i++)
        {
            defaultCellValue.Add(new Panel(WhiteCanvas()));
            table.AddColumn(Constants.ALPHABET[i], column =>
            {
                column.Alignment(Justify.Center);
            });
        }

        foreach (var row in minefield.Rows)
        {
            var index = (row.RowIndex + 1).ToString();

            if (row.RowIndex + 1 < 10)
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
        var minefield = new MineField();
        for (var i = 0; i < width; i++)
        {
            minefield.Columns.Add(new MinefieldColumn(i));
        }

        for (var i = 0; i < height; i++)
        {
            minefield.Rows.Add(new MinefieldRow(i));
        }

        var numberOfMines = Math.Round(width * height * (percentOfMines / 100));
        var random = new Random();

        AddMines(width, height, minefield, numberOfMines, random);

        return minefield;
    }

    private void AddMines(int width, int height, MineField minefield, decimal numberOfMines, Random random)
    {
        while (minefield.MineIndexes.Values.SelectMany(v => v).Count() < numberOfMines)
        {
            var x = random.Next(0, height);
            var y = random.Next(1, width + 1);

            var existingX = minefield.MineIndexes.ContainsKey(x);
            if (!existingX)
            {
                minefield.MineIndexes.Add(x, new List<int> {
                    y
                });
                continue;
            }
            else
            {
                var existingList = minefield.MineIndexes[x];
                if (existingList.Contains(y))
                {
                    continue;
                }
                else
                {
                    existingList.Add(y);
                }
            }
        }
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