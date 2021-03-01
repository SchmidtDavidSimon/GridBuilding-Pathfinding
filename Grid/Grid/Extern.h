#pragma once
#include "Grid.h"

namespace Extern
{
	extern "C"
	{
		__declspec(dllexport) void CreatePoint(int* p, int x, int y);
	}
}
