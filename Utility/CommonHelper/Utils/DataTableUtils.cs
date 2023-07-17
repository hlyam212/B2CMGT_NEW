using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace CommonHelper
{
    /// <summary>
    /// 針對DataSet、DataTable相關功能
    /// </summary>
    public class DataTableUtils
    {
        /// <summary>
        /// 建立空白的DataTable
        /// </summary>
        /// <param name="ColumnNames">DataTable的欄位</param>
        public static DataTable CreateTable(params string[] ColumnNames)
        {
            string tableSchema = "<Table>";
            for (int i = 0; i < ColumnNames.Length; i++)
            {
                tableSchema += $@"<{ColumnNames[i]}></{ColumnNames[i]}>";
            }
            tableSchema += "</Table>";

            DataSet dsTemp = new DataSet();
            dsTemp.ReadXml(XDocument.Parse(tableSchema).CreateReader());
            dsTemp.Clear();

            return dsTemp.Tables[0];
        }
    }
}
