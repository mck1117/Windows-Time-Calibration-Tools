/*++

Copyright (c) Microsoft Corporation

Module Name:

    TimeSampler.c

Abstract:
    
    This module captures the system time and the current RDTSC at configured intervals.

Author:

    Alan Jowett (alanjo) 19-March-2016

--*/

#include <stdio.h>
#include <stdlib.h>
#include <windows.h>

int main(int argc, char ** argv)
{
    DWORD64 tscStart;
    DWORD64 tscEnd;
    FILETIME ft;
	LARGE_INTEGER pc;
    size_t interations;
    DWORD interval;
	DWORD method;

    if (argc != 4) {
        printf("Usage: %s method interval count\n", argv[0]);
        return -1;
    }

    printf("TSC_START, TSC_END, SYSTEM_TIME, TIME_ADJ, TIME_INC, TIME_ADJ_ACTIVE\n");
	method = atoi(argv[1]);
    interval = atoi(argv[2]);
    interations = atoi(argv[3]);

    for (size_t i = 0; i < interations; i++) {
        DWORD timeAdjustment = 0;
        DWORD timeIncrement = 0;
        BOOL timeAdjEnabled = FALSE;
        Sleep(interval);

		switch (method) {
		case 0:
			tscStart = __rdtsc();
			GetSystemTimeAsFileTime(&ft);
			tscEnd = __rdtsc();
			break;
		case 1:
			tscStart = __rdtsc();
			GetSystemTimePreciseAsFileTime(&ft);
			tscEnd = __rdtsc();
			break;
		case 2:
			tscStart = __rdtsc();
			QueryPerformanceCounter(&pc);
			tscEnd = __rdtsc();

			// dump in to a filetime
			ft.dwHighDateTime = pc.HighPart;
			ft.dwLowDateTime = pc.LowPart;
			
			break;
		}

        GetSystemTimeAdjustment(&timeAdjustment, &timeIncrement, &timeAdjEnabled);

        printf("%lld, %lld, %lld, %d, %d, %s\n", tscStart, tscEnd, *(DWORD64*)&ft, timeAdjustment, timeIncrement, !timeAdjEnabled ? "true" : "false");
		fflush(NULL);
    }

    return 0;
}

