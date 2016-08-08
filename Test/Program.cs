using System;
using CsNet.Util;

namespace Test
{
    class Program
    {
        static void Main(string[] args)
        {
            AvlTreeTest test = new AvlTreeTest();
            test.TestValidity();
            test.TestPerformace();

            Console.ReadKey();
        }
    }
}
