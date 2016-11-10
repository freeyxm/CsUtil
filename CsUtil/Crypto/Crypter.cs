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
        protected ICryptoTransform m_encryptor;
        protected ICryptoTransform m_decryptor;

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
            KeyObsolete();
        }

        public void SetKey(byte[] key, byte[] IV)
        {
            m_algorithm.Key = key;
            m_algorithm.IV = IV;
            KeyObsolete();
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
                if (m_encryptor == null)
                    m_encryptor = m_algorithm.CreateEncryptor();

                using (MemoryStream memoryStream = new MemoryStream())
                using (CryptoStream cryptoStream = new CryptoStream(memoryStream, m_encryptor, CryptoStreamMode.Write))
                {
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
                if (m_decryptor == null)
                    m_decryptor = m_algorithm.CreateDecryptor();

                using (MemoryStream memoryStream = new MemoryStream(data))
                using (CryptoStream cryptoStream = new CryptoStream(memoryStream, m_decryptor, CryptoStreamMode.Read))
                {
                    byte[] buffer = new byte[data.Length];
                    int readLen = cryptoStream.Read(buffer, 0, buffer.Length);
                    if (readLen > 0)
                    {
                        if (readLen != buffer.Length)
                        {
                            output = new byte[readLen]; // 无法预先知道data长度，多一次内存拷贝！
                            Buffer.BlockCopy(buffer, 0, output, 0, readLen);
                        }
                        else
                        {
                            output = buffer;
                        }
                    }
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
                if (m_encryptor == null)
                    m_encryptor = m_algorithm.CreateEncryptor();

                using (CryptoStream cryptoStream = new CryptoStream(outStream, m_encryptor, CryptoStreamMode.Write))
                {
                    int BUFFER_LEN = m_encryptor.InputBlockSize * 10;
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
                if (m_decryptor == null)
                    m_decryptor = m_algorithm.CreateDecryptor();

                using (CryptoStream cryptoStream = new CryptoStream(inStream, m_decryptor, CryptoStreamMode.Read))
                {
                    int BUFFER_LEN = m_decryptor.OutputBlockSize * 10;
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

        private void KeyObsolete()
        {
            if (m_encryptor != null)
            {
                m_encryptor.Dispose();
                m_encryptor = null;
            }
            if (m_decryptor != null)
            {
                m_decryptor.Dispose();
                m_decryptor = null;
            }
        }

        public void Dispose()
        {
            if (m_encryptor != null)
            {
                m_encryptor.Dispose();
                m_encryptor = null;
            }
            if (m_decryptor != null)
            {
                m_decryptor.Dispose();
                m_decryptor = null;
            }
            if (m_algorithm != null)
            {
                m_algorithm.Clear();
                m_algorithm = null;
            }
        }
    }
}
