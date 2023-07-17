using EncryptionHelper.Enums;
using EncryptionHelper.Utils;
using System.Configuration;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace EncryptionHelper
{
    /// <summary>
    /// 針對加解密的相關功能
    /// </summary>
    public static partial class EncryptionOrgService
    {
        public static string GDPREncrypt(string as_Content)
        {
            return EncryptExtention(CipherMode.CFB, as_Content, "GDPR_KEY");
        }
        public static string GDPRDecrypt(string as_Content)
        {
            return DecryptExtention(CipherMode.CFB, as_Content, "GDPR_KEY");
        }
        public static string POSTEncrypt(string as_Content)
        {
            return EncryptExtention(CipherMode.CBC, as_Content, "POST_KEY");
        }
        public static string POSTDecrypt(string as_Content)
        {
            return DecryptExtention(CipherMode.CBC, as_Content, "POST_KEY");
        }
        public static string AES256Encrypt(string text, string key, string iv)
        {
            AESAlgorithm lo_AES = new AESAlgorithm() { Mode = CipherMode.CBC };
            string result = lo_AES.aesEncryptBase64(text, key, iv);
            return result;
        }
        public static string AES256Decrypt(string text, string key, string iv)
        {
            AESAlgorithm lo_AES = new AESAlgorithm() { Mode = CipherMode.CBC };
            string result = lo_AES.aesDecryptBase64(text, key, iv);
            return result;
        }

        private static string EncryptExtention(CipherMode mode, string as_Content, string key)
        {
            AESAlgorithm lo_AES = new AESAlgorithm() { Mode = mode };
            string result = lo_AES.of_EncryptAES(as_Content, Util.of_GetIniObject(key).SecureCode);
            return result;
        }
        private static string DecryptExtention(CipherMode mode, string as_Content, string key)
        {
            AESAlgorithm lo_AES = new AESAlgorithm() { Mode = mode };
            string result = lo_AES.of_DecryptAES(as_Content, Util.of_GetIniObject(key).SecureCode);
            return result;
        }
    }

    public class AESAlgorithm : IDisposable
    {
        private String ps_Errmsg;
        public CipherMode Mode { get; set; }
        private static Byte[] AES_IV = initIv(16);
        private static Byte[] initIv(int blockSize)
        {
            Byte[] iv = new byte[blockSize];
            for (int i = 0; i < blockSize; i++)
            {
                iv[i] = (Byte)0x0;
            }
            return iv;

        }

        public String of_EncryptAES(String input, String pass)
        {
            String encrypted = "";
            Byte[] Buffer;

            try
            {
                Byte[] temp = ConvertStringToByte(pass);
                using (RijndaelManaged AES = new RijndaelManaged())
                {
                    AES.Key = temp;
                    AES.Mode = this.Mode;   //CipherMode改由外部傳入  2019/12/09  SIMON
                    AES.Padding = PaddingMode.Zeros;
                    AES.IV = AES_IV;
                    ICryptoTransform DESEncrypter = AES.CreateEncryptor();

                    Buffer = Encoding.UTF8.GetBytes(HttpUtility.HtmlEncode(input));
                    encrypted = ConvertByteToString(DESEncrypter.TransformFinalBlock(Buffer, 0, Buffer.Length));
                }
            }
            catch (Exception ex)
            {
                ps_Errmsg = ex.Message;
            }
            finally
            {
                Buffer = null;
            }
            return encrypted;
        }

        public String of_DecryptAES(String input, String pass)
        {
            String decrypted = "";
            Byte[] Buffer;
            String[] la_input = input.Split(';');
            try
            {
                Byte[] temp = ConvertStringToByte(pass);
                using (RijndaelManaged AES = new RijndaelManaged())
                {
                    AES.Key = temp;
                    AES.Mode = this.Mode;   //CipherMode改由外部傳入  2019/12/09  SIMON
                    AES.Padding = PaddingMode.Zeros;
                    AES.IV = AES_IV;
                    using (ICryptoTransform DESDecrypter = AES.CreateDecryptor())
                    {
                        Buffer = ConvertStringToByte(input);
                        decrypted = HttpUtility.HtmlDecode(Encoding.UTF8.GetString(DESDecrypter.TransformFinalBlock(Buffer, 0, Buffer.Length))).TrimEnd('\0');
                    }
                }
            }
            catch (Exception ex)
            {
                ps_Errmsg = ex.Message;
            }
            finally
            {
                Buffer = null;
            }
            return decrypted;
        }

        private static String ConvertByteToString(Byte[] inputData)
        {
            StringBuilder sb = new StringBuilder(inputData.Length * 2);
            foreach (Byte b in inputData)
            {
                sb.Append(b.ToString("X2"));
            }
            return sb.ToString();
        }

        private static Byte[] ConvertStringToByte(String inputString)
        {
            if (inputString == null || inputString.Length < 2)
            {
                throw new ArgumentException();
            }
            int len = inputString.Length / 2;
            Byte[] result = new Byte[len];
            for (int i = 0; i < len; i++)
            {
                result[i] = Convert.ToByte(inputString.Substring(2 * i, 2), 16);
            }
            return result;
        }

        #region AES
        /// <summary>
        /// 
        /// </summary>
        /// <param name="text"></param>
        /// <param name="key"></param>
        /// <param name="iv"></param>
        /// <returns></returns>
        public string aesEncryptBase64(string text, string key, string iv)
        {
            var sourceBytes = Encoding.UTF8.GetBytes(text);
            var aes = new RijndaelManaged();
            aes.Mode = Mode;
            aes.Padding = PaddingMode.PKCS7;
            aes.Key = Encoding.UTF8.GetBytes(key);
            aes.IV = Encoding.UTF8.GetBytes(iv);
            var transform = aes.CreateEncryptor();
            return Convert.ToBase64String(transform.TransformFinalBlock(sourceBytes, 0, sourceBytes.Length));
        }

        /// <summary>
        /// 字串解密(非對稱式)
        /// </summary>
        /// <param name="Source">解密前字串</param>
        /// <param name="CryptoKey">解密金鑰</param>
        /// <returns>解密後字串</returns>
        public string aesDecryptBase64(string text, string key, string iv)
        {
            var encryptBytes = Convert.FromBase64String(text);
            var aes = new RijndaelManaged();
            aes.Mode = Mode;
            aes.Padding = PaddingMode.PKCS7;
            aes.Key = Encoding.UTF8.GetBytes(key);
            aes.IV = Encoding.UTF8.GetBytes(iv);
            var transform = aes.CreateDecryptor();
            return Encoding.UTF8.GetString(transform.TransformFinalBlock(encryptBytes, 0, encryptBytes.Length));
        }
        #endregion

        #region Hash Algorithm
        public String of_EncryptHmacSHA256(String input, String key)
        {
            if (input == null || key == null)
            {
                throw new ArgumentException();
            }

            String ls_EnSHA256 = "";

            try
            {
                UTF8Encoding Encoder = new UTF8Encoding();
                Byte[] ls_DataBytes = Encoder.GetBytes(input);
                Byte[] ls_KeyBytes = Encoder.GetBytes(key);
                HMACSHA256 lo_HmacSHA256 = new HMACSHA256(ls_KeyBytes);
                Byte[] ls_Hash256Bytes = lo_HmacSHA256.ComputeHash(ls_DataBytes);

                ls_EnSHA256 = BitConverter.ToString(ls_Hash256Bytes);
                ls_EnSHA256 = ls_EnSHA256.Replace("-", "").ToLower();
            }
            catch (Exception ex)
            {
                ps_Errmsg = ex.Message;
            }

            return ls_EnSHA256;
        }

        public String of_EncryptHmacSHA512(String input, String key)
        {
            if (input == null || key == null)
            {
                throw new ArgumentException();
            }

            String ls_EnSHA512 = "";

            try
            {
                UTF8Encoding Encoder = new UTF8Encoding();
                Byte[] ls_DataBytes = Encoder.GetBytes(input);
                Byte[] ls_KeyBytes = Encoder.GetBytes(key);
                HMACSHA512 lo_HmacSHA512 = new HMACSHA512(ls_KeyBytes);
                Byte[] ls_Hash512Bytes = lo_HmacSHA512.ComputeHash(ls_DataBytes);

                ls_EnSHA512 = BitConverter.ToString(ls_Hash512Bytes);
                ls_EnSHA512 = ls_EnSHA512.Replace("-", "").ToLower();
            }
            catch (Exception ex)
            {
                ps_Errmsg = ex.Message;
            }

            return ls_EnSHA512;
        }

        #endregion


        void IDisposable.Dispose()
        {

        }
    }

    public class Util
    {
        public static iniModel of_GetIniObject(String as_ID)
        {
            iniModel lo_result = new iniModel()
            {
                Key = as_ID,
            };

            using (ConnStr lo_ConnStr = new ConnStr())
            {
                String[] la_Key = lo_ConnStr.ConnectString(as_ID).Split(new String[] { ";" }, StringSplitOptions.RemoveEmptyEntries);
                foreach (String ls_item in la_Key)
                {
                    switch (ls_item.Split('=').First())
                    {
                        case "DATA SOURCE":
                            lo_result.DataSource = ls_item.Split('=').Last();
                            break;
                        case "USER ID":
                            lo_result.UserID = ls_item.Split('=').Last();
                            break;
                        case "PASSWORD":
                            lo_result.SecureCode = ls_item.Substring(ls_item.IndexOf('=') + 1);
                            break;
                        default:
                            lo_result.Other = ls_item;
                            lo_result.OtherDict = new Dictionary<String, String>();
                            foreach (String ls_otherItem in lo_result.Other.Split(new String[] { ";" }, StringSplitOptions.RemoveEmptyEntries))
                            {
                                if (ls_otherItem.Contains("="))
                                {
                                    lo_result.OtherDict.Add(ls_otherItem.Split('=').First(), ls_otherItem.Split('=').Last());
                                }
                            }
                            break;
                    }
                }
            }
            return lo_result;
        }

        public class iniModel
        {
            public String Key { get; set; }
            public String DataSource { get; set; }
            public String UserID { get; set; }
            public String SecureCode { get; set; }
            public String Other { get; set; }
            public Dictionary<String, String> OtherDict { get; set; }
            public iniModel()
            {
                this.Key = "";
                this.DataSource = "";
                this.UserID = "";
                this.SecureCode = "";
                this.Other = "";
                this.OtherDict = new Dictionary<String, String>();
            }
        }

        public static String of_GetIniString(String as_ID)
        {
            String ls_result = "";
            using (ConnStr lo_ConnStr = new ConnStr())
            {
                ls_result = lo_ConnStr.ConnectString(as_ID);
            }
            return ls_result;
        }
    }

    public class ConnStr : IDisposable
    {
        public String ConnectString(String p_Type)
        {
            String ls_result = "";
            //修改ini路徑改從Config取得 2017/08/18  SIMON
            String p_IniFilePath = "" + ConfigurationManager.AppSettings["INIPath"];
            if (p_IniFilePath.Length == 0)
            {
                //如沒有設定INI路徑，預設抓C:\Terminal\
                p_IniFilePath = @"C:\Terminal\ConnectString.ini";
            }
            //String p_IniFilePath = "C:\\Terminal\\ConnectString.ini";
            String C_String = "";
            String pS_Line;
            String ps_LogFilename = "ConnectString_" + Environment.MachineName;

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
                    ls_result = DES_Decrypt(C_String, "evaairno");
                }
            }
            catch (Exception ex)
            {
            }
            return ls_result;
        }

        public static String DES_Decrypt(String pToDecrypt, String sKey = "evaairno")
        {
            String lo_result = "";
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
                lo_result = Encoding.Default.GetString(ms.ToArray());
            }
            catch (Exception ex)
            {
                lo_result = "";
            }
            return lo_result;
        }

        public static String DES_Encrypt(String pToEncrypt, String sKey = "evaairno")
        {
            String lo_result = "";
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
                lo_result = ret.ToString();
            }
            catch (Exception ex)
            {
                lo_result = "";
            }
            return lo_result;
        }

        void IDisposable.Dispose()
        {

        }
    }
}
