using EncryptionHelper.Enums;
using EncryptionHelper.Utils;
using System.Security.Cryptography;
using System.Text;

namespace EncryptionHelper
{
    /// <summary>
    /// 針對加解密的相關功能
    /// </summary>
    public static class EncryptionService 
    {
        #region 進行 特殊符號移除 的加解密
        /// <summary>
        /// 使用AES進行加密 (特殊符號移除，包含+,/,=)
        /// </summary>
        /// <param name="rawString">明文</param>
        /// <param name="key">金鑰</param>
        /// <param name="salt">SALT</param>
        /// <param name="encryptionMode">加密模式，預設為不加入日期</param>
        /// <returns>密文</returns>
        public static string AesEncrypt(this string rawString, string key, string salt, EncryptionMode encryptionMode = EncryptionMode.NotByDate)
        {
            var aes = Aes.Create();
            var keyIv = new AesKeyIv(aes, key, salt, encryptionMode);
            aes.Key = keyIv.Key;
            aes.IV = keyIv.IV;
            var rawData = Encoding.UTF8.GetBytes(rawString);
            using (var ms = new MemoryStream())
            {
                using (var cs = new CryptoStream(ms, aes.CreateEncryptor(aes.Key, aes.IV), CryptoStreamMode.Write))
                {
                    cs.Write(rawData, 0, rawData.Length);
                    cs.FlushFinalBlock();
                    return Convert.ToBase64String(ms.ToArray()).Replace("+", "-").Replace("/", "_").Replace("=", "."); //在url中特殊字元會影響資料,需要將其替換;
                }
            }
        }

        /// <summary>
        /// (特殊符號移除，包含+,/,=)
        /// </summary>
        /// <param name="cipherText">密文</param>
        /// <param name="key">金鑰</param>
        /// <param name="salt">SALT</param>
        /// <param name="encryptionMode">加密模式，預設為不加入日期</param>
        /// <returns>明文</returns>
        public static string AesDecrypt(this string cipherText, string key, string salt, EncryptionMode encryptionMode = EncryptionMode.NotByDate)
        {
            var aes = Aes.Create();
            var keyIv = new AesKeyIv(aes, key, salt, encryptionMode);
            aes.Key = keyIv.Key;
            aes.IV = keyIv.IV;
            var encData = Convert.FromBase64String(cipherText.Replace("-", "+").Replace("_", "/").Replace(".", "="));
            byte[] buffer = new byte[encData.Length];
            using (var ms = new MemoryStream(encData))
            {
                using (var cs = new CryptoStream(ms, aes.CreateDecryptor(aes.Key, aes.IV), CryptoStreamMode.Read))
                {
                    using (var sr = new StreamReader(cs))
                    {
                        using (var dec = new MemoryStream())
                        {
                            cs.CopyTo(dec);
                            return Encoding.UTF8.GetString(dec.ToArray());
                        }
                    }
                }
            }
        }
        #endregion

        #region 無 進行特殊符號 的加解密
        /// <summary>
        /// 使用AES進行加密 (無進行特殊符號)
        /// </summary>
        /// <param name="rawString">明文</param>
        /// <param name="key">金鑰</param>
        /// <returns>密文</returns>
        public static string AesEncrypt(this string rawString, string key)
        {
            var encodingKey = Encoding.UTF8.GetBytes(key);

            if (encodingKey.Length != 32)
            {
                throw new ArgumentException("Invalid key size. Key size must be 256 bits.");
            }

            using (var aesAlg = Aes.Create())
            {
                aesAlg.KeySize = 256;
                aesAlg.Mode = CipherMode.CBC;
                aesAlg.Padding = PaddingMode.PKCS7;
                aesAlg.Key = encodingKey;

                using (var encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV))
                {
                    using (var msEncrypt = new MemoryStream())
                    {
                        using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                        using (var swEncrypt = new StreamWriter(csEncrypt))
                        {
                            swEncrypt.Write(rawString);
                        }

                        var iv = aesAlg.IV;

                        var decryptedContent = msEncrypt.ToArray();

                        var result = new byte[iv.Length + decryptedContent.Length];

                        Buffer.BlockCopy(iv, 0, result, 0, iv.Length);
                        Buffer.BlockCopy(decryptedContent, 0, result, iv.Length, decryptedContent.Length);

                        return Convert.ToBase64String(result);
                    }
                }
            }
        }

        /// <summary>
        /// AES解密  (無進行特殊符號)
        /// </summary>
        /// <param name="cipherText">密文</param>
        /// <param name="key">金鑰</param>
        /// <returns>明文</returns>
        public static string AesDecrypt(this string cipherText, string key)
        {
            var fullCipher = Convert.FromBase64String(cipherText);

            var iv = new byte[16];
            var cipher = new byte[fullCipher.Length - iv.Length]; // Fix: cipher should contain the remaining bytes of fullCipher after iv

            Buffer.BlockCopy(fullCipher, 0, iv, 0, iv.Length);
            Buffer.BlockCopy(fullCipher, iv.Length, cipher, 0, cipher.Length); // Fix: copy the remaining bytes into cipher array
            var encodingKey = Encoding.UTF8.GetBytes(key);

            if (encodingKey.Length != 32)
            {
                throw new ArgumentException("Invalid key size. Key size must be 256 bits.");
            }

            using (var aesAlg = Aes.Create())
            {
                aesAlg.KeySize = 256;
                aesAlg.Mode = CipherMode.CBC; // 密碼分組連結
                aesAlg.Padding = PaddingMode.PKCS7; // 填充模式 - PKCS7 不僅僅是對 8 Byte填充，其BlockSize範圍是 1-255 Byte
                aesAlg.Key = encodingKey;

                using (var decryptor = aesAlg.CreateDecryptor(aesAlg.Key, iv))
                {
                    string result;
                    using (var msDecrypt = new MemoryStream(cipher))
                    {
                        using (var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                        {
                            using (var srDecrypt = new StreamReader(csDecrypt))
                            {
                                result = srDecrypt.ReadToEnd();
                            }
                        }
                    }

                    return result;
                }
            }
        }
        #endregion
    }
}
