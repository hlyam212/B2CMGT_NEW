using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace CommonHelper
{
    /// <summary>
    /// 針對Xml的相關延伸處理
    /// </summary>
    public static class XmlExtensions
    {
        /// <summary>
        /// 移除值為空的xml tag
        /// </summary>
        /// <param name="xml">欲移除空白值的Xml Element</param>
        /// <param name="nullList">移除的對象(Xml Tag)</param>
        public static string RemoveNull(this XElement xml, List<string> nullList)
        {
            if (string.IsNullOrEmpty(xml.Value))
            {
                nullList.Add(xml.Name.ToString());
                return "";
            }
            return xml.Value;
        }

        /// <summary>
        /// 組成XML的字串
        /// </summary>
        public static string SetXml(this string keyName, string value)
        {
            if (string.IsNullOrEmpty(value)) return string.Empty;

            return $@"<{keyName}>{value}</{keyName}>";
        }

        /// <summary>
        /// 將物件(object)轉成XML型態的字串(string)
        /// </summary>
        /// <param name="obj">欲轉換的物件(object)</param>
        public static string Serialize(object obj)
        {
            if (obj == null) return string.Empty;

            var emptyNamespaces = new XmlSerializerNamespaces(new[] { XmlQualifiedName.Empty });
            var serializer = new XmlSerializer(obj.GetType());
            var settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.OmitXmlDeclaration = true;

            using (var stream = new StringWriter())
            using (var writer = XmlWriter.Create(stream, settings))
            {
                serializer.Serialize(writer, obj, emptyNamespaces);
                return stream.ToString();
            }
        }
        /// <summary>
        /// 取得XML的值(string)
        /// </summary>
        /// <param name="element">欲取得值的Xml Element</param>
        public static string GetXmlValue(this XElement element)
        {
            return (element != null)? element.Value : "";
        }

        /// <summary>
        /// 取得該XML Element中，指定Key Name的值
        /// </summary>
        /// <param name="element">欲取得值的Xml Element</param>
        /// <param name="keyName">欲取得的Xml Tag Name</param>
        /// <returns></returns>
        public static string? GetXmlValue(this XElement element, string keyName)
        {
            if (element == null) return string.Empty;

            XNamespace ns = element.Name.Namespace; 
            
            try
            {
                var keyValue = element.Descendants(ns + keyName)
                    .Select(x => x.Value)
                    .FirstOrDefault();
                return (keyValue != null) ? keyValue.Trim() : "";
            }
            catch
            {
                return "";
            }
        }

        /// <summary>
        /// 字串轉換xdocument
        /// </summary>
        /// <param name="xmlstring"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public static System.Xml.Linq.XDocument ConvertToXDocument(this String xmlstring)
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


        public static XElement GetInnerXml(this XElement mElement, string Xname)
        {
            if (Xname.Substring(0, 1) == ".")
            {
                Xname = Xname.Substring(2, (Xname.Count() - 2));
            }
            string[] Xptahs = Xname.Split('/');
            XNamespace ns = mElement.Name.NamespaceName;

            IEnumerable<XElement> xml_Elements = from l_Element in mElement.Elements(ns + Xptahs[0])
                                                 select l_Element;
            if (xml_Elements.Count() == 0)
            { return null; }

            XElement xml_value = xml_Elements.FirstOrDefault();
            if (Xptahs.Count() == 1)
                return xml_value;
            else
                for (int i = 1; i < Xptahs.Count(); i++)
                {
                    xml_value = (from l_Element in xml_value.Elements(ns + Xptahs[i])
                                 select l_Element).FirstOrDefault();
                }
            return xml_value;
        }

    }
}
