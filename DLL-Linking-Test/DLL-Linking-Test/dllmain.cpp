// dllmain.cpp : Defines the entry point for the DLL application.
#include "pch.h"

//#define MyFunctions ; //macro. Tells the compiler that it is exportable
#if defined(_MSC_VER)
    //  Microsoft 
#define EXPORT __declspec(dllexport)
#elif defined(__GNUC__)
    //  GCC
#define EXPORT __attribute__((visibility("default")))
#endif
extern "C" {
    EXPORT int addNumbers(int a, int b)
    {
        return a + b;
    }
}

/*
BOOL APIENTRY DllMain( HMODULE hModule,
                       DWORD  ul_reason_for_call,
                       LPVOID lpReserved
                     )
{
    switch (ul_reason_for_call)
    {
    case DLL_PROCESS_ATTACH:
    case DLL_THREAD_ATTACH:
    case DLL_THREAD_DETACH:
    case DLL_PROCESS_DETACH:
        break;
    }
    return TRUE;
}
*/
