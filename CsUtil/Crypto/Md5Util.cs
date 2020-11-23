using System;
using System.Security.Cryptography;
using System.IO;
using System.Text;
using CsUtil.Log;

namespace CsUtil.Crypto
{
    public static class Md5Util
    {
        private static MD5 m_md5 = new MD5CryptoServiceProvider();

        public static string Md5Sum(byte[] bytes)
        {
            return CryptoHelper.ToHexString(m_md5.ComputeHash(bytes));
        }

        public static string Md5Sum(Stream inputStream)
        {
            return CryptoHelper.ToHexString(m_md5.ComputeHash(inputStream));
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
    }
}