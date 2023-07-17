using Microsoft.Extensions.Configuration;
//using Microsoft.Extensions.Hosting;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace OracleHelper.TransactSql
{
    public class InIService
    {
        /// <summary>
        /// 傳入INI中設定的Key值取出Connection String
        /// </summary>
        /// <param name="configRoot"></param>
        /// <param name="connName"></param>
        /// <returns></returns>
        /// <exception cref="OracleHelperException"></exception>
        public Dictionary<string, string> ConnectStringGet(IConfigurationRoot configRoot, string connName = "Default")
        {
            if (string.IsNullOrEmpty(connName))
            {
                throw new OracleHelperException("connName is empty.");
            }

            Dictionary<string, string> result = new Dictionary<string, string>();
            try
            {
                string getINIPath = configRoot.GetValue<string>("ConnectionSettings:INIPath");
                getINIPath = string.IsNullOrWhiteSpace(getINIPath) ? @"C:\Terminal\ConnectString.ini" : getINIPath;

                List<string> members = configRoot.GetSection("ConnectionSettings:Members")
                                                 .AsEnumerable()
                                                 .Where(x => string.IsNullOrEmpty(x.Value) == false && x.Value != connName)
                                                 .Select(x => x.Value.ToSafeString()).ToList();
                members.Insert(0, connName);

                string key = "";
                string ecryptStr = "";
                StreamReader objReader = new StreamReader(getINIPath);
                string readIni = "Start";
                while (string.IsNullOrEmpty(readIni) == false)
                {
                    readIni = objReader.ReadLine();
                    if (string.IsNullOrEmpty(readIni)) break;

                    key = readIni.Split(' ').First();
                    if (result.ContainsKey(key)) continue;
                    if (members.Contains(key))
                    {
                        ecryptStr = readIni.Substring(readIni.IndexOf(' ') + 1).TrimEnd('\n');
                        result.Add(key, Decrypt(ecryptStr));
                        continue;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new OracleHelperException($"{MethodBase.GetCurrentMethod()} Exception.", ex);
            }
            return result;
        }

        private string Decrypt(string pToDecrypt, string sKey = "evaairno")
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
                result = "";
            }
            return result;
        }

        private String Encrypt(String pToEncrypt, String sKey = "evaairno")
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
    }
}
