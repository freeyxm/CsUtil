using System;
using System.Text;
using System.Security.Cryptography;

public class CryptoHelper
{
    private static char[] m_hex_char = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'a', 'b', 'c', 'd', 'e', 'f' };
    public static string ToHexString(byte[] data)
    {
        //return BitConverter.ToString(output).Replace("-", "").ToLower();

        char[] output = new char[data.Length * 2];
        for (int i = 0; i < data.Length; ++i)
        {
            output[i * 2] = m_hex_char[(data[i] >> 4)];
            output[i * 2 + 1] = m_hex_char[(data[i] & 0xf)];
        }
        return new string(output);

        //string str = new string((char)0, data.Length * 2);
        //unsafe
        //{
        //    fixed (char* p = str)
        //    {
        //        char* ps = p;
        //        for (int i = 0; i < data.Length; ++i)
        //        {
        //            *ps++ = m_hex_char[(data[i] >> 4)];
        //            *ps++ = m_hex_char[(data[i] & 0xf)];
        //        }
        //    }
        //}
        //return str;
    }

    public static string ComputeMD5(string str)
    {
        byte[] data = Encoding.UTF8.GetBytes(str);
        return ComputeMD5(data);
    }

    public static string ComputeMD5(byte[] data)
    {
        MD5 md5 = new MD5CryptoServiceProvider();
        byte[] output = md5.ComputeHash(data);
        string result = ToHexString(output);
        return result;
    }

    public static string ComputeMD5(byte[] data, int offset, int count)
    {
        MD5 md5 = new MD5CryptoServiceProvider();
        byte[] output = md5.ComputeHash(data, offset, count);
        string result = ToHexString(output);
        return result;
    }

    public static string ComputeHmacSha1(string text, string key)
    {
        byte[] byteData = Encoding.UTF8.GetBytes(text);
        byte[] byteKey = Encoding.UTF8.GetBytes(key);
        HMACSHA1 hmac = new HMACSHA1(byteKey);
        byte[] hash = hmac.ComputeHash(byteData);
        string result = ToHexString(hash);
        return result;
    }
}

