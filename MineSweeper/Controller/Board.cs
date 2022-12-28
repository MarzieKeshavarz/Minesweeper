using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MineSweeper.Enum;
using MineSweeper.Model;

namespace MineSweeper.Controller
{
    public class Board : Game
    {
        private readonly int BoardSize;
        private readonly int MinesCount;
        private readonly int CellWidth;
        private readonly Cell[,] Cells;
        private GraphicsDeviceManager graphicsDevice;
        private SpriteBatch spriteBatch;
        private Texture2D Blank;
        private Texture2D Bomb;
        private Texture2D Flag;
        private MouseState mouse, prevMouse;
        private int FlagCount, MinesFoundedCount;
        private GameStatus GameStatus = GameStatus.Playing;

        public Board(int boardSize, double minesPercent, int cellWidth)
        {
            BoardSize = boardSize;
            MinesCount = (int)(boardSize * boardSize * minesPercent);
            Cells = new Cell[BoardSize + 2, BoardSize + 2];
            CellWidth = cellWidth;

            graphicsDevice = new GraphicsDeviceManager(this);

            graphicsDevice.PreferredBackBufferHeight = BoardSize * CellWidth;
            graphicsDevice.PreferredBackBufferWidth = BoardSize * CellWidth;

            Content.RootDirectory = "Content";

        }
        protected override void Initialize()
        {
            this.IsMouseVisible = true;
            base.Initialize();
        }
        private void InitializeMines()
        {
            bool[] array = new bool[BoardSize * BoardSize];

            for (int i = 0; i < array.Length; i++)
            {
                if (i < array.Length - MinesCount)
                    array[i] = false;
                else
                    array[i] = true;
            }

            Random rnd = new Random();

            for (int i = 0; i < array.Length; i++)
            {
                int pos = rnd.Next(BoardSize * BoardSize);

                bool swp = array[i];
                array[i] = array[pos];
                array[pos] = swp;
            }
            for (int i = 0; i < array.Length; i++)
            {
                int row = i / 10 + 1;
                int col = i % 10 + 1;

                Cells[row, col] = new Cell((col - 1) * CellWidth, (row - 1) * CellWidth, CellWidth);
                Cells[row, col].HasBomb = array[i];
                Cells[row, col].Texture = Blank;


                if (Cells[row, col].HasBomb)
                    Console.Write("*");
                else
                    Console.Write("-");

                if ((i + 1) % 10 == 0)
                    Console.WriteLine("");

            }


            for (int i = 1; i <= BoardSize; i++)
            {
                for (int j = 1; j <= BoardSize; j++)
                {

                    if (Cells[i - 1, j]?.HasBomb == true)
                        Cells[i, j].NeighbourMinesCount++;

                    if (Cells[i - 1, j - 1]?.HasBomb == true)
                        Cells[i, j].NeighbourMinesCount++;

                    if (Cells[i, j - 1]?.HasBomb == true)
                        Cells[i, j].NeighbourMinesCount++;

                    if (Cells[i + 1, j]?.HasBomb == true)
                        Cells[i, j].NeighbourMinesCount++;

                    if (Cells[i + 1, j + 1]?.HasBomb == true)
                        Cells[i, j].NeighbourMinesCount++;

                    if (Cells[i, j + 1]?.HasBomb == true)
                        Cells[i, j].NeighbourMinesCount++;

                    if (Cells[i - 1, j + 1]?.HasBomb == true)
                        Cells[i, j].NeighbourMinesCount++;

                    if (Cells[i + 1, j - 1]?.HasBomb == true)
                        Cells[i, j].NeighbourMinesCount++;

                }
            }


        }
        protected override void LoadContent()
        {

            spriteBatch = new SpriteBatch(GraphicsDevice);
            Blank = LoadPicture("Content\\blank.png");
            Bomb = LoadPicture("Content\\bomb.png");
            Flag = LoadPicture("Content\\flag.png");
            InitializeMines();
            base.LoadContent();

        }
        protected override void Update(GameTime gameTime)
        {
            mouse = Mouse.GetState();
            int row, col;

            row = mouse.Y / CellWidth + 1;
            col = mouse.X / CellWidth + 1;

            if (row > BoardSize || col > BoardSize)
                return;

            if (mouse.LeftButton == ButtonState.Pressed && prevMouse.LeftButton == ButtonState.Released)
            {
                if (Cells[row, col].HasCover && Cells[row, col].HasBomb)
                {
                    GameStatus = GameStatus.Loose;
                }
                else if (Cells[row, col].HasCover)
                {
                    Cells[row, col].HasCover = false;
                    Cells[row, col].Texture = LoadPicture("Content\\" + Cells[row, col].NeighbourMinesCount + ".png");
                }
            }
            if (mouse.RightButton == ButtonState.Pressed && prevMouse.RightButton == ButtonState.Released)
            {

                if (Cells[row, col].HasCover)
                {
                    if (!Cells[row, col].HasFlag)
                    {
                        if (FlagCount == 10)
                            return;

                        if (Cells[row, col].HasBomb)
                            MinesFoundedCount++;

                        FlagCount++;
                        Cells[row, col].HasFlag = true;
                    }
                    else
                    {
                        if (Cells[row, col].HasBomb)
                            MinesFoundedCount--;

                        FlagCount--;
                        Cells[row, col].HasFlag = false;
                    }

                    if (MinesFoundedCount == MinesCount)
                        GameStatus = GameStatus.Win;

                }

            }


            prevMouse = mouse;
            base.Update(gameTime);
        }
        protected override void Draw(GameTime gameTime)
        {

            if (GameStatus != GameStatus.Playing)
            {
                if (GameStatus == GameStatus.Loose)
                    GraphicsDevice.Clear(Color.IndianRed);
                else
                    GraphicsDevice.Clear(Color.LightGreen);

                spriteBatch.Begin();

                for (int i = 1; i <= BoardSize; i++)
                {
                    for (int j = 1; j <= BoardSize; j++)
                    {
                        if (Cells[i, j].HasBomb)
                            spriteBatch.Draw(Bomb, destinationRectangle: Cells[i, j].Position, Color.White);
                        else
                            spriteBatch.Draw(Blank, destinationRectangle: Cells[i, j].Position, Color.White);
                    }
                }
            }
            else
            {
                GraphicsDevice.Clear(Color.LightPink);
                spriteBatch.Begin();
                for (int i = 1; i <= BoardSize; i++)
                {
                    for (int j = 1; j <= BoardSize; j++)
                    {
                        if (Cells[i, j].HasFlag)
                            spriteBatch.Draw(Flag, destinationRectangle: Cells[i, j].Position, Color.White);
                        else if (!Cells[i, j].HasCover && Cells[i, j].HasBomb && GameStatus == GameStatus.Playing)
                            spriteBatch.Draw(Bomb, destinationRectangle: Cells[i, j].Position, Color.White);
                        else if (!Cells[i, j].HasCover)
                            spriteBatch.Draw(Cells[i, j].Texture == null ? Blank : Cells[i, j].Texture, destinationRectangle: Cells[i, j].Position, Color.White);
                        else
                            spriteBatch.Draw(Blank, destinationRectangle: Cells[i, j].Position, Color.White);
                    }
                }


            }
            spriteBatch.End();

            base.Draw(gameTime);
        }
        private Texture2D LoadPicture(string filename)
        {
            FileStream setStream = File.Open(filename, FileMode.Open);
            Texture2D NewTexture = Texture2D.FromStream(GraphicsDevice, setStream);
            setStream.Dispose();
            return NewTexture;
        }
    }
}
