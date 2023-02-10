using System.Data;

namespace analysistools.api.Contracts
{
    public interface IDbContext
    {
        DataTable RunQuery(string Query);
    }
}
