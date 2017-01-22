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
       /* This is the defaault state */{ -1, -1, -1, -1, -1, -1, -1, -1 }, { -1, -1, -1, 1, 0, -1, -1, -1 },
       /* of the game board          */{ -1, -1, -1, 0, 1, -1, -1, -1 }, { -1, -1, -1, -1, -1, -1, -1, -1 },
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
            InitializeComponent();      // Starts the form
            Array.Clear(legals, 0, legals.Length);  // Clear the legal array before scanning
            for (int i = 0; i < 8; i++) for (int j = 0; j < 8; j++) Scan(i, j);           // Initial sweep of legal moves
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

        /* void Scan() ~ Scans the point in all directions to locate possible
         *               legal moves for the current player                    */
        private void Scan(int col, int row)
        {
            for (int j = 1; j + row < 8; j++)
            {
                if (board[col, row + j] == turn % 2 || board[col, row + j] == -1) break;
                else if (board[col, row + j] != turn % 2 && board[col, row + j] != -1)
                    for (int k = j + row + 1; k < 8; k++)
                        if (board[col, k] == turn % 2)
                        { legals[col, row] = (int)Flips.Down; break; }
            }
            for (int j = 1; row - j >= 0; j++)
            {
                if (board[col, row - j] == turn % 2 || board[col, row - j] == -1) break;
                else if (board[col, row - j] != turn % 2 && board[col, row - j] != -1)
                    for (int k = row - j - 1; k >= 0; k--)
                        if (board[col, k] == turn % 2)
                        { legals[col, row] = 10 * legals[col, row] + (int)Flips.Up; break; }
            }
            for (int i = 1; col + i < 8; i++)
            {
                if (board[col + i, row] == turn % 2 || board[col + i, row] == -1) break;
                else if (board[col + i, row] != turn % 2 && board[col + i, row] != -1)
                    for (int k = col + i + 1; k < 8; k++)
                        if (board[k, row] == turn % 2)
                        { legals[col, row] = 10 * legals[col, row] + (int)Flips.Right; break; }
            }
            for (int i = 1; col - i >= 0; i++)
            {
                if (board[col - i, row] == turn % 2 || board[col - i, row] == -1) break;
                else if (board[col - i, row] != turn % 2 && board[col - i, row] != -1)
                    for (int k = col - i - 1; k >= 0; k--)
                        if (board[k, row] == turn % 2) legals[col, row] = 10 * legals[col, row] + (int)Flips.Left;
                        else break;
            }
            for (int j = 1; j + row < 8; j++)
            {
                if (j + col > 7 || board[col + j, row + j] == turn % 2 || board[col + j, row + j] == -1) break;
                else if (board[col + j, row + j] != turn % 2 && board[col + j, row + j] != -1)
                    for (int k = j + 1; row + k < 8; k++)
                        if (col + k < 8 && board[col + k, row + k] == turn % 2)
                        { legals[col, row] = 10 * legals[col, row] + (int)Flips.DownRight; break; }
            }
            for (int j = 1; j + row < 8; j++)
            {
                if (col - j < 0 || board[col - j, row + j] == turn % 2 || board[col - j, row + j] == -1) break;
                else if (board[col - j, row + j] != turn % 2 && board[col - j, row + j] != -1)
                    for (int k = j + 1; row + k < 8; k++)
                        if (col - k >= 0 && board[col - k, row + k] == turn % 2)
                        { legals[col, row] = 10 * legals[col, row] + (int)Flips.DownLeft; break; }
            }
            for (int j = 1; row - j >= 0; j++)
            {
                if (j + col > 7 || board[col + j, row - j] == turn % 2 || board[col + j, row - j] == -1) break;
                else if (board[col + j, row - j] != turn % 2 && board[col + j, row - j] != -1)
                    for (int k = j + 1; row - k >= 0; k++)
                        if (col + k < 8 && board[col + k, row - k] == turn % 2)
                        { legals[col, row] = 10 * legals[col, row] + (int)Flips.UpRight; break; }
            }
            for (int j = 1; row - j >= 0; j++)
            {
                if (col - j < 0 || board[col - j, row - j] == turn % 2 || board[col - j, row - j] == -1) break;
                else if (board[col - j, row - j] != turn % 2 && board[col - j, row - j] != -1)
                    for (int k = j + 1; row - k >= 0; k++)
                        if (col - k >= 0 && board[col - k, row - k] == turn % 2)
                        { legals[col, row] = 10 * legals[col, row] + (int)Flips.UpLeft; break; }
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
                if (possible[i] == (int)Flips.Down) FlipInDirection(Flips.Down, col, row);
                if (possible[i] == (int)Flips.Up) FlipInDirection(Flips.Up, col, row);
                if (possible[i] == (int)Flips.Right) FlipInDirection(Flips.Right, col, row);
                if (possible[i] == (int)Flips.Left) FlipInDirection(Flips.Left, col, row);
                if (possible[i] == (int)Flips.DownRight) FlipInDirection(Flips.DownRight, col, row);
                if (possible[i] == (int)Flips.DownLeft) FlipInDirection(Flips.DownLeft, col, row);
                if (possible[i] == (int)Flips.UpRight) FlipInDirection(Flips.UpRight, col, row);
                if (possible[i] == (int)Flips.UpLeft) FlipInDirection(Flips.UpLeft, col, row);
            }
        }

        private void FlipInDirection(Flips dir, int col, int row)
        {
            if (dir == Flips.Down)
            {
                for (int j = 1; j + row < 8; j++)
                {
                    if (board[col, j + row] == turn % 2) break;
                    if (board[col, j + row] != turn % 2 && board[col, j + row] != -1) board[col, j + row] = turn % 2;
                } 
            }
            if (dir == Flips.Up)
            {
                for (int j = 1; row - j >= 0; j++)
                {
                    if (board[col, row - j] == turn % 2) break;
                    if (board[col, row - j] != turn % 2 && board[col, row - j] != -1) board[col, row - j] = turn % 2;
                }
            }
            if (dir == Flips.Right)
            {
                for (int i = 1; i + col < 8; i++)
                {
                    if (board[col + i, row] == turn % 2) break;
                    if (board[col + i, row] != turn % 2 && board[col + i, row] != -1) board[col + i, row] = turn % 2;
                }
            }
            if (dir == Flips.Left)
            {
                for (int i = 1; col - i >= 0; i++)
                {
                    if (board[col - i, row] == turn % 2) break;
                    if (board[col - i, row] != turn % 2 && board[col - i, row] != -1) board[col - i, row] = turn % 2;
                }
            }
            if (dir == Flips.DownRight)
            {
                for (int i = 1; i + row < 8; i++)
                {
                    if (col + i > 7 || board[col + i, row + i] == turn % 2) break;
                    if (board[col + i, row + i] != turn % 2 && board[col + i, row + i] != -1) board[col + i, row + i] = turn % 2;
                }
            }
            if (dir == Flips.DownLeft)
            {
                for (int i = 1; i + row < 8; i++)
                {
                    if (col - i < 0 || board[col - i, row + i] == turn % 2) break;
                    if (board[col - i, row + i] != turn % 2 && board[col - i, row + i] != -1) board[col - i, row + i] = turn % 2;
                }
            }
            if (dir == Flips.UpRight)
            {
                for (int i = 1; row - i >= 0; i++)
                {
                    if (col + i > 7 || board[col + i, row - i] == turn % 2) break;
                    if (board[col + i, row - i] != turn % 2 && board[col + i, row - i] != -1) board[col + i, row - i] = turn % 2;
                }
            }
            if (dir == Flips.UpLeft)
            {
                for (int i = 1; row - i >= 0; i++)
                {
                    if (col - i < 0 || board[col - i, row - i] == turn % 2) break;
                    if (board[col - i, row - i] != turn % 2 && board[col - i, row - i] != -1) board[col - i, row - i] = turn % 2;
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
            Array.Clear(legals, 0, legals.Length);
            for (int i = 0; i < 8; i ++)
            {
                for (int j = 0; j < 8; j++) if (board[i,j] == -1) Scan(i, j);
            }
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
