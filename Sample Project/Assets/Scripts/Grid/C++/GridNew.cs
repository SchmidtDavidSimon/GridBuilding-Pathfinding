using System;
using System.Collections.Generic;

namespace Grid
{
    /// <summary>
    /// TODO: Move to C++
    /// </summary>
    public class GridNew
    {
        public int Width { get; }
        public int Height { get; }
        
        private readonly bool[,] _grid;
        private readonly List<Point> _setPoints = new List<Point>();

        public bool this[int x, int y]
        {
            get => _grid[x, y];
            set
            {
                if (value)
                {
                    _setPoints.Add(new Point(x,y));
                }
                else
                {
                    _setPoints.Remove(new Point(x, y));
                }
                _grid[x, y] = value;
            } 
                
        }
        
        public GridNew(int width, int height)
        {
            Width = width;
            Height = height;

            _grid = new bool[width,height];
        }

        public bool IsCellSet(Point coord) => _grid[coord.X, coord.Y];

        public Point GetRandomSetPoint()
        {
            var rand = new Random();
            return _setPoints[rand.Next(0, _setPoints.Count - 1)];
        }

        public List<Point> GetAdjacentCells(Point cell)
        {
            var adjacentCells = new List<Point>();
            if (cell.X > 0)
            {
                adjacentCells.Add(new Point(cell.X - 1,cell.Y));
            }
            if (cell.X < Width -1)
            {
                adjacentCells.Add(new Point(cell.X + 1, cell.Y));
            }
            if (cell.Y > 0)
            {
             adjacentCells.Add(new Point(cell.X,cell.Y - 1));   
            }
            if (cell.Y < Height -1)
            {
                adjacentCells.Add(new Point(cell.X,cell.Y + 1));   
            }
            return adjacentCells;
        }
    }
}
