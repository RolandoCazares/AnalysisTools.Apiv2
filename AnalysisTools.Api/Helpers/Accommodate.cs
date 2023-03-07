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

                //Agrupar por proceso y contar
                //Dictionary<string, int> totalProducedByProcess = producedAndFilteredList
                //    .GroupBy(x => x.Var)
                //    .ToDictionary(g => g.Key, g => g.Sum(x => x.Amount));

                //Agrupar por proceso y estacion y contar
                Dictionary<(string, string), int> totalProducedByProcessAndStation = producedAndFilteredList
                .GroupBy(x => (x.Var, x.Name))
                .ToDictionary(g => g.Key, g => g.Sum(x => x.Amount));

                //Agrupar por proceso y modelo y contar
                Dictionary<(string, string), int> totalProducedByProcessAndModel = producedAndFilteredList
                .GroupBy(x => (x.Var, x.Material))
                .ToDictionary(g => g.Key, g => g.Sum(x => x.Amount));

                //Por proceso
                //List<Dictionary<string, int>> totalProducedByProcessList = new List<Dictionary<string, int>>();
                //totalProducedByProcessList.Add(totalProducedByProcess);

                //Por proceso y estacion
                List<Dictionary<(string, string), int>> totalProducedByProcessAndStationList = new List<Dictionary<(string, string), int>>();
                totalProducedByProcessAndStationList.Add(totalProducedByProcessAndStation);

                //Por proceso y Modelo
                List<Dictionary<(string, string), int>> totalProducedByProcessAndModelList = new List<Dictionary<(string, string), int>>();
                totalProducedByProcessAndStationList.Add(totalProducedByProcessAndModel);

                //-----------------------------Intenta Acomodar Falladas------------------//

                int totalFailures = FailuresAndFilter.Count;

                //Dictionary<string, int> totalFailuresByProcess = FailuresAndFilter
                //.GroupBy(x => x.VAR)
                //.ToDictionary(g => g.Key, g => g.Count());

                Dictionary<(string, string), int> totalFailuresByProcessAndStation = FailuresAndFilter
                .GroupBy(x => (x.VAR, x.NAME))
                .ToDictionary(g => (g.Key.VAR, g.Key.NAME), g => g.Count());


                Dictionary<(string, string), int> totalFailuresByProcessAndModel = FailuresAndFilter
                .GroupBy(x => (x.VAR, x.MATERIAL))
                .ToDictionary(g => (g.Key.VAR, g.Key.MATERIAL), g => g.Count());

                //Por proceso
                //List<Dictionary<string, int>> totalFailuresByProcessList = new List<Dictionary<string, int>>();
                //totalFailuresByProcessList.Add(totalFailuresByProcess);

                //Por proceso y estacion
                List<Dictionary<(string, string), int>> totalFailuresByProcessAndStationList = new List<Dictionary<(string, string), int>>();
                totalFailuresByProcessAndStationList.Add(totalFailuresByProcessAndStation);

                //Por proceso y Modelo
                List<Dictionary<(string, string), int>> totalFailuresByProcessAndModelList = new List<Dictionary<(string, string), int>>();
                totalFailuresByProcessAndStationList.Add(totalFailuresByProcessAndModel);

                //---------------------FPY porcentaje por proceso------------------------//

                //Dictionary<string, double> FPYbyProcess = new Dictionary<string, double>();
                //List<Dictionary<string, double>> TotalFPYbyProcess = new List<Dictionary<string, double>>();

                //foreach (string var in totalProducedByProcess.Keys)
                //{
                //    if (totalFailuresByProcess.ContainsKey(var))
                //    {
                //        double fpy = (double)(totalProducedByProcess[var] - totalFailuresByProcess[var]) / totalProducedByProcess[var] * -1 + 100;
                //        FPYbyProcess.Add(var, fpy);
                //    }
                //    else
                //    {
                //        FPYbyProcess.Add(var, 100);
                //    }
                //}

                //TotalFPYbyProcess.Add(FPYbyProcess);

                //-------------------FPY reporte por proceso y estacion--------//

                Dictionary<(string, string), double> FPYbyProcessAndStation = new Dictionary<(string, string), double>();
                List<Dictionary<(string, string), double>> TotalFPYbyProcessAndStation = new List<Dictionary<(string, string), double>>();

                foreach ((string var, string name) in totalProducedByProcessAndStation.Keys)
                {
                    if (totalFailuresByProcessAndStation.ContainsKey((var, name)))
                    {
                        double fpy = (double)(totalProducedByProcessAndStation[(var, name)] - totalFailuresByProcessAndStation[(var, name)]) / totalProducedByProcessAndStation[(var, name)] * 100;
                        FPYbyProcessAndStation.Add((var, name), fpy);
                    }
                    else
                    {
                        FPYbyProcessAndStation.Add((var, name), 100);
                    }
                }

                TotalFPYbyProcessAndStation.Add(FPYbyProcessAndStation);

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
                    ReportFPY= productionDataList,
                    TotalsByProcess = TotalsByProcess,
                    DataByProcess = DataByProcess,
                };


                result.Add(Response);
            }
            catch (Exception) { }
            return result;
        }

        public async Task<List<Response>> DataTableAccommodateReportFPY(List<ProducedAndFilteredFPY> producedAndFilteredList, List<FailureFPY> FailuresAndFilter) {
            List<Response> result = new List<Response>();
            try
            { 
                //-----------Producidas------------//
                //Agrupar por proceso y contar
                Dictionary<string, int> totalProducedByProcess = producedAndFilteredList
                    .GroupBy(x => x.Var)
                    .ToDictionary(g => g.Key, g => g.Sum(x => x.Amount));

                List<Dictionary<string, int>> totalProducedByProcessList = new List<Dictionary<string, int>>();
                totalProducedByProcessList.Add(totalProducedByProcess);

                //------------Falladas-------------//
                Dictionary<string, int> totalFailuresByProcess = FailuresAndFilter
                .GroupBy(x => x.VAR)
                .ToDictionary(g => g.Key, g => g.Count());

                List<Dictionary<string, int>> totalFailuresByProcessList = new List<Dictionary<string, int>>();
                totalFailuresByProcessList.Add(totalFailuresByProcess);

                //-------FPY porcentaje por proceso-------//

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


            }
            catch (Exception) { }
            return result;
        }
    }
}
