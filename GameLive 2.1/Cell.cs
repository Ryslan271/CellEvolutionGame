using System.Collections.Generic;
using System.Windows.Media;
using System.Windows.Shapes;
using static System.Net.Mime.MediaTypeNames;

namespace GameOfLife
{
    public class Cell
    {
        public Brush FillCell { get; set;}

        public Rectangle Rectangle { get; set; }

        public bool IsAlive { get; set; }

        public int X { get; set; }
        public int Y { get; set; }

        public List<Cell> Cells { get; set; } = new List<Cell>(8);

        public Cell(int x, int y, bool isAlive)
        {
            X = x;
            Y = y;
            IsAlive = isAlive;

            Rectangle = new Rectangle
            {
                Width = Properties.Settings.Default.CELL_SIZE,
                Height = Properties.Settings.Default.CELL_SIZE,
                StrokeThickness = Properties.Settings.Default.CELL_BORDER_STROKE_THICKNESS,
                Stroke = Brushes.Black
            };
        }
    }
}
