using analysistools.api.Contracts;
using analysistools.api.Data;
using analysistools.api.Helpers;
using analysistools.api.Models.Continental;
using System.Data;

namespace analysistools.api.Repositories
{
    /// <summary>
    /// This repository allows to search data from an access database file.
    /// </summary>
    public class AccessRepository : IAccessRepository
    {
        private IDbContext _context = AccessDbContext.Instance;

        /// <summary>
        /// Runs a query to retrieve data from LDM relfashed units table.
        /// </summary>
        /// <returns></returns>
        public List<LdmFlashUnit> GetData()
        {            
            DataTable data =_context.RunQuery("SELECT ESN, Turno, Grupo, EOL, Box FROM RFLSC3P1 ");

            List<LdmFlashUnit> result = DataTableHelper.DataTableToLdmFlashList(data);

            return result;
        }

        public List<IccidUnit> GetDataIccid()
        {
            DataTable data = _context.RunQuery("SELECT serie, proyecto, iccid, fecha, host, motivo FROM ICCID_Scrap");

            List<IccidUnit> result = DataTableHelper.DataTableToIccid(data);

            return result;

        }
    }
}
