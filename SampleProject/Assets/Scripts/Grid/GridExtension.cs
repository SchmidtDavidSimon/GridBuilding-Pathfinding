using System.Collections.Generic;
using System.Linq;
using ScriptableObjects;
using UnityEngine;

namespace Grid
{
    public class GridExtension 
    {
        private static Grid _typeGrid;
        private static Grid _costGrid;
        private static Grid _appealGrid;
        public GridExtension(int width, int height)
        {
            _typeGrid = new Grid(width, height);
            _costGrid = new Grid(width, height, int.MaxValue);
            _appealGrid = new Grid(width,height,0);
        }
        
        #region public methods

        #region setter & getter

        public static int GridWidth => _typeGrid.Width; 
        public static int GridHeight => _typeGrid.Height;
        
        public static bool SetCell(Vector3Int cell, CellContent content)
        {
            for (var i = cell.x; i < cell.x + content.Width; i++)
            {
                for (var j = cell.z; j < cell.z + content.Height; j++)
                {
                    if (i >= _typeGrid.Width || j >= _typeGrid.Height) return false;
                    _typeGrid[i, j] = (int) content.Type;
                }
            }

            if (content.AllowsMovement)
            {
                for (var i = cell.x; i < cell.x + content.Width; i++)
                {
                    for (var j = cell.z; j < cell.z + content.Height; j++)
                    {
                        _costGrid[i, j] = content.MovementCost;
                    }
                }
            }

            if (content.HasAppeal)
            {
                for (var i = cell.x; i < cell.x + content.Width; i++)
                {
                    for (var j = cell.z; j < cell.z + content.Height; j++)
                    {
                        _appealGrid[i, j] = content.Appeal;
                    }
                }
            }

            return true;
        }

        public static CellContentType GetCellType(Vector3Int cell) => (CellContentType) _typeGrid[cell.x, cell.z];
        public static bool EmptyCell(Vector3Int pos)
        {
            if (!CellIsInBound(pos)) return false;
            _typeGrid[pos.x, pos.z] = _typeGrid.DefaultValue;
            _costGrid[pos.x, pos.z] = _costGrid.DefaultValue;
            _appealGrid[pos.x, pos.z] = -_appealGrid.DefaultValue;
            return true;
        }

        #endregion

        #region bool conditions

        public static bool CellIsInBound(Vector3Int position) => position.x >= 0 && position.x < _typeGrid.Width &&
                                                                 position.z >= 0 && position.z < _typeGrid.Height;
        
        public static bool CellIsOfType(Vector3Int position, CellContentType type) => _typeGrid[position.x, position.z] == (int) type;
        
        public static bool CellIsFree(Vector3Int position) =>
            _typeGrid[position.x, position.z] == _typeGrid.DefaultValue;

        #endregion

        #region neighbor stuff

        public static CellContentType[] GetNeighborTypes(Vector3Int pos)
        {
            var neighbors = _typeGrid.GetAdjacentPointTypes(Vector3ToPoint(pos));
            var neighborTypes = new []
            {
                CellContentType.OutOfBounds, //Left
                CellContentType.OutOfBounds, //Top
                CellContentType.OutOfBounds, //Right
                CellContentType.OutOfBounds  //Down
            };

            for (var i = 0; i < neighbors.Length; i++)
            {
                if (neighbors[i] == _typeGrid.OutOfBoundsValue) continue;
                if (neighbors[i] == _typeGrid.DefaultValue)
                {
                    neighborTypes[i] = CellContentType.None;
                    continue;
                }
                neighborTypes[i] = (CellContentType) neighbors[i];
            }

            return neighborTypes;
        }

        public static List<Vector3Int> GetNeighborsOfType(Vector3Int pos, CellContentType type)
        {
            var neighborPoints = _typeGrid.GetAdjacentPointsOfType(Vector3ToPoint(pos), (int) type);
            return PointsToVector3s(neighborPoints);
        }
        
        public static List<Vector3Int> GetNeighborPositions(Vector3Int position) =>
            PointsToVector3s(_typeGrid.GetAdjacentPoints(Vector3ToPoint(position)));

        #endregion

        #region pathfinding stuff

        public static List<Vector3Int> GetPathOfTypeBetween(Vector3Int startPos, Vector3Int endPos, CellContentType[] typesAllowedToBeOverwritten, bool useCost = false)
        {
            var startPoint = Vector3ToPoint(startPos);
            var endPoint = Vector3ToPoint(endPos);
            var usableTypes = new int[typesAllowedToBeOverwritten.Length];
            for (var i = 0; i < typesAllowedToBeOverwritten.Length; i++)
            {
                usableTypes[i] = (int) typesAllowedToBeOverwritten[i];
            }
            var typeInfo = new AStartTypeInfo
            {
                typeGrid = _typeGrid,
                useableTypes = usableTypes
            };
            var resultPath = useCost 
                ? _costGrid.AStarSearchWithCost(startPoint,endPoint,typeInfo) 
                :_typeGrid.AStarSearchWithoutCost(startPoint, endPoint, typeInfo);
            return resultPath.Select(point => new Vector3Int(point.X, 0, point.Y)).ToList();
        }

        #endregion
        
        
        #endregion

        #region private methods

        private static List<Vector3Int> PointsToVector3s(List<Point> points) => points.Select(PointToVector3).ToList();

        private static Vector3Int PointToVector3(Point point) => new Vector3Int(point.X,0,point.Y);

        private static Point Vector3ToPoint(Vector3Int vector3) => new Point(vector3.x, vector3.z);
        
        #endregion
    }
}
        