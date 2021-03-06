#include "pch.h"
#include "Extern.h"
#include <vector>

int* Extern::CreateGrid(int width, int height, int defaultValue, int outOfBoundsValue)
{
	auto grid = new Grid(width, height, defaultValue, outOfBoundsValue);
	int* retVal = (int*) grid;
	return retVal;
}

void Extern::DeleteGrid(int* grid)
{
	Grid* g = (Grid*)grid;
	delete g;
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

int* Extern::GetRandomCoordinateOfValue(int* grid, int value)
{
	Grid* g = (Grid*)grid;
	
	int* retVal = new int[2];
	auto coord = g->GetRandomCooridanteOfValue(value);
	retVal[0] = coord.X;
	retVal[1] = coord.Y;
	
	return retVal;
}

int* Extern::GetAdjacentValues(int* grid, int x, int y)
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

int* Extern::GetAdjacentValidCoordinates(int* grid, int x, int y)
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

int* Extern::GetAdjacentValidValuesOfTypes(int* grid, int x, int y, int* values)
{
	Coordinate coord{ x,y };
	Grid* g = (Grid*)grid;
	
	auto size = values[0];
	std::vector<int> valueVec;
	for (auto i = 1; i < size + 1; i++)
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

int* Extern::AStarSearchWithTypeInfo(int* grid, int startX, int startY, int endX, int endY, bool useCost, int* usableValues, int* valueGrid)
{
	Coordinate start{ startX,startY };
	Coordinate end{ endX,endY };
	Grid* g = (Grid*)grid;
	Grid* vG = (Grid*)valueGrid;
	
	auto size = usableValues[0];
	std::vector<int> valueVec;
	for (auto i = 1; i < size + 1; i++)
	{
		valueVec.push_back(usableValues[i]);
	}
	AStarValueInfo info {valueVec,vG};
	
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
