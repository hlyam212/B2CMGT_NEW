using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Configuration;

namespace CommonHelper
{
    /// <summary>
    /// 針對加解密的相關功能
    /// </summary>
    public class EncryptionUtils
    {
        public enum EncryptionBy
        {
            ByDate,
            NotByDate
        }

        private string key { get; init; }
        private string salt { get; init; }
        private EncryptionBy encryptionBy { get; init; }

        /// <summary>
        /// 自己給key和salt
        /// </summary>
        /// <param name="key"></param>
        /// <param name="salt"></param>
        /// <param name="encryptionBy"></param>
        public EncryptionUtils(string key, string salt, EncryptionBy encryptionBy = EncryptionBy.NotByDate)
        {
            this.key = key;
            this.salt = salt;
            this.encryptionBy = encryptionBy;
        }

        /// <summary>
        /// 固定至主專案檔的appSetting.json下，指定的section取key和salt
        /// </summary>
        /// <param name="keySectionName"></param>
        /// <param name="encryptionBy"></param>
        public EncryptionUtils(string keySectionName, EncryptionBy encryptionBy = EncryptionBy.NotByDate)
        {
            this.key = $@"{keySectionName}:key".GetSetting().Get<string>() ?? "";
            this.salt = $@"{keySectionName}:salt".GetSetting().Get<string>() ?? "";
            this.encryptionBy = encryptionBy;
        }

        /// <summary> 
        /// Key (& Salt)、IV的處理
        /// </summary>
        private class AesKeyIV
        {
            public Byte[] Key = new Byte[16];
            public Byte[] IV = new Byte[16];

            public AesKeyIV(Aes aes, string strKey, string strSalt, EncryptionBy encryptionBy)
            {
                // 為了避免產生的值都相同,加上以"日"為單位的變數值,ex. ddMMMyyyydddd => 14五月2015星期四
                string hashDate = DateTime.Now.ConvertString(format: "ddMMMyyyydddd", cultureInfo: "zh-TW");

                string tmpKey = (encryptionBy == EncryptionBy.NotByDate) ? strKey : $@"{strKey}{hashDate}";
                byte[] tmpSalt = Encoding.Unicode.GetBytes(
                    (encryptionBy == EncryptionBy.NotByDate) ? strSalt : $@"{strSalt}{hashDate}");

                // 使用指定的 password, salt ，由 Rfc2898DeriveBytes 產生一組亂數的 Key, IV
                Rfc2898DeriveBytes rfc = new Rfc2898DeriveBytes(tmpKey, tmpSalt);
                aes.Key = rfc.GetBytes(aes.KeySize / 8);
                aes.IV = rfc.GetBytes(aes.BlockSize / 8);

                var sha = SHA256.Create(); //Key:金鑰 --> 使用SHA256做雜湊
                var md5 = MD5.Create(); //IV:初始向量 --> 使用MD5做雜湊

                Array.Copy(sha.ComputeHash(aes.Key), 0, Key, 0, 16);
                Array.Copy(md5.ComputeHash(aes.IV), 0, IV, 0, 16);
            }
        }

        /// <summary>
        /// 使用AES進行加密
        /// </summary>
        /// <param name="rawString">明文</param>
        /// <returns>加密後的密文</returns>
        public string AesEncrypt(string rawString)
        {
            var aes = Aes.Create();
            var keyIv = new AesKeyIV(aes, key, salt, encryptionBy);
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
        ///  使用AES進行解密
        /// </summary>
        /// <param name="encString">密文</param>
        /// <returns>解密後的明文</returns>
        public string AesDecrypt(string encString)
        {
            var aes = Aes.Create();
            var keyIv = new AesKeyIV(aes, key, salt, encryptionBy);
            aes.Key = keyIv.Key;
            aes.IV = keyIv.IV;
            var encData = Convert.FromBase64String(encString.Replace("-", "+").Replace("_", "/").Replace(".", "="));
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
    }
}
