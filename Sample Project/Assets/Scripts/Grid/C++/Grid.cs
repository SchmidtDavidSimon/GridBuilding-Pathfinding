using System;
using System.Collections.Generic;

namespace Grid
{
    public class Grid
    {
        private readonly CellType[,] _grid;
        
        
        
        private readonly int _width;
        private readonly int _height;
        public int Width => _width;
        public int Height => _height;

        private List<Point> _roadList = new List<Point>();
        private List<Point> _specialStructure = new List<Point>();

        public Grid(int width, int height)
        {
            _width = width;
            _height = height;
            _grid = new CellType[width, height];
        }

        // Adding index operator to our Grid class so that we can use grid[][] to access specific cell from our grid. 
        public CellType this[int i, int j]
        {
            get => _grid[i, j];
            set
            {
                if (value == CellType.Street)
                {
                    _roadList.Add(new Point(i, j));
                }
                else
                {
                    _roadList.Remove(new Point(i, j));
                }
                if (value == CellType.Special)
                {
                    _specialStructure.Add(new Point(i, j));
                }
                else
                {
                    _specialStructure.Remove(new Point(i, j));
                }
                _grid[i, j] = value;
            }
        }

        private static bool IsCellWakable(CellType cellType, bool aiAgent = false)
        {
            if (aiAgent) return cellType == CellType.Street;
        
            return cellType == CellType.Empty || cellType == CellType.Street;
        }

        public Point GetRandomRoadPoint()
        {
            var rand = new Random();
            return _roadList[rand.Next(0, _roadList.Count - 1)];
        }

        public Point GetRandomSpecialStructurePoint()
        {
            var rand = new Random();
            return _roadList[rand.Next(0, _roadList.Count - 1)];
        }

        public List<Point> GetAdjacentCells(Point cell, bool isAgent)
        {
            return GetWalkableAdjacentCells(cell.X, cell.Y, isAgent);
        }

        public float GetCostOfEnteringCell(Point cell)
        {
            return 1;
        }

        private List<Point> GetAllAdjacentCells(int x, int y)
        {
            var adjacentCells = new List<Point>();
            if (x > 0)
            {
                adjacentCells.Add(new Point(x - 1, y));
            }
            if (x < _width - 1)
            {
                adjacentCells.Add(new Point(x + 1, y));
            }
            if (y > 0)
            {
                adjacentCells.Add(new Point(x, y - 1));
            }
            if (y < _height - 1)
            {
                adjacentCells.Add(new Point(x, y + 1));
            }
            return adjacentCells;
        }

        private List<Point> GetWalkableAdjacentCells(int x, int y, bool isAgent)
        {
            var adjacentCells = GetAllAdjacentCells(x, y);
            for (var i = adjacentCells.Count - 1; i >= 0; i--)
            {
                if(IsCellWakable(_grid[adjacentCells[i].X, adjacentCells[i].Y], isAgent)==false)
                {
                    adjacentCells.RemoveAt(i);
                }
            }
            return adjacentCells;
        }

        public List<Point> GetAdjacentCellsOfType(int x, int y, CellType type)
        {
            var adjacentCells = GetAllAdjacentCells(x, y);
            for (var i = adjacentCells.Count - 1; i >= 0; i--)
            {
                if (_grid[adjacentCells[i].X, adjacentCells[i].Y] != type)
                {
                    adjacentCells.RemoveAt(i);
                }
            }
            return adjacentCells;
        }
        
        public List<Point> GetAdjacentCellsForAStar(int x, int y, CellType cellType, bool isAgent = false)
        {
            if (isAgent) return GetWalkableAdjacentCells(x, y, true);
            var adjacentCells = GetAllAdjacentCells(x, y);
            for (var i = adjacentCells.Count - 1; i >= 0; i--)
            {
                if (_grid[adjacentCells[i].X, adjacentCells[i].Y] == cellType) continue;
                if (_grid[adjacentCells[i].X, adjacentCells[i].Y] == CellType.Empty) continue;
                
                adjacentCells.RemoveAt(i);
            }

            return adjacentCells;
        }

        /// <summary>
        /// Returns array [Left neighbour, Top neighbour, Right neighbour, Down neighbour]
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public CellType[] GetAllAdjacentCellTypes(int x, int y)
        {
            CellType[] neighbours = { CellType.None, CellType.None, CellType.None, CellType.None };
            if (_grid == null) return neighbours;
            if (x > 0)
            {
                neighbours[0] = _grid[x - 1, y];
            }
            if (x < _width - 1)
            {
                neighbours[2] = _grid[x + 1, y];
            }
            if (y > 0)
            {
                neighbours[3] = _grid[x, y - 1];
            }
            if (y < _height - 1)
            {
                neighbours[1] = _grid[x, y + 1];
            }
            return neighbours;
        }
    }
}