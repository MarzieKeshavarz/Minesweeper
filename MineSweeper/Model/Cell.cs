using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MineSweeper.Model
{
    public class Cell
    {
        public Cell(int x, int y, int width)
        {
            HasBomb = false;
            HasCover = true;
            HasFlag = false;
            NeighbourMinesCount = 0;
            Position = new Microsoft.Xna.Framework.Rectangle(x, y, width, width);
        }
        public bool HasBomb { get; set; } = false;

        public bool HasCover { get; set; } = true;

        public bool HasFlag { get; set; } = false;

        public int NeighbourMinesCount { get; set; } = 0;

        public Microsoft.Xna.Framework.Rectangle Position { get; set; } = new Microsoft.Xna.Framework.Rectangle();

        public Texture2D Texture { get; set; }

    }
}
