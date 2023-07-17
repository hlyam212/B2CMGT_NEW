using OracleAttribute.Attributes;
using System.Text.Json.Serialization;

namespace OracleHelper.TransactSql.Entities
{
    [TableName("TABLE_SEQUENCE")]
    public class TableSequence
    {
        #region Constructor
        public TableSequence() { }

        [JsonConstructor]
        public TableSequence(string sequenceType, string sequenceIndex, int sequenceNumber, DateTime lastUpdateDate)
        {
            this.sequenceType = sequenceType;
            this.sequenceIndex = sequenceIndex;
            this.sequenceNumber = sequenceNumber;
            this.lastUpdateDate = lastUpdateDate;
        }
        #endregion


        [ColumnName(name: "SEQUENCE_TYPE", isPrimaryKey: true)]
        public string sequenceType { get; private set; }

        [ColumnName(name: "SEQUENCE_INDEX", isPrimaryKey: true)]
        public string sequenceIndex { get; private set; }

        [ColumnName(name: "SEQUENCE_NUMBER")]
        public int sequenceNumber { get; private set; }

        [ColumnName(name: "LAST_UPDATE_DATE")]
        public DateTime lastUpdateDate { get; private set; }

        public static TableSequence Create(string sequenceType, string sequenceIndex, int sequenceNumber)
        {
            TableSequence tableSequence = new TableSequence();
            tableSequence.sequenceType = sequenceType;
            tableSequence.sequenceIndex = sequenceIndex;
            tableSequence.sequenceNumber = sequenceNumber;
            tableSequence.lastUpdateDate = DateTime.Now;

            return tableSequence;
        }

        public TableSequence ShallowCopy()
        {
            return (TableSequence)this.MemberwiseClone();
        }

        public TableSequence ChangeSeqNumber(int sequenceNumber)
        {
            this.sequenceNumber = sequenceNumber;
            this.lastUpdateDate = DateTime.Now;

            return this;
        }
    }
}
