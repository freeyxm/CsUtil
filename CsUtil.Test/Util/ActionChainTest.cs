using System;
using CsUtil.Util;

namespace CsUtil.Test.Util
{
    public class ActionChainTest : TestBase<ActionChainTest>
    {
        protected override void Test()
        {
            Func<bool> act1 = () =>
            {
                Logger.Debug("action 1");
                return true;
            };

            Func<bool> act2 = () =>
            {
                Logger.Debug("action 2");
                return true;
            };

            ActionChain chain = new ActionChain();
            chain.Add(act1);
            chain.Add(act2);
            chain.Execute(() =>
            {
                Logger.Debug("action done");
            });
        }
    }
}
