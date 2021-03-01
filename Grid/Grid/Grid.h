#pragma once
#include <limits.h>
#include "Structs.h"
#include <map>
#include <vector>

class Grid
{
public:
	Grid(int width, int height, int defaultValue = -1, int outOfBoundsValue = INT_MIN);
	Grid(const Grid &g);

	int GetGridContent(Coordinate cooridnate);
	void SetGridContent(Coordinate cooridnate, int value);

	bool IsPositionSet(Coordinate cooridnate);
	Coordinate GetRandomCooridanteOfValue(int type);

	int GetAdjacentValidCoordinatesCount(Coordinate coordinate);
	std::vector<Coordinate> GetAdjacentVaildCoordinates(Coordinate coordinate);
	std::vector<Coordinate> GetAdjacentValidCoordinatesWithValues(Coordinate coordinate, std::vector<int> value);
	std::vector<int> GetAdjacentValidValues(Coordinate coordinate);
	std::vector<int> GetAdjacentValues(Coordinate coordinate);

	std::vector<Coordinate> AStarSearch(Coordinate start, Coordinate end, bool useCost);
	std::vector<Coordinate> AStarSearch(Coordinate start, Coordinate end, bool useCost, AStarValueInfo typeInfo);
	
	int Width, Height, DefaultValue, OutOfBoundsValue;

private:
	int CoordinateToGridIdx(Coordinate cell);
	Coordinate GridIdxToCoordinate(int pos);
	
	Coordinate GetClosestCoordinate(std::vector<Coordinate> coordsToChek, std::map<Coordinate, float> distanceMap);
	std::vector<Coordinate>::iterator GetCurrentIterator(std::vector<Coordinate> vector, Coordinate current);
	std::vector<Coordinate> GeneratePath(std::map<Coordinate, Coordinate> parentsMap, Coordinate end);
	float ManhattanDistance(Coordinate current, Coordinate end);
	std::vector<int> m_Grid;
};

