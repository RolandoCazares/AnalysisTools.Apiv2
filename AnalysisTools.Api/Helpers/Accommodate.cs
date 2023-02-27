using analysistools.api.Models.ProductionTests;
using System.Data;
using analysistools.api.Models.FPY;
using analysistools.api.Models.FPY.HELPERS;
using analysistools.api.Contracts;
using analysistools.api.Data;

namespace analysistools.api.Helpers
{
    public class Accommodate : IAccommodate
    {
        private readonly AppDbContext _context;

        public Accommodate(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Response>> DataTableAccommodate(List<ProducedAndFilteredFPY> producedAndFilteredList, List<FailureFPY> FailuresAndFilter)
        {
            List<Response> result = new List<Response>();
            try
            {
                //---------------------------Intenta Acomodar Producidas--------------//
                int totalProduced = producedAndFilteredList.Sum(x => x.Amount);

                Dictionary<string, int> totalProducedByProcess = producedAndFilteredList
                    .GroupBy(x => x.Var)
                    .ToDictionary(g => g.Key, g => g.Sum(x => x.Amount));

                Dictionary<string, List<object>> dataProducedByProcess = producedAndFilteredList
                .GroupBy(x => x.Var)
                .ToDictionary(
                    g => g.Key,
                    g => g.Select(x => new { x.Date, x.Material, x.Name, x.Var, x.IdType, x.Amount })
                          .ToList<object>()
                );

                List<Dictionary<string, int>> totalProducedByProcessList = new List<Dictionary<string, int>>();
                totalProducedByProcessList.Add(totalProducedByProcess);

                //-----------------------------Intenta Acomodar Falladas------------------//

                int totalFailures = FailuresAndFilter.Count;

                Dictionary<string, int> totalFailuresByProcess = FailuresAndFilter
                .GroupBy(x => x.VAR)
                .ToDictionary(g => g.Key, g => g.Count());

                Dictionary<string, List<object>> dataFailuresByProcess = FailuresAndFilter
                .GroupBy(x => x.VAR)
                .ToDictionary(
                    g => g.Key,
                    g => g.Select(x => new { x.SerialNumber, x.AUFTR, x.DATE, x.MATERIAL, x.NAME, x.VAR, x.IDTYPE, x.BEZ})
                          .ToList<object>()
                );

                List<Dictionary<string, int>> totalFailuresByProcessList = new List<Dictionary<string, int>>();
                totalFailuresByProcessList.Add(totalFailuresByProcess);

                //-----------------Encapsulamiento-------------//
                var Totals = new TotalsFPY
                {
                    TotalProduced = totalProduced,
                    TotalFailures = totalFailures,
                };

                var TotalsByProcess = new TotalsFPYByProcess
                {
                    TotalProducedByProcess = totalProducedByProcessList,
                    TotalFailuresByProcess = totalFailuresByProcessList,
                };

                var DataByProcess = new TotalsFPYDataByProcess
                {
                    ProducedByProcessData = dataProducedByProcess,
                    FailuresByProcessData = dataFailuresByProcess,
                };

                var Response = new Response
                {
                    Totals = Totals,
                    TotalsByProcess = TotalsByProcess,
                    DataByProcess = DataByProcess,
                };


                result.Add(Response);
            }
            catch (Exception) { }
            return result;
        }
    }
}
