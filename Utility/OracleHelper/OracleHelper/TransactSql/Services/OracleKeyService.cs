using OracleHelper.TransactSql.Entities;

namespace OracleHelper.TransactSql
{
    public class OracleKeyService
    {
        public int GenerateKey(string sequenceType, string sequenceIndex, int addSeqCount = 1)
        {
            int retrunKey = 0;

            OracleTransactionService oraService = new OracleTransactionService();

            try
            {
                string oraSql = $@"
                   SELECT * FROM TABLE_SEQUENCE
                   WHERE SEQUENCE_TYPE = :SEQUENCE_TYPE AND SEQUENCE_INDEX = :SEQUENCE_INDEX
                   ORDER BY SEQUENCE_NUMBER DESC";

                oraService.SetOraParameters("SEQUENCE_TYPE", sequenceType, OraDataType.Varchar2);
                oraService.SetOraParameters("SEQUENCE_INDEX", sequenceIndex, OraDataType.Varchar2);

                List<TableSequence> sequences = oraService.Select<TableSequence>(oraSql);

                if (sequences == null || sequences.Count == 0)
                {
                    retrunKey = retrunKey + addSeqCount;

                    oraService.Insert(TableSequence.Create(
                        sequenceType: sequenceType,
                        sequenceIndex: sequenceIndex,
                        sequenceNumber: retrunKey));
                }
                else
                {
                    TableSequence originalSequence = sequences.FirstOrDefault();

                    retrunKey = originalSequence.sequenceNumber + addSeqCount;

                    TableSequence modifiedSequence = originalSequence
                        .ShallowCopy()
                        .ChangeSeqNumber(retrunKey);

                    oraService.UpdateByPrimaryKey(updateMoodel: modifiedSequence, originalModel: originalSequence);
                }

                oraService.Commit();
            }
            catch (Exception ex)
            {
                throw new OracleHelperException(ex.Message, ex);
            }

            return retrunKey;
        }

        public long? GenerateKeyWithDual(string seqName)
        {
            if (string.IsNullOrEmpty(seqName)) { return 0; }

            OracleService oraService = new OracleService();

            string oraSql = $@"SELECT {seqName}.NEXTVAL AS ID FROM DUAL";

            return oraService.Select<long>(oraSql).FirstOrDefault();
        }
    }
}
