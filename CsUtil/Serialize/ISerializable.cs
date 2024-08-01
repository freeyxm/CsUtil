using System.IO;

namespace CsUtil.Serialize
{
    /// <summary>
    /// 二进制序列化接口。
    /// </summary>
    public interface ISerializable
    {
        void Serialize(SerializeWriter writer);

        void Deserialize(SerializeReader reader);
    }

    public class SerializeWriter : BinaryWriter
    {
        public SerializeWriter(Stream stream) : base(stream)
        {
        }

        public override void Write(string value)
        {
            bool hasValue = (value != null);
            Write(hasValue);
            if (hasValue)
            {
                base.Write(value);
            }
        }
    }

    public class SerializeReader : BinaryReader
    {
        public SerializeReader(Stream stream) : base(stream)
        {
        }

        public override string ReadString()
        {
            bool hasValue = ReadBoolean();
            if (hasValue)
            {
                return base.ReadString();
            }
            return null;
        }
    }
}
