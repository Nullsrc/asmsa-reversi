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

        public Form1()
        {
            InitializeComponent();
            UpdateSize();
            DoubleBuffered = true;
            this.Size = new System.Drawing.Size(620, 620);
            this.MinimumSize = new System.Drawing.Size(220, 220);
            this.MaximumSize = new System.Drawing.Size(1020, 1020);
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            UpdateSize();
            Refresh();
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

        protected override void OnPaint(PaintEventArgs e)
        {
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    Rectangle rect = new Rectangle(x + i * cellSize, y + j * cellSize, cellSize, cellSize);
                    if ((i % 2) + (j % 2) % 2 == 1)
                    {
                        e.Graphics.FillRectangle(Brushes.LightGray, rect);
                    }
                    else e.Graphics.FillRectangle(Brushes.Gray, rect);
                    e.Graphics.DrawRectangle(Pens.Black, rect);
                    System.Drawing.Font font = new System.Drawing.Font("Ubuntu", cellSize / 4 * 100 / 96);
                    System.Drawing.StringFormat drawFormat = new System.Drawing.StringFormat();
                }
            }
        }
    }
}
