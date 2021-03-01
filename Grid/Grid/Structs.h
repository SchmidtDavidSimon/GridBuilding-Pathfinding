#pragma once
#include <vector>

struct Coordinate
{
	int X;
	int Y;

	bool operator==(Coordinate cOther);
	bool operator!=(Coordinate cOther);
};


class Grid;

struct AStarTypeInfo
{
public:
	std::vector<int> UseableTypes;
	Grid* TypeGrid;
};