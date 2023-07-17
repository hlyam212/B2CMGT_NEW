using EncryptionHelper.Enums;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;

namespace EncryptionHelper.Utils
{
    public class AesKeyIv
    {
        public Byte[] Key = new Byte[16];
        public Byte[] IV = new Byte[16];

        public AesKeyIv(Aes aes, string strKey, string strSalt, EncryptionMode encryptionBy)
        {
            // 為了避免產生的值都相同,加上以"日"為單位的變數值,ex. ddMMMyyyydddd => 14五月2015星期四
            string hashDate = DateTime.Now.ToString(format: "ddMMMyyyydddd", provider: CultureInfo.CreateSpecificCulture("zh-TW"));

            string tmpKey = (encryptionBy == EncryptionMode.NotByDate) ? strKey : $@"{strKey}{hashDate}";
            byte[] tmpSalt = Encoding.Unicode.GetBytes(
                (encryptionBy == EncryptionMode.NotByDate) ? strSalt : $@"{strSalt}{hashDate}");

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
}
