using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = System.Random;

namespace Grid
{
    /// <summary>
    /// TODO: Move to C++
    /// </summary>
    public struct AStartTypeInfo
    {
        public Grid typeGrid;
        public int[] useableTypes;
    }
    
    public class Grid
    {
        public int Width { get; }
        public int Height { get; }
        public int DefaultValue { get; }
        
        public int OutOfBoundsValue { get; }
        
        private readonly int[,] _grid;

        public int this[int x, int y]
        {
            get => _grid[x, y];
            set => _grid[x, y] = value;
        }
        
        public Grid(int width, int height, int defaultValue = -1, int outOfBoundsValue = Int32.MinValue)
        {
            Width = width;
            Height = height;
            OutOfBoundsValue = outOfBoundsValue;
            DefaultValue = defaultValue;

            _grid = new int[width,height];
            for (var i = 0; i < width; i++)
            {
                for (var j = 0; j < height; j++)
                {
                    _grid[i, j] = defaultValue;
                }
            }
        }

        public bool IsPointSet(Point coord) => _grid[coord.X, coord.Y] != DefaultValue;

        public Point GetRandomPointOfType(int type)
        {
            var pointsOfType = new List<Point>();
            
            for (var i = 0; i < Width; i++)
            {
                for (var j = 0; j < Height; j++)
                {
                    if (_grid[i,j] != type) continue;
                    pointsOfType.Add(new Point(i,j));
                }
            }
            var rand = new Random();
            return pointsOfType[rand.Next(0, pointsOfType.Count - 1)];
        }

        public List<Point> GetAdjacentPoints(Point point)
        {
            var adjacentCells = new List<Point>();
            if (point.X > 0)
            {
                adjacentCells.Add(new Point(point.X - 1,point.Y));
            }
            if (point.X < Width -1)
            {
                adjacentCells.Add(new Point(point.X + 1, point.Y));
            }
            if (point.Y > 0)
            {
             adjacentCells.Add(new Point(point.X,point.Y - 1));   
            }
            if (point.Y < Height -1)
            {
                adjacentCells.Add(new Point(point.X,point.Y + 1));   
            }
            return adjacentCells;
        }

        public List<Point> GetAdjacentPointsOfType(Point point, int type)
        {
            var adjacentPoints = GetAdjacentPoints(point);
            for (var i = adjacentPoints.Count - 1; i >= 0; i--)
            {
                    if (PointIsOfType(point, type)) continue;
                    adjacentPoints.RemoveAt(i);
            }
            return adjacentPoints;
        }
        
        private List<Point> GetAdjacentPointsOfTypes(Point point, int[] acceptedTypes)
        {
            var adjacentPoints = GetAdjacentPoints(point);
            for (var i = adjacentPoints.Count - 1; i >= 0; i--)
            {
                var isOfType = acceptedTypes.Any(type => PointIsOfType(adjacentPoints[i], type));
                if (isOfType) continue;
                adjacentPoints.RemoveAt(i);
            }
            return adjacentPoints;
        }

        private bool PointIsOfType(Point point, int type) => _grid[point.X, point.Y] == type;
        
        /// <summary>
        ///  
        /// </summary>
        /// <param name="point"> Point to get neighbors of</param>
        /// <returns>Array [Left neighbor, Top neighbor, Right neighbor, Down neighbor]</returns>
        public int[] GetAdjacentPointTypes(Point point)
        {
            var adjacentTypes = new [] {OutOfBoundsValue, OutOfBoundsValue, OutOfBoundsValue, OutOfBoundsValue};
            if (point.X > 0)
            {
                adjacentTypes[0] = _grid[point.X - 1, point.Y];
            }
            if (point.Y < Height - 1)
            {
                adjacentTypes[1] = _grid[point.X, point.Y + 1];
            }
            if (point.X < Width - 1)
            {
                adjacentTypes[2] = _grid[point.X + 1, point.Y];
            }
            if (point.Y > 0)
            {
                adjacentTypes[3] = _grid[point.X, point.Y - 1];
            }
            

            return adjacentTypes;
        }

        public List<Point> AStarSearchWithoutCost(Point startPoint, Point endPoint, AStartTypeInfo? typeInfo = null) =>
            AStarSearch(startPoint, endPoint, false, typeInfo);

        public List<Point> AStarSearchWithCost(Point startPoint, Point endPoint, AStartTypeInfo? typeInfo = null)
            => AStarSearch(startPoint, endPoint, true, typeInfo);

        private List<Point> AStarSearch(Point startPoint, Point endPoint, bool useCost, AStartTypeInfo? typeInfo = null)
        {
            var path = new List<Point>();

            var pointsToCheck = new List<Point>();
            var costDictionary = new Dictionary<Point,int>();
            var priorityDictionary = new Dictionary<Point, float>();
            var parentsDictionary = new Dictionary<Point,Point>();
            
            pointsToCheck.Add(startPoint);
            costDictionary.Add(startPoint, 0);
            priorityDictionary.Add(startPoint, 0);
            parentsDictionary.Add(startPoint, null);

            while (pointsToCheck.Count > 0)
            {
                var currentPoint = GetClosestPoint(pointsToCheck, priorityDictionary);
                pointsToCheck.Remove(currentPoint);
                Debug.Log(currentPoint.ToString() + endPoint);
                if (currentPoint.Equals(endPoint))
                {
                    path = GeneratePath(parentsDictionary, currentPoint);
                    return path;
                }

                var neighbors = typeInfo == null 
                    ? GetAdjacentPoints(currentPoint) 
                    : typeInfo.Value.typeGrid.GetAdjacentPointsOfTypes(currentPoint, typeInfo.Value.useableTypes);
                foreach (var neighbor in neighbors)
                {
                    var newCost = useCost 
                        ? costDictionary[currentPoint] + _grid[neighbor.X, neighbor.Y]
                        : 0;
                    if (costDictionary.ContainsKey(neighbor) && !(newCost < costDictionary[neighbor])) continue;
                    costDictionary[neighbor] = newCost;

                    var priority = newCost + ManhattanDistance(neighbor, endPoint);
                    pointsToCheck.Add(neighbor);
                    priorityDictionary[neighbor] = priority;

                    parentsDictionary[neighbor] = currentPoint;
                }
            }
            return path;
        }

        private float ManhattanDistance(Point point, Point endPoint) =>
            Math.Abs(endPoint.X - point.X) + Math.Abs(endPoint.Y - point.Y);

        private Point GetClosestPoint(List<Point> pointsToCheck, Dictionary<Point, float> distanceMap)
        {
            var candidate = pointsToCheck[0];
            foreach (var point in pointsToCheck.Where(point => distanceMap[point] < distanceMap[candidate]))
            {
                candidate = point;
            }

            return candidate;
        }
        
        private List<Point> GeneratePath(Dictionary<Point,Point> parentsDictionary, Point endPoint)
        {
            var path = new List<Point>();
            var parent = endPoint;
            while (parent != null && parentsDictionary.ContainsKey(parent))
            {
                path.Add(parent);
                parent = parentsDictionary[parent];
            }

            return path;
        }
    }
}
