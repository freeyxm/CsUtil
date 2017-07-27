using System;

namespace CsUtil.Test
{
    class Program
    {
        static void Main(string[] args)
        {
            new Crypto.TestCrypter().RunTest();

            new Util.ActionChainTest().RunTest();

            Console.Read();
        }
    }
}
