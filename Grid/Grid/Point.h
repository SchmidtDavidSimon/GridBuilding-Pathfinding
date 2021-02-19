#pragma once

extern "C"
{
	class __declspec(dllexport) Point
	{
	public:
		Point(int x, int y);
		Point(Point &p);
		~Point();

		int Sum();

		int X, Y;
	};
}