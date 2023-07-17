using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Unicode;

namespace CommonHelper
{
    /// <summary>
    /// 針對Json的相關延伸處理
    /// </summary>
    public static class JsonExtensions
    {
        private readonly static JavaScriptEncoder Encoder = JavaScriptEncoder.Create(UnicodeRanges.All);


        /// <summary>
        /// 將物件(Object)轉成字串(string)
        /// </summary>
        /// <param name="obj">要轉換的物件</param>
        /// <param name="camelCase">駝峰式大小寫，預設為false</param>
        /// <returns></returns>
        public static string? Serialize(this object obj, bool camelCase = false, JavaScriptEncoder? customEncoder = null)
        {
            if (obj == null)
            {
                return null;
            }
            var objName = obj.GetType().Name;

            if (objName == "DataSet") {
                var serializeOptions = new JsonSerializerOptions { Converters = { new DataSetConverter() } };
                if (customEncoder != null)
                {
                    serializeOptions.Encoder = customEncoder;
                }
                return JsonSerializer.Serialize(obj, serializeOptions);
            }
            if (objName == "DataTable")
            {
                var serializeOptions = new JsonSerializerOptions { Converters = { new DataTableConverter() } };
                if (customEncoder != null)
                {
                    serializeOptions.Encoder = customEncoder;
                }
                return JsonSerializer.Serialize(obj, serializeOptions);
            }
            JsonSerializerOptions opt = new JsonSerializerOptions { 
                Encoder = Encoder,
            };

            if (customEncoder != null) {
                opt.Encoder = customEncoder;
            }
            if (camelCase)
            {
                opt.DictionaryKeyPolicy = JsonNamingPolicy.CamelCase;
                opt.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
                opt.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
            }
            return JsonSerializer.Serialize(obj, obj.GetType(), opt);
        }
        /// <summary>
        /// 將字串(string)轉成物件(Object)
        /// </summary>
        /// <typeparam name="T">要轉出的物件(Object)</typeparam>
        /// <param name="json">要轉譯成Json格式的字串(string)</param>
        /// <param name="camelCase">駝峰式大小寫，預設為false</param>
        /// <returns></returns>
        public static T? Deserialize<T>(this string json, bool camelCase = false, JavaScriptEncoder? customEncoder = null)
        {
            if (json == null)
            {
                return default(T);
            }

            var objName = typeof(T).Name;            
            if (objName == "DataSet") {
                var serializeOptions = new JsonSerializerOptions { Converters = { new DataSetConverter() } };
                if (customEncoder != null)
                {
                    serializeOptions.Encoder = customEncoder;
                }
                return JsonSerializer.Deserialize<T>(json, serializeOptions);
            }
            if (objName == "DataTable")
            {
                var serializeOptions = new JsonSerializerOptions { Converters = { new DataTableConverter() } };
                if (customEncoder != null)
                {
                    serializeOptions.Encoder = customEncoder;
                }
                return JsonSerializer.Deserialize<T>(json, serializeOptions);
            }

            JsonSerializerOptions opt = new JsonSerializerOptions 
            {
                Encoder = Encoder,
            };
            if (customEncoder != null)
            {
                opt.Encoder = customEncoder;
            }
            if (camelCase)
            {
                opt.DictionaryKeyPolicy = JsonNamingPolicy.CamelCase;
                opt.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
            }

            return JsonSerializer.Deserialize<T>(json, opt);
        }

        public static T? ConvertObject<T>(this JsonObject json, bool camelCase = false)
        {            
            return json.ToString().Deserialize<T>();
        }

        public static T? ConvertObject<T>(this JsonNode json, bool camelCase = false)
        {
            return json.ToString().Deserialize<T>();
        }

        /// 當變數可能會是null 轉string 可以用ToSafeString 會回傳string.Empty
        /// <summary>
        /// 當變數可能會是null 轉string 可以用ToSafeString 會回傳string.Empty
        /// </summary>
        /// <returns></returns>
        public static string ToSafeString(this object SourceObject)
        {
            return (SourceObject ?? string.Empty).ToString();
        }
    }
}
