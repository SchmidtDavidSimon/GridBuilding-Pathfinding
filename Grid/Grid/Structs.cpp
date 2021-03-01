#include "pch.h"
#include "Structs.h"

bool Coordinate::operator==(Coordinate cOther)
{
    return X == cOther.X && Y == cOther.Y;
}

bool Coordinate::operator!=(Coordinate cOther)
{
    return X != cOther.X || Y != cOther.Y;
}

bool Coordinate::operator<(const Coordinate& cOther) const
{
	if (X == cOther.X)
	{
		return Y < cOther.Y;
	}
	return X < cOther.X;
}
