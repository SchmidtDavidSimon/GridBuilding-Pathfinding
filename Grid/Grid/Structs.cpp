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
