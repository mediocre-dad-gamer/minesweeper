namespace Minesweeper.Tests;

public class ClickResultTests
{
    [Fact]
    public void ClickResultToStringReturnsCorrectValue()
    {
        var clickResult = new ClickResult();
        clickResult.XCoordinate = 1;
        clickResult.YCoordinate = 1;
        clickResult.WasMine = true;
        clickResult.NumberOfNeighborMines = 1;

        var expectedString = "[1,1]; WasMine: True; NumberOfNeighborMines: 1";

        var actualString = clickResult.ToString();

        Assert.Equal(expectedString, actualString);

        clickResult.WasMine = false;

        expectedString = "[1,1]; WasMine: False; NumberOfNeighborMines: 1";

        actualString = clickResult.ToString();

        Assert.Equal(expectedString, actualString);
    }
}