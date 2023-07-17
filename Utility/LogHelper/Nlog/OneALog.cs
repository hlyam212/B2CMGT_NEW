using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NLog;
using NLog.Web;
using NLog.Targets;
using System.Xml.Linq;
using System.Xml;
using CommonHelper;
using Microsoft.Extensions.Configuration;

namespace LogHelper.Nlog
{
    public class OneALog
    {
        private Logger logger;


        public OneALog()
        {
            String env = System.Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            logger = NLogBuilder.ConfigureNLog($"NLog.{env}.config").GetLogger("OneA");
            FunArr.Add("Security_Authenticate");
            FunArr.Add("PAY_ValidatePayment");
            LogPath = "";
        }

        private string OneAFileName;

        private List<string> FunArr = new List<string>();

        public class LogInfo
        {
            public string logNameSpace { get; set; }
            public string FunName { get; set; }
            public string UserId { get; set; }
            public string SessionId { get; set; }
            public string SequenceNumber { get; set; }

            public string SecurityToken { get; set; }


            public string SessionStage { get; set; }

        }

        private string LogPath;

        public void LogRequest(string myLogStr, LogInfo setting)
        {
            string LogXML = "";
            LogPath = "";

            #region Log header
            LogXML = "<OperationLOG> ";
            //寫入接收Request時間
            LogXML += "  <RequestTime>" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.fff") + "</RequestTime> ";
            LogXML += "  <ResponseTime></ResponseTime> ";
            //LogXML += "  <DllNameSpace>" + ((setting.logNameSpace != null) ? setting.logNameSpace : "") + "</DllNameSpace> ";
            LogXML += "  <FunName>" + ((setting.FunName != null) ? setting.FunName : "") + "</FunName> ";
            LogXML += "  <UserId>" + ((setting.UserId != null) ? setting.UserId : "") + "</UserId> ";
            //Response 時要回填
            LogXML += "  <SessionId>" + ((setting.SessionId != null) ? setting.SessionId : "") + "</SessionId> ";
            LogXML += "  <SequenceNumber>" + ((setting.SequenceNumber != null) ? setting.SequenceNumber : "") + "</SequenceNumber> ";
            LogXML += "  <SecurityToken>" + ((setting.SecurityToken != null) ? setting.SecurityToken : "") + "</SecurityToken> ";
            #endregion

            #region Request body
            LogXML += "  <LogRequest_Body> ";
            LogXML += myLogStr;
            LogXML += "  </LogRequest_Body> ";
            #endregion

            #region Log footer
            LogXML += "</OperationLOG>";
            #endregion

            if (FunArr.Contains(setting.FunName))
            {
                LogXML = RemoveLogXmlNode(LogXML);//移除信用卡特殊節點
            }
            XDocument myLog = XDocument.Parse(LogXML);
            Save(myLog, setting.UserId, setting.SessionId, setting.FunName);

            FileTarget OneALogTarget = (FileTarget)LogManager.Configuration.FindTargetByName("OneA");
            var logEvent = new LogEventInfo { TimeStamp = DateTime.Now };
            LogPath = OneALogTarget.FileName.Render(logEvent);

        }

        public void LogResponse(string myLog, LogInfo setting)
        {
            if (String.IsNullOrEmpty(LogPath))
            {
                throw new Exception("RequestLogPath is null ,should do LogRequest First");
            }
            if (File.Exists(LogPath) == false)
            {
                throw new Exception("RequestLogPath is not Exists,check LogResponse");
            }

            #region Write Log
            //=============================

            string LogXML = "";
            XDocument totalLogXml = new XDocument();
            if ("EncryptLog".GetSetting().Get<string>() == "Y")
            {
                totalLogXml = ConvertToXDocument(DecryptStringNoByDate(System.IO.File.ReadAllText(LogPath)));
            }
            else
            {
                totalLogXml = XDocument.Load(LogPath);
            }



            #region Response body
            LogXML += "  <LogResponse_Body>";
            LogXML += myLog.ToString();
            LogXML += "  </LogResponse_Body>";
            XElement xmlelement = XElement.Parse(LogXML);
            totalLogXml.Element("OperationLOG").Add(xmlelement);
            #endregion

            #region update xml
            totalLogXml.Element("OperationLOG").Element("SessionId").Value = (setting.SessionId != null) ? setting.SessionId : "";
            totalLogXml.Element("OperationLOG").Element("SequenceNumber").Value = (setting.SequenceNumber != null) ? setting.SequenceNumber : "";
            totalLogXml.Element("OperationLOG").Element("SecurityToken").Value = (setting.SecurityToken != null) ? setting.SecurityToken : "";
            totalLogXml.Element("OperationLOG").Element("ResponseTime").Value = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.fff");
            #endregion


            Save(totalLogXml, setting.UserId, setting.SessionId, setting.FunName);

            //=============================
            #endregion



        }

        private void Save(XDocument myLog, string UserId, string SessionId, string FunName)
        {
            logger = LogManager.GetLogger("OneA");
            if (!String.IsNullOrEmpty(LogPath))
            {
                OneAFileName = Path.GetFileNameWithoutExtension(LogPath);
            }//response
            else
            {
                CryptoRandom mRND_No = new CryptoRandom();
                OneAFileName = DateTime.Now.ToString("yyyyMMddHHmmss") + "_" + FunName + "_" + mRND_No.Next(0, 2147483647).ToString().PadLeft(10, '0');
            }


            NLog.GlobalDiagnosticsContext.Set("OneAFileName", OneAFileName);

            if (String.IsNullOrEmpty(UserId) == false) NLog.GlobalDiagnosticsContext.Set("UserId", UserId);
            if (String.IsNullOrEmpty(SessionId) == false) NLog.GlobalDiagnosticsContext.Set("SessionId", SessionId);
            string logStr = "";
            if ("EncryptLog".GetSetting().Get<string>() == "Y")
            {
                logStr = EncryptStringNoByDate(myLog.ToString());
            }
            else
            {
                logStr = myLog.ToString();
            }

            logger.Info(logStr);

        }
        public System.Xml.Linq.XDocument ConvertToXDocument(String xmlstring)
        {
            try
            {
                XDocument tmpXDocument = new XDocument();
                //tmpXDocument = XDocument.Parse(checkString(xmlstring));
                XmlTextReader MyReader = new XmlTextReader(new System.IO.StringReader(xmlstring)) { EntityHandling = EntityHandling.ExpandCharEntities };
                MyReader.Normalization = false;
                tmpXDocument = XDocument.Load(MyReader);

                return tmpXDocument;
            }
            catch (Exception ex)
            {
                throw new Exception("ConvertToXDocument Fail");
            }
        }
        //儲存XML檔

        private string RemoveLogXmlNode(string LogXML)
        {
            XDocument mXmld_TempLogXML = ConvertToXDocument(LogXML);
            List<string> HideNode = new List<string>();
            HideNode.Add("securityId");
            HideNode.Add("passwordInfo");
            foreach (string HideName in HideNode)
            {
                (from a1 in mXmld_TempLogXML.Root.DescendantsAndSelf()
                 where a1.Name.LocalName == HideName
                 select a1).Remove();
            }
            LogXML = mXmld_TempLogXML.ToString();

            return LogXML;
        }


        private string EncryptStringNoByDate(string str)
        {
            CommonHelper.EncryptionUtils encryp = new CommonHelper.EncryptionUtils("CallApiEncrypt");
            str = encryp.AesEncrypt(str);

            return str;
        }

        private string DecryptStringNoByDate(string str)
        {

            CommonHelper.EncryptionUtils encryp = new CommonHelper.EncryptionUtils("CallApiEncrypt");
            str = encryp.AesDecrypt(str);

            return str;
        }

    }
}
