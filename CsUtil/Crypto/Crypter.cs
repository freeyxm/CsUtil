using System;
using System.Security.Cryptography;
using System.IO;
using System.Text;
using CsUtil.Util;

namespace CsUtil.Crypto
{
    public class Crypter<T> : IDisposable where T : SymmetricAlgorithm, new()
    {
        protected T m_algorithm;

        public Crypter()
        {
            m_algorithm = new T();
        }

        public Crypter(byte[] key, byte[] IV)
        {
            m_algorithm = new T();
            SetKey(key, IV);
        }

        public Crypter(string key, string IV)
        {
            m_algorithm = new T();
            SetKey(key, IV);
        }

        ~Crypter()
        {
            Dispose();
        }

        public byte[] Key
        {
            get { return m_algorithm.Key; }
        }

        public byte[] IV
        {
            get { return m_algorithm.IV; }
        }

        /// <summary>
        /// 生成随机的Key和IV
        /// </summary>
        public void RandomKey()
        {
            m_algorithm.GenerateKey();
            m_algorithm.GenerateIV();
        }

        public void SetKey(byte[] key, byte[] IV)
        {
            m_algorithm.Key = key;
            m_algorithm.IV = IV;
        }

        public void SetKey(string key, string IV)
        {
            byte[] keyBytes = GetPasswordBytes(key, m_algorithm.LegalKeySizes[0].MinSize / 8);
            byte[] ivBytes = GetPasswordBytes(IV, m_algorithm.LegalBlockSizes[0].MinSize / 8);
            SetKey(keyBytes, ivBytes);
        }

        private byte[] _rgbSalt;
        private byte[] GetPasswordBytes(string key, int cb)
        {
            if (_rgbSalt == null)
            {
                _rgbSalt = Encoding.ASCII.GetBytes("1a7b58fc65e5310e320be7a8fdd0c950"); // random
            }
            PasswordDeriveBytes _passwordBytes = new PasswordDeriveBytes(key, _rgbSalt, "SHA1", 2);
            return _passwordBytes.GetBytes(cb);
        }


        /// <summary>
        /// 加密数据块
        /// </summary>
        /// <param name="data"></param>
        /// <returns>加密后的数据，异常时返回null</returns>
        public byte[] Encrypt(byte[] data)
        {
            byte[] output = null;
            try
            {
                using (var encryptor = m_algorithm.CreateEncryptor())
                using (MemoryStream memoryStream = new MemoryStream())
                using (CryptoStream cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
                {
                    byte[] lenBytes = BitConverter.GetBytes((int)data.Length);
                    cryptoStream.Write(lenBytes, 0, lenBytes.Length); // 存储数据长度，以减少解密时的内存拷贝。
                    cryptoStream.Write(data, 0, data.Length);
                    cryptoStream.FlushFinalBlock();
                    output = memoryStream.ToArray();
                }
            }
            catch (Exception e)
            {
                output = null;
                Logger.Error(e.ToString());
            }
            return output;
        }

        /// <summary>
        /// 解密数据块
        /// </summary>
        /// <param name="data"></param>
        /// <returns>解密后的数据，异常时返回null</returns>
        public byte[] Decrypt(byte[] data)
        {
            byte[] output = null;
            try
            {
                using (var decryptor = m_algorithm.CreateDecryptor())
                using (MemoryStream memoryStream = new MemoryStream(data))
                using (CryptoStream cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read))
                {
                    byte[] lenBytes = new byte[sizeof(int)];
                    int readLen = cryptoStream.Read(lenBytes, 0, lenBytes.Length);
                    if (readLen != lenBytes.Length)
                        throw new Exception("data corrupted.");

                    int dataLen = BitConverter.ToInt32(lenBytes, 0);
                    byte[] buffer = new byte[dataLen];
                    readLen = cryptoStream.Read(buffer, 0, buffer.Length);
                    if (readLen != dataLen)
                        throw new Exception("data corrupted.");

                    output = buffer;
                }
            }
            catch (Exception e)
            {
                output = null;
                Logger.Error(e.ToString());
            }
            return output;
        }

        /// <summary>
        /// 加密数据流
        /// </summary>
        /// <param name="inStream"></param>
        /// <param name="outStream"></param>
        /// <returns></returns>
        public bool Encrypt(Stream inStream, Stream outStream)
        {
            try
            {
                using (var encryptor = m_algorithm.CreateEncryptor())
                using (CryptoStream cryptoStream = new CryptoStream(outStream, encryptor, CryptoStreamMode.Write))
                {
                    int BUFFER_LEN = encryptor.InputBlockSize * 10;
                    byte[] buffer = new byte[BUFFER_LEN];
                    while (true)
                    {
                        int count = inStream.Read(buffer, 0, BUFFER_LEN);
                        if (count <= 0)
                            break;
                        cryptoStream.Write(buffer, 0, count);
                    }
                    cryptoStream.FlushFinalBlock();
                }
                return true;
            }
            catch (Exception e)
            {
                Logger.Error(e.ToString());
                return false;
            }
        }

        /// <summary>
        /// 解密数据流
        /// </summary>
        /// <param name="inStream"></param>
        /// <param name="outStream"></param>
        /// <returns></returns>
        public bool Decrypt(Stream inStream, Stream outStream)
        {
            try
            {
                using (var decryptor = m_algorithm.CreateDecryptor())
                using (CryptoStream cryptoStream = new CryptoStream(inStream, decryptor, CryptoStreamMode.Read))
                {
                    int BUFFER_LEN = decryptor.OutputBlockSize * 10;
                    byte[] buffer = new byte[BUFFER_LEN];
                    while (true)
                    {
                        int count = cryptoStream.Read(buffer, 0, BUFFER_LEN);
                        if (count <= 0)
                            break;
                        outStream.Write(buffer, 0, count);
                    }
                    outStream.Flush();
                }
                return true;
            }
            catch (Exception e)
            {
                Logger.Error(e.ToString());
                return false;
            }
        }

        public void Dispose()
        {
            if (m_algorithm != null)
            {
                m_algorithm.Clear();
                m_algorithm = null;
            }
        }
    }
}
