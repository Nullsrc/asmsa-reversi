using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Reversi
{
    public partial class Form1 : Form
    {
        int margin = 10;
        int x;
        int y;
        int cellSize;
        int row;
        int col;
        int turn = 0;
        int cursorCol;
        int cursorRow;
        int[,] board = new int[8, 8] { { -1, -1, -1, -1, -1, -1, -1, -1 }, { -1, -1, -1, -1, -1, -1, -1, -1 }, 
                                       { -1, -1, -1, -1, -1, -1, -1, -1 }, { -1, -1, -1, 0, 1, -1, -1, -1 }, 
                                       { -1, -1, -1, 1, 0, -1, -1, -1 }, { -1, -1, -1, -1, -1, -1, -1, -1 }, 
                                       { -1, -1, -1, -1, -1, -1, -1, -1 }, { -1, -1, -1, -1, -1, -1, -1, -1 } };
        int[,] legals = new int[8, 8];
        int[] possible = new int[8];
        int numOfPossible;
        enum Flips { None, Down, Up, Right, Left, DownRight, DownLeft, UpRight, UpLeft }


        public Form1()
        {
            InitializeComponent();
            UpdateSize();
            DoubleBuffered = true;
            this.Size = new System.Drawing.Size(620, 620);
            this.MinimumSize = new System.Drawing.Size(220, 220);
            this.MaximumSize = new System.Drawing.Size(1020, 1020);
        }

        private void UpdateSize()
        {
            cellSize = (Math.Min(ClientSize.Width, ClientSize.Height) - 2 * margin) / 8;
            if (ClientSize.Width > ClientSize.Height)
            {
                y = margin;
                x = (ClientSize.Width - 8 * cellSize) / 2;
            }
            else
            {
                x = margin;
                y = (ClientSize.Height - 8 * cellSize) / 2;
            }
        }

        private void FindLegalMoves()
        {
            Array.Clear(legals, 0, legals.Length);
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    // Vertical Down Pass:
                    if (j < 6 && j >= 0)
                    {
                        if ((board[i, j + 1] != board[i, j]) && (board[i, j + 2] != board[i, j + 1]) && (board[i, j + 2] == turn % 2) && (board[i, j] == -1))
                            legals[i, j] = (int)Flips.Down;
                    }
                    
                    // Vertical Up Pass
                    if (j < 8 && j >= 2)
                    {
                        if ((board[i, j - 1] != board[i, j]) && (board[i, j - 2] != board[i, j - 1]) && (board[i, j - 2] == turn % 2) && (board[i, j] == -1))
                            legals[i, j] = legals[i, j] * 10 + (int)Flips.Up;
                    }

                    // Horizontal Right Pass
                    if (i < 6 && i >= 0)
                    {
                        if ((board[i + 1, j] != board[i, j]) && (board[i + 2, j] != board[i + 1, j]) && (board[i + 2, j] == turn % 2) && (board[i, j] == -1))
                            legals[i, j] = legals[i, j] * 10 + (int)Flips.Right;
                    }

                    //Horizontal Left Pass
                    if (i < 8 && i >= 2)
                    {
                        if ((board[i - 1, j] != board[i, j]) && (board[i - 2, j] != board[i - 1, j]) && (board[i - 2, j] == turn % 2) && (board[i, j] == -1))
                            legals[i, j] = legals[i, j] * 10 + (int)Flips.Left;
                    }

                    // Diagonal Down-Right Pass
                    if (j < 6 && j >= 0 && i < 6 && i >= 0)
                    {
                        if ((board[i + 1, j + 1] != board[i, j]) && (board[i + 2, j + 2] != board[i + 1, j + 1]) && (board[i + 2, j + 2] == turn % 2) && (board[i, j] == -1))
                            legals[i, j] = legals[i,j] * 10 + (int)Flips.DownRight;
                    }
                    
                    // Diagonal Down-Left Pass
                    if (j < 6 && j >= 0 && i < 8 && i >= 2)
                    {
                        if ((board[i - 1, j + 1] != board[i, j]) && (board[i - 2, j + 2] != board[i - 1, j + 1]) && (board[i - 2, j + 2] == turn % 2) && (board[i, j] == -1))
                            legals[i, j] = legals[i, j] * 10 + (int)Flips.DownLeft;
                    }

                    // Diagonal Up-Right Pass
                    if (j < 8 && j >= 2 && i < 6 && i >= 0)
                    {
                        if ((board[i + 1, j - 1] != board[i, j]) && (board[i + 2, j - 2] != board[i + 1, j - 1]) && (board[i + 2, j - 2] == turn % 2) && (board[i, j] == -1))
                            legals[i, j] = legals[i, j] * 10 + (int)Flips.UpRight;
                    }

                    // Diagonal Up-Left Pass
                    if (j < 8 && j >= 2 && i < 8 && i >= 2)
                    {
                        if ((board[i - 1, j - 1] != board[i, j]) && (board[i - 2, j - 2] != board[i - 1, j - 1]) && (board[i - 2, j - 2] == turn % 2) && (board[i, j] == -1))
                            legals[i, j] = legals[i, j] * 10 + (int)Flips.UpLeft;
                    }
                }
            }
        }

        private void GenerateAllFlips(int col, int row)
        {
            numOfPossible = legals[col, row].ToString().Length;
            for (int i = 0; i < numOfPossible; i++)
            {
                possible[i] = legals[col, row] % 10;
                legals[col, row] = legals[col, row] / 10;
            }
        }

        private void Place(int col, int row)
        {
            board[col, row] = turn % 2;
            GenerateAllFlips(col, row);
            for (int i = 0; i < numOfPossible; i++)
            {
                switch (possible[i])
                {
                    case (int)Flips.Down: board[col, row + 1] = turn % 2; break;
                    case (int)Flips.Up: board[col, row - 1] = turn % 2; break;
                    case (int)Flips.Right: board[col + 1, row] = turn % 2; break;
                    case (int)Flips.Left: board[col - 1, row] = turn % 2; break;
                    case (int)Flips.DownRight: board[col + 1, row + 1] = turn % 2; break;
                    case (int)Flips.DownLeft: board[col - 1, row + 1] = turn % 2; break;
                    case (int)Flips.UpRight: board[col + 1, row - 1] = turn % 2; break;
                    case (int)Flips.UpLeft: board[col - 1, row - 1] = turn % 2; break;
                    default: break;
                } 
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            cursorCol = (int)Math.Floor((e.X - x) * 1.0 / cellSize);
            cursorRow = (int)Math.Floor((e.Y - y) * 1.0 / cellSize);
            FindLegalMoves();
            Refresh();
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            col = (int)Math.Floor((e.X - x) * 1.0 / cellSize);
            row = (int)Math.Floor((e.Y - y) * 1.0 / cellSize);
            if ((col < 8 && col >= 0) && (row < 8 && row >= 0))
            {
                if (legals[col, row] > 0)
                {
                    Place(col, row);
                    turn++;
                }
            }
            Refresh();
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            
            UpdateSize();
            Refresh();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    Rectangle rect = new Rectangle(x + i * cellSize, y + j * cellSize, cellSize, cellSize);
                    Rectangle ellipse = new Rectangle(x + (i * cellSize) + cellSize / 8, y + (j * cellSize) + cellSize / 8, 3 * cellSize / 4, 3 * cellSize / 4);
                    if ((i % 2) + (j % 2) % 2 == 1) e.Graphics.FillRectangle(Brushes.LightGray, rect);
                    else e.Graphics.FillRectangle(Brushes.Gray, rect);
                    if (i == cursorCol && j == cursorRow) e.Graphics.FillRectangle(Brushes.Yellow, rect);
                    if (legals[i, j] > 0 && turn % 2 == 0) e.Graphics.DrawEllipse(Pens.DarkBlue, ellipse);
                    if (legals[i, j] > 0 && turn % 2 == 1) e.Graphics.DrawEllipse(Pens.DarkOrange, ellipse);
                    e.Graphics.DrawRectangle(Pens.Black, rect);
                    System.Drawing.Font font = new System.Drawing.Font("Ubuntu", cellSize / 4 * 100 / 96);
                    System.Drawing.StringFormat drawFormat = new System.Drawing.StringFormat();
                    if (board[i, j] == 0) e.Graphics.FillEllipse(Brushes.Blue, ellipse);
                    if (board[i, j] == 1) e.Graphics.FillEllipse(Brushes.Orange, ellipse);
                }
            }
        }
    }
}
