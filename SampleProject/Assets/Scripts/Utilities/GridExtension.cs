using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using JetBrains.Annotations;
using ScriptableObjects;
using UnityEngine;

namespace Utilities
{
    /// <summary>
    /// Class to handle the integration of the c++ dll
    /// </summary>
    public class GridExtension
    {
        #region dllImports

        [DllImport("Grid.dll", CallingConvention = CallingConvention.Cdecl)] 
        private static extern IntPtr CreateGrid(int width, int height, int defaultValue = -1, int outOfBoundsValue = Int32.MinValue);

        [DllImport("Grid.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern void DeleteGrid(IntPtr grid);
        
        [DllImport("Grid.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern void DeleteArray(IntPtr arr);
        
        [DllImport("Grid.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern int GetWidth(IntPtr grid);
        
        [DllImport("Grid.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern int GetHeight(IntPtr grid);

        [DllImport("Grid.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern int GetOutOfBoundsValue(IntPtr grid);
        
        [DllImport("Grid.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern void SetCoordinateContent(IntPtr grid, int x, int y, int value);
        
        [DllImport("Grid.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern int GetCoordinateContent(IntPtr grid, int x, int y);
        
        [DllImport("Grid.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr GetAdjacentValues(IntPtr grid, int x, int y);
        
        [DllImport("Grid.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr GetAdjacentValidValuesOfTypes(IntPtr grid, int x, int y, IntPtr values);
        
        [DllImport("Grid.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr GetAdjacentValidCoordinates(IntPtr grid, int x, int y);
        
        [DllImport("Grid.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr AStarSearch(IntPtr grid, int startX, int startY, int endX, int endY, bool useCost);
        
        [DllImport("Grid.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr AStarSearchWithTypeInfo(IntPtr grid, int startX, int startY, int endX, int endY, bool useCost, IntPtr usableValues, IntPtr valueGrid);
        
        

        #endregion

        #region fields

        private static IntPtr _typeGrid;
        private static IntPtr _costGrid;
        private static IntPtr _appealGrid;

        private const int TypeDefault = (int) CellContentType.None;
        private const int CostDefault = Int32.MaxValue;
        private const int AppealDefault = 0;

        #endregion

        #region public methods

        public GridExtension(int width, int height)
        {
            _typeGrid = CreateGrid(width, height, TypeDefault);
            _costGrid = CreateGrid(width, height, CostDefault);
            _appealGrid = CreateGrid(width, height, AppealDefault);
        }

        public static int GridWidth => GetWidth(_typeGrid);
        
        public static int GridHeight => GetHeight(_typeGrid);

        public static CellContentType GetCell(Vector3Int cell) => (CellContentType) GetCoordinateContent(_typeGrid, cell.x, cell.y);

        public static bool SetCell(Vector3Int cell, CellContent content)
        {
            var retVal = SetCell(_typeGrid, cell.x, cell.z, content.Width, content.Height, (int) content.Type);
            if (!retVal)
            {
                EmptyCell(cell);
            }
            
            retVal = SetCell(_costGrid, cell.x, cell.z, content.Width, content.Height, content.MovementCost);
            if (!retVal)
            {
                EmptyCell(cell);
            }
            
            retVal = SetCell(_appealGrid, cell.x, cell.z, content.Width, content.Height, content.Appeal);
            if (!retVal)
            {
                EmptyCell(cell);
            }
            
            return retVal;
        }

        public static bool EmptyCell(Vector3Int cell) => SetCell(_costGrid, cell.x, cell.z, 1, 1, CostDefault) 
                                                         && SetCell(_appealGrid, cell.x, cell.z, 1, 1, AppealDefault)
                                                         && SetCell(_typeGrid, cell.x, cell.z, 1, 1, TypeDefault);
        

        public static bool CellIsInBound(Vector3Int cell) => cell.x >= 0 
                                                             && cell.x < GetWidth(_typeGrid) 
                                                             && cell.z >= 0 && cell.z < GetHeight(_typeGrid);

        public static bool CellIsOfType(Vector3Int cell, CellContentType type) => type == GetCell(cell);

        public static bool CellIsFree(Vector3Int cell) =>
            GetCoordinateContent(_typeGrid, cell.x, cell.z) == TypeDefault;

        public static CellContentType[] GetNeighborTypes(Vector3Int cell)
        {
            var neighborsPtr = GetAdjacentValues(_typeGrid, cell.x, cell.z);
            var neighborsArr = new int[4];
            Marshal.Copy(neighborsPtr, neighborsArr, 0, 4);
            var neighborTypes = new[]
            {
                CellContentType.OutOfBounds,
                CellContentType.OutOfBounds,
                CellContentType.OutOfBounds,
                CellContentType.OutOfBounds
            };

            for (var i = 0; i < neighborsArr.Length; i++)
            {
                if (neighborsArr[i] == GetOutOfBoundsValue(_typeGrid)) continue;
                if (neighborsArr[i] == TypeDefault)
                {
                    neighborTypes[i] = CellContentType.None;
                    continue;
                }

                neighborTypes[i] = (CellContentType) neighborsArr[i];
            }
            
            DeleteArray(neighborsPtr);
            return neighborTypes;
        }

        public static List<Vector3Int> GetNeighborsOfTypes(Vector3Int cell, [CanBeNull] List<CellContentType> types)
        {
            IntPtr neighbors;
            if (types == null)
            {
                neighbors = GetAdjacentValidCoordinates(_typeGrid, cell.x, cell.z);
            }
            else
            {
                var typeArr = TypeListToIntArray(types);

                var typePtr = ArrayToIntPtr(typeArr);
            
                neighbors = GetAdjacentValidValuesOfTypes(_typeGrid, cell.x, cell.z, typePtr);
            }
            
            var retVal = IntPtrToVector3Ints(neighbors);

            DeleteArray(neighbors);
            return retVal;
        }

        public static List<Vector3Int> GetPathOfTypeBetween(Vector3Int start, Vector3Int end, [CanBeNull] List<CellContentType> allowedTypes, bool useCost = false)
        {
            IntPtr path;
            if (allowedTypes == null)
            {
                path = useCost
                    ? AStarSearch(_costGrid, start.x, start.z, end.x, end.z, true)
                    : AStarSearch(_typeGrid, start.x, start.z, end.x, end.z, false);
            }
            else
            {
                var typeArr = TypeListToIntArray(allowedTypes);
                var typePtr = ArrayToIntPtr(typeArr);

                path = useCost
                    ? AStarSearchWithTypeInfo(_costGrid, start.x, start.z, end.x, end.z, true, typePtr, _typeGrid)
                    : AStarSearchWithTypeInfo(_costGrid, start.x, start.z, end.x, end.z, false, typePtr, _typeGrid);
            }

            var retVal = IntPtrToVector3Ints(path);
            
            DeleteArray(path);
            return retVal;
        }

        public void Shutdown()
        {
            DeleteGrid(_typeGrid);
            DeleteGrid(_costGrid);
            DeleteGrid(_appealGrid);
        }

        #endregion

        #region private methods

        private static bool SetCell(IntPtr grid, int x, int y, int width, int height, int value)
        {
            for (var i = x; i < x + width; i++)
            {
                for (var j = y; j < y + height; j++)
                {
                    if (i >= GetWidth(grid) || j >= GetHeight(grid)) return false;
                    SetCoordinateContent(grid,i,j,value);
                }
            }

            return true;
        }

        private static List<Vector3Int> IntPtrToVector3Ints(IntPtr ptr)
        {
            var size = Marshal.ReadInt32(ptr);
            var temp = new int[size * 2 + 1];
            Marshal.Copy(ptr, temp, 0, size * 2 + 1);
            var neighborsArr = new int[size * 2];
            for (var i = 0; i < size * 2; i++)
            {
                neighborsArr[i] = temp[i + 1];
            }

            var x = 0;
            var retVal = new List<Vector3Int>();
            for (var i = 0; i < size * 2; i++)
            {
                if (i % 2 == 0)
                {
                    x = neighborsArr[i];
                }
                else
                {
                    var y = neighborsArr[i];
                    retVal.Add(new Vector3Int(x, 0, y));
                }
            }

            return retVal;
        }

        private static IntPtr ArrayToIntPtr(int[] arr)
        {
            IntPtr retVal;
            var handle = GCHandle.Alloc(arr, GCHandleType.Pinned);
            try
            {
                retVal = handle.AddrOfPinnedObject();
            }
            finally
            {
                if (handle.IsAllocated)
                {
                    handle.Free();
                }
            }

            return retVal;
        }

        private static int[] TypeListToIntArray(List<CellContentType> types)
        {
            var retVal = new int[types.Count + 1];
            retVal[0] = types.Count;
            for (var i = 1; i < types.Count + 1; i++)
            {
                retVal[i] = (int) types[i - 1];
            }

            return retVal;
        }
        
        #endregion
    }
}
        