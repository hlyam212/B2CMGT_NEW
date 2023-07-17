using System.Text.Json;
using System.Text.Json.Serialization;
using System.Data;

namespace CommonHelper
{
    /// <summary>
    /// System.Text.Json 序列化與反序列化設定
    /// JsonExtensions內使用
    /// </summary>
    public class DataSetConverter : JsonConverter<DataSet>
    {
        private class DataSetSerializer
        {
            public string SchemaString { get; set; }
            public string DataString { get; set; }
            private static string GetSchema(DataSet ds)
            {
                using (var sw = new StringWriter())
                {
                    ds.WriteXmlSchema(sw);
                    return sw.ToString();
                }
            }
            private static string GetData(DataSet ds)
            {
                using (var sw = new StringWriter())
                {
                    ds.WriteXml(sw, XmlWriteMode.DiffGram);
                    return sw.ToString();
                }
            }
            private DataSet GetDataSet()
            {
                var ds = new DataSet();
                using (var sr1 = new StringReader(SchemaString))
                {
                    ds.ReadXmlSchema(sr1);
                }
                using (var sr2 = new StringReader(DataString))
                {
                    ds.ReadXml(sr2, XmlReadMode.DiffGram);
                }
                return ds;
            }
            public static string Serialize(DataSet ds)
            {
                var serializer = new DataSetSerializer() { SchemaString = GetSchema(ds), DataString = GetData(ds) };
                return JsonSerializer.Serialize<DataSetSerializer>(serializer);
            }
            public static DataSet DeSerialize(string s)
            {
                var serializer = JsonSerializer.Deserialize<DataSetSerializer>(s);
                return serializer.GetDataSet();
            }
        }
        public override DataSet Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return DataSetSerializer.DeSerialize(reader.GetString());
        }

        public override void Write(Utf8JsonWriter writer, DataSet value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(DataSetSerializer.Serialize(value));
        }
    }
}
