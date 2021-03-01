#pragma once
#include "Grid.h"

namespace Extern
{
	extern "C"
	{
		__declspec(dllexport) int* CreateGrid(int width, int height, int defaultValue = -1, int outOfBoundsValue = INT_MIN);
		__declspec(dllexport) void DeleteGrid(int* grid);

		__declspec(dllexport) int GetWidth(int* grid);
		__declspec(dllexport) int GetHeight(int* grid);
		__declspec(dllexport) int GetDefaultValue(int* grid);
		__declspec(dllexport) int GetOutOfBoundsValue(int* grid);
		
		__declspec(dllexport) int GetCoordinateContent(int* grid, int x, int y);
		__declspec(dllexport) void SetCoordinateContent(int* grid, int x, int y, int value);
		__declspec(dllexport) int* GetAjacentValues(int* grid, int x, int y);
		__declspec(dllexport) int* GetAjacentValidCoordinates(int* grid, int x, int y);
		__declspec(dllexport) int* GetAjacentValidValuesOfTypes(int* grid, int x, int y, int* values);
		
		__declspec(dllexport) int* AStarSearch(int* grid, int startX, int startY, int endX, int endY, bool useCost);
		__declspec(dllexport) int* AStarSearchWithTypeInfo(int* grid, int startX, int startY, int endX, int endY, bool useCost, int* useableValues, int* valueGrid);
	
		__declspec(dllexport) void DeleteArray(int* arr);
	}
}
