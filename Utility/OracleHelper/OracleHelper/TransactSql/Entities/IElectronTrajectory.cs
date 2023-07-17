namespace OracleHelper.TransactSql
{
    public interface IElectronTrajectory
    {
        string oraSql { get; set; }
        List<string> oraSqlList { get; set; }
        List<OraParameters> oraParams { get; set; }
        List<List<OraParameters>> oraParamsList { get; set; }

        void SetOraParameters(string name, string value, OraDataType type);
        void SetOraParameters(string name, DateTime value, OraDataType type);
        void AddOraParametersList(string oraSql);

        void Excute();
    }
}
