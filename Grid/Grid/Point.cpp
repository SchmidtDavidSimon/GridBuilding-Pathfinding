#include "pch.h"
#include "Point.h"

Point::Point(int x, int y) 
	: X(x)
	, Y(y)
{}

Point::Point(Point & p)
{
}

Point::~Point()
{
}

int Point::Sum()
{
	return X + Y;
}
