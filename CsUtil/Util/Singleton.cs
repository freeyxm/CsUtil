using System;

namespace CsUtil.Util
{
    public class Singleton<T> where T : Singleton<T>, new()
    {
        public static T Instance { get { return Nested.instance; } }

        private class Nested
        {
            static Nested()
            {
            }

            internal static readonly T instance = new T();
        }
    }
}
