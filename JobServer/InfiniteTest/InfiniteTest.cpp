// InfiniteTest.cpp : Defines the entry point for the console application.
//

#include "stdafx.h"
#include <iostream>
#include <Windows.h>

using namespace std;

int main(int argc, char argv[])
{
	cout << "Started an infinite loop" << endl;
	cerr << "IAMA_ERROR_AMA" << endl;
	while (true) {
		Sleep(500);
	}

	return 0;
}

