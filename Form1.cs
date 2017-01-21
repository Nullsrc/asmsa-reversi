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
        int blackScore = 0;
        int whiteScore = 0;
        string winner = "";
        int lastLegalMax;
        int[,] board = new int[8, 8] { { -1, -1, -1, -1, -1, -1, -1, -1 }, { -1, -1, -1, -1, -1, -1, -1, -1 }, 
                                       { -1, -1, -1, -1, -1, -1, -1, -1 }, { -1, -1, -1, 0, 1, -1, -1, -1 }, 
                                       { -1, -1, -1, 1, 0, -1, -1, -1 }, { -1, -1, -1, -1, -1, -1, -1, -1 }, 
                                       { -1, -1, -1, -1, -1, -1, -1, -1 }, { -1, -1, -1, -1, -1, -1, -1, -1 } };
        int[,] legals = new int[8, 8];
        int[] possible = new int[8];
        int numOfPossible;
        enum Flips { None, Down, Up, Right, Left, DownRight, DownLeft, UpRight, UpLeft }
        Pen blackPen = new Pen(Color.Black, (float)2.0);

        public Form1()
        {
            InitializeComponent();
            FindLegalMoves();
            Score();
            UpdateSize();
            DoubleBuffered = true;
            this.Size = new System.Drawing.Size(620, 784);
            this.MinimumSize = new System.Drawing.Size(620, 784);
            this.MaximumSize = new System.Drawing.Size(620, 784);
            this.BackColor = Color.FromKnownColor(KnownColor.BurlyWood);
        }

        private void UpdateSize()
        {
            cellSize = (ClientSize.Width - 2 * margin) / 8;
            y = margin;
            x = margin;
        }

        private void Score()
        {
            blackScore = 0;
            whiteScore = 0; 
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                    if (board[i, j] == 0) blackScore++;
                    else if (board[i, j] == 1) whiteScore++;
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

        private void CheckWinConditions()
        {
            if (board.Cast<int>().Min() == 0)
            {
                if (blackScore > whiteScore) winner = "Black is the winner!";
                else if (whiteScore > blackScore) winner = "White is the winner!";
                else winner = "Draw...";
            }
            else if (legals.Cast<int>().Max() == 0 && lastLegalMax != 0)
            {
                lastLegalMax = 0;
                turn++;
                return;
            }
            else if (legals.Cast<int>().Max() == 0 && lastLegalMax == 0)
            {
                if (blackScore > whiteScore) winner = "Black is the winner!";
                else if (whiteScore > blackScore) winner = "White is the winner!";
                else winner = "Draw...";
            }
            else
            {
                lastLegalMax = 1;
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            cursorCol = (int)Math.Floor((e.X - x) * 1.0 / cellSize);
            cursorRow = (int)Math.Floor((e.Y - y) * 1.0 / cellSize);
            Refresh();
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            CheckWinConditions();
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
            Score();
            FindLegalMoves();
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
            System.Drawing.Font font = new System.Drawing.Font("Ubuntu", cellSize / 2);
            System.Drawing.StringFormat drawFormat = new System.Drawing.StringFormat();
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    Rectangle rect = new Rectangle(x + i * cellSize, y + j * cellSize, cellSize, cellSize);
                    Rectangle ellipse = new Rectangle(x + (i * cellSize) + cellSize / 8, y + (j * cellSize) + cellSize / 8, 3 * cellSize / 4, 3 * cellSize / 4);
                    e.Graphics.FillRectangle(Brushes.DarkGreen, rect);
                    e.Graphics.DrawRectangle(blackPen, rect);
                    if (legals[i, j] > 0 && turn % 2 == 0) e.Graphics.DrawString("+", font, Brushes.Black, (i * cellSize) + x/2 + 1 + cellSize / 4, (j * cellSize) + y/3 + cellSize / 4);
                    if (legals[i, j] > 0 && turn % 2 == 1) e.Graphics.DrawString("+", font, Brushes.White, (i * cellSize) + x/2 + 1 + cellSize / 4, (j * cellSize) + y/3 + cellSize / 4);
                    if (i == cursorCol && j == cursorRow && turn % 2 == 0) e.Graphics.FillEllipse(Brushes.Black, ellipse);
                    if (i == cursorCol && j == cursorRow && turn % 2 == 1) e.Graphics.FillEllipse(Brushes.White, ellipse);
                    if (board[i, j] == 0) e.Graphics.FillEllipse(Brushes.Black, ellipse);
                    if (board[i, j] == 1) e.Graphics.FillEllipse(Brushes.White, ellipse);
                }
            }

            Rectangle BlackHUDEllipse = new Rectangle(x, 2 * y + 8 * cellSize, 3 * cellSize / 4, 3 * cellSize / 4);
            Rectangle WhiteHUDEllipse = new Rectangle(x + 4 * cellSize, 2 * y + 8 * cellSize, 3 * cellSize / 4, 3 * cellSize / 4);
            e.Graphics.FillEllipse(Brushes.Black, BlackHUDEllipse);
            e.Graphics.FillEllipse(Brushes.White, WhiteHUDEllipse);
            e.Graphics.DrawString(blackScore.ToString(), font, Brushes.Black, 2 * x + 17 * cellSize / 16, 2 * y + 8 * cellSize);
            e.Graphics.DrawString(whiteScore.ToString(), font, Brushes.White, 8 * x + 17 * cellSize / 4, 2 * y + 8 * cellSize);
            e.Graphics.DrawString(winner, font, Brushes.Black, x, 2 * y + 9 * cellSize);
        }
    }
}
