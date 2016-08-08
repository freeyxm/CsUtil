using System;
using CsNet.Util;

namespace Test
{
    class Program
    {
        static void Main(string[] args)
        {
            //AvlTreeTest test = new AvlTreeTest();
            //test.TestValidity();
            //test.TestPerformace();

            RBTreeTest test2 = new RBTreeTest();
            test2.TestValidity();
            test2.TestPerformace();

            Console.ReadKey();
        }
    }
}
