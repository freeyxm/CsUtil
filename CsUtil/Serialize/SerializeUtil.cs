using System.Collections.Generic;
using System.IO;

namespace CsUtil.Serialize
{
    public static class SerializeUtil
    {
        public static byte[] Serialize(this ISerializable data)
        {
            MemoryStream stream = new MemoryStream();
            SerializeWriter writer = new SerializeWriter(stream);
            data.Serialize(writer);
            writer.Flush();
            byte[] bytes = stream.ToArray();
            return bytes;
        }

        public static void Deserialize(this ISerializable data, byte[] bytes)
        {
            MemoryStream stream = new MemoryStream(bytes);
            SerializeReader reader = new SerializeReader(stream);
            data.Deserialize(reader);
        }

        public static T Deserialize<T>(byte[] bytes) where T : ISerializable, new()
        {
            T data = new T();
            data.Deserialize(bytes);
            return data;
        }


        public static void Write(this SerializeWriter writer, ISerializable data)
        {
            bool hasValue = (data != null);
            writer.Write(hasValue);
            if (hasValue)
            {
                data.Serialize(writer);
            }
        }

        public static T ReadData<T>(this SerializeReader reader, T data = default(T)) where T : ISerializable, new()
        {
            bool hasValue = reader.ReadBoolean();
            if (hasValue)
            {
                if (data == null)
                    data = new T();
                data.Deserialize(reader);
            }
            return data;
        }


        public static void Write(this SerializeWriter writer, int? data)
        {
            bool hasValue = (data != null);
            writer.Write(hasValue);
            if (hasValue)
            {
                writer.Write((int)data);
            }
        }

        public static int? ReadInt32Null(this SerializeReader reader)
        {
            bool hasValue = reader.ReadBoolean();
            if (hasValue)
            {
                return reader.ReadInt32();
            }
            return null;
        }


        public static void Write(this SerializeWriter writer, long? data)
        {
            bool hasValue = (data != null);
            writer.Write(hasValue);
            if (hasValue)
            {
                writer.Write((long)data);
            }
        }

        public static long? ReadInt64Null(this SerializeReader reader)
        {
            bool hasValue = reader.ReadBoolean();
            if (hasValue)
            {
                return reader.ReadInt64();
            }
            return null;
        }


        public static void Write(this SerializeWriter writer, double? data)
        {
            bool hasValue = (data != null);
            writer.Write(hasValue);
            if (hasValue)
            {
                writer.Write((double)data);
            }
        }

        public static double? ReadDoubleNull(this SerializeReader reader)
        {
            bool hasValue = reader.ReadBoolean();
            if (hasValue)
            {
                return reader.ReadDouble();
            }
            return null;
        }


        public static void Write<T>(this SerializeWriter writer, T[] array) where T : ISerializable
        {
            bool hasValue = (array != null);
            writer.Write(hasValue);
            if (hasValue)
            {
                writer.Write(array.Length);
                foreach (var value in array)
                {
                    writer.Write(value);
                }
            }
        }

        public static T[] ReadData<T>(this SerializeReader reader, T[] array) where T : ISerializable, new()
        {
            bool hasValue = reader.ReadBoolean();
            if (hasValue)
            {
                int count = reader.ReadInt32();
                if (array == null || array.Length != count)
                {
                    array = new T[count];
                }
                for (int i = 0; i < count; i++)
                {
                    T value = reader.ReadData<T>();
                    array[i] = value;
                }
            }
            return array;
        }


        public static void Write<T>(this SerializeWriter writer, List<T> list) where T : ISerializable
        {
            bool hasValue = (list != null);
            writer.Write(hasValue);
            if (hasValue)
            {
                writer.Write(list.Count);
                foreach (var value in list)
                {
                    writer.Write(value);
                }
            }
        }

        public static List<T> ReadData<T>(this SerializeReader reader, List<T> list) where T : ISerializable, new()
        {
            bool hasValue = reader.ReadBoolean();
            if (hasValue)
            {
                int count = reader.ReadInt32();
                if (list == null)
                {
                    list = new List<T>(count);
                }
                for (int i = 0; i < count; i++)
                {
                    T value = reader.ReadData<T>();
                    list.Add(value);
                }
            }
            return list;
        }


        public static void Write<T>(this SerializeWriter writer, Dictionary<int, T> dict) where T : ISerializable
        {
            bool hasValue = (dict != null);
            writer.Write(hasValue);
            if (hasValue)
            {
                writer.Write(dict.Count);
                foreach (var pair in dict)
                {
                    writer.Write(pair.Key);
                    writer.Write(pair.Value);
                }
            }
        }

        public static Dictionary<int, T> ReadData<T>(this SerializeReader reader, Dictionary<int, T> dict) where T : ISerializable, new()
        {
            bool hasValue = reader.ReadBoolean();
            if (hasValue)
            {
                int count = reader.ReadInt32();
                if (dict == null)
                {
                    dict = new Dictionary<int, T>(count);
                }
                for (int i = 0; i < count; i++)
                {
                    int key = reader.ReadInt32();
                    T value = reader.ReadData<T>();
                    dict[key] = value;
                }
            }
            return dict;
        }


        public static void Write<T>(this SerializeWriter writer, Dictionary<string, T> dict) where T : ISerializable
        {
            bool hasValue = (dict != null);
            writer.Write(hasValue);
            if (hasValue)
            {
                writer.Write(dict.Count);
                foreach (var pair in dict)
                {
                    writer.Write(pair.Key);
                    writer.Write(pair.Value);
                }
            }
        }

        public static Dictionary<string, T> ReadData<T>(this SerializeReader reader, Dictionary<string, T> dict) where T : ISerializable, new()
        {
            bool hasValue = reader.ReadBoolean();
            if (hasValue)
            {
                int count = reader.ReadInt32();
                if (dict == null)
                {
                    dict = new Dictionary<string, T>(count);
                }
                for (int i = 0; i < count; i++)
                {
                    string key = reader.ReadString();
                    T value = reader.ReadData<T>();
                    dict[key] = value;
                }
            }
            return dict;
        }
    }
}
