using analysistools.api.Models.Continental;
using analysistools.api.Models.ProductionTests;
using analysistools.api.Models.Screen;
using analysistools.api.Models.IDR;
using System.Data;
using System.Globalization;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
using analysistools.api.Contracts;
using analysistools.api.Data;
using analysistools.api.Models.IDR.DTO;
using analysistools.api.Models.FPY;
using System.Security.Policy;
using System.Xml.Linq;
using System;
using NuGet.Configuration;

namespace analysistools.api.Helpers
{
    /// <summary>
    /// Data table helper allows to convert retrieved data from databases to actual object to use them trhough the application.
    /// </summary>
    public static class DataTableHelper
    {
        private static readonly IDbContext _context = OracleDbContext.Instance;
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
            static void FillComponentFilter(bool analysisCommentHasNumber, bool partNumberCommentHasNumber, string rawComponent, ref Failure failure)
            {
                if (partNumberCommentHasNumber)
                {
                    failure.Component = rawComponent.ToUpper();
                    //Console.WriteLine("Component detected in part number, failure.Component now is: " + failure.Component);
                }
                else if (analysisCommentHasNumber)
                {
                    var m = Regex.Match(failure.AnalysisComment, @".{2}[0-9]+");
                    failure.Component = m.Groups[0].Value.ToUpper();
                    //Console.WriteLine("Component detected in analysis comment, now failure.Component is: " + failure.Component);
                }
                //if ((!analysisCommentHasNumber && partNumberCommentHasNumber) || (analysisCommentHasNumber && partNumberCommentHasNumber)) failure.Component = rawComponent.ToUpper();
                //if (!analysisCommentHasNumber && partNumberCommentHasNumber) failure.Component = rawComponent.ToUpper();
                //if (analysisCommentHasNumber && !partNumberCommentHasNumber)
                //{
                //    var m = Regex.Match(failure.AnalysisComment, @".{2}[0-9]+");
                //    failure.Component = m.Groups[0].Value.ToUpper();
                //};
            }
            static string GetStationQuery(string SerialNumber)
            {
                return $"SELECT AL1.RUNID, AL1.RUN_STATE, AL1.RUN_DATE, AL6.BMT_NAME, AL1.RUNID_TYPE FROM EVAPROD.PD_LFD_RUN AL1, EVAPROD.PD_STM_PRP AL3, EVACOMP.PD_LFD_BMN AL6 WHERE (AL6.BMT_DAT_ID=AL1.BMT_DAT_ID AND AL1.PRP_DATE_ID=AL3.PRP_DATE_ID)  AND (AL3.PRP_VAR LIKE '%ICT%' AND AL1.RUNID='{SerialNumber}' AND (AL6.BMT_NAME LIKE '%ICT%'))";
            }
            List<Failure> result = new List<Failure>();
            foreach (DataRow row in data.Rows)
            {
                try
                {
                    //Console.WriteLine("==========");
                    string dateTimeAsString = row["RUN_DATE"].ToString();
                    Failure failure = new Failure
                    {
                        Serial_Number = row["RUNID"].ToString(),
                        Test_status_Result = row["RUN_STATE"].ToString(),
                        Date_Time = DateTime.Parse(dateTimeAsString),
                        // Id del defectoO
                        MRK_WERT = row["MRK_WERT"].ToString(),
                        Work_Area = row["BMT_NAME"].ToString(),
                        Component = row["MRK_NUM"].ToString(),
                        ProductModel = row["PDK_MATERIAL"].ToString(),
                        // ID de la estacion donde fallo
                        ID_TYPE = row["RUNID_TYPE"].ToString(),
                        ComponentPartNumber = row["MRK_TXT"].ToString(),
                        Defect_Mode = row["ASLP_KOM"].ToString(),
                        AnalysisComment = row["TXT_INFO"].ToString()
                    };
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

                    if (!ignore){
                        string getICTStationQuery = GetStationQuery(failure.Serial_Number);
                        failure.StationID = string.Empty;
                        DataTable stationResult = _context.RunQuery(getICTStationQuery);
                        foreach (DataRow stationrow in stationResult.Rows)
                        {
                            failure.StationID = stationrow["BMT_NAME"].ToString();
                            break;
                        }
                        result.Add(failure);
                    }

                    //Console.WriteLine("==========" + Environment.NewLine);
                }
                catch (Exception){}

            }
            result = result.GroupBy(f => f.Serial_Number).Select(f => f.First()).ToList();

            return result;

        }

        public static List<ProducedUnitsDTO> DataTableToGetAllUnitsProduced(string FamilyICTs, int FamilyID, DateTime FromDate, DateTime ToDate)
        {
            List<ProducedUnitsDTO> result = new List<ProducedUnitsDTO>();
            double diffDays = (ToDate - FromDate).TotalDays;
            if (!(diffDays > 0 && diffDays <= 31)) throw new ArgumentException("Solo se permite maximo 31 dias");

            for (int i = 0; i < diffDays; i++)
            {
                DateTime _FromDate = FromDate.AddDays(i);
                DateTime _ToDate = FromDate.AddDays(i + 1);
                string fromDate = _FromDate.ToString("dd-MM-yyyy HH:mm:ss");
                string toDate = _ToDate.ToString("dd-MM-yyyy HH:mm:ss");
                Console.WriteLine($"Produced from Family with ID {FamilyID} from: {fromDate} to {toDate}");
                string producedQuery = $"SELECT COUNT(*) AS PRODUCED FROM (SELECT DISTINCT RUNID FROM (SELECT AL1.RUNID, AL1.RUN_STATE, AL1.RUN_DATE, AL4.BMT_NAME, AL2.PDK_MATERIAL, AL3.PRP_VAR, AL1.RUNID_TYPE, AL5.PDK_AUFTR FROM EVAPROD.PD_LFD_RUN AL1, EVAPROD.PD_LFD_MAT AL2, EVAPROD.PD_STM_PRP AL3, EVACOMP.PD_LFD_BMN AL4, EVAPROD.PD_LFD_AUF AL5 WHERE (AL2.PRD_MAT_SID=AL1.PRD_MAT_SID AND AL4.BMT_DAT_ID=AL1.BMT_DAT_ID AND AL1.PRP_DATE_ID=AL3.PRP_DATE_ID AND AL5.PRD_SPC_SID=AL1.PRD_SPC_SID)  AND (AL3.PRP_VAR LIKE '%ICT%' AND AL1.RUN_DATE BETWEEN TO_DATE('{fromDate}', 'DD-MM-yyyy HH24:MI:SS') AND TO_DATE('{toDate}', 'DD-MM-yyyy HH24:MI:SS') AND AL2.PDK_MATERIAL LIKE 'A%' AND AL1.RUN_STATE='P' AND (NOT AL5.PDK_AUFTR LIKE '%gold%') AND AL4.BMT_NAME IN ({FamilyICTs}))))";
                DataTable queryResult = _context.RunQuery(producedQuery);
                result.Add(new ProducedUnitsDTO()
                {
                    Quantity = int.Parse(queryResult.Rows[0]["PRODUCED"].ToString()),
                    Date = _FromDate,
                    FamilyID = FamilyID
                });
            }
            return result;
        }

        public static List<ProducedAndFilteredFPY> DataTableToProducedAndFilter(DataTable data)
        {

            List<ProducedAndFilteredFPY> result = new List<ProducedAndFilteredFPY>();
            foreach (DataRow row in data.Rows)
            {
                try
                {
                    string UNIT_ID_IN = row["UNIT_ID_IN"].ToString();
                    int PASS = int.Parse(row["PASS"].ToString());
                    string PRODUCT_DEFINITION = row["PRODUCT_DEFINITION"].ToString();
                    string SUB_DEVICE = row["SUB_DEVICE"].ToString();
                    string DST_EQUIPMENT = row["DST_EQUIPMENT"].ToString();
                    string UNIT_ID_IN_TYPE = row["UNIT_ID_IN_TYPE"].ToString();
                    string dateFROMDATE = row["START_DATE"].ToString();
                    string dateTODATE = row["END_DATE"].ToString();
                    DateTime start_date = DateTime.ParseExact(dateFROMDATE, "dd-MMM-yyyy HH:mm:ss", null);
                    DateTime end_date = DateTime.ParseExact(dateTODATE, "dd-MMM-yyyy HH:mm:ss", null);

                    result.Add(new ProducedAndFilteredFPY()
                    {
                        UNIT_ID_IN = UNIT_ID_IN,
                        PASS = PASS,

                    PRODUCT_DEFINITION = PRODUCT_DEFINITION,
                        SUB_DEVICE = SUB_DEVICE,
                        DST_EQUIPMENT = DST_EQUIPMENT,
                        UNIT_ID_IN_TYPE = UNIT_ID_IN_TYPE,
                        START_DATE = start_date,
                        END_DATE = end_date,
                    });
                }
                catch (Exception) { }
            }
            return result;
        }

        public static List<PRODUCEDMAX> DataTableToProducedMAX(DataTable data)
        {

            List<PRODUCEDMAX> result = new List<PRODUCEDMAX>();
            foreach (DataRow row in data.Rows)
            {
                try
                {
                    string product_definition = row["PRODUCT_DEFINITION"].ToString();
                    string dateFROMDATE = row["START_DATE"].ToString();
                    string dateTODATE = row["END_DATE"].ToString();
                    string sub_device = row["SUB_DEVICE"].ToString();
                    string dst_equipment = row["DST_EQUIPMENT"].ToString();
                    DateTime start_date = DateTime.ParseExact(dateFROMDATE, "dd-MMM-yyyy HH:mm:ss", null);
                    DateTime end_date = DateTime.ParseExact(dateTODATE, "dd-MMM-yyyy HH:mm:ss", null);
                    int TOTAL_PASS = int.Parse(row["TOTAL_PASS"].ToString());
                    int TOTAL_FAIL = int.Parse(row["TOTAL_FAIL"].ToString());
                    int TOTAL = int.Parse(row["TOTAL"].ToString());
                    double FPY = double.Parse(row["FPY"].ToString());
                    
                    result.Add(new PRODUCEDMAX()
                    {
                        PRODUCT_DEFINITION= product_definition,
                        SUB_DEVICE =sub_device,
                        PROCESS= dst_equipment,
                        START_DATE=start_date,
                        END_DATE=end_date,
                        TOTAL_PASS=TOTAL_PASS,
                        TOTAL_FAIL=TOTAL_FAIL,
                        TOTAL=TOTAL,
                        FPY = FPY,
                    });
                }

                catch (Exception) { }
            }
            return result;
        }

        public static List<ProducedRAWFPY> DataTableToProducedRAW(DataTable data)
        {

            List<ProducedRAWFPY> result = new List<ProducedRAWFPY>();
            foreach (DataRow row in data.Rows)
            {
                try
                {
                    string dateTimeAsString = row["RUN_DATE"].ToString();
                    ProducedRAWFPY rawData = new ProducedRAWFPY
                    {
                        SerialNumber = row["RUNID"].ToString(),
                        AUFTR = row["PDK_AUFTR"].ToString(),
                        STATE = row["RUN_STATE"].ToString(),
                        DATE = DateTime.Parse(dateTimeAsString),
                        MATERIAL = row["PDK_MATERIAL"].ToString(),
                        NAME = row["BMT_NAME"].ToString(),
                        VAR = row["PRP_VAR"].ToString(),
                        IDTYPE = (row["RUNID_TYPE"].ToString())
                    };
                    result.Add(rawData);
                }

                catch (Exception) { }
            }
            return result;
        }

        public static List<FailureFPY> DataTableToFailFPY(DataTable data)
        {

            List<FailureFPY> result = new List<FailureFPY>();
            foreach (DataRow row in data.Rows)
            {
                try
                {
                    //Console.WriteLine("==========");
                    string dateTimeAsString = row["RUN_DATE"].ToString();
                    FailureFPY failure = new FailureFPY
                    {
                        SerialNumber = row["RUNID"].ToString(),
                        AUFTR = row["PDK_AUFTR"].ToString(),
                        STATE = row["RUN_STATE"].ToString(),
                        DATE = DateTime.Parse(dateTimeAsString),
                        MATERIAL = row["PDK_MATERIAL"].ToString(),
                        NAME = row["BMT_NAME"].ToString(),
                        VAR = row["PRP_VAR"].ToString(),
                        IDTYPE = row["RUNID_TYPE"].ToString(),
                        NUM = row["MRK_NUM"].ToString(),
                        BEZ = row["MRK_BEZ"].ToString(),
                    };


                    result.Add(failure);

                }

                catch (Exception) { }

            }
            return result;

        }

        public static List<PiecesAnalyzed> DataTableToPiecesAnalyzed(DataTable data)
        {
            List<PiecesAnalyzed> result = new List<PiecesAnalyzed>();
            foreach (DataRow row in data.Rows)
            {
                try
                {
                    string dateAsString = row["Run_Date"].ToString();
                    PiecesAnalyzed ana = new PiecesAnalyzed
                    {
                        SerialNumber = row["Runid"].ToString(),
                        Date = DateTime.Parse(dateAsString),
                        Component = row["Mrk_Num"].ToString(),
                        ComponentPartNumber = row["Mrk_Txt"].ToString(),
                        AnalisisComment = row["Txt_Info"].ToString(),
                        Carrier = row["Car_Id"].ToString(),
                        Equipment = row["Equipment"].ToString(),
                    };

                    result.Add(ana);
                }
                catch (Exception)
                {
                }
            }
            return result;
        }



    }
}
