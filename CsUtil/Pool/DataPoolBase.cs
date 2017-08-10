using System;

namespace CsUtil.Pool
{
    public class DataPoolBase<T> where T : DataPoolBase<T>, new()
    {
        public static T Create()
        {
            if (DataPool<T>.Instance != null)
                return DataPool<T>.Instance.Create();
            else
                return new T();
        }

        public static void Delete(T data)
        {
            if (DataPool<T>.Instance != null)
                DataPool<T>.Instance.Delete(data);
        }

        public void Delete()
        {
            if (DataPool<T>.Instance != null)
                DataPool<T>.Instance.Delete(this as T);
        }
    }
}
