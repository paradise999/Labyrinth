using System.Collections.Generic;
using System.Linq;
using System.Drawing;

namespace WindowsFormsApplication1
{
    class Labyrinth
    {

        public Labyrinth(int[,] arr) : this(arr, new Point(0, 0), new Size(30, 30))
        { }

        public Labyrinth(int[,] arr, Point location) : this(arr, location, new Size(30, 30))
        { }

        public Labyrinth(int[,] arr, Point location, Size cellsize)
        {
            cells = FromIntArray(arr);
            LabyrinthCell.Size = cellsize;
            Location = location;
        }
        private Point location;
        public Point Location
        {
            get { return location; }
            set
            {
                location = value;
                for (int i = 0; i < cells.GetLength(0); i++)
                    for (int j = 0; j < cells.GetLength(1); j++)
                        cells[i, j].Location = new Point(this.Location.X + i * LabyrinthCell.Size.Width, this.Location.Y + j * LabyrinthCell.Size.Height);
            }
        }

        private List<EmptyCell> SelectedCells = new List<EmptyCell>();
        private List<Point> CrossedPoints = new List<Point>();
        public Size CellSize { get { return LabyrinthCell.Size; } set { LabyrinthCell.Size = value; Location = location; } }

        public Size Size { get { return new Size(LabyrinthCell.Size.Width * cells.GetLength(0), LabyrinthCell.Size.Height * cells.GetLength(1)); } }

        public Rectangle Rect { get { return new Rectangle(Location, Size); } }


        public bool SetPath()
        {
            if (SelectedCells.Count != 2) return false;
            foreach (EmptyCell cl in AllCells().Where(x => (typeof(EmptyCell) == x.GetType()))) cl.Crossed = false;
            CrossedPoints.Clear();
            bool result = SetPathRecursive(new List<EmptyCell>(), SelectedCells[0], SelectedCells[1]);
            if (result) CrossedPoints.Add(new Point(SelectedCells[0].Location.X + LabyrinthCell.Size.Width / 2, SelectedCells[0].Location.Y + LabyrinthCell.Size.Height / 2));
            else CrossedPoints.Clear();
            return result;
        }
        public bool SetPathRecursive(List<EmptyCell> used, EmptyCell current, EmptyCell end)
        {
            if (current == end) return true;
            used.Add(current);
            foreach (EmptyCell cell in current.Connections)
                if (!used.Contains(cell))
                    if (SetPathRecursive(used, cell, end))
                    {
                        cell.Crossed = true;
                        CrossedPoints.Add(new Point(cell.Location.X + LabyrinthCell.Size.Width / 2, cell.Location.Y + LabyrinthCell.Size.Height / 2));
                        return true;
                    }
            return false;
        }


        public bool ClickAndSetPath(Point p)
        {
            EmptyCell cell = (EmptyCell)AllCells().FirstOrDefault(x => x.Rect.Contains(p) && x.GetType() == typeof(EmptyCell));
            if (cell != null)
            {
                if (cell.Selected) { SelectedCells.Remove(cell); CrossedPoints.Clear(); }
                else { if (SelectedCells.Count > 1) return true; SelectedCells.Add(cell); }
                cell.Selected = !cell.Selected;
                if (SelectedCells.Count == 2)
                {
                    bool res = SetPath();
                    if (!res) { SelectedCells.Remove(cell); cell.Selected = false; }
                    return res;
                }
            }
            return true;
        }
        private IEnumerable<LabyrinthCell> AllCells()
        {
            for (int i = 0; i < cells.GetLength(0); i++)
                for (int j = 0; j < cells.GetLength(1); j++)
                    yield return cells[i, j];
        }
        private LabyrinthCell[,] cells;
        private LabyrinthCell[,] FromIntArray(int[,] arr)
        {
            LabyrinthCell[,] res = new LabyrinthCell[arr.GetLength(0), arr.GetLength(1)];
            int m = arr.GetLength(0);
            int n = arr.GetLength(1);
            for (int i = 0; i < m; i++)
                for (int j = 0; j < n; j++)
                {
                    if (res[i, j] == null)
                    {
                        if (arr[i, j] == 0)
                        {
                            res[i, j] = new EmptyCell();
                        }
                        else { res[i, j] = new FilledCell(); continue; }
                    }
                    else if (typeof(EmptyCell) != res[i, j].GetType()) continue;
                    var cell = (EmptyCell)res[i, j];
                    cell.Location = new Point(this.Location.X + i * LabyrinthCell.Size.Width, this.Location.Y + j * LabyrinthCell.Size.Height);
                    foreach (Point p in patternPoints)
                    {
                        var current = new Point(i, j);
                        current.Offset(p.X, p.Y);
                        if (new Rectangle(0, 0, m, n).Contains(current))
                        {
                            if (arr[current.X, current.Y] == 0)
                            {
                                if (res[current.X, current.Y] == null)
                                {
                                    var newcell = new EmptyCell();
                                    res[current.X, current.Y] = newcell;
                                    cell.Connections.Add(newcell);
                                }
                                else cell.Connections.Add((EmptyCell)res[current.X, current.Y]);
                            }
                            else { if (res[current.X, current.Y] == null) res[current.X, current.Y] = new FilledCell(); }
                        }
                    }
                }
            return res;
        }

        private static Point[] patternPoints = new Point[] {
            new Point(-1, 0),
            new Point(0, -1),
            new Point(1, 0),
            new Point(0, 1),
            };
        public void Draw(Graphics g)
        {
            foreach (LabyrinthCell cl in AllCells()) cl.Draw(g);
            if (CrossedPoints.Count > 0)
                g.DrawLines(Pens.Red, CrossedPoints.ToArray());
        }

        public abstract class LabyrinthCell
        {
            public Rectangle Rect { get { return new Rectangle(Location, Size); } }
            public static Size Size { get; set; } = new Size(20, 20);
            public Point Location { get; set; }
            public abstract void Draw(Graphics g);
        }

        public class EmptyCell : LabyrinthCell
        {
            public bool Selected { get; set; } = false;
            public bool Crossed { get; set; } = false;
            public override void Draw(Graphics g)
            {
                if (Selected) g.FillRectangle(Brushes.Pink, this.Rect);
            }
            public List<EmptyCell> Connections = new List<EmptyCell>();
        }

        public class FilledCell : LabyrinthCell
        {
            public override void Draw(Graphics g)
            {
                g.FillRectangle(Brushes.Blue, new Rectangle(Location, Size));
            }
        }
    }
}