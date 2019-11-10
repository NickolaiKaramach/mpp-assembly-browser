using System;

namespace TestingAssembly
{
    public static class A
    {
        public static int ExtMethod(this C C, int param)
        {
            return param;
        }
    }
}