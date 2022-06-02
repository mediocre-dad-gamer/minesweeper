namespace Minesweeper;

public class ClickResult
{
    public bool WasFlag { get; set; }
    public bool Deflag { get; set; }
    public bool WasMine { get; set; }
    public bool WasWinningClick { get; set; }
    public int XCoordinate { get; set; }
    public int YCoordinate { get; set; }
    public bool HasNeighborMines => NumberOfNeighborMines > 0;
    public int NumberOfNeighborMines { get; set; }

    public override string ToString()
    {
        return $"[{XCoordinate},{YCoordinate}]; WasMine: {WasMine}; NumberOfNeighborMines: {NumberOfNeighborMines}";
    }
}