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
		dllFunc int* CreateGrid(int width, int height, int defaultValue = -1, int outOfBoundsValue = INT_MIN);
		dllFunc void DeleteGrid(int* grid);

		dllFunc int GetWidth(int* grid);
		dllFunc int GetHeight(int* grid);
		dllFunc int GetDefaultValue(int* grid);
		dllFunc int GetOutOfBoundsValue(int* grid);
		
		dllFunc int GetCoordinateContent(int* grid, int x, int y);
		dllFunc void SetCoordinateContent(int* grid, int x, int y, int value);
		
		dllFunc int* GetRandomCoordinateOfValue(int* grid, int value);

		dllFunc int* GetAdjacentValues(int* grid, int x, int y);
		dllFunc int* GetAdjacentValidCoordinates(int* grid, int x, int y);
		dllFunc int* GetAdjacentValidValuesOfTypes(int* grid, int x, int y, int* values);
		
		dllFunc int* AStarSearch(int* grid, int startX, int startY, int endX, int endY, bool useCost);
		dllFunc int* AStarSearchWithTypeInfo(int* grid, int startX, int startY, int endX, int endY, bool useCost, int* usebleValues, int* valueGrid);
	
		dllFunc void DeleteArray(int* arr);
	}
}
