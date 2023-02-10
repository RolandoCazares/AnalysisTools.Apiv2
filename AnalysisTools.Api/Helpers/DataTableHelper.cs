using analysistools.api.Models.Continental;
using analysistools.api.Models.ProductionTests;
using analysistools.api.Models.Screen;
using analysistools.api.Models.IDR;
using System.Data;
using System.Globalization;

namespace analysistools.api.Helpers
{
    /// <summary>
    /// Data table helper allows to convert retrieved data from databases to actual object to use them trhough the application.
    /// </summary>
    public static class DataTableHelper
    {
        public static List<TestDetails> DataTableToTestDetailsList(DataTable dataTable)
        {
            List<TestDetails> result = new List<TestDetails>();
            foreach (DataRow row in dataTable.Rows)
            {
                TestDetails details = new TestDetails()
                {
                    Date = DateTime.ParseExact(row["RUN_DATE"].ToString(), "dd-MM-yyyy HH:mm:ss", null),
                    Model = row["PDK_MATERIAL"].ToString(),
                    Number = row["Mrk_Num"].ToString(),
                    Description = row["Mrk_Bez"].ToString(),
                    Value = row["Mrk_Wert"].ToString(),
                    LSL = row["Mrk_Usg"].ToString(),
                    USL = row["Mrk_Osg"].ToString(),
                    Result = row["Mrk_Ein_Gut"].ToString()
                };
                result.Add(details);
            }
            return result;
        }

        public static List<Test> DataTableToTestList(DataTable dataTable)
        {
            List<Test> result = new List<Test>();
            foreach (DataRow row in dataTable.Rows)
            {
                string testDateAsString = row["TestDate"].ToString();
                DateTime testDateTime = DateTime.ParseExact(testDateAsString, "dd-MM-yyyy HH:mm:ss", null);
                string status = row["STATE"].ToString();
                string station = row["STATION"].ToString();

                result.Add(new Test()
                {
                    Date = testDateTime,
                    Result = status,
                    Station = station
                });
            }
            return result;
        }

        public static List<ScrnDetails> DataTableToScreenProcess(DataTable data)
        {
            List<ScrnDetails> result = new List<ScrnDetails>();
            foreach (DataRow row in data.Rows)
            {
                try
                {
                    string serialNumber = row["Unit_Id_In"].ToString();
                    string idPrueba = row["Unit_Id_In_Type"].ToString();
                    string estationID = row["Dst_Equipment"].ToString();
                    string idEstation = row["Operator"].ToString();
                    string dateAsString = row["Created"].ToString();
                    DateTime date = DateTime.ParseExact(dateAsString, "dd-MM-yyyy HH:mm:ss", null);
                    string transaction = row["Transaction_Type"].ToString();

                    result.Add(new ScrnDetails()
                    {
                        SerialNumber = serialNumber,
                        IdPrueba = idPrueba,
                        Estation = estationID,
                        IdEstation = idEstation,
                        Date = date,
                        Transaction = transaction,
                    });
                }
                catch (Exception)
                {
                }
            }
            return result;
        }

        public static List<Scrn2020Details> DataTableToScreenProcess2020(DataTable data)
        {
            List<Scrn2020Details> result = new List<Scrn2020Details>();
            foreach (DataRow row in data.Rows)
            {
                try
                {
                    var cultureInfo = new CultureInfo("de-DE");
                    string serialNumber = row["Unit_Id_In"].ToString();
                    string idType = row["Unit_Id_In_Type"].ToString();
                    string dateAsString = row["Created"].ToString();
                    DateTime date = DateTime.Parse(dateAsString, cultureInfo); //DateTime.ParseExact(dateAsString, "dd-MM-yyyy hh:mm:ss", null);
                    string model = row["Product_Definition"].ToString();
                    string cOMCell = row["Operator"].ToString();
                    string box = row["Car_Id"].ToString();
                    string lastProcess = row["Equipment"].ToString();

                    result.Add(new Scrn2020Details()
                    {
                        SerialNumber = serialNumber,
                        IdType = idType,
                        Date = date,
                        Model = model,
                        COMCell = cOMCell,
                        Box = box,
                        LastProcess = lastProcess,
                    });
                }
                catch (Exception)
                {
                }
            }
            return result;
        }

        public static List<LdmFlashUnit> DataTableToLdmFlashList(DataTable dataTable)
        {
            List<LdmFlashUnit> result = new List<LdmFlashUnit>();
            foreach (DataRow row in dataTable.Rows)
            {
                try
                {
                    string serialNumber = row["ESN"].ToString();
                    string dateAsString = row["Grupo"].ToString();
                    DateTime date = DateTime.ParseExact(dateAsString, "dd-MMM-yy h:mm:ss tt", null);
                    int shift = int.Parse(row["Turno"].ToString());
                    string pack = row["Box"].ToString();
                    string eol = row["EOL"].ToString();
                    result.Add(new LdmFlashUnit()
                    {
                        SerialNumber = serialNumber,
                        Date = date.ToString("dd-MMM-yyyy"),
                        Shift = shift,
                        Pack = pack,
                        EOL = eol
                    });
                }
                catch (Exception)
                {
                }
            }
            return result;
        }

        public static List<IccidUnit> DataTableToIccid(DataTable dataTable)
        {
            List<IccidUnit> result = new List<IccidUnit>();
            foreach (DataRow row in dataTable.Rows)
            {
                try
                {
                    string serie = row["Serie"].ToString();
                    string proyect = row["Proyecto"].ToString();
                    string iccid = row["Iccid"].ToString();
                    string dateAsString = row["Fecha"].ToString();
                    // string fecha = DateTime.ParseExact(dateAsString, "dd-MMM-yy h:mm:ss tt", null);
                    string host = row["Host"].ToString();
                    string motivo = row["Motivo"].ToString();
                    result.Add(new IccidUnit()
                    {
                        Serie = serie,
                        Proyect = proyect,
                        Iccid = iccid,
                        Fecha = dateAsString,
                        Host = host,
                        Motivo = motivo
                    });
                }
                catch (Exception)
                {
                }
            }
            return result;
        }

        public static List<ICTTest> DataTableToICTTests(DataTable data)
        {
            List<ICTTest> result = new List<ICTTest>();
            foreach (DataRow row in data.Rows)
            {
                try
                {
                    string serialNumber = row["RUNID"].ToString();
                    string runState = row["RUN_STATE"].ToString();
                    string dateAsString = row["RUN_DATE"].ToString();
                    DateTime date = DateTime.ParseExact(dateAsString, "dd-MM-yyyy HH:mm:ss", null);
                    string stationID = row["BMT_NAME"].ToString();
                    string model = row["PDK_MATERIAL"].ToString();
                    string testNumber = row["MRK_NUM"].ToString();
                    string description = row["MRK_BEZ"].ToString();
                    string value = row["MRK_WERT"].ToString();
                    string unit = row["MRK_EINHEIT"].ToString();
                    string usl = row["MRK_OSG"].ToString();
                    string lsl = row["MRK_USG"].ToString();
                    result.Add(new ICTTest()
                    {
                        SerialNumber = serialNumber,
                        RunState = runState,
                        RunDate = date,
                        StationId = stationID,
                        Model = model,
                        TestNumber = testNumber,
                        Description = description,
                        Value = value,
                        Units = unit,
                        USL = usl,
                        LSL = lsl
                    });
                }
                catch (Exception)
                {
                }
            }
            return result;
        }

        public static List<Failure> DataTableToFailure(DataTable data)
        {
            List<Failure> result = new List<Failure>();
            foreach (DataRow row in data.Rows)
            {
                try
                {
                    string Serial_Number = row["RUNID"].ToString();
                    string Test_status_Result = row["RUN_STATE"].ToString();
                    string dateTimeAsString = row["RUN_DATE"].ToString();
                    DateTime Date_Time = DateTime.Parse(dateTimeAsString);
                    // Id del defectoO
                    string MRK_WERT = row["MRK_WERT"].ToString();
                    string Work_Area = row["BMT_NAME"].ToString();
                    string Component = row["MRK_NUM"].ToString();
                    string ProductModel = row["PDK_MATERIAL"].ToString();
                    // ID de la estacion donde fallo
                    string ID_TYPE = row["RUNID_TYPE"].ToString();
                    string ComponentPartNumber = row["MRK_TXT"].ToString();
                    string Defect_Mode = row["ASLP_KOM"].ToString();
                    string AnalysisComment = row["TXT_INFO"].ToString();


                    //Console.WriteLine("AnalysisComment: " + failure.AnalysisComment);
                    bool analysisCommentHasNumber = failure.AnalysisComment.Any(c => char.IsDigit(c));
                    //Console.WriteLine("AnalysisCommet has number: " + analysisCommentHasNumber);
                    //Regex formatedPartNumberRegex = new Regex(@"x[a-z]:[A-Z0-9]{2}-[A-Z0-9]{4}-[A-Z0-9]{4}-[A-Z0-9]-[A-Z0-9]{2}");
                    //Regex rawPartNumberRegex = new Regex(@"x[a-z]:[A-Z0-9]{11}");
                    //Console.WriteLine("ComponentPartNumber: " + failure.ComponentPartNumber);
                    string rawComponent = Regex.Replace(failure.ComponentPartNumber, @"#[a-z]:[A-Z0-9]{2}-[A-Z0-9]{4}-[A-Z0-9]{4}-[A-Z0-9]-[A-Z0-9]{2}", "");
                    rawComponent = Regex.Replace(rawComponent, @"#[a-z]:[A-Z0-9]{11}", "");
                    rawComponent = Regex.Replace(rawComponent, @"#[a-z]:", "");
                    //Console.WriteLine("Component extracted from ComponentPartNumber: " + rawComponent);

                    bool partNumberCommentHasNumber = rawComponent.Any(c => char.IsDigit(c));
                    bool componentHasNumber = failure.Component.Any(c => char.IsDigit(c));

                    //Console.WriteLine("Extracted component has number: " + partNumberCommentHasNumber);
                    //Console.WriteLine("Component has number: " + componentHasNumber);

                    // Para PCB, CAUSER y BOM
                    if (!componentHasNumber)
                    {
                        FillComponentFilter(analysisCommentHasNumber, partNumberCommentHasNumber, rawComponent, ref failure);
                    }
                    componentHasNumber = failure.Component.Any(c => char.IsDigit(c));

                    if (failure.Component.Contains("NTF"))
                    {
                        FillComponentFilter(analysisCommentHasNumber, partNumberCommentHasNumber, rawComponent, ref failure);
                    }
                    componentHasNumber = failure.Component.Any(c => char.IsDigit(c));

                    // Ignorar si no hay informacion de la falla (NTF, y no tiene designador en comentario
                    // BUSCAR MANERA DE ORGANIZAR MEJOR LOS FILTROS
                    // RECOMENDACION: Crear una clase de validacion
                    // Filtros para evitar fallas ocasionadas por FrontEnd
                    string defectMode = failure.Defect_Mode.ToLower();
                    bool isShort = defectMode.Contains("corto");
                    bool isSoldering = defectMode.Contains("soldadura") || defectMode.Contains("componente desoldado");
                    bool isMisPlacement = defectMode.Contains("defasamiento") || defectMode.Contains("defasa") || defectMode.Contains("orientacion") || defectMode.Contains("pin alto") || defectMode.Contains("pin sumido") || defectMode.Contains("inserción de pines") || defectMode.Contains("desalineado") || defectMode.Contains("componente levantado");
                    bool isContamination = defectMode.Contains("contaminacion") || defectMode.Contains("contaminación") || defectMode.Contains("contaminado");
                    bool isMistaken = defectMode.Contains("equivocado") || defectMode.Contains("equivocada");
                    bool isICrelated = defectMode.Contains("pin levantado") || defectMode.Contains("terminal levantada");
                    bool isMissing = defectMode.Contains("componente faltante") || defectMode.Contains("faltante");
                    bool isExcess = defectMode.Contains("exceso");
                    bool isPolarity = defectMode.Contains("polaridad") || defectMode.Contains("invertida");
                    bool isAmbientTest = defectMode.Contains("ambient test");
                    bool isESD = defectMode.Contains("esd");
                    bool isElectric = defectMode.Contains("falla electrica");
                    bool isOther = defectMode.Contains("capacitancia baja") || defectMode.Contains("baja resistencia") || defectMode.Contains("tombstone");
                    //Filtro para evitar fallas por integrados
                    string component = failure.Component.ToLower();
                    bool isIC = component.Contains("ic");



                    bool ignore = ((!componentHasNumber || failure.Component.Contains("NTF")) && !analysisCommentHasNumber && !partNumberCommentHasNumber) ||
                        (isShort || isSoldering || isMisPlacement || isContamination || isMistaken || isICrelated || isMissing || isExcess || isESD || isIC || isPolarity || isAmbientTest || isElectric || isOther);
                    //bool ignore = (failure.Component.Contains("NTF") || failure.Component.Contains("PCB") || failure.Component.Contains("BOM") || failure.Component.Contains("CAUSER")) && !analysisCommentHasNumber && !partNumberCommentHasNumber;

                    //Console.WriteLine("====> component: " + failure.Component + ", defect mode: " + failure.Defect_Mode);
                    //Console.WriteLine("====> Will be ignored? " + ignore);

                    if (!ignore)

                    {
                        string getICTStationQuery = GetStationQuery(failure.Serial_Number);
                        failure.StationID = string.Empty;
                        DataTable stationResult = _context.RunQuery(getICTStationQuery);
                        foreach (DataRow stationrow in stationResult.Rows)
                        {
                            failure.StationID = stationrow["BMT_NAME"].ToString();
                            break;
                        }
                        Resultado.Add(failure);
                    }
                }
                catch (Exception)
                {
                }
            }
            return result;

        }
    }
}
