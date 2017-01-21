using System;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Reversi
{
    public partial class Form1 : Form
    {
        #region Variables
        int x;                  // Defines the width of the margin
        int y;                  // Defines the height of the margin
        int cellSize;           // Defines width and height of a cell
        int row;                // Used to determine where mouse clicks are
        int col;                // Used to determine where mouse clicks are
        int turn = 0;           // Used to alternate turns
        int cursorCol;          // Used to determine where the cursor is
        int cursorRow;          // Used to determine where the cursor is
        int blackScore = 0;     // Defines score of the black player
        int whiteScore = 0;     // Defines score of the white player
        string winner = "";     // Printed when a win condition is met.
        int lastLegalMax;       // Used to switch turns if one player does not have any possible moves
        int[,] board = new int[8, 8] { { -1, -1, -1, -1, -1, -1, -1, -1 }, { -1, -1, -1, -1, -1, -1, -1, -1 },  
       /* This is the defaault state */{ -1, -1, -1, -1, -1, -1, -1, -1 }, { -1, -1, -1, 0, 1, -1, -1, -1 },
       /* of the game board          */{ -1, -1, -1, 1, 0, -1, -1, -1 }, { -1, -1, -1, -1, -1, -1, -1, -1 },
                                       { -1, -1, -1, -1, -1, -1, -1, -1 }, { -1, -1, -1, -1, -1, -1, -1, -1 } };
        int[,] legals = new int[8, 8];  // Secondary board which tracks legal plays
        int[] possible = new int[8];    // Array which tracks all possible legal plays on a given cell
        int numOfPossible;              // Cross-function var that tracks number of non-zero values in possible[]
        enum Flips { None, Down, Up, Right, Left, DownRight, DownLeft, UpRight, UpLeft } // Easy enumeration (0-8) of all possible moves
        Pen blackPen = new Pen(Color.Black, (float)2.0);    // A black pen with 2x thickness
        Pen whitePen = new Pen(Color.White, (float)2.0);    // A white pen with 2x thickness
        #endregion

        #region Constructor
        public Form1()
        {
            InitializeComponent();      // Starts the form itself
            FindLegalMoves();           // Initial sweep of legal moves
            Score();                    // Take the initial score (2-2)
            UpdateSize();               // Set the sizes of the margins and cells
            DoubleBuffered = true;      // Drawing is smoother

            // These lock the size at 620x784
            this.Size = new System.Drawing.Size(620, 784);             
            this.MinimumSize = new System.Drawing.Size(620, 784);
            this.MaximumSize = new System.Drawing.Size(620, 784);

            this.BackColor = Color.FromKnownColor(KnownColor.BurlyWood);    // Sets a light background color
        } 

        /* void UpdateSize() ~ Sets the size of the cell and margins */
        private void UpdateSize()
        {
            cellSize = (ClientSize.Width - 2 * 10) / 8;
            y = 10;
            x = 10;
        }
        #endregion

        #region Legality and Flip Generation

        /* void FindLegalMoves() ~ Scans the board in all directions to locate
         *                         possible legal moves for the current player */
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
                            legals[i, j] = legals[i, j] * 10 + (int)Flips.DownRight;
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

        /* void GenerateAllFlips(int, int) ~ Given a cell's coordinates, access
         *                                   the concurrent legals[,] value and
         *                                   determine all legal moves          */
        private void GenerateAllFlips(int col, int row)
        {
            numOfPossible = legals[col, row].ToString().Length;
            for (int i = 0; i < numOfPossible; i++)
            {
                possible[i] = legals[col, row] % 10;
                legals[col, row] = legals[col, row] / 10;
            }
        }
        #endregion

        #region Placement
        
        /* void Place(int, int) ~ Given a cell's coordinates, generate all legal
         *                        moves and apply them                          */
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
        #endregion

        #region Scoring
        
        /* void CheckWinConditions() ~ Look for certain properties of the board
                                       to determine if a player has won         */ 
        private void CheckWinConditions()
        {
            if (board.Cast<int>().Min() == 0)   // If the board is full
            {
                if (blackScore > whiteScore) winner = "Black is the winner!";
                else if (whiteScore > blackScore) winner = "White is the winner!";
                else winner = "Draw...";
            }
            else if (legals.Cast<int>().Max() == 0 && lastLegalMax != 0)    // If the board is not full, but the current player cannot play
            {
                lastLegalMax = 0;
                turn++;
                return;
            }
            else if (legals.Cast<int>().Max() == 0 && lastLegalMax == 0)    // If the board is not full, but neither player can play
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

        /* void Score ~ Tally values of board[,] */
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
        #endregion

        #region Base Overrides
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
        #endregion

        #region Painting Override
        protected override void OnPaint(PaintEventArgs e)
        {
            // Basic fonting options
            System.Drawing.Font font = new System.Drawing.Font("Ubuntu", cellSize / 2);
            System.Drawing.Font boardFont = new System.Drawing.Font("Courier New", cellSize / 4);
            System.Drawing.StringFormat drawFormat = new System.Drawing.StringFormat();

            // These for loops iterate over the screen and generate the game board
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
/* Square Def   */  Rectangle rect = new Rectangle(x + i * cellSize, y + j * cellSize, cellSize, cellSize);
/* Piece Def    */  Rectangle ellipse = new Rectangle(x + (i * cellSize) + cellSize / 8, y + (j * cellSize) + cellSize / 8, 3 * cellSize / 4, 3 * cellSize / 4);
/* Draw Board   */  e.Graphics.FillRectangle(Brushes.DarkGreen, rect);
/* Draw Tiles   */  e.Graphics.DrawRectangle(blackPen, rect);
/* Draw Legals  */  if (legals[i, j] > 0 && turn % 2 == 0) e.Graphics.DrawString("+" + legals[i, j].ToString().Length, boardFont, Brushes.Black, (i * cellSize) + x + cellSize / 4, (j * cellSize) + y + cellSize / 3);
/* Draw Legals  */  if (legals[i, j] > 0 && turn % 2 == 1) e.Graphics.DrawString("+" + legals[i, j].ToString().Length, boardFont, Brushes.White, (i * cellSize) + x + cellSize / 4, (j * cellSize) + y + cellSize / 3);
/* Draw Cursor  */  if (i == cursorCol && j == cursorRow && turn % 2 == 0) e.Graphics.DrawEllipse(blackPen, ellipse);
/* Draw Cursor  */  if (i == cursorCol && j == cursorRow && turn % 2 == 1) e.Graphics.DrawEllipse(whitePen, ellipse);
/* Draw Pieces  */  if (board[i, j] == 0) e.Graphics.FillEllipse(Brushes.Black, ellipse);
/* Draw Pieces  */  if (board[i, j] == 1) e.Graphics.FillEllipse(Brushes.White, ellipse);
                }
            }

            // Draw the HUD elements.
            Rectangle BlackHUDEllipse = new Rectangle(x, 2 * y + 8 * cellSize, 3 * cellSize / 4, 3 * cellSize / 4);
            Rectangle WhiteHUDEllipse = new Rectangle(x + 4 * cellSize, 2 * y + 8 * cellSize, 3 * cellSize / 4, 3 * cellSize / 4);
            e.Graphics.FillEllipse(Brushes.Black, BlackHUDEllipse);
            e.Graphics.FillEllipse(Brushes.White, WhiteHUDEllipse);
            e.Graphics.DrawString(blackScore.ToString(), font, Brushes.Black, 2 * x + 17 * cellSize / 16, 2 * y + 8 * cellSize);
            e.Graphics.DrawString(whiteScore.ToString(), font, Brushes.White, 8 * x + 17 * cellSize / 4, 2 * y + 8 * cellSize);
            e.Graphics.DrawString(winner, font, Brushes.Black, x, 2 * y + 9 * cellSize);
        } 
        #endregion
    }
}
