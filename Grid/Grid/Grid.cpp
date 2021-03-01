#include "pch.h"
#include "Grid.h"


Grid::Grid(int width, int height, int defaultValue, int outOfBoundsValue)
	:Width(width)
	,Height(height)
	,DefaultValue(defaultValue)
	,OutOfBoundsValue(outOfBoundsValue)
{
	for (int i = 0; i < width*height; i++)
	{
		m_Grid[i] = defaultValue;
	}
}

Grid::Grid(const Grid &g)
	:Width(g.Width)
	,Height(g.Height)
	,DefaultValue(g.DefaultValue)
	,OutOfBoundsValue(g.OutOfBoundsValue)
{
	for (int i = 0; i < g.Width* g.Height; i++)
	{
		m_Grid[i] = g.DefaultValue;
	}
}

int Grid::GetGridContent(Coordinate cooridnate)
{
	int pos = CoordinateToGridIdx(cooridnate);
	return m_Grid[pos];
}

void Grid::SetGridContent(Coordinate cooridnate, int value)
{
	int pos = CoordinateToGridIdx(cooridnate);
	m_Grid[pos] = value;
}

bool Grid::IsPositionSet(Coordinate cooridnate)
{
	int pos = CoordinateToGridIdx(cooridnate);
	return m_Grid[pos] != DefaultValue;
}

Coordinate Grid::GetRandomCooridanteOfValue(int value)
{
	std::vector<Coordinate> coordinatesOfType;
	for (int i = 0; i < Width * Height; i++)
	{
		if (m_Grid[i] == value)
		{
			Coordinate coordinate = GridIdxToCoordinate(i);
			coordinatesOfType.push_back(coordinate);
		}
	}

	int rand = (std::rand() % (coordinatesOfType.size() + 1));
	return coordinatesOfType[rand];
}

int Grid::GetAdjacentValidCoordinatesCount(Coordinate coordinate)
{
	int retval = 0;
	if (coordinate.X > 0)
	{
		retval++;
	}
	if (coordinate.X < Width - 1)
	{
		retval++;
	}
	if (coordinate.Y > 0)
	{
		retval++;
	}
	if (coordinate.Y < Height - 1)
	{
		retval++;
	}
	return retval;
}

std::vector<Coordinate> Grid::GetAdjacentVaildCoordinates(Coordinate coordinate)
{
	std::vector<Coordinate> retVal;
	if (coordinate.X > 0)
	{
		retVal.push_back({ coordinate.X - 1, coordinate.Y });
	}
	if (coordinate.X < Width - 1)
	{
		retVal.push_back({ coordinate.X + 1, coordinate.Y });
	}
	if (coordinate.Y > 0)
	{
		retVal.push_back({ coordinate.X, coordinate.Y - 1 });
	}
	if (coordinate.Y < Height - 1)
	{
		retVal.push_back({ coordinate.X, coordinate.Y + 1 });
	}
	return retVal;
}

std::vector<Coordinate> Grid::GetAdjacentValidCoordinatesWithValue(Coordinate coordinate, std::vector<int> value)
{
	std::vector<Coordinate> retVal;
	if (coordinate.X > 0)
	{
		Coordinate neighbor = { coordinate.X - 1, coordinate.Y };
		int gridIdx = CoordinateToGridIdx(neighbor);
		if (std::find(value.begin(), value.end(), m_Grid[gridIdx]) != value.end())
		{
			retVal.push_back(neighbor);
		}
	}
	if (coordinate.X < Width - 1)
	{
		Coordinate neighbor = { coordinate.X + 1, coordinate.Y };
		int gridIdx = CoordinateToGridIdx(neighbor);
		if (std::find(value.begin(), value.end(), m_Grid[gridIdx]) != value.end())
		{
			retVal.push_back(neighbor);
		}
	}
	if (coordinate.Y > 0)
	{
		Coordinate neighbor = { coordinate.X, coordinate.Y - 1};
		int gridIdx = CoordinateToGridIdx(neighbor);
		if (std::find(value.begin(), value.end(), m_Grid[gridIdx]) != value.end())
		{
			retVal.push_back(neighbor);
		}
	}
	if (coordinate.Y < Height - 1)
	{
		Coordinate neighbor = { coordinate.X, coordinate.Y + 1};
		int gridIdx = CoordinateToGridIdx(neighbor);
		if (std::find(value.begin(), value.end(), m_Grid[gridIdx]) != value.end())
		{
			retVal.push_back(neighbor);
		}
	}
	return retVal;
}

std::vector<int> Grid::GetAdjacentValidValues(Coordinate coordinate)
{
	std::vector<int> retVal;
	if (coordinate.X > 0)
	{
		int gridIdx = CoordinateToGridIdx({ coordinate.X + 1, coordinate.Y });
		retVal.push_back(m_Grid[gridIdx]);
	}
	if (coordinate.X < Width - 1)
	{
		int gridIdx = CoordinateToGridIdx({ coordinate.X - 1, coordinate.Y });
		retVal.push_back(m_Grid[gridIdx]);
	}
	if (coordinate.Y > 0)
	{
		int gridIdx = CoordinateToGridIdx({ coordinate.X, coordinate.Y - 1});
		retVal.push_back(m_Grid[gridIdx]);
	}
	if (coordinate.Y < Height - 1)
	{
		int gridIdx = CoordinateToGridIdx({ coordinate.X, coordinate.Y + 1});
		retVal.push_back(m_Grid[gridIdx]);
	}
	return retVal;
}

std::vector<Coordinate> Grid::AStarSearch(Coordinate start, Coordinate end, bool useCost)
{
	std::vector<Coordinate> path;

	std::vector<Coordinate> coordsToCheck;
	std::map<Coordinate, int> costMap;
	std::map<Coordinate, float> priorityMap;
	std::map<Coordinate, Coordinate> parentsMap;

	coordsToCheck.push_back(start);
	costMap[start] = 0;
	priorityMap[start] = 0;
	parentsMap[start] = { OutOfBoundsValue,OutOfBoundsValue };

	while (coordsToCheck.size() > 0)
	{
		auto current = GetClosestCoordinate(coordsToCheck, priorityMap);
		auto it = GetCurrentIterator(coordsToCheck, current);
		coordsToCheck.erase(it);
		
		if (current == end)
		{
			path = GeneratePath(parentsMap, current);
			return path;
		}

		auto neighbors = GetAdjacentVaildCoordinates(current);
		for (auto neighbor : neighbors)
		{
			auto newCost = useCost
				? costMap[current] + m_Grid[CoordinateToGridIdx(neighbor)]
				: 0;

			if (costMap.count(neighbor) && !(newCost < costMap[neighbor])) continue;
			costMap[neighbor] = newCost;

			auto priority = newCost + ManhattanDistance(neighbor, end);
			coordsToCheck.push_back(neighbor);
			priorityMap[neighbor] = priority;

			parentsMap[neighbor] = current;
		}
	}
	return path;
}

std::vector<Coordinate> Grid::AStarSearch(Coordinate start, Coordinate end, bool useCost, AStarTypeInfo typeInfo)
{
	std::vector<Coordinate> path;

	std::vector<Coordinate> coordsToCheck;
	std::map<Coordinate, int> costMap;
	std::map<Coordinate, float> priorityMap;
	std::map<Coordinate, Coordinate> parentsMap;

	coordsToCheck.push_back(start);
	costMap[start] = 0;
	priorityMap[start] = 0;
	parentsMap[start] = { OutOfBoundsValue,OutOfBoundsValue };

	if (std::find(typeInfo.UseableTypes.begin(), typeInfo.UseableTypes.end(), typeInfo.TypeGrid[CoordinateToGridIdx(start)]) != typeInfo.UseableTypes.end()) return path;

	while (coordsToCheck.size() > 0)
	{
		auto current = GetClosestCoordinate(coordsToCheck, priorityMap);
		auto it = GetCurrentIterator(coordsToCheck, current);
		coordsToCheck.erase(it);

		if (current == end)
		{
			path = GeneratePath(parentsMap, current);
			return path;
		}

		auto neighbors = typeInfo.TypeGrid->GetAdjacentValidCoordinatesWithValue(current, typeInfo.UseableTypes);
		for (auto neighbor : neighbors)
		{
			auto newCost = useCost
				? costMap[current] + m_Grid[CoordinateToGridIdx(neighbor)]
				: 0;

			if (costMap.count(neighbor) && !(newCost < costMap[neighbor])) continue;
			costMap[neighbor] = newCost;

			auto priority = newCost + ManhattanDistance(neighbor, end);
			coordsToCheck.push_back(neighbor);
			priorityMap[neighbor] = priority;

			parentsMap[neighbor] = current;
		}
	}
	return path;
}

int Grid::CoordinateToGridIdx(Coordinate cell)
{
	return cell.Y * Width + cell.X;
}

Coordinate Grid::GridIdxToCoordinate(int pos)
{
	return { pos / Width,pos % Width };
}

Coordinate Grid::GetClosestCoordinate(std::vector<Coordinate> coordsToChek, std::map<Coordinate, float> distanceMap)
{
	Coordinate candidate = coordsToChek[0];

	for (auto &coord : coordsToChek)
	{
		if (distanceMap[coord] < distanceMap[candidate])
		{
			candidate = coord;
		}
	}
	return candidate;
}

std::vector<Coordinate>::iterator Grid::GetCurrentIterator(std::vector<Coordinate> vector, Coordinate current)
{
	auto retVal = vector.begin();

	for (auto& coord : vector)
	{
		if (coord == current)
		{
			return retVal;
		}
		retVal++;
	}
	return vector.begin();
}

std::vector<Coordinate> Grid::GeneratePath(std::map<Coordinate, Coordinate> parentsMap, Coordinate end)
{
	std::vector<Coordinate> path;
	Coordinate parent = end;
	while (parent != Coordinate{OutOfBoundsValue, OutOfBoundsValue} && parentsMap.count(parent))
	{
		path.push_back(parent);
		parent = parentsMap[parent];
	}
	return path;
}

float Grid::ManhattanDistance(Coordinate current, Coordinate end)
{
	return fabs(end.X - current.X) + fabs(end.Y - current.Y);
}
