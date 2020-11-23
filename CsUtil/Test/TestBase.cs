using System;
using CsUtil.Log;

namespace CsUtil.Test
{
    public abstract class TestBase<T>
    {
        public void RunTest()
        {
            Logger.Info("[Test] Run test {0} start", typeof(T));
            Test();
            Logger.Info("[Test] Run test {0} end", typeof(T));
        }

        protected abstract void Test();
    }
}
