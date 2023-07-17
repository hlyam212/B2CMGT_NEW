namespace OracleAttribute.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class TableNameAttribute : Attribute
    {
        public TableNameAttribute(string name, bool isTrack = false)
        {
            Name = name;
            IsTrack = isTrack;
        }

        /// <summary>
        /// Table Name
        /// </summary>
        public string Name { get; private set; }

        public bool IsTrack { get; private set; }
    }
}
