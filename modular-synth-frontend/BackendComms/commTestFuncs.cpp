//#define MyFunctions ; //macro. Tells the compiler that it is exportable
#if defined(_MSC_VER)
    //  Microsoft 
    #define EXPORT __declspec(dllexport)
#elif defined(__GNUC__)
    //  GCC
    #define EXPORT __attribute__((visibility("default")))
#endif
extern "C" {
    EXPORT int AddNumbers(int a, int b)
    {
        return a + b;
    }
}