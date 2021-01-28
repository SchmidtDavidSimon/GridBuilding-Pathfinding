using System;
using System.Collections.Generic;
using System.Linq;

namespace Grid
{
    /// <summary>
    /// Source https://github.com/lordjesus/Packt-Introduction-to-graph-algorithms-for-game-developers
    /// Altered by David Schmidt
    /// </summary>
    public class GridSearch {

        public struct SearchResult
        {
            public List<Point> Path { get; private set; }
        }

        public static List<Point> AStarSearch(Grid grid, Point startPosition, Point endPosition, CellType cellType, bool isAgent = false)
        {
            var path = new List<Point>();

            var positionsToCheck = new List<Point>();
            var costDictionary = new Dictionary<Point, float>();
            var priorityDictionary = new Dictionary<Point, float>();
            var parentsDictionary = new Dictionary<Point, Point>();

            positionsToCheck.Add(startPosition);
            priorityDictionary.Add(startPosition, 0);
            costDictionary.Add(startPosition, 0);
            parentsDictionary.Add(startPosition, null);

            while (positionsToCheck.Count > 0)
            {
                var currentPoint = GetClosestVertex(positionsToCheck, priorityDictionary);
                positionsToCheck.Remove(currentPoint);
                if (currentPoint.Equals(endPosition))
                {
                    path = GeneratePath(parentsDictionary, currentPoint);
                    return path;
                }

                foreach (var neighbour in grid.GetAdjacentCellsForAStar(currentPoint.X,currentPoint.Y, cellType, isAgent))
                {
                    var newCost = costDictionary[currentPoint] + grid.GetCostOfEnteringCell(neighbour);
                    if (costDictionary.ContainsKey(neighbour) && !(newCost < costDictionary[neighbour])) continue;
                    costDictionary[neighbour] = newCost;

                    var priority = newCost + ManhattanDistance(endPosition, neighbour);
                    positionsToCheck.Add(neighbour);
                    priorityDictionary[neighbour] = priority;

                    parentsDictionary[neighbour] = currentPoint;
                }
            }
            return path;
        }

        private static Point GetClosestVertex(List<Point> list, Dictionary<Point, float> distanceMap)
        {
            var candidate = list[0];
            foreach (var vertex in list.Where(vertex => distanceMap[vertex] < distanceMap[candidate]))
            {
                candidate = vertex;
            }
            return candidate;
        }

        private static float ManhattanDistance(Point endPos, Point point)
        {
            return Math.Abs(endPos.X - point.X) + Math.Abs(endPos.Y - point.Y);
        }

        public static List<Point> GeneratePath(Dictionary<Point, Point> parentMap, Point endState)
        {
            var path = new List<Point>();
            var parent = endState;
            while (parent != null && parentMap.ContainsKey(parent))
            {
                path.Add(parent);
                parent = parentMap[parent];
            }
            return path;
        }
    }
}
