#pragma once
#include "Cell.h"
struct CellList
{
public:
	CellList(Cell* cells, int size);
	CellList(CellList& cl);
	~CellList();

	Cell* Cells;
	int Size;
};

