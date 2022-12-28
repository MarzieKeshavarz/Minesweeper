using MineSweeper.Consts;
using MineSweeper.Controller;

namespace MineSweeper
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Board board = new Board(Constants.BoardSize, Constants.MinesPercent, Constants.CellWidth);
            board.Run();
        }
    }
}