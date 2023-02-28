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

                //Dictionary<string, List<object>> dataProducedByProcess = producedAndFilteredList
                //.GroupBy(x => x.Var)
                //.ToDictionary(
                //    g => g.Key,
                //    g => g.Select(x => new { x.Date, x.Material, x.Name, x.Var, x.IdType, x.Amount })
                //          .ToList<object>()
                //);

                List<Dictionary<string, int>> totalProducedByProcessList = new List<Dictionary<string, int>>();
                totalProducedByProcessList.Add(totalProducedByProcess);

                //-----------------------------Intenta Acomodar Falladas------------------//

                int totalFailures = FailuresAndFilter.Count;

                Dictionary<string, int> totalFailuresByProcess = FailuresAndFilter
                .GroupBy(x => x.VAR)
                .ToDictionary(g => g.Key, g => g.Count());

                //Dictionary<string, List<object>> dataFailuresByProcess = FailuresAndFilter
                //.GroupBy(x => x.VAR)
                //.ToDictionary(
                //    g => g.Key,
                //    g => g.Select(x => new { x.SerialNumber, x.AUFTR, x.DATE, x.MATERIAL, x.NAME, x.VAR, x.IDTYPE, x.BEZ})
                //          .ToList<object>()
                //);

                List<Dictionary<string, int>> totalFailuresByProcessList = new List<Dictionary<string, int>>();
                totalFailuresByProcessList.Add(totalFailuresByProcess);

                //---------------------FPY porcentaje------------------------//

                Dictionary<string, double> FPYbyProcess = new Dictionary<string, double>();
                List<Dictionary<string, double>> TotalFPYbyProcess = new List<Dictionary<string, double>>();

                foreach (string var in totalProducedByProcess.Keys)
                {
                    if (totalFailuresByProcess.ContainsKey(var))
                    {
                        double fpy = (double)(totalProducedByProcess[var] - totalFailuresByProcess[var]) / totalProducedByProcess[var] * -1 + 100;
                        FPYbyProcess.Add(var, fpy);
                    }
                    else
                    {
                        FPYbyProcess.Add(var, 100);
                    }
                }

                TotalFPYbyProcess.Add(FPYbyProcess);

                //------------------Top 3 fallas por proceso ------------------//

                Dictionary<string, List<object>> Top3FailsByProcess = FailuresAndFilter
                .GroupBy(x => x.VAR)
                .ToDictionary(
                    g => g.Key,
                    g => g.Select(x => new { x.SerialNumber, x.AUFTR, x.DATE, x.MATERIAL, x.NAME, x.VAR, x.IDTYPE, x.BEZ })
                          .GroupBy(x => x.BEZ)
                          .Select(group => new { BEZ = group.Key, Total = group.Count() })
                          .OrderByDescending(x => x.Total)
                          .Take(3)
                          .ToList<object>()
                );

                //------------------Reporte tabla FPY------------------//

                List<ReportFPY> productionDataList = new List<ReportFPY>();

                foreach (string var in totalProducedByProcess.Keys)
                {
                    if (totalFailuresByProcess.ContainsKey(var) && FPYbyProcess.ContainsKey(var))
                    {
                        ReportFPY data = new ReportFPY
                        {
                            Var = var,
                            TotalProduced = totalProducedByProcess[var],
                            TotalFailures = totalFailuresByProcess[var],
                            FPY = FPYbyProcess[var]
                        };
                        productionDataList.Add(data);
                    }
                }
                productionDataList = productionDataList.OrderBy(x => x.Var).ToList();

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
                    TotalFPYbyProcess = TotalFPYbyProcess,
                };

                var DataByProcess = new TotalsFPYDataByProcess
                {
                    //ProducedByProcessData = dataProducedByProcess,
                    //FailuresByProcessData = dataFailuresByProcess,
                    Top3FailsByProcess = Top3FailsByProcess
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
