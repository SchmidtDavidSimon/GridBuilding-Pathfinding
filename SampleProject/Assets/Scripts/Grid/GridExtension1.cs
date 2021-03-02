using System;
using System.Runtime.InteropServices;

namespace Grid
{
    /// <summary>
    /// Class to handle the integration of the c++ dll
    /// </summary>
    public unsafe class GridExtension1
    {
        [DllImport("Grid.dll")]
        private static extern IntPtr CreateGrid(int width, int height, int defaultValue = -1, int outOfBoundsValue = Int32.MinValue);

        [DllImport("Grid.dll")]
        private static extern void DeleteGrid(IntPtr grid);
        
        private IntPtr _grid;
        public GridExtension1(int width, int height)
        {
            _grid = CreateGrid(width, height);
        }

        public void Shutdown()
        {
            DeleteGrid(_grid);
        }
    }
}
        