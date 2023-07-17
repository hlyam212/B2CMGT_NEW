using System.Text.Json;
using System.Text.Json.Serialization;
using System.Data;


namespace CommonHelper
{
    public class DataTableConverter : JsonConverter<DataTable>
    {
        private class DataTableSerializer
        {
            public string SchemaString { get; set; }
            public string DataString { get; set; }

            private static string GetSchema(DataTable dt)
            {
                using (var sw = new StringWriter())
                {
                    dt.WriteXmlSchema(sw);
                    return sw.ToString();
                }
            }
            private static string GetData(DataTable dt)
            {
                using (var sw = new StringWriter())
                {
                    dt.WriteXml(sw, XmlWriteMode.DiffGram);
                    return sw.ToString();
                }
            }
            private DataTable GetDataTable()
            {
                var dt = new DataTable();
                using (var sr1 = new StringReader(SchemaString))
                {
                    dt.ReadXmlSchema(sr1);
                }
                using (var sr2 = new StringReader(DataString))
                {
                    dt.ReadXml(sr2);
                }
                return dt;
            }
            public static string Serialize(DataTable dt)
            {
                //var serializer = new DataTableSerializer() { SchemaString = GetSchema(dt), DataString = GetData(dt) };
                //return JsonConvert.SerializeObject(serializer);


                var serializer = new DataTableSerializer() { SchemaString = GetSchema(dt), DataString = GetData(dt) };
                return JsonSerializer.Serialize<DataTableSerializer>(serializer);
            }
            public static DataTable DeSerialize(string s)
            {
                //var serializer = JsonConvert.DeserializeObject<DataTableSerializer>(s);
                //return serializer.GetDataTable();

                var serializer = JsonSerializer.Deserialize<DataTableSerializer>(s);
                return serializer.GetDataTable();
            }
        }
        public override DataTable Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return DataTableSerializer.DeSerialize(reader.GetString());
        }

        public override void Write(Utf8JsonWriter writer, DataTable  value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(DataTableSerializer.Serialize(dt: value));
        }
        //public override void WriteJson(JsonWriter writer, DataTable value, JsonSerializer serializer)
        //{
        //    if (value == null)
        //        throw new Exception("Passed in DataTable is null");

        //    if (string.IsNullOrEmpty(value.TableName))
        //        throw new Exception("Passed in DataTable Name is null or empty");

        //    writer.WriteValue(DataTableSerializer.Serialize(dt: value));
        //}

        //public override DataTable ReadJson(JsonReader reader, Type objectType, DataTable existingValue, bool hasExistingValue, JsonSerializer serializer)
        //{
        //    return DataTableSerializer.DeSerialize((string)reader.Value);
        //}
    }

}
