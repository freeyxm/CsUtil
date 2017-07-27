using System;

namespace CsUtil.Test
{
    class Program
    {
        static void Main(string[] args)
        {
            new Crypto.TestCrypter().Test();

            new Util.ActionChainTest().Test();

            Console.Read();
        }
    }
}
