namespace OracleAttribute.Attributes
{
    /// <summary>
    /// (針對Enum) 定義列舉值轉換至資料庫的值
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public class DataValueAttribute : Attribute
    {
        public DataValueAttribute(string value)
        {
            Value = value;
        }

        /// <summary>
        /// 讀寫 Database 存入值
        /// </summary>
        public string Value { get; private set; }
    }
}
