using System;
using System.Text;
using System.Security.Cryptography;

public static class CryptoHelper
{
    private static char[] HEX_CHAR = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'a', 'b', 'c', 'd', 'e', 'f' };
    public static string ToHexString(byte[] bytes)
    {
        // implation 1:
        char[] hex = new char[bytes.Length * 2];
        for (int i = 0; i < bytes.Length; ++i)
        {
            hex[i * 2] = HEX_CHAR[((bytes[i] >> 4) & 0x0F)];
            hex[i * 2 + 1] = HEX_CHAR[(bytes[i] & 0x0F)];
        }
        return new string(hex);

        // implation 2:
        //return BitConverter.ToString(bytes).Replace("-", "").ToLower();

        // implation 3:
        //string str = new string((char)0, bytes.Length * 2);
        //unsafe
        //{
        //    fixed (char* p = str)
        //    {
        //        char* ps = p;
        //        for (int i = 0; i < bytes.Length; ++i)
        //        {
        //            *ps++ = HEX_CHAR[(bytes[i] >> 4)];
        //            *ps++ = HEX_CHAR[(bytes[i] & 0xf)];
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
        byte[] dataBytes = Encoding.UTF8.GetBytes(text);
        byte[] keyBytes = Encoding.UTF8.GetBytes(key);
        HMACSHA1 hmac = new HMACSHA1(keyBytes);
        byte[] hash = hmac.ComputeHash(dataBytes);
        string result = ToHexString(hash);
        return result;
    }
}

