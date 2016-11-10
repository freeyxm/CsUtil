using System;
using System.Security.Cryptography;

namespace CsUtil.Crypto
{
    public class CrypterAES : Crypter<RijndaelManaged>
    {
    }

    public class CrypterDES : Crypter<DESCryptoServiceProvider>
    {
    }
}
