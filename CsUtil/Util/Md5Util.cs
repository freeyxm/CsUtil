using System;
using System.Security.Cryptography;
using System.IO;
using System.Text;

namespace CsUtil.Util
{
    public class Md5Util
    {
        private static MD5 m_md5 = new MD5CryptoServiceProvider();

        public static string Md5Sum(byte[] bytes)
        {
            return ToHexStr(m_md5.ComputeHash(bytes));
        }

        public static string Md5Sum(Stream inputStream)
        {
            return ToHexStr(m_md5.ComputeHash(inputStream));
        }

        public static string Md5Sum(string str)
        {
            return Md5Sum(Encoding.UTF8.GetBytes(str));
        }

        public static string Md5SumFile(string path)
        {
            FileStream file = null;
            try
            {
                file = new FileStream(path, FileMode.Open);
                return Md5Sum(file);
            }
            catch (Exception e)
            {
                Logger.Error(e.Message);
                return "";
            }
            finally
            {
                if (file != null) file.Close();
            }
        }

        public static bool CheckMd5(byte[] bytes, string md5)
        {
            return CompareMd5(Md5Sum(bytes), md5);
        }

        public static bool CheckMd5(Stream inputStream, string md5)
        {
            return CompareMd5(Md5Sum(inputStream), md5);
        }

        public static bool CheckMd5(string str, string md5)
        {
            return CompareMd5(Md5Sum(str), md5);
        }

        public static bool CheckMd5File(string path, string md5)
        {
            return CompareMd5(Md5SumFile(path), md5);
        }

        public static bool CompareMd5(string m1, string m2)
        {
            return string.Equals(m1, m2, StringComparison.OrdinalIgnoreCase);
        }

        public static string ToHexStr(byte[] bytes)
        {
            char[] hex = new char[bytes.Length * 2];
            for (int i = 0; i < bytes.Length; ++i)
            {
                hex[i * 2] = HEX_CHAR[((bytes[i] >> 4) & 0x0F)];
                hex[i * 2 + 1] = HEX_CHAR[(bytes[i] & 0x0F)];
            }
            return new string(hex);
        }

        private static readonly char[] HEX_CHAR = {
            '0','1','2','3','4','5','6','7','8','9',
            'A','B','C','D','E','F',
        };
    }
}