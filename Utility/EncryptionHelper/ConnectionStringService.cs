using System.Configuration;
using System.Security.Cryptography;
using System.Text;

namespace EncryptionHelper
{
    public static class ConnectionStringService
    {
        public static string ConnectString(string p_Type)
        {
            string result = "";

            #region 取得INI Path
            //修改ini路徑改從Config取得 2017/08/18  SIMON
            string p_IniFilePath = ConfigurationManager.AppSettings["INIPath"];
            if (string.IsNullOrEmpty(p_IniFilePath))
            {
                //如沒有設定INI路徑，預設抓C:\Terminal\
                p_IniFilePath = @"C:\Terminal\ConnectString.ini";
            }
            #endregion

            string C_String = "";
            string pS_Line;

            try
            {
                using (StreamReader objReader = new StreamReader(p_IniFilePath, System.Text.Encoding.Default))
                {
                    pS_Line = objReader.ReadLine();
                    while (string.IsNullOrEmpty(pS_Line) == false)
                    {
                        if (pS_Line.Split(' ').First() == p_Type)
                        {
                            C_String = pS_Line.Substring(pS_Line.IndexOf(' ') + 1).TrimEnd('\n');
                            break;
                        }
                        pS_Line = objReader.ReadLine();
                    }
                    result = Decrypt(C_String, "evaairno");
                }
            }
            catch (Exception ex)
            {
                result = null;
            }

            return result;
        }

        public static string Decrypt(string pToDecrypt, string sKey = "evaairno")
        {
            string result = "";
            try
            {
                DESCryptoServiceProvider des = new DESCryptoServiceProvider();
                int len = pToDecrypt.Length / 2;
                Byte[] inputByteArray = new Byte[len];
                int x, i;
                for (x = 0; x < len; x++)
                {
                    i = Convert.ToInt32(pToDecrypt.Substring(x * 2, 2), 16);
                    inputByteArray[x] = (Byte)i;
                }
                des.Key = ASCIIEncoding.ASCII.GetBytes(sKey);
                des.IV = ASCIIEncoding.ASCII.GetBytes(sKey);
                MemoryStream ms = new MemoryStream();
                CryptoStream cs = new CryptoStream(ms, des.CreateDecryptor(), CryptoStreamMode.Write);
                cs.Write(inputByteArray, 0, inputByteArray.Length);
                cs.FlushFinalBlock();
                result = Encoding.Default.GetString(ms.ToArray());
            }
            catch (Exception ex)
            {
                result = null;
            }
            return result;
        }

        public static string Encrypt(string pToEncrypt, string sKey = "evaairno")
        {
            string result = "";
            try
            {
                DESCryptoServiceProvider des = new DESCryptoServiceProvider();
                Byte[] inputByteArray;
                inputByteArray = Encoding.Default.GetBytes(pToEncrypt);
                des.Key = ASCIIEncoding.ASCII.GetBytes(sKey);
                des.IV = ASCIIEncoding.ASCII.GetBytes(sKey);
                MemoryStream ms = new MemoryStream();
                CryptoStream cs = new CryptoStream(ms, des.CreateEncryptor(), CryptoStreamMode.Write);
                cs.Write(inputByteArray, 0, inputByteArray.Length);
                cs.FlushFinalBlock();
                StringBuilder ret = new StringBuilder();
                foreach (Byte b in ms.ToArray())
                {
                    ret.AppendFormat("{0:X2}", b);
                }
                result = ret.ToString();
            }
            catch (Exception ex)
            {
                result = null;
            }
            return result;
        }
    }
}
