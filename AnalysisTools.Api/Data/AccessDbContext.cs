using analysistools.api.Contracts;
using System.Data;
using System.Data.OleDb;

namespace analysistools.api.Data
{
    /// <summary>
    /// This class allows to query data from an access database.
    /// </summary>
    public class AccessDbContext : IDbContext
    {
        private static readonly Lazy<AccessDbContext> lazy = new Lazy<AccessDbContext>(() => new AccessDbContext());
        public static AccessDbContext Instance { get => lazy.Value; }

        private OleDbConnection accessConnection;
        OleDbDataReader reader;

        // NOTE
        // Is recomended to not store this information in the code.
        string connectionString = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=\\ngd0123w\03_Source\ASGARD.mdb;Jet OLEDB:Database Password=Hardwareinfo;";
        private AccessDbContext()
        {
            accessConnection = new OleDbConnection(connectionString);
        }

        public DataTable RunQuery(string Query)
        {
            DataTable result = new DataTable();

            try
            {
                accessConnection.Open();
                OleDbCommand command = new OleDbCommand(Query, accessConnection);
                OleDbDataAdapter adapter = new OleDbDataAdapter(command);
                adapter.Fill(result);
                accessConnection.Close();
            }
            catch (Exception ex) { return result; }
            return result;
        }
    }
}

