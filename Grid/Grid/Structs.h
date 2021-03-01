#pragma once
#include <vector>

struct Coordinate
{
	int X;
	int Y;

	bool operator==(Coordinate cOther);
	bool operator!=(Coordinate cOther);
	bool operator<(const Coordinate& cOther) const;
};

class Grid;

struct AStarValueInfo
{
	std::vector<int> UseableValues;
	Grid* ValueGrid;
};