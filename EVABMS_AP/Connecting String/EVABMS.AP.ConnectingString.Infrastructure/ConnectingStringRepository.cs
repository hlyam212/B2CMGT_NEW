using System.Reflection;
using OracleHelper.TransactSql;
using EVABMS.AP.ConnectingString.Domain.Entities;
using EncryptionHelper;
using System.Text;
using CommonHelper;

namespace EVABMS.AP.ConnectingString.Infrastructure
{
    public class ConnectingStringRepository
    {
        public readonly string address = $"{Path.GetDirectoryName(Assembly.GetEntryAssembly().Location)}\\connlibrary";
        public readonly string defaultININame = @"ConnectString.ini";

        public List<string> Getfilename(string address)
        {
            var executingPath = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            DirectoryInfo directoryInfo = new(address);
            List<string> files = (from file in directoryInfo.GetFiles()
                                  where file.Name.Contains(".ini")
                                  select file.Name).ToList();
            return files;
        }

        public string Getfile(string address = null, string defaultININame = null)
        {
            address= address.IsNullOrEmpty()?this.address: address;
            defaultININame = defaultININame.IsNullOrEmpty() ? this.defaultININame : defaultININame;

            string iniPath = $@"{address}\{defaultININame}";

            string result = "";
            using (StreamReader reader = new StreamReader(iniPath))
            {
                result = reader.ReadToEnd();
                reader.Dispose();
            }

            return result;
        }

        public List<ConnectingStringQuery> InIDecrypt(string inicontent)
        {
            byte[] buffer = Encoding.ASCII.GetBytes(inicontent);
            MemoryStream stream = new MemoryStream(buffer);

            StreamReader reader = new(stream);
            string readstring = reader.ReadLine();
            string Valuestring;
            List<ConnectingStringQuery> connectingStringQueryModels = new();
            long no = 1;
            while (readstring != null)
            {

                //分解原字串
                List<string> content = readstring.Split(' ').ToList();

                //解密後資料
                Valuestring = ConnectionStringService.Decrypt(content[1], "evaairno");

                #region 處理解密後字串
                List<string> col = new List<string> { "DATA SOURCE", "USER ID", "PASSWORD", "USER ROLE", "Others" };
                ConnectingStringQuery data = new ConnectingStringQuery();
                foreach (var type in col)
                {
                    List<string> tempCont = null;
                    if (type == "Others")
                    {
                        tempCont = new List<string> { type, Valuestring };
                    }
                    else
                    {
                        if (Valuestring.IsNullOrEmpty())
                        {
                            tempCont = new List<string>() { type, "" };
                        }
                        else
                        {
                            tempCont = Valuestring.Substring(0, Valuestring.IndexOf(';')).Split(new char[] { '=' }, 2).ToList();
                            if (type != tempCont[0]) continue;
                            Valuestring = Valuestring.Remove(0, Valuestring.IndexOf(";") + 1);
                        }

                    }
                    data.SetValue(tempCont[0], tempCont[1]);
                }
                #endregion

                connectingStringQueryModels.Add(
                    ConnectingStringQuery.Create(no,
                                                 content[0],
                                                 data.DataSource,
                                                 data.UserId,
                                                 data.Password,
                                                 data.Other,
                                                 data.UserRole.IsNullOrEmpty() ? "" : data.UserRole)
                );
                readstring = reader.ReadLine();
                no++;
            }
            reader.Close();
            return connectingStringQueryModels;

        }

        public string InIEncrypt(List<ConnectingStringQuery> models)
        {
            StringBuilder result = new StringBuilder();

            string encstring;
            string unenc;
            string ininame;
            foreach (ConnectingStringQuery model in models)
            {
                ininame = model.Name;
                //unenc = $"DATA SOURCE={model.DataSource};USER ID={model.UserId};PASSWORD={model.Password};USER ROLE={model.UserRole};{model.Other}";
                //[2023/07/10 Helen]因為其他舊功能讀INI寫得很爛，加欄位就會死，所以先把USER ROLE拿掉，以後把爛功能修好再存User Role
                unenc = $"DATA SOURCE={model.DataSource};USER ID={model.UserId};PASSWORD={model.Password};{model.Other}";
                encstring = ConnectionStringService.Encrypt(unenc);

                result.AppendLine(ininame + " " + encstring);
            }
            return result.ToString();
        }

        public bool UpdateRecord(List<ConnectingStringQuery> oldmodel, List<ConnectingStringQuery> newmodel, string userid)
        {
            bool result = false;

            OracleService ora = new();
            OracleKeyService orakey = new();
            ModelComparer<ConnectingStringQuery> compares = ModelComparer.Create(oldmodel, newmodel, m => m.Name);

            long? id = orakey.GenerateKeyWithDual("MINH_SEQ");
            ConnectStringDBModel model = ConnectStringDBModel.Create(id == null ? 0 : id.Value,
                                                                     $@"{DateTime.Now:yyMMdd_HHmm}.ini",
                                                                     $"Insert:{compares.Insert.Count()} Delete:{compares.Delete.Count()} Update:{compares.Update.Count()}",
                                                                     DateTime.Now,
                                                                     userid,
                                                                     null
                                                                     );
            if (id == 0) return false;
            result = ora.Insert(model);
            return result;
        }

        public bool InIFileManagement(string InIEncryped)
        {
            bool result;

            string now = DateTime.Now.ToString("yyMMdd_HHmm");
            string ConnectPath = $@"{address}\{defaultININame}";
            string bakPath = $@"{address}\bak_{now}.ini";

            //Backup
            if (File.Exists(bakPath))
            {
                File.Delete(bakPath);
            }
            File.Copy(ConnectPath, bakPath);

            using (var stream = File.OpenWrite(ConnectPath)) { }
            File.Delete(ConnectPath);

            //Create a file to write to.
            FileInfo step2 = new(ConnectPath);
            StreamWriter writer = step2.CreateText();
            writer.Write(InIEncryped);
            writer.Close();

            result = true;

            return result;
        }
    }
}