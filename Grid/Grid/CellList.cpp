#include "pch.h"
#include "CellList.h"

CellList::CellList(Cell* cells, int size)
	:Cells(cells)
	,Size(size)
{}

CellList::CellList(CellList & cl)
	:Cells(cl.Cells)
	,Size(cl.Size)
{}

CellList::~CellList()
{
	delete Cells;
}
