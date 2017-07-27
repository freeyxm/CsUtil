using System;
using System.Text;
using System.IO;
using CsUtil.Crypto;
using CsUtil.Util;

namespace CsUtil.Test.Crypto
{
    class TestCrypter : TestBase<TestCrypter>
    {
        protected override void Test()
        {
            string str = "abcde12345";
            string str2 = "1234567890";

            CrypterAES encryptor = new CrypterAES();
            CrypterAES decryptor = new CrypterAES();

            //encryptor.RandomKey();
            //decryptor.SetKey(encryptor.Key, encryptor.IV);
            encryptor.SetKey("123", "abc");
            decryptor.SetKey("123", "abc");

            do
            {
                byte[] inputData = Encoding.UTF8.GetBytes(str);
                byte[] encryptedData = encryptor.Encrypt(inputData);
                if (encryptedData == null)
                    break;
                byte[] decryptedData = decryptor.Decrypt(encryptedData);
                if (decryptedData == null)
                    break;
                string decryptedStr = Encoding.UTF8.GetString(decryptedData);
                Logger.Debug("result1 = {0}", str == decryptedStr);
            } while (false);

            do
            {
                byte[] inputData = Encoding.UTF8.GetBytes(str2);
                byte[] encryptedData = encryptor.Encrypt(inputData);
                if (encryptedData == null)
                    break;
                byte[] decryptedData = decryptor.Decrypt(encryptedData);
                if (decryptedData == null)
                    break;
                string decryptedStr = Encoding.UTF8.GetString(decryptedData);
                Logger.Debug("result2 = {0}", str2 == decryptedStr);
            } while (false);

            string file = "E:/Temp/test.png";
            FileUtil.OpenFile(file, (inStream) =>
            {
                FileUtil.OpenFile(file + ".cry", FileMode.Create, (outStream) =>
                  {
                      Logger.Debug("Encrypt begin");
                      encryptor.Encrypt(inStream, outStream);
                      Logger.Debug("Encrypt end");
                  });
            });

            FileUtil.OpenFile(file + ".cry", (inStream) =>
              {
                  FileUtil.OpenFile(file + ".out", FileMode.Create, (outStream) =>
                  {
                      Logger.Debug("Decrypt begin");
                      decryptor.Decrypt(inStream, outStream);
                      Logger.Debug("Decrypt end");
                  });
              });
        }
    }
}
