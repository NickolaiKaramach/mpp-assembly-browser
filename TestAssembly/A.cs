using System;

namespace TestAssembly
{
    public static class A
    {
        public static int ExtMethod(this C C, int param)
        {
            return param;
        }
    }
}