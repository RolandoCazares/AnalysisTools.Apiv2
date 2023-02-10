using analysistools.api.Contracts;
using Oracle.ManagedDataAccess.Client;
using System.Data;

namespace analysistools.api.Data
{
    /// <summary>
    /// This class allows to query data from Continental MES database.
    /// </summary>
    public class OracleDbContext : IDbContext
    {
        private static readonly Lazy<OracleDbContext> lazy = new Lazy<OracleDbContext>(() => new OracleDbContext());
        public static OracleDbContext Instance { get => lazy.Value; }

        private OracleConnection oracleConnection;
        private OracleDataAdapter oracleDataAdapter;
        // NOTE
        // Store this information outside the project.
        private static string connectionString = "Data Source=(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST=NGDB021-vip.auto.contiwan.com)(PORT=1521))(CONNECT_DATA=(SERVICE_NAME=REPORTING.auto.contiwan.com)));User ID=ANALYSIS_IC;;Password=BigDataAIC123;Min Pool Size=0;";

        private OracleDbContext()
        {
            oracleConnection = new OracleConnection(connectionString);
        }

        public DataTable RunQuery(string Query)
        {
            DataTable result = new DataTable();
            try
            {
                oracleDataAdapter = new OracleDataAdapter(Query, oracleConnection);

                oracleConnection.Open();

                oracleDataAdapter.Fill(result);

                oracleConnection.Close();

            }
            catch (System.Exception)
            {
                return result;
            }
            return result;
        }
    }
}