using System;

namespace CsUtil.Util
{
    public class Singleton<T> where T : Singleton<T>
    {
        protected static T m_instance;

        public static T Instance
        {
            get { return m_instance; }
        }

        public Singleton()
        {
            m_instance = this as T;
        }
    }
}
