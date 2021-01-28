using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Grid
{
    public enum CellType
    {
        Empty,
        Street,
        House,
        Special,
        None
    }
    
    public class GridExtension 
    {
        private static Grid _placementGrid;
        public GridExtension(int width, int height)
        {
            _placementGrid = new Grid(width, height);
        }

        public static bool PositionIsInBound(Vector3Int position) => position.x >= 0 && position.x < _placementGrid.Width &&
                                                                     position.z >= 0 && position.z < _placementGrid.Height;

        public static bool PositionIsFreeAndInBound(Vector3Int position) =>
            PositionIsFree(position) && PositionIsInBound(position);
        public static bool PositionIsFree(Vector3Int position) => CheckIfPositionIsOfType(position, CellType.Empty);

        public static bool PositionIsFreeOrOfType(Vector3Int position, CellType type) =>
            PositionIsFree(position) || PositionIsOfType(position, type);

        private static bool PositionIsOfType(Vector3Int position, CellType type) =>
            _placementGrid[position.x, position.z] == type;

        private static bool CheckIfPositionIsOfType(Vector3Int position, CellType cellType) => _placementGrid[position.x, position.z] == cellType;

        public static CellType[] GetNeighborTypes(Vector3Int pos) =>
            _placementGrid.GetAllAdjacentCellTypes(pos.x, pos.z);

        public static List<Vector3Int> GetNeighborsOfType(Vector3Int pos, CellType type)
        {
            var neighborPoints = _placementGrid.GetAdjacentCellsOfType(pos.x, pos.z, type);

            return neighborPoints.Select(point => new Vector3Int(point.X, 0, point.Y)).ToList();
        }

        public static List<Vector3Int> GetPathOfTypeBetween(Vector3Int startPos, Vector3Int endPos, CellType type)
        {
            var resultPath = GridSearch.AStarSearch(_placementGrid, new Point(startPos.x, startPos.z),
                new Point(endPos.x, endPos.z), type);
            return resultPath.Select(point => new Vector3Int(point.X, 0, point.Y)).ToList();
        }

        public static void SetGridCell(Point point, CellType cellType) => _placementGrid[point.X, point.Y] = cellType;
    }
}
    