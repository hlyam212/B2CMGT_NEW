namespace OracleAttribute.Attributes
{
    [AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
    public class ColumnNameAttribute : Attribute
    {
        public ColumnNameAttribute(string name, bool isPrimaryKey = false, bool isEncrypted = false, int[]? indexes = null, bool onlyQuery = false)
        {
            Name = name;
            IsPrimaryKey = isPrimaryKey;
            IsEncrypted = isEncrypted;

            if (indexes != null)
            {
                IsIndex = true;
                IndexCombination = indexes.ToList<int>();
            }

            this.onlyQuery = onlyQuery;
        }

        /// <summary>
        /// Column Name
        /// </summary>
        public string Name { get; private set; }
        /// <summary>
        /// PrimaryKey
        /// </summary>
        public bool IsPrimaryKey { get; private set; }
        /// <summary>
        /// Index
        /// </summary>
        public bool IsIndex { get; private set; } = false;
        /// <summary>
        /// Index 的組合
        /// </summary>
        public List<int> IndexCombination { get; private set; } = new List<int>();
        /// <summary>
        /// 加密欄位
        /// </summary>
        public bool IsEncrypted { get; private set; }
        /// <summary>
        /// 欄位用途
        /// </summary>
        public bool onlyQuery { get; private set; }
    }
}