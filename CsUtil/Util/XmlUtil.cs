using System;
using System.IO;
using System.Xml.Serialization;

namespace CsUtil.Util
{
    /// <summary>
    /// Xml序列化与反序列化
    /// </summary>
    public class XmlUtil
    {
        #region 反序列化
        /// <summary>
        /// 反序列化
        /// </summary>
        /// <param name="type">类型</param>
        /// <param name="xml">XML字符串</param>
        /// <returns></returns>
        public static object Deserialize(string xml, Type type)
        {
            try
            {
                using (StringReader sr = new StringReader(xml))
                {
                    XmlSerializer xmldes = new XmlSerializer(type);
                    return xmldes.Deserialize(sr);
                }
            }
            catch (Exception e)
            {
                Logger.Error(e.Message);
                return null;
            }
        }

        public static bool Deserialize<T>(string xml, out T obj)
        {
            try
            {
                using (StringReader sr = new StringReader(xml))
                {
                    XmlSerializer xmldes = new XmlSerializer(typeof(T));
                    obj = (T)xmldes.Deserialize(sr);
                    return true;
                }
            }
            catch (Exception e)
            {
                Logger.Error(e.Message);
                obj = default(T);
                return false;
            }
        }

        /// <summary>
        /// 反序列化
        /// </summary>
        /// <param name="type"></param>
        /// <param name="xml"></param>
        /// <returns></returns>
        public static object Deserialize(Stream stream, Type type)
        {
            try
            {
                XmlSerializer xmldes = new XmlSerializer(type);
                return xmldes.Deserialize(stream);
            }
            catch (Exception e)
            {
                Logger.Error(e.Message);
                return null;
            }
        }

        public static bool Deserialize<T>(Stream stream, out T obj)
        {
            try
            {
                XmlSerializer xmldes = new XmlSerializer(typeof(T));
                obj = (T)xmldes.Deserialize(stream);
                return true;
            }
            catch (Exception e)
            {
                Logger.Error(e.Message);
                obj = default(T);
                return false;
            }
        }
        #endregion

        #region 序列化
        /// <summary>
        /// 序列化
        /// </summary>
        /// <param name="type">类型</param>
        /// <param name="obj">对象</param>
        /// <returns></returns>
        public static string Serializer(object obj, Type type)
        {
            string result = null;
            MemoryStream stream = new MemoryStream();
            StreamReader reader = new StreamReader(stream);
            try
            {
                XmlSerializer xml = new XmlSerializer(type);
                xml.Serialize(stream, obj);
                stream.Position = 0;
                result = reader.ReadToEnd();
            }
            catch (Exception e)
            {
                Logger.Error(e.Message);
            }
            finally
            {
                reader.Dispose();
                stream.Dispose();
            }
            return result;
        }

        public static string Serializer<T>(T obj)
        {
            string result = null;
            using (MemoryStream stream = new MemoryStream())
            {
                using (StreamReader reader = new StreamReader(stream))
                {
                    try
                    {
                        XmlSerializer xml = new XmlSerializer(typeof(T));
                        xml.Serialize(stream, obj); // 序列化对象
                        stream.Position = 0;
                        result = reader.ReadToEnd();
                    }
                    catch (Exception e)
                    {
                        Logger.Error(e.Message);
                    }
                }
            }
            return result;
        }
        #endregion
    }
}
