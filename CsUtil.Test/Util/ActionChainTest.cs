using System;
using CsUtil.Util;

namespace CsUtil.Test.Util
{
    public class ActionChainTest : TestBase
    {
        public override void Test()
        {
            Func<bool> act1 = () =>
            {
                Console.WriteLine("action 1");
                return true;
            };

            Func<bool> act2 = () =>
            {
                Console.WriteLine("action 2");
                return true;
            };

            ActionChain chain = new ActionChain();
            chain.Add(act1);
            chain.Add(act2);
            chain.Execute(() =>
            {
                Console.WriteLine("action done");
            });
        }
    }
}
