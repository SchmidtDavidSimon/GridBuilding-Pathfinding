#include "pch.h"
#include "Extern.h"
#include <vector>

int* Extern::CreateGrid(int width, int height, int defaultValue, int outOfBoundsValue)
{
	auto grid = new Grid(width, height, defaultValue, outOfBoundsValue);
	int* retVal = (int*) &grid;
	return retVal;
}

void Extern::DeleteGrid(int* grid)
{
	delete grid;
}

int Extern::GetWidth(int* grid)
{
	Grid* g = (Grid*) grid;
	return g->Width;
}

int Extern::GetHeight(int* grid)
{
	Grid* g = (Grid*) grid;
	return g->Height;
}

int Extern::GetDefaultValue(int* grid)
{
	Grid* g = (Grid*) grid;
	return g->DefaultValue;
}

int Extern::GetOutOfBoundsValue(int* grid)
{
	Grid* g = (Grid*)grid;
	return g->OutOfBoundsValue;
}

int Extern::GetCoordinateContent(int* grid, int x, int y)
{
	Coordinate coord{ x,y };
	Grid* g = (Grid*) grid;
	return g->GetGridContent(coord);
}

void Extern::SetCoordinateContent(int* grid, int x, int y, int value)
{
	Coordinate coord{ x,y };
	Grid* g = (Grid*)grid;
	g->SetGridContent(coord, value);
}

int* Extern::GetAjacentValues(int* grid, int x, int y)
{
	Coordinate coord{ x,y };
	Grid* g = (Grid*)grid;
	auto values = g->GetAdjacentValues(coord);
	
	int* retVal = new int[4];
	retVal[0] = values[0];
	retVal[1] = values[1];
	retVal[2] = values[2];
	retVal[3] = values[3];
	
	return retVal;
}

int* Extern::GetAjacentValidCoordinates(int* grid, int x, int y)
{
	Coordinate coord{ x,y };
	Grid* g = (Grid*)grid;
	auto vec = g->GetAdjacentVaildCoordinates(coord);
	
	auto retVal = new int[vec.size() * 2 + 1];
	retVal[0] = vec.size();
	for (auto i = 0; i < vec.size(); i++)
	{
		retVal[i * 2 + 1] = vec[i].X;
		retVal[i * 2 + 2] = vec[i].Y;
	}
	
	return retVal;
}

int* Extern::GetAjacentValidValuesOfTypes(int* grid, int x, int y, int* values)
{
	Coordinate coord{ x,y };
	Grid* g = (Grid*)grid;
	
	auto size = values[0];
	std::vector<int> valueVec;
	for (auto i = 1; i < size; i++)
	{
		valueVec.push_back(values[i]);
	}
	
	auto vec = g->GetAdjacentValidCoordinatesWithValues(coord, valueVec);
	
	auto retVal = new int[vec.size() * 2 + 1];
	retVal[0] = vec.size();
	for (auto i = 0; i < vec.size(); i++)
	{
		retVal[i * 2 + 1] = vec[i].X;
		retVal[i * 2 + 2] = vec[i].Y;
	
	}
	return retVal;
}

int* Extern::AStarSearch(int* grid, int startX, int startY, int endX, int endY, bool useCost)
{
	Coordinate start{ startX,startY };
	Coordinate end{ endX,endY };
	Grid* g = (Grid*)grid;
	auto vec = g->AStarSearch(start, end, useCost);
	
	auto retVal = new int[vec.size() * 2 + 1];
	retVal[0] = vec.size();
	for (auto i = 0; i < vec.size(); i++)
	{
		retVal[i * 2 + 1] = vec[i].X;
		retVal[i * 2 + 2] = vec[i].Y;
	}
	
	return retVal;
}

int* Extern::AStarSearchWithTypeInfo(int* grid, int startX, int startY, int endX, int endY, bool useCost, int* useableValues, int* valueGrid)
{
	Coordinate start{ startX,startY };
	Coordinate end{ endX,endY };
	Grid* g = (Grid*)grid;
	Grid* vG = (Grid*)valueGrid;
	
	auto size = useableValues[0];
	std::vector<int> valueVec;
	for (auto i = 1; i < size; i++)
	{
		valueVec.push_back(useableValues[i]);
	}
	AStarValueInfo info { valueVec,vG};
	
	auto vec = g->AStarSearch(start, end, useCost, info);
	
	auto retVal = new int[vec.size() * 2 + 1];
	retVal[0] = vec.size();
	for (auto i = 0; i < vec.size(); i++)
	{
		retVal[i * 2 + 1] = vec[i].X;
		retVal[i * 2 + 2] = vec[i].Y;
	}
	
	return retVal;
}

void Extern::DeleteArray(int* arr)
{
	delete[] arr;
}
