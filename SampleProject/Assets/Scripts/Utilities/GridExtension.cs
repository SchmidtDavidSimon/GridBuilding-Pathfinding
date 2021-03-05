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

        /// <summary>
        /// Creates 3 grids handled by dll. One fpr storing cell-type information, one for storing cell-movement-cost information and one for storing cell-appeal information.
        /// </summary>
        /// <param name="width">Width of the created grids</param>
        /// <param name="height">Height of the created grids</param>
        public GridExtension(int width, int height)
        {
            _typeGrid = CreateGrid(width, height, TypeDefault);
            _costGrid = CreateGrid(width, height, CostDefault);
            _appealGrid = CreateGrid(width, height, AppealDefault);
        }

        public static int GridWidth => GetWidth(_typeGrid);
        
        public static int GridHeight => GetHeight(_typeGrid);

        /// <summary>
        /// Returns the value of the type grid on the given cell.
        /// </summary>
        public static CellContentType GetCell(Vector3Int cell) => (CellContentType) GetCoordinateContent(_typeGrid, cell.x, cell.y);

        /// <summary>
        /// Sets the values of each of the grids to the respective value of the content on the given cell.
        /// 1. ContentType on the typeGrid
        /// 2. MovementCost on the costGrid
        /// 3. ContentAppeal on the appealGrid
        /// </summary>
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

        /// <summary>
        /// Sets all grids to their respective default value on the given cell
        /// </summary>
        public static bool EmptyCell(Vector3Int cell) => SetCell(_costGrid, cell.x, cell.z, 1, 1, CostDefault) 
                                                         && SetCell(_appealGrid, cell.x, cell.z, 1, 1, AppealDefault)
                                                         && SetCell(_typeGrid, cell.x, cell.z, 1, 1, TypeDefault);
        

        /// <summary>
        /// Checks if the given cell is in bounds of the grid.
        /// Using the type grid as all grids will always have the same width and height
        /// </summary>
        public static bool CellIsInBound(Vector3Int cell) => cell.x >= 0 
                                                             && cell.x < GetWidth(_typeGrid) 
                                                             && cell.z >= 0
                                                             && cell.z < GetHeight(_typeGrid);

        /// <summary>
        /// Checks if the value of the typeGrid on the given cell is equal to the given type 
        /// </summary>
        public static bool CellIsOfType(Vector3Int cell, CellContentType type) => type == GetCell(cell);

        /// <summary>
        /// Checks if the value of the typeGrid on the given cell is equal to the TypeDefault 
        /// </summary>
        public static bool CellIsFree(Vector3Int cell) => GetCoordinateContent(_typeGrid, cell.x, cell.z) == TypeDefault;

        /// <summary>
        /// Returns an array of the given cells neighbor types
        /// The Array is build up like so: {LEFT-neighbor, TOP-neighbor, RIGHT-neighbor, BOTTOM-neighbor}
        /// 1. Get the needed info from the grid as a pointer
        /// 2. Create an array to store the info
        /// 3. Copy the info from the pointer to the array
        /// 4. Create the return value array. Assume that all neighbors are out of bounds.
        /// 5. Check for each neighbor:
        ///     1. If the info matches the typeGrids out of bounds value. If so skip to the next neighbor
        ///     2. If the info matches the typeDefault. If so set the return value of that neighbor zo 'None' and skip to the next neighbor
        ///     3. Cast the info of the neighbor the corresponding ContentType
        /// 6. Delete the info-pointer
        /// </summary>
        public static CellContentType[] GetNeighborTypes(Vector3Int cell)
        {
            var neighborsPtr = GetAdjacentValues(_typeGrid, cell.x, cell.z);
            var neighborsArr = new int[4];
            Marshal.Copy(neighborsPtr, neighborsArr, 0, 4);
            var retVal = new[]
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
                    retVal[i] = CellContentType.None;
                    continue;
                }

                retVal[i] = (CellContentType) neighborsArr[i];
            }
            
            DeleteArray(neighborsPtr);
            return retVal;
        }

        /// <summary>
        /// Returns a list of all neighbors
        /// 1. Create the target pointer for the neighbors
        /// 2. If types is null save all neighbors on the Grid into the target pointer
        /// 3. If not
        ///     1. Convert the allowed types into an array
        ///     2. Convert the array into an IntPtr
        ///     3. Save all neighbors with one of the given types into the target pointer
        /// 4. Convert the target pointer into a list of Vector3Ints
        /// 5.Delete the created pointer
        /// </summary>
        /// <param name="types">Types of cells to look for in neighbors. Use null for all types</param>
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

        /// <summary>
        /// Returns a list of all cells on a path between the given start and end cell
        /// 1. Create the target pointer for the path
        /// 2. If allowedTypes is null save the path of the appropriate grid determined by useCost in the target poniter
        /// 3. If not
        ///     1. Convert allowedTypes into an array
        ///     2. Convert the array into an IntPtr
        ///     3. Save the path of the appropriate grid determined by useCost un the target poniter
        /// 4. Convert the target pointer into a list of Vector3Ints
        /// 5. Delete the target pointer
        /// </summary>
        /// <param name="allowedTypes">Types of cells to be used for pathfinding. Use null for all types</param>
        /// <param name="useCost">Search for path using the cost grid, otherwise use type grid</param>
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

        /// <summary>
        /// Delete all created grids
        /// </summary>
        public void Shutdown()
        {
            DeleteGrid(_typeGrid);
            DeleteGrid(_costGrid);
            DeleteGrid(_appealGrid);
        }

        #endregion

        #region private methods

        /// <summary>
        /// For all coordinates of the origin coordinate and the given width and height check if the coordinate is in bounds of the grid.
        /// If not set the given grid on the calculated coordinate to the given value
        /// </summary>
        /// <param name="x">X Coordinate of the origin cell</param>
        /// <param name="y">Y coordinate of the origin cell</param>
        /// <returns>Returns true if all cells over widthxheight are in bounds of the array</returns>
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

        /// <summary>
        /// Converts a given IntPtr of the correct format written in the dll into a list of Vector3Ints
        /// 1. Read the first int to determine the size of the pointer in cells
        /// 2. Create an array for the data to go into, the arrays length is composed as follows
        ///     {1 for the size + 1x size for x coordinates + 1x size for y coordinates}
        /// 3. Copy the data from the IntPtr into the array for better usage
        /// 4. Create a new array with a lenght of 2* size for just the coordinate data
        /// 5. Fill the new array with the coordinate data
        /// 6. Create the return value list
        /// 7. Go throw the new array as follows:
        ///     1. If an index is even save it in a temp variable
        ///     2. if an index is odd add a new Vector3Int to the return value with the saved even value as it's x an the current odd value as it's z coordinate
        /// </summary>
        private static List<Vector3Int> IntPtrToVector3Ints(IntPtr ptr)
        {
            var size = Marshal.ReadInt32(ptr);
            var temp = new int[1 + size * 2];
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

        /// <summary>
        /// Converts a given array into an IntPtr by using a GCHandle
        /// </summary>
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

        /// <summary>
        /// Converts a list of contentTypes into an int array in format for usage by the dll
        /// 1. Create a new array whose size is composed as follows
        ///     {1 for the size of the following values + the lists count for the}
        /// 2. Save the lists count at the index 0 of the array
        /// 3. Save the types casted into ints into the open indices of the array 
        /// </summary>
        private static int[] TypeListToIntArray(List<CellContentType> types)
        {
            var retVal = new int[1 + types.Count];
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
        