using System;
using CsNet.Collections;

namespace Test
{
    class Program
    {
        static void Main(string[] args)
        {
            //var test = new AvlTreeTest();
            //test.TestValidity();
            //test.TestPerformace();

            var test2 = new RBTreeTest();
            test2.TestValidity();
            test2.TestPerformace();

            Console.ReadKey();
        }
    }
}
