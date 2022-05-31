using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

namespace MineSweeper;

public class MineField
{
    private Dictionary<int, List<int>> _flaggedSquares = new Dictionary<int, List<int>>();
    private Dictionary<int, List<int>> _clickedSquares = new Dictionary<int, List<int>>();
    private int _totalSquares => Columns.Count * Rows.Count;
    private int _totalMineSquares => MineIndexes.SelectMany(mi => mi.Value).Count();
    private int _winCriteria => _totalSquares - _totalMineSquares;

    public List<MinefieldColumn> Columns { get; set; } = new List<MinefieldColumn>();
    public List<MinefieldRow> Rows { get; set; } = new List<MinefieldRow>();
    public Dictionary<int, List<int>> MineIndexes { get; set; } = new Dictionary<int, List<int>>();

    public List<ClickResult> ClickSquare(string command)
    {
        var splitString = command.Split(" ");
        var flagCommand = false;
        string squareCommand;
        if (splitString.Length > 1)
        {
            if (!splitString[0].Equals("flag", StringComparison.CurrentCultureIgnoreCase))
            {
                return null;
            }
            flagCommand = true;
            squareCommand = splitString[1];
        }
        else
        {
            squareCommand = splitString[0];
        }

        var columnAlpha = Regex.Match(squareCommand, "[a-zA-Z]").Value;
        var row = Convert.ToInt32(Regex.Match(squareCommand, "\\d+").Value) - 1;
        var column = Array.IndexOf(Constants.ALPHABET, columnAlpha.ToUpper()) + 1;

        if (row >= Rows.Count || column > Columns.Count)
        {
            return null;
        }

        if (flagCommand)
        {
            return PerformFlag(row, column);
        }

        if (_flaggedSquares.ContainsKey(row) && _flaggedSquares[row].Contains(column))
        {
            return null;
        }

        var clickResults = PerformClick(row, column);

        return clickResults;
    }

    private List<ClickResult> PerformFlag(int row, int column)
    {
        var clickResult = new ClickResult
        {
            WasFlag = true,
            XCoordinate = column,
            YCoordinate = row
        };
        var values = _flaggedSquares.GetValueOrDefault(row);
        if (values == null)
        {
            values = new List<int>();
            _flaggedSquares.Add(row, values);
        }
        if (values.Contains(column))
        {
            clickResult.WasFlag = false;
            clickResult.Deflag = true;
            values.Remove(column);
        }
        else
        {
            values.Add(column);
        }
        return new List<ClickResult>()
        {
            clickResult
        };
    }

    private List<ClickResult> PerformClick(int row, int column, List<ClickResult> clickResults = null)
    {
        Console.WriteLine($"Clicking [{row},{column}]");

        if (clickResults == null)
        {
            clickResults = new List<ClickResult>();
        }

        var clickResult = new ClickResult();
        clickResults.Add(clickResult);

        if (MineIndexes.ContainsKey(row) && MineIndexes[row].Contains(column))
        {
            clickResult.WasMine = true;
        }

        if (!clickResult.WasMine && _clickedSquares.SelectMany(cs => cs.Value).Count() == _winCriteria)
        {
            clickResult.WasWinningClick = true;
        }

        clickResult.XCoordinate = column;
        clickResult.YCoordinate = row;

        if (clickResult.WasMine)
        {
            return clickResults;
        }

        var values = _clickedSquares.GetValueOrDefault(row);
        if (values == null)
        {
            values = new List<int>();
            _clickedSquares.Add(row, values);
        }
        values.Add(column);

        clickResult.NumberOfNeighborMines = FindNeighborMines(row, column);

        if (!clickResult.HasNeighborMines && !clickResult.WasMine)
        {
            Console.WriteLine($"[{row},{column}] has no neighbor mines. Clicking Neighbors");
            ClickNeighbors(clickResult, clickResults);
        }

        return clickResults;
    }

    private void ClickNeighbors(ClickResult clickResult, [NotNull] List<ClickResult> clickResults)
    {
        var column = clickResult.XCoordinate;
        var row = clickResult.YCoordinate;
        var columnsToCheck = new List<int> { column - 1, column, column + 1 };
        var rowsToCheck = new List<int> { row - 1, row, row + 1 };
        Console.WriteLine($"Checking neighbors of [{row},{column}]");
        foreach (var rowIndex in rowsToCheck)
        {
            if (rowIndex >= Rows.Count || rowIndex < 0)
            {
                Console.WriteLine($"{rowIndex} isn't valid. Skipping");
                continue;
            }
            foreach (var columnIndex in columnsToCheck)
            {
                Console.WriteLine($"Checking square [{rowIndex}, {columnIndex}]");
                if (columnIndex > Columns.Count || columnIndex < 1
                    || clickResults.Any(cr => cr.XCoordinate == columnIndex && cr.YCoordinate == rowIndex))
                {
                    continue;
                }

                PerformClick(rowIndex, columnIndex, clickResults);
            }
        }
    }

    private int FindNeighborMines(int row, int column)
    {
        var columnsToCheck = new List<int> { column - 1, column, column + 1 };
        var rowsToCheck = new List<int> { row - 1, row, row + 1 };
        var neighborMines = 0;
        foreach (var rowIndex in rowsToCheck)
        {
            if (rowIndex >= Rows.Count || rowIndex < 0)
            {
                continue;
            }

            foreach (var columnIndex in columnsToCheck)
            {
                if (columnIndex > Columns.Count || columnIndex < 1 || (rowIndex == row && columnIndex == column))
                {
                    continue;
                }
                if (MineIndexes.ContainsKey(rowIndex) && MineIndexes[rowIndex].Contains(columnIndex))
                {
                    neighborMines++;
                }
            }
        }

        return neighborMines;
    }
}