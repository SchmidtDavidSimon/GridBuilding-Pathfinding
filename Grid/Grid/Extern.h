#pragma once
#include "Grid.h"

#ifdef _EXPORTING
#define dllFunc __declspec(dllexport)
#else
#define dllFunc __declspec(dllimport)
#endif // _EXPORTING



namespace Extern
{
	extern "C"
	{
		/// <summary>
		/// Creates a pointer to a grid class and casts it into an int*
		/// </summary>
		/// <param name="width">Width of the grid</param>
		/// <param name="height">Height of the Grid</param>
		/// <param name="defaultValue">Default Value the grid is filled with</param>
		/// <param name="outOfBoundsValue">Value to compare OutOfBounds values to</param>
		dllFunc int* CreateGrid(int width, int height, int defaultValue = -1, int outOfBoundsValue = INT_MIN);
		
		/// <summary>
		/// Casts the given int* into a Grid* and delets it
		/// </summary>
		dllFunc void DeleteGrid(int* grid);
		
		/// <summary>
		/// Casts the given int* into a Grid* and returns its Width
		/// </summary>
		dllFunc int GetWidth(int* grid);

		/// <summary>
		/// Casts the given int* into a Grid* and returns its Height
		/// </summary>
		dllFunc int GetHeight(int* grid);

		/// <summary>
		/// Casts the given int* into a Grid* and returns its DefaultValue
		/// </summary>
		dllFunc int GetDefaultValue(int* grid);

		/// <summary>
		/// Casts the given int* into a Grid* and returns its OutOfBoundsValue
		/// </summary>
		dllFunc int GetOutOfBoundsValue(int* grid);
		
		/// <summary>
		/// Casts the given int* into a Grid* and returns the saved int on a given coordinate
		/// </summary>
		dllFunc int GetCoordinateContent(int* grid, int x, int y);

		/// <summary>
		/// Casts the given int* into a Grid* and sets the value of the given coordinate to the given value
		/// </summary>
		dllFunc void SetCoordinateContent(int* grid, int x, int y, int value);
		
		/// <summary>
		/// Casts the given int* into a Grid* and returns an int* with x and y of a random coordinate of the given type
		/// </summary>
		dllFunc int* GetRandomCoordinateOfValue(int* grid, int value);

		/// <summary>
		/// Casts the given int* into a Grid* and returns an int* with adjaciant values. The int* is structured as follows:
		/// {LEFT,TOP,RIGHT,BOTTOM}
		/// </summary>
		dllFunc int* GetAdjacentValues(int* grid, int x, int y);

		/// <summary>
		/// Casts the given int* into a Grid* and returns an int* with adjacent valid coordinates. The int* is structured as follows:
		/// [0] = Num of valid coordinates
		/// [1] = X Coordinate of 1. valid Coordinate
		/// [2] = Y Coordinate of 1. valid Coordinate
		/// Repeat [1]&[2] for each valid Coordinate
		/// </summary>
		dllFunc int* GetAdjacentValidCoordinates(int* grid, int x, int y);

		/// <summary>
		/// Casts the given int* into a Grid* and returns an int* with adjacent valid coordinates of one of the given values. The int* is structured as follows:
		/// [0] = Num of valid coordinates
		/// [1] = X Coordinate of 1. valid Coordinate
		/// [2] = Y Coordinate of 1. valid Coordinate
		/// Repeat [1]&[2] for each valid Coordinate
		/// </summary>
		dllFunc int* GetAdjacentValidValuesOfTypes(int* grid, int x, int y, int* values);
		
		/// <summary>
		/// Casts the given int* into a Grid* and returns an int* with coordinates on a path from start to end. The int* is structured as follows:
		/// [0] = Num of coordinates on path
		/// [1] = X coordinate of 1. Coordinate on the path
		/// [2] = Y coordinate of 1. Coordinate on the path
		/// Repeat [1]&[2] for each Coordinate on the path
		/// </summary>
		dllFunc int* AStarSearch(int* grid, int startX, int startY, int endX, int endY, bool useCost);
		
		/// <summary>
		/// Casts the given int* into a Grid* and returns an int* with coordinates on a path from start to end by only using values specified in the usableValues on the value Grid. The int* is structured as follows:
		/// [0] = Num of coordinates on path
		/// [1] = X coordinate of 1. Coordinate on the path
		/// [2] = Y coordinate of 1. Coordinate on the path
		/// Repeat [1]&[2] for each Coordinate on the path
		/// </summary>
		dllFunc int* AStarSearchWithTypeInfo(int* grid, int startX, int startY, int endX, int endY, bool useCost, int* usebleValues, int* valueGrid);
	
		/// <summary>
		/// Delete the int* with delete[]
		/// </summary>
		dllFunc void DeleteArray(int* arr);
	}
}
