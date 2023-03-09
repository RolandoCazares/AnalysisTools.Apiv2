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
                #region ReporteFPY por proceso

                //-----------Producidas------------//
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

                //---------------Sumatoria--------------//

                Dictionary<string, int> totalsByProcess = new Dictionary<string, int>();

                foreach (Dictionary<string, int> produced in totalProducedByProcessList)
                {
                    foreach (KeyValuePair<string, int> pair in produced)
                    {
                        int totalProduced = pair.Value;
                        int totalFailures = 0;

                        if (totalFailuresByProcessList.Any(failures => failures.ContainsKey(pair.Key)))
                        {
                            totalFailures = totalFailuresByProcessList
                                .Where(failures => failures.ContainsKey(pair.Key))
                                .Select(failures => failures[pair.Key])
                                .Sum();
                        }

                        int total = totalProduced + totalFailures;

                        if (!totalsByProcess.ContainsKey(pair.Key))
                        {
                            totalsByProcess.Add(pair.Key, total);
                        }
                        else
                        {
                            totalsByProcess[pair.Key] += total;
                        }
                    }
                }


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

                //------------------Reporte tabla FPY------------------//

                List<ReportFPY> ReportFPYbyProcess = new List<ReportFPY>();

                foreach (string var in totalProducedByProcess.Keys)
                {
                    if (totalFailuresByProcess.ContainsKey(var) && FPYbyProcess.ContainsKey(var) && totalsByProcess.ContainsKey(var))
                    {
                        ReportFPY data = new ReportFPY
                        {
                            Var = var,
                            TotalProduced = totalProducedByProcess[var],
                            TotalFailures = totalFailuresByProcess[var],
                            Total = totalsByProcess[var],
                            FPY = FPYbyProcess[var],
                        };
                        ReportFPYbyProcess.Add(data);
                    }
                }
                ReportFPYbyProcess = ReportFPYbyProcess.OrderBy(x => x.Var).ToList();

                #endregion

                #region ReporteFPY por proceso y estacion

                //-------------------Produced--------------------//
                Dictionary<(string, string), int> totalProducedByProcessAndStation = producedAndFilteredList
                .GroupBy(x => (x.Var, x.Name))
                .ToDictionary(g => g.Key, g => g.Sum(x => x.Amount));

                List<Dictionary<(string, string), int>> totalProducedByProcessAndStationList = new List<Dictionary<(string, string), int>>();
                totalProducedByProcessAndStationList.Add(totalProducedByProcessAndStation);

                //-------------------Failures--------------------//
                Dictionary<(string, string), int> totalFailuresByProcessAndStation = FailuresAndFilter
                .GroupBy(x => (x.VAR, x.NAME))
                .ToDictionary(g => (g.Key.VAR, g.Key.NAME), g => g.Count());

                List<Dictionary<(string, string), int>> totalFailuresByProcessAndStationList = new List<Dictionary<(string, string), int>>();
                totalFailuresByProcessAndStationList.Add(totalFailuresByProcessAndStation);

                //---------Sumar fallas y producidas por proceso y estacion-----//
                Dictionary<(string, string), int> totalsByProcessAndStation = totalProducedByProcessAndStation
                .Join(totalFailuresByProcessAndStation, x => x.Key, y => y.Key, (x, y) => new { Key = x.Key, Value = x.Value + y.Value })
                .ToDictionary(x => x.Key, x => x.Value);

                List<Dictionary<(string, string), int>> totalsByProcessAndStationList = new List<Dictionary<(string, string), int>>();
                totalsByProcessAndStationList.Add(totalsByProcessAndStation);

                //-------Calcular el porcentaje de FPY por proceso y estación-----//

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

                //-------------Genera reporte de FPY por proceso y estacion-----------//

                List<ReportFPYByProcessAndStation> ReportFPYbyProcessAndStation = new List<ReportFPYByProcessAndStation>();

                foreach ((string var, string name) in totalProducedByProcessAndStation.Keys)
                {
                    if (totalFailuresByProcessAndStation.ContainsKey((var, name)) && FPYbyProcessAndStation.ContainsKey((var, name)) && totalsByProcessAndStation.ContainsKey((var, name)))
                    {
                        ReportFPYByProcessAndStation data = new ReportFPYByProcessAndStation
                        {
                            Var = var,
                            Name = name,
                            TotalProduced = totalProducedByProcessAndStation[(var, name)],
                            TotalFailures = totalFailuresByProcessAndStation[(var, name)],
                            Total = totalsByProcessAndStation[(var, name)],
                            FPY = FPYbyProcessAndStation[(var, name)],
                        };
                        ReportFPYbyProcessAndStation.Add(data);
                    }
                }

                #endregion

                #region ReporteFPY por proceso y modelo

                //-------------------Produced--------------------//
                Dictionary<(string, string), int> totalProducedByProcessAndModel = producedAndFilteredList
                .GroupBy(x => (x.Var, x.Material))
                .ToDictionary(g => g.Key, g => g.Sum(x => x.Amount));

                List<Dictionary<(string, string), int>> totalProducedByProcessAndModelList = new List<Dictionary<(string, string), int>>();
                totalProducedByProcessAndModelList.Add(totalProducedByProcessAndModel);

                //-------------------Failures--------------------//
                Dictionary<(string, string), int> totalFailuresByProcessAndModel = FailuresAndFilter
                .GroupBy(x => (x.VAR, x.MATERIAL))
                .ToDictionary(g => (g.Key.VAR, g.Key.MATERIAL), g => g.Count());

                List<Dictionary<(string, string), int>> totalFailuresByProcessAndModelList = new List<Dictionary<(string, string), int>>();
                totalFailuresByProcessAndModelList.Add(totalFailuresByProcessAndModel);

                //---------Sumar fallas y producidas por proceso y modelo-----//
                Dictionary<(string, string), int> totalsByProcessAndModel = totalProducedByProcessAndModel
                .Join(totalFailuresByProcessAndModel, x => x.Key, y => y.Key, (x, y) => new { Key = x.Key, Value = x.Value + y.Value })
                .ToDictionary(x => x.Key, x => x.Value);

                List<Dictionary<(string, string), int>> totalsByProcessAndModelList = new List<Dictionary<(string, string), int>>();
                totalsByProcessAndModelList.Add(totalsByProcessAndModel);

                //-------Calcular el porcentaje de FPY por proceso y modelo-----//

                Dictionary<(string, string), double> FPYbyProcessAndModel = new Dictionary<(string, string), double>();
                List<Dictionary<(string, string), double>> TotalFPYbyProcessAndModel = new List<Dictionary<(string, string), double>>();

                foreach ((string var, string name) in totalProducedByProcessAndModel.Keys)
                {
                    if (totalFailuresByProcessAndModel.ContainsKey((var, name)))
                    {
                        double fpy = (double)(totalProducedByProcessAndModel[(var, name)] - totalFailuresByProcessAndModel[(var, name)]) / totalProducedByProcessAndModel[(var, name)] * 100;
                        FPYbyProcessAndModel.Add((var, name), fpy);
                    }
                    else
                    {
                        FPYbyProcessAndModel.Add((var, name), 100);
                    }
                }

                TotalFPYbyProcessAndModel.Add(FPYbyProcessAndModel);

                //-------------Genera reporte de FPY por proceso y modelo-----------//

                List<ReportFPYByProcessAndModel> ReportFPYbyProcessAndModel = new List<ReportFPYByProcessAndModel>();

                foreach ((string var, string Material) in totalProducedByProcessAndModel.Keys)
                {
                    if (totalFailuresByProcessAndModel.ContainsKey((var, Material)) && FPYbyProcessAndModel.ContainsKey((var, Material)) && totalsByProcessAndModel.ContainsKey((var, Material)))
                    {
                        ReportFPYByProcessAndModel data = new ReportFPYByProcessAndModel
                        {
                            Var = var,
                            Material = Material,
                            TotalProduced = totalProducedByProcessAndModel[(var, Material)],
                            TotalFailures = totalFailuresByProcessAndModel[(var, Material)],
                            Total = totalsByProcessAndModel[(var, Material)],
                            FPY = FPYbyProcessAndModel[(var, Material)],
                        };
                        ReportFPYbyProcessAndModel.Add(data);
                    }
                }

                #endregion

                #region ReporteFPY por proceso y modelo simplificado para grafica

                //-------------Genera reporte de FPY por proceso y modelo-----------//
                #region prueba
                //List<string> uniqueProcessNames = producedAndFilteredList.Select(m => m.Var).Distinct().ToList();

                //Dictionary<string, List<object>> ProcesoModelos = new Dictionary<string, List<object>>();
                //foreach (string var in uniqueProcessNames)
                //{
                //    ProcesoModelos.Add(var, producedAndFilteredList.Select(x => new { x.Material, x.Amount, x.Var }).Where(x => x.Var == var).ToList<object>());
                //}

                //List<ReportFPYByProcessAndModel> ReportFPYbyProcessAndModel2 = new List<ReportFPYByProcessAndModel>();
                //foreach ((string var, string Material) in totalProducedByProcessAndModel.Keys)
                //{
                //    if (FPYbyProcessAndModel.ContainsKey((var, Material)))
                //    {
                //        ReportFPYByProcessAndModel data = new ReportFPYByProcessAndModel
                //        {
                //            Material = Material,
                //            FPY = FPYbyProcessAndModel[(var, Material)],
                //        };
                //        ReportFPYbyProcessAndModel2.Add(data);
                //    }
                //}

                #endregion

                Dictionary<string, List<object>> dataDict = new Dictionary<string, List<object>>();
                List<string> uniqueProcessNames = producedAndFilteredList.Select(m => m.Var).Distinct().ToList();

                foreach (string processName in uniqueProcessNames)
                {
                    List<object> dataList = new List<object>();
                    foreach ((string var, string Material) in totalProducedByProcessAndModel.Keys)
                    {
                        if (var == processName)
                        {
                            if (FPYbyProcessAndModel.ContainsKey((var, Material)))
                            {
                                ReportFPYByProcessAndModel data = new ReportFPYByProcessAndModel
                                {
                                    Material = Material,
                                    FPY = FPYbyProcessAndModel[(var, Material)],
                                };
                                dataList.Add(data);
                            }
                        }
                    }
                    dataDict[processName] = dataList;
                }



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

                //-----------------Encapsulamiento-------------//
                //var Totals = new TotalsFPY
                //{
                //    TotalProduced = totalProduced,
                //    TotalFailures = totalFailures,
                //};

                //var TotalsByProcess = new TotalsFPYByProcess
                //{
                //    TotalProducedByProcess = totalProducedByProcessList,
                //    TotalFailuresByProcess = totalFailuresByProcessList,
                //    TotalFPYbyProcess = TotalFPYbyProcess,
                //};

                var DataByProcess = new TotalsFPYDataByProcess
                {
                    //ProducedByProcessData = dataProducedByProcess,
                    //FailuresByProcessData = dataFailuresByProcess,
                    Top3FailsByProcess = Top3FailsByProcess
                };

                var Response = new Response
                {
                    //Totals = Totals,
                    ReportFPY= ReportFPYbyProcess,
                    //TotalsByProcess = TotalsByProcess,
                    ReportFPYByProcessAndStation = ReportFPYbyProcessAndStation,
                    ReportFPYByProcessAndModel= ReportFPYbyProcessAndModel,
                    ChartFPYPS = dataDict,
                    DataByProcess = DataByProcess,
                };


                result.Add(Response);
            }
            catch (Exception) { }
            return result;
        }

        
    }
}


















//public async Task<List<Response>> DataTableAccommodateReportFPY(List<ProducedAndFilteredFPY> producedAndFilteredList, List<FailureFPY> FailuresAndFilter) {
//    List<Response> result = new List<Response>();
//    try
//    { 
//        //-----------Producidas------------//
//        Dictionary<string, int> totalProducedByProcess = producedAndFilteredList
//            .GroupBy(x => x.Var)
//            .ToDictionary(g => g.Key, g => g.Sum(x => x.Amount));

//        List<Dictionary<string, int>> totalProducedByProcessList = new List<Dictionary<string, int>>();
//        totalProducedByProcessList.Add(totalProducedByProcess);

//        //------------Falladas-------------//
//        Dictionary<string, int> totalFailuresByProcess = FailuresAndFilter
//        .GroupBy(x => x.VAR)
//        .ToDictionary(g => g.Key, g => g.Count());

//        List<Dictionary<string, int>> totalFailuresByProcessList = new List<Dictionary<string, int>>();
//        totalFailuresByProcessList.Add(totalFailuresByProcess);

//        //---------------Sumatoria--------------//

//        Dictionary<string, int> totalsByProcess = new Dictionary<string, int>();

//        foreach (Dictionary<string, int> produced in totalProducedByProcessList)
//        {
//            foreach (KeyValuePair<string, int> pair in produced)
//            {
//                int totalProduced = pair.Value;
//                int totalFailures = 0;

//                if (totalFailuresByProcessList.Any(failures => failures.ContainsKey(pair.Key)))
//                {
//                    totalFailures = totalFailuresByProcessList
//                        .Where(failures => failures.ContainsKey(pair.Key))
//                        .Select(failures => failures[pair.Key])
//                        .Sum();
//                }

//                int total = totalProduced + totalFailures;

//                if (!totalsByProcess.ContainsKey(pair.Key))
//                {
//                    totalsByProcess.Add(pair.Key, total);
//                }
//                else
//                {
//                    totalsByProcess[pair.Key] += total;
//                }
//            }
//        }


//        //-------FPY porcentaje por proceso-------//

//        Dictionary<string, double> FPYbyProcess = new Dictionary<string, double>();
//        List<Dictionary<string, double>> TotalFPYbyProcess = new List<Dictionary<string, double>>();

//        foreach (string var in totalProducedByProcess.Keys)
//        {
//            if (totalFailuresByProcess.ContainsKey(var))
//            {
//                double fpy = (double)(totalProducedByProcess[var] - totalFailuresByProcess[var]) / totalProducedByProcess[var] * -1 + 100;
//                FPYbyProcess.Add(var, fpy);
//            }
//            else
//            {
//                FPYbyProcess.Add(var, 100);
//            }
//        }

//        TotalFPYbyProcess.Add(FPYbyProcess);

//        //------------------Reporte tabla FPY------------------//

//        List<ReportFPY> ReportFPYbyProcess = new List<ReportFPY>();

//        foreach (string var in totalProducedByProcess.Keys)
//        {
//            if (totalFailuresByProcess.ContainsKey(var) && FPYbyProcess.ContainsKey(var) && totalsByProcess.ContainsKey(var))
//            {
//                ReportFPY data = new ReportFPY
//                {
//                    Var = var,
//                    TotalProduced = totalProducedByProcess[var],
//                    TotalFailures = totalFailuresByProcess[var],
//                    Total = totalsByProcess[var],
//                    FPY = FPYbyProcess[var],
//                };
//                ReportFPYbyProcess.Add(data);
//            }
//        }
//        ReportFPYbyProcess = ReportFPYbyProcess.OrderBy(x => x.Var).ToList();
//    }
//    catch (Exception) { }
//    return result;
//}

//public async Task<List<Response>> DataTableAccommodateReportFPYProcessAndStation(List<ProducedAndFilteredFPY> producedAndFilteredList, List<FailureFPY> FailuresAndFilter)
//{
//    List<Response> result = new List<Response>();
//    try
//    {
//        //-------------------Produced--------------------//
//        Dictionary<(string, string), int> totalProducedByProcessAndStation = producedAndFilteredList
//        .GroupBy(x => (x.Var, x.Name))
//        .ToDictionary(g => g.Key, g => g.Sum(x => x.Amount));

//        List<Dictionary<(string, string), int>> totalProducedByProcessAndStationList = new List<Dictionary<(string, string), int>>();
//        totalProducedByProcessAndStationList.Add(totalProducedByProcessAndStation);

//        //-------------------Failures--------------------//
//        Dictionary<(string, string), int> totalFailuresByProcessAndStation = FailuresAndFilter
//        .GroupBy(x => (x.VAR, x.NAME))
//        .ToDictionary(g => (g.Key.VAR, g.Key.NAME), g => g.Count());

//        List<Dictionary<(string, string), int>> totalFailuresByProcessAndStationList = new List<Dictionary<(string, string), int>>();
//        totalFailuresByProcessAndStationList.Add(totalFailuresByProcessAndStation);

//        //---------Sumar fallas y producidas por proceso y estacion-----//
//        Dictionary<(string, string), int> totalsByProcessAndStation = totalProducedByProcessAndStation
//        .Join(totalFailuresByProcessAndStation, x => x.Key, y => y.Key, (x, y) => new { Key = x.Key, Value = x.Value + y.Value })
//        .ToDictionary(x => x.Key, x => x.Value);

//        List<Dictionary<(string, string), int>> totalsByProcessAndStationList = new List<Dictionary<(string, string), int>>();
//        totalsByProcessAndStationList.Add(totalsByProcessAndStation);

//        //-------Calcular el porcentaje de FPY por proceso y estación-----//

//        Dictionary<(string, string), double> FPYbyProcessAndStation = new Dictionary<(string, string), double>();
//        List<Dictionary<(string, string), double>> TotalFPYbyProcessAndStation = new List<Dictionary<(string, string), double>>();

//        foreach ((string var, string name) in totalProducedByProcessAndStation.Keys)
//        {
//            if (totalFailuresByProcessAndStation.ContainsKey((var, name)))
//            {
//                double fpy = (double)(totalProducedByProcessAndStation[(var, name)] - totalFailuresByProcessAndStation[(var, name)]) / totalProducedByProcessAndStation[(var, name)] * 100;
//                FPYbyProcessAndStation.Add((var, name), fpy);
//            }
//            else
//            {
//                FPYbyProcessAndStation.Add((var, name), 100);
//            }
//        }

//        TotalFPYbyProcessAndStation.Add(FPYbyProcessAndStation);

//        //-------------Genera reporte de FPY por proceso y estacion-----------//

//        List<ReportFPYByProcessAndStation> ReportFPYbyProcessAndStation = new List<ReportFPYByProcessAndStation>();

//        foreach ((string var, string name) in totalProducedByProcessAndStation.Keys)
//        {
//            if (totalFailuresByProcessAndStation.ContainsKey((var, name)) && FPYbyProcessAndStation.ContainsKey((var, name)) && totalsByProcessAndStation.ContainsKey((var, name)))
//            {
//                ReportFPYByProcessAndStation data = new ReportFPYByProcessAndStation
//                {
//                    Var = var,
//                    Name = name,
//                    TotalProduced = totalProducedByProcessAndStation[(var, name)],
//                    TotalFailures = totalFailuresByProcessAndStation[(var, name)],
//                    Total = totalsByProcessAndStation[(var, name)],
//                    FPY = FPYbyProcessAndStation[(var, name)],
//                };
//                ReportFPYbyProcessAndStation.Add(data);
//            }
//        }


//    }
//    catch (Exception) { }
//    return result;
//}

//public async Task<List<Response>> DataTableAccommodateReportFPYProcessAndModel(List<ProducedAndFilteredFPY> producedAndFilteredList, List<FailureFPY> FailuresAndFilter)
//{
//    List<Response> result = new List<Response>();
//    try
//    {
//        //-------------------Produced--------------------//
//        Dictionary<(string, string), int> totalProducedByProcessAndModel = producedAndFilteredList
//        .GroupBy(x => (x.Var, x.Material))
//        .ToDictionary(g => g.Key, g => g.Sum(x => x.Amount));

//        List<Dictionary<(string, string), int>> totalProducedByProcessAndModelList = new List<Dictionary<(string, string), int>>();
//        totalProducedByProcessAndModelList.Add(totalProducedByProcessAndModel);

//        //-------------------Failures--------------------//
//        Dictionary<(string, string), int> totalFailuresByProcessAndModel = FailuresAndFilter
//        .GroupBy(x => (x.VAR, x.MATERIAL))
//        .ToDictionary(g => (g.Key.VAR, g.Key.MATERIAL), g => g.Count());

//        List<Dictionary<(string, string), int>> totalFailuresByProcessAndModelList = new List<Dictionary<(string, string), int>>();
//        totalFailuresByProcessAndModelList.Add(totalFailuresByProcessAndModel);

//        //---------Sumar fallas y producidas por proceso y modelo-----//
//        Dictionary<(string, string), int> totalsByProcessAndModel = totalProducedByProcessAndModel
//        .Join(totalFailuresByProcessAndModel, x => x.Key, y => y.Key, (x, y) => new { Key = x.Key, Value = x.Value + y.Value })
//        .ToDictionary(x => x.Key, x => x.Value);

//        List<Dictionary<(string, string), int>> totalsByProcessAndModelList = new List<Dictionary<(string, string), int>>();
//        totalsByProcessAndModelList.Add(totalsByProcessAndModel);

//        //-------Calcular el porcentaje de FPY por proceso y modelo-----//

//        Dictionary<(string, string), double> FPYbyProcessAndModel = new Dictionary<(string, string), double>();
//        List<Dictionary<(string, string), double>> TotalFPYbyProcessAndModel = new List<Dictionary<(string, string), double>>();

//        foreach ((string var, string name) in totalProducedByProcessAndModel.Keys)
//        {
//            if (totalFailuresByProcessAndModel.ContainsKey((var, name)))
//            {
//                double fpy = (double)(totalProducedByProcessAndModel[(var, name)] - totalFailuresByProcessAndModel[(var, name)]) / totalProducedByProcessAndModel[(var, name)] * 100;
//                FPYbyProcessAndModel.Add((var, name), fpy);
//            }
//            else
//            {
//                FPYbyProcessAndModel.Add((var, name), 100);
//            }
//        }

//        TotalFPYbyProcessAndModel.Add(FPYbyProcessAndModel);

//        //-------------Genera reporte de FPY por proceso y modelo-----------//

//        List<ReportFPYByProcessAndModel> ReportFPYbyProcessAndModel = new List<ReportFPYByProcessAndModel>();

//        foreach ((string var, string Material) in totalProducedByProcessAndModel.Keys)
//        {
//            if (totalFailuresByProcessAndModel.ContainsKey((var, Material)) && FPYbyProcessAndModel.ContainsKey((var, Material)) && totalsByProcessAndModel.ContainsKey((var, Material)))
//            {
//                ReportFPYByProcessAndModel data = new ReportFPYByProcessAndModel
//                {
//                    Var = var,
//                    Material = Material,
//                    TotalProduced = totalProducedByProcessAndModel[(var, Material)],
//                    TotalFailures = totalFailuresByProcessAndModel[(var, Material)],
//                    Total = totalsByProcessAndModel[(var, Material)],
//                    FPY = FPYbyProcessAndModel[(var, Material)],
//                };
//                ReportFPYbyProcessAndModel.Add(data);
//            }
//        }

//    }
//    catch (Exception) { }
//    return result;
//}
#endregion