using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using analysistools.api.Data;
using analysistools.api.Models.FPY;
using analysistools.api.Models.FPY.PRODUCTS;
using System.Globalization;

namespace analysistools.api.Controllers.FPYControllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class Z_Controller : ControllerBase
    {
        private readonly AppDbContext _context;

        public Z_Controller(AppDbContext context)
        {
            _context = context;
        }

        //[HttpGet("MES/DataByDay")]
        ////[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Administrator")]
        //public async Task<ActionResult<List<RAW_DATA>>> GetDATAFromMESByDay()
        //{

        //    DateTime currentDate = DateTime.Now.Date;
        //    DateTime fromDate = currentDate.AddDays(-1);
        //    List<RAW_DATA> FPYData = _mesRepository.GetRAW_DATAs(fromDate, currentDate);

        //    // Agregar los datos a la tabla RAW_DATAs
        //    await _context.RAW_DATAs.AddRangeAsync(FPYData);
        //    await _context.SaveChangesAsync();

        //// Agrupar los datos por día, material, var e idtype, y obtener la cantidad de elementos en cada grupo
        //var groupedData = FPYData
        //    .GroupBy(data => new { data.DATE, data.MATERIAL, data.VAR, data.IDTYPE })
        //    .Select(group => new
        //    {
        //        CantidadAgrupados = group.Count(),
        //        Dia = group.Key.DATE,
        //        Material = group.Key.MATERIAL,
        //        Var = group.Key.VAR,
        //        IDType = group.Key.IDTYPE
        //    })
        //    .ToList();

        //List<RAW_DATAFILTER> FPYDataFiler = groupedData
        //.Select(data => new RAW_DATAFILTER
        //{
        //    CANTIDAD = data.CantidadAgrupados,
        //    DATE = data.Dia,
        //    MATERIAL = data.Material,
        //    VAR = data.Var,
        //    IDTYPE = data.IDType
        //})
        //.ToList();

        //await _context.RAW_DATAsFilter.AddRangeAsync(FPYDataFiler);

        // Guardar los cambios en la base de datos
        //await _context.SaveChangesAsync();

        //    return Ok(FPYData);
        //}

        //[HttpGet("ProducedAndFilterByLocalDBByFamily/{FamilyId}/{fromDate}/{toDate}")]
        //public async Task<ActionResult> GetProducedAndFilterByLocalDBByFamily(int FamilyId, string fromDate, string toDate)
        //{
        //    DateTime FromDate = DateTime.ParseExact(fromDate, "dd-MM-yyyy", CultureInfo.InvariantCulture);
        //    DateTime ToDate = DateTime.ParseExact(toDate, "dd-MM-yyyy", CultureInfo.InvariantCulture);

        //    List<ProducedAndFilteredFPY> result = new List<ProducedAndFilteredFPY>();

            //double diffDays = (ToDate - FromDate).TotalDays;
            //if (!(diffDays > 0 && diffDays <= 7)) return BadRequest("Solo se permite maximo 7 dias");

            //FamilyFPY family = await _context.FamiliesFPY.FindAsync(FamilyId);
            //if (family == null) return NotFound("The family doesn`t exist.");
            //List<LineFPY> lines = _context.LinesFPY.Where(l => l.FamilyId == family.Id).ToList();

            //List<ProcessFPY> processes = new List<ProcessFPY>();
            //foreach (LineFPY line in lines)
            //{
            //    processes.AddRange(_context.ProcessesFPY.Where(w => w.LineID == line.Id).ToList());
            //}

            //List<StationFPY> stations = new List<StationFPY>();
            //foreach (ProcessFPY process in processes)
            //{
            //    stations.AddRange(_context.StationsFPY.Where(s => s.ProcessID == process.Id).ToList());
            //}

            //List<ModelFPY> models = new List<ModelFPY>();
            //foreach (StationFPY station in stations)
            //{
            //    models.AddRange(_context.ModelsFPY.Where(s => s.StationID == station.Id).ToList());
            //}

            //List<ProducedAndFilteredFPY> ProducedFilteredByStation = new List<ProducedAndFilteredFPY>();
            //foreach (StationFPY station in stations)
            //{
            //    ProducedFilteredByStation.AddRange(_context.ProducedAndFilteredFPYs.Where(f => f.Name == station.Name && f.Date >= FromDate && f.Date <= ToDate).ToList());
            //}

            //List<ModelFPY> ProducedFilteredByModel = new List<ModelFPY>();
            //foreach (ProducedAndFilteredFPY falla in ProducedFilteredByStation)
            //{
            //    ProducedFilteredByModel.AddRange(_context.ModelsFPY.Where(f => f.Name_Model == falla.Material).ToList());
            //}
            //result.AddRange(ProducedFilteredByStation);
            //result = result.OrderBy(f => f.Date).ToList();

        //    var producedList = await _GetDataLocalDB.FilterProducedByFamily(FamilyId, FromDate, ToDate)

        //    //CONTADOR de piezas producidas
        //    int CANTIDADTOTAL = result.Sum(x => x.Amount);

        //    //CONTEO POR PROCESO
        //    int CountUMG = result.Where(s => s.Var == "UMG_PIN").Sum(x => x.Amount);
        //    int CountICT = result.Where(s => s.Var == "ICT" || s.Var == "ICT_LDM3-4").Sum(x => x.Amount);
        //    int CountFLASH = result.Where(s => s.Var == "FLASH").Sum(x => x.Amount);
        //    int CountHSTAKE = result.Where(s => s.Var == "HSTAKE").Sum(x => x.Amount);
        //    int CountLDM3_RTV = result.Where(s => s.Var == "LDM3_RTV").Sum(x => x.Amount);
        //    int CountLDM3_GFILL = result.Where(s => s.Var == "LDM3_GFILL").Sum(x => x.Amount);
        //    int CountLDM2_CRI = result.Where(s => s.Var == "LDM2_CRIMPING_ST1_1" || s.Var == "LDM2_CRIMPING_ST3" || s.Var == "LDM2_CRIMPING_ST1_2" || s.Var == "LDM2_CRIMPING_ST2").Sum(x => x.Amount);
        //    int CountSCREW = result.Where(s => s.Var == "SCREW").Sum(x => x.Amount);
        //    int CountLDM2_FIN = result.Where(s => s.Var == "LDM2_FIN").Sum(x => x.Amount);

        //    //Objeto guarda Conteo por proceso
        //    var TotalsCountByProcess = new
        //    {
        //        CountUMG,
        //        CountICT,
        //        CountFLASH,
        //        CountHSTAKE,
        //        CountLDM3_RTV,
        //        CountLDM3_GFILL,
        //        CountLDM2_CRI,
        //        CountSCREW,
        //        CountLDM2_FIN,
        //    };

        //    //FILTRADO POR PROCESO
        //    var UMG = result.Where(s => s.Var == "UMG_PIN").ToList();
        //    var ICT = result.Where(s => s.Var == "ICT" || s.Var == "ICT_LDM3-4").ToList();
        //    var FLASH = result.Where(s => s.Var == "FLASH").ToList();
        //    var HSTAKE = result.Where(s => s.Var == "HSTAKE").ToList();
        //    var LDM3_RTV = result.Where(s => s.Var == "LDM3_RTV").ToList();
        //    var LDM3_GFILL = result.Where(s => s.Var == "LDM3_GFILL").ToList();
        //    var LDM2_CRI = result.Where(s => s.Var == "LDM2_CRIMPING_ST1_1" || s.Var == "LDM2_CRIMPING_ST3" || s.Var == "LDM2_CRIMPING_ST1_2" || s.Var == "LDM2_CRIMPING_ST2").ToList();
        //    var SCREW = result.Where(s => s.Var == "SCREW").ToList();
        //    var LDM2_FIN = result.Where(s => s.Var == "LDM2_FIN").ToList();
        //    //Objeto guarda FILTRADO por proceso
        //    var ProducedByProcess = new
        //    {
        //        UMG = UMG,
        //        ICT = ICT,
        //        FLASH = FLASH,
        //        HSTAKE = HSTAKE,
        //        LDM3_RTV = LDM3_RTV,
        //        LDM3_GFILL = LDM3_GFILL,
        //        LDM2_CRI = LDM2_CRI,
        //        SCREW = SCREW,
        //        LDM2_FIN = LDM2_FIN,
        //    };

        //    //Objeto final piezas producidas
        //    var response = new
        //    {
        //        TotalProduced = CANTIDADTOTAL,
        //        TotalsCountByProcess = TotalsCountByProcess,
        //        ProducedByProcess = ProducedByProcess,
        //        Granularidad = result,
        //    };

        //    return Ok(response);
        //}

        // GET: api/FPYLDM2FAIL/All/dd-MM-yyyy/dd-MM-yyyy
        [HttpGet("All/{fromDate}/{toDate}")]
        public async Task<ActionResult> GetAllProducedFromLocalDB(string fromDate, string toDate)
        {
            DateTime FromDate = DateTime.ParseExact(fromDate, "dd-MM-yyyy", CultureInfo.InvariantCulture);
            DateTime ToDate = DateTime.ParseExact(toDate, "dd-MM-yyyy", CultureInfo.InvariantCulture);

            double diffDays = (ToDate - FromDate).TotalDays;
            if (!(diffDays > 0 && diffDays <= 30)) return BadRequest("Query to the local database are only allowed for a maximum of 30 days");

            List<FPYLDM2PRO> result = await _context.ProducedUnitsLDM2FPY.Where(f => f.DIA >= FromDate && f.DIA <= ToDate && f.PROCESO != "SCRAP" && f.PROCESO != "ANALYSE_ICT" && f.PROCESO != "REPAIR"
            && f.PROCESO != "ANALYSE_EOL" && f.PROCESO != "ANALYSE_RTV").OrderBy(f => f.DIA).ToListAsync();
            result = result.OrderBy(f => f.DIA).ToList();
            return Ok(result);
        }

        // GET: api/FPYLDM2FAIL/AllWithProcesses/LineID/dd-MM-yyyy/dd-MM-yyyy
        [HttpGet("AllWithProcesses/{fromDate}/{toDate}")]
        public async Task<ActionResult> GetAllProducedFromLocalDBProcess(string fromDate, string toDate)
        {
            DateTime FromDate = DateTime.ParseExact(fromDate, "dd-MM-yyyy", CultureInfo.InvariantCulture);
            DateTime ToDate = DateTime.ParseExact(toDate, "dd-MM-yyyy", CultureInfo.InvariantCulture);

            double diffDays = (ToDate - FromDate).TotalDays;
            if (!(diffDays > 0 && diffDays <= 30))
            {
                return BadRequest("Query to the local database are only allowed for a maximum of 30 days");
            }

            List<FPYLDM2PRO> UnitsPass = await _context.ProducedUnitsLDM2FPY.Where(f => f.DIA >= FromDate && f.DIA <= ToDate).OrderBy(f => f.DIA).ToListAsync();

            UnitsPass = UnitsPass.OrderBy(f => f.DIA).ToList();
            int CANTIDADTOTAL = UnitsPass.Sum(x => x.CANTIDAD);
            var DespreciarAnalisis = UnitsPass.Where(u => !string.IsNullOrEmpty(u.PROCESO) && u.PROCESO != "A&R_ANA_BSA02" && u.PROCESO != "A&R_ANA_BSA04" && u.PROCESO != "A&R_ANA_BSA06"
            && u.PROCESO != "AR_ANA_BS02" && u.PROCESO != "AR_ANA_BS06" && u.PROCESO != "TEST" && u.PROCESO != "REP_EOL" && u.PROCESO != "REP_HSG" && u.PROCESO != "REP_FLSH" && u.PROCESO != "REP_CLBL").ToList();

            //CONTEO POR PROCESO
            int TotalFailuresUMG = DespreciarAnalisis.Where(s => s.PROCESO == "UMG_PIN").Sum(x => x.CANTIDAD);
            int TotalFailuresICT = DespreciarAnalisis.Where(s => s.PROCESO == "ICT" || s.PROCESO == "ICT_LDM3-4").Sum(x => x.CANTIDAD);
            int TotalFailuresFLASH = DespreciarAnalisis.Where(s => s.PROCESO == "FLASH").Sum(x => x.CANTIDAD);
            int TotalFailuresHSTAKE = DespreciarAnalisis.Where(s => s.PROCESO == "HSTAKE").Sum(x => x.CANTIDAD);
            int TotalFailuresLDM3_RTV = DespreciarAnalisis.Where(s => s.PROCESO == "LDM3_RTV").Sum(x => x.CANTIDAD);
            int TotalFailuresLDM3_GFILL = DespreciarAnalisis.Where(s => s.PROCESO == "LDM3_GFILL").Sum(x => x.CANTIDAD);
            int TotalFailuresLDM2_CRI = DespreciarAnalisis.Where(s => s.PROCESO == "LDM2_CRIMPING_ST1_1" || s.PROCESO == "LDM2_CRIMPING_ST3" || s.PROCESO == "LDM2_CRIMPING_ST1_2" || s.PROCESO == "LDM2_CRIMPING_ST2").Sum(x => x.CANTIDAD);
            int TotalFailuresSCREW = DespreciarAnalisis.Where(s => s.PROCESO == "SCREW").Sum(x => x.CANTIDAD);
            int TotalFailuresLDM2_FIN = DespreciarAnalisis.Where(s => s.PROCESO == "LDM2_FIN").Sum(x => x.CANTIDAD);
            //Objeto guarda Conteo por proceso
            var TotalsByProcess = new
            {
                UMG = TotalFailuresUMG,
                ICT = TotalFailuresICT,
                FLASH = TotalFailuresFLASH,
                HSTAKE = TotalFailuresHSTAKE,
                LDM3_RTV = TotalFailuresLDM3_RTV,
                LDM3_GFILL = TotalFailuresLDM3_GFILL,
                LDM2_CRI = TotalFailuresLDM2_CRI,
                SCREW = TotalFailuresSCREW,
                LDM2_FIN = TotalFailuresLDM2_FIN,
            };
            //FILTRADO POR PROCESO
            var FailsByUMG = DespreciarAnalisis.Where(s => s.PROCESO == "UMG_PIN").ToList();
            var FailsByICT = DespreciarAnalisis.Where(s => s.PROCESO == "ICT" || s.PROCESO == "ICT_LDM3-4").ToList();
            var FailsByFLASH = DespreciarAnalisis.Where(s => s.PROCESO == "FLASH").ToList();
            var FailsByHSTAKE = DespreciarAnalisis.Where(s => s.PROCESO == "HSTAKE").ToList();
            var FailsByLDM3_RTV = DespreciarAnalisis.Where(s => s.PROCESO == "LDM3_RTV").ToList();
            var FailsByLDM3_GFILL = DespreciarAnalisis.Where(s => s.PROCESO == "LDM3_GFILL").ToList();
            var FailsByLDM2_CRI = DespreciarAnalisis.Where(s => s.PROCESO == "LDM2_CRIMPING_ST1_1" || s.PROCESO == "LDM2_CRIMPING_ST3" || s.PROCESO == "LDM2_CRIMPING_ST1_2" || s.PROCESO == "LDM2_CRIMPING_ST2").ToList();
            var FailsBySCREW = DespreciarAnalisis.Where(s => s.PROCESO == "SCREW").ToList();
            var FailsByLDM2_FIN = DespreciarAnalisis.Where(s => s.PROCESO == "LDM2_FIN").ToList();
            //Objeto guarda FILTRADO por proceso
            var FailsByProcess = new
            {
                UMG = FailsByUMG,
                ICT = FailsByICT,
                FLASH = FailsByFLASH,
                HSTAKE = FailsByHSTAKE,
                LDM3_RTV = FailsByLDM3_RTV,
                LDM3_GFILL = FailsByLDM3_GFILL,
                LDM2_CRI = FailsByLDM2_CRI,
                SCREW = FailsBySCREW,
                LDM2_FIN = FailsByLDM2_FIN,
            };

            var response = new
            {
                TotalPRoducidas = CANTIDADTOTAL,
                TotalsByProcess,
                FailsByProcess,
            };
            return Ok(response);
        }

        // GET: api/FPYLDM2FAIL/FailsByLine/LineID/dd-MM-yyyy/dd-MM-yyyy
        [HttpGet("FailsByLine/{LineID}/{fromDate}/{toDate}")]
        public async Task<ActionResult> GetProFromLocalDBByLine(int LineID, string fromDate, string toDate)
        {
            DateTime FromDate = DateTime.ParseExact(fromDate, "dd-MM-yyyy", CultureInfo.InvariantCulture);
            DateTime ToDate = DateTime.ParseExact(toDate, "dd-MM-yyyy", CultureInfo.InvariantCulture);

            List<FPYLDM2FAIL> result = new List<FPYLDM2FAIL>();

            double diffDays = (ToDate - FromDate).TotalDays;
            if (!(diffDays > 0 && diffDays <= 30)) return BadRequest("Queries to the local database are only allowed for a maximum of 30 days");

            LineFPY line = await _context.LinesFPY.FindAsync(LineID);
            if (line == null) return NotFound("The line doesn`t exist.");


            List<ProcessFPY> processes = _context.ProcessesFPY.Where(l => l.LineID == line.Id).ToList();

            List<StationFPY> stations = new List<StationFPY>();
            foreach (ProcessFPY proces in processes)
            {
                stations.AddRange(_context.StationsFPY.Where(w => w.ProcessID == proces.Id).ToList());
            }

            List<ModelFPY> models = new List<ModelFPY>();
            foreach (StationFPY station in stations)
            {
                models.AddRange(_context.ModelsFPY.Where(s => s.StationID == station.Id).ToList());
            }

            List<FPYLDM2FAIL> FaultsFilteredByStation = new List<FPYLDM2FAIL>();
            foreach (StationFPY station in stations)
            {
                FaultsFilteredByStation.AddRange(_context.FailsUnitsLDM2FPY.Where(f => f.STATION == station.Name && f.DATE >= FromDate && f.DATE <= ToDate).ToList());
            }

            List<ModelFPY> FaultsFilteredByModel = new List<ModelFPY>();
            foreach (FPYLDM2FAIL falla in FaultsFilteredByStation)
            {
                FaultsFilteredByModel.AddRange(_context.ModelsFPY.Where(f => f.Name_Model == falla.MATERIAL).ToList());
            }

            FaultsFilteredByStation = FaultsFilteredByStation.OrderBy(f => f.DATE).ToList();
            int totalFailures = FaultsFilteredByStation
                .Where(u => !string.IsNullOrEmpty(u.FAILURE) && u.PROCESS != "SCRAP" && u.PROCESS != "ANALYSE_ICT" && u.PROCESS != "REPAIR"
            && u.PROCESS != "ANALYSE_EOL" && u.PROCESS != "ANALYSE_RTV").Count();
            //CONTEO POR PROCESO
            int TotalFailuresUMG = FaultsFilteredByStation.Where(s => s.PROCESS == "UMG_PIN").Count();
            int TotalFailuresICT = FaultsFilteredByStation.Where(s => s.PROCESS == "ICT" || s.PROCESS == "ICT_LDM3-4").Count();
            int TotalFailuresFLASH = FaultsFilteredByStation.Where(s => s.PROCESS == "FLASH").Count();
            int TotalFailuresHSTAKE = FaultsFilteredByStation.Where(s => s.PROCESS == "HSTAKE").Count();
            int TotalFailuresLDM3_RTV = FaultsFilteredByStation.Where(s => s.PROCESS == "LDM3_RTV").Count();
            int TotalFailuresLDM3_GFILL = FaultsFilteredByStation.Where(s => s.PROCESS == "LDM3_GFILL").Count();
            int TotalFailuresLDM2_CRI = FaultsFilteredByStation.Where(s => s.PROCESS == "LDM2_CRIMPING_ST1_1" || s.PROCESS == "LDM2_CRIMPING_ST3" || s.PROCESS == "LDM2_CRIMPING_ST1_2" || s.PROCESS == "LDM2_CRIMPING_ST2").Count();
            int TotalFailuresSCREW = FaultsFilteredByStation.Where(s => s.PROCESS == "SCREW").Count();
            int TotalFailuresLDM2_FIN = FaultsFilteredByStation.Where(s => s.PROCESS == "LDM2_FIN").Count();
            //Objeto guarda Conteo por proceso
            var TotalsByProcess = new
            {
                TotalFailuresUMG,
                TotalFailuresICT,
                TotalFailuresFLASH,
                TotalFailuresHSTAKE,
                TotalFailuresLDM3_RTV,
                TotalFailuresLDM3_GFILL,
                TotalFailuresLDM2_CRI,
                TotalFailuresSCREW,
                TotalFailuresLDM2_FIN,
            };
            //FILTRADO POR PROCESO
            var FailsByUMG = FaultsFilteredByStation.Where(s => s.PROCESS == "UMG_PIN").ToList();
            var FailsByICT = FaultsFilteredByStation.Where(s => s.PROCESS == "ICT" || s.PROCESS == "ICT_LDM3-4").ToList();
            var FailsByFLASH = FaultsFilteredByStation.Where(s => s.PROCESS == "FLASH").ToList();
            var FailsByHSTAKE = FaultsFilteredByStation.Where(s => s.PROCESS == "HSTAKE").ToList();
            var FailsByLDM3_RTV = FaultsFilteredByStation.Where(s => s.PROCESS == "LDM3_RTV").ToList();
            var FailsByLDM3_GFILL = FaultsFilteredByStation.Where(s => s.PROCESS == "LDM3_GFILL").ToList();
            var FailsByLDM2_CRI = FaultsFilteredByStation.Where(s => s.PROCESS == "LDM2_CRIMPING_ST1_1" || s.PROCESS == "LDM2_CRIMPING_ST3" || s.PROCESS == "LDM2_CRIMPING_ST1_2" || s.PROCESS == "LDM2_CRIMPING_ST2").ToList();
            var FailsBySCREW = FaultsFilteredByStation.Where(s => s.PROCESS == "SCREW").ToList();
            var FailsByLDM2_FIN = FaultsFilteredByStation.Where(s => s.PROCESS == "LDM2_FIN").ToList();
            //Objeto guarda FILTRADO por proceso
            var FailsByProcess = new
            {
                UMG = FailsByUMG,
                ICT = FailsByICT,
                FLASH = FailsByFLASH,
                HSTAKE = FailsByHSTAKE,
                LDM3_RTV = FailsByLDM3_RTV,
                LDM3_GFILL = FailsByLDM3_GFILL,
                LDM2_CRI = FailsByLDM2_CRI,
                SCREW = FailsBySCREW,
                LDM2_FIN = FailsByLDM2_FIN,
            };

            var response = new
            {
                line = line.Name,
                lineId = line.Id,
                TotalFailures = totalFailures,
                TotalsByProcess,
                FailsByProcess
            };

            return Ok(response);
        }

        // GET: api/FPYLDM2FAIL/OnlyByOneProcessAndDate/ProcessID/dd-MM-yyyy/dd-MM-yyyy
        [HttpGet("OnlyByOneProcessAndDate/{ProcessID}/{fromDate}/{toDate}")]
        public async Task<ActionResult> GetDataFromLocalDBbyProcess(int ProcessID, string fromDate, string toDate)
        {
            DateTime FromDate = DateTime.ParseExact(fromDate, "dd-MM-yyyy", CultureInfo.InvariantCulture);
            DateTime ToDate = DateTime.ParseExact(toDate, "dd-MM-yyyy", CultureInfo.InvariantCulture);

            List<FPYLDM2FAIL> result = new List<FPYLDM2FAIL>();

            double diffDays = (ToDate - FromDate).TotalDays;
            if (!(diffDays > 0 && diffDays <= 30)) return BadRequest("Queries to the local database are only allowed for a maximum of 30 days");

            ProcessFPY Process = await _context.ProcessesFPY.FindAsync(ProcessID);
            if (Process == null) return NotFound("The Process doesn`t exist.");
            List<StationFPY> stations = _context.StationsFPY.Where(l => l.ProcessID == Process.Id).ToList();

            List<FPYLDM2FAIL> FaultsFilteredByStation = new List<FPYLDM2FAIL>();
            foreach (StationFPY station in stations)
            {
                FaultsFilteredByStation.AddRange(_context.FailsUnitsLDM2FPY.Where(f => f.STATION == station.Name && f.DATE >= FromDate && f.DATE <= ToDate).ToList());
            }

            List<ModelFPY> FaultsFilteredByModel = new List<ModelFPY>();
            foreach (FPYLDM2FAIL falla in FaultsFilteredByStation)
            {
                FaultsFilteredByModel.AddRange(_context.ModelsFPY.Where(f => f.Name_Model == falla.MATERIAL).ToList());
            }
            FaultsFilteredByStation = FaultsFilteredByStation.OrderBy(f => f.DATE).ToList();
            int totalFailures = FaultsFilteredByStation
                .Where(u => !string.IsNullOrEmpty(u.FAILURE) && u.PROCESS != "SCRAP" && u.PROCESS != "ANALYSE_ICT" && u.PROCESS != "REPAIR"
            && u.PROCESS != "ANALYSE_EOL" && u.PROCESS != "ANALYSE_RTV").Count();
            //CONTEO POR PROCESO
            int TotalFailuresUMG = FaultsFilteredByStation.Where(s => s.PROCESS == "UMG_PIN").Count();
            int TotalFailuresICT = FaultsFilteredByStation.Where(s => s.PROCESS == "ICT" || s.PROCESS == "ICT_LDM3-4").Count();
            int TotalFailuresFLASH = FaultsFilteredByStation.Where(s => s.PROCESS == "FLASH").Count();
            int TotalFailuresHSTAKE = FaultsFilteredByStation.Where(s => s.PROCESS == "HSTAKE").Count();
            int TotalFailuresLDM3_RTV = FaultsFilteredByStation.Where(s => s.PROCESS == "LDM3_RTV").Count();
            int TotalFailuresLDM3_GFILL = FaultsFilteredByStation.Where(s => s.PROCESS == "LDM3_GFILL").Count();
            int TotalFailuresLDM2_CRI = FaultsFilteredByStation.Where(s => s.PROCESS == "LDM2_CRIMPING_ST1_1" || s.PROCESS == "LDM2_CRIMPING_ST3" || s.PROCESS == "LDM2_CRIMPING_ST1_2" || s.PROCESS == "LDM2_CRIMPING_ST2").Count();
            int TotalFailuresSCREW = FaultsFilteredByStation.Where(s => s.PROCESS == "SCREW").Count();
            int TotalFailuresLDM2_FIN = FaultsFilteredByStation.Where(s => s.PROCESS == "LDM2_FIN").Count();
            //Objeto guarda Conteo por proceso
            var TotalsByProcess = new
            {
                TotalFailuresUMG,
                TotalFailuresICT,
                TotalFailuresFLASH,
                TotalFailuresHSTAKE,
                TotalFailuresLDM3_RTV,
                TotalFailuresLDM3_GFILL,
                TotalFailuresLDM2_CRI,
                TotalFailuresSCREW,
                TotalFailuresLDM2_FIN,
            };
            //FILTRADO POR PROCESO
            var FailsByUMG = FaultsFilteredByStation.Where(s => s.PROCESS == "UMG_PIN").ToList();
            var FailsByICT = FaultsFilteredByStation.Where(s => s.PROCESS == "ICT" || s.PROCESS == "ICT_LDM3-4").ToList();
            var FailsByFLASH = FaultsFilteredByStation.Where(s => s.PROCESS == "FLASH").ToList();
            var FailsByHSTAKE = FaultsFilteredByStation.Where(s => s.PROCESS == "HSTAKE").ToList();
            var FailsByLDM3_RTV = FaultsFilteredByStation.Where(s => s.PROCESS == "LDM3_RTV").ToList();
            var FailsByLDM3_GFILL = FaultsFilteredByStation.Where(s => s.PROCESS == "LDM3_GFILL").ToList();
            var FailsByLDM2_CRI = FaultsFilteredByStation.Where(s => s.PROCESS == "LDM2_CRIMPING_ST1_1" || s.PROCESS == "LDM2_CRIMPING_ST3" || s.PROCESS == "LDM2_CRIMPING_ST1_2" || s.PROCESS == "LDM2_CRIMPING_ST2").ToList();
            var FailsBySCREW = FaultsFilteredByStation.Where(s => s.PROCESS == "SCREW").ToList();
            var FailsByLDM2_FIN = FaultsFilteredByStation.Where(s => s.PROCESS == "LDM2_FIN").ToList();
            //Objeto guarda FILTRADO por proceso
            var FailsByProcess = new
            {
                FailsByUMG,
                FailsByICT,
                FailsByFLASH,
                FailsByHSTAKE,
                FailsByLDM3_RTV,
                FailsByLDM3_GFILL,
                FailsByLDM2_CRI,
                FailsBySCREW,
                FailsByLDM2_FIN,
            };

            var response = new
            {
                Process = Process.Name,
                ProcessID = Process.Id,
                TotalFailures = totalFailures,
                TotalsByProcess,
                FailsByProcess
            };


            return Ok(response);
        }

        // GET: api/FPYLDM2FAIL/ByStationAndDate/LineID/dd-MM-yyyy/dd-MM-yyyy
        [HttpGet("ByStationAndDateold/{StationID}/{fromDate}/{toDate}")]
        public async Task<ActionResult> GetDataFromLocalDBbyStationold(int StationID, string fromDate, string toDate)
        {
            DateTime FromDate = DateTime.ParseExact(fromDate, "dd-MM-yyyy", CultureInfo.InvariantCulture);
            DateTime ToDate = DateTime.ParseExact(toDate, "dd-MM-yyyy", CultureInfo.InvariantCulture);

            List<FPYLDM2FAIL> result = new List<FPYLDM2FAIL>();

            double diffDays = (ToDate - FromDate).TotalDays;
            if (!(diffDays > 0 && diffDays <= 30)) return BadRequest("Queries to the local database are only allowed for a maximum of 30 days");

            StationFPY station = await _context.StationsFPY.FindAsync(StationID);
            if (station == null) return NotFound("The Station doesn`t exist.");
            List<FPYLDM2FAIL> PassFilteredByStation = _context.FailsUnitsLDM2FPY.Where(l => l.STATION == station.Name && l.DATE >= FromDate && l.DATE <= ToDate).ToList();

            List<ModelFPY> FaultsFilteredByModel = new List<ModelFPY>();
            foreach (FPYLDM2FAIL falla in PassFilteredByStation)
            {
                FaultsFilteredByModel.AddRange(_context.ModelsFPY.Where(f => f.Name_Model == falla.MATERIAL).ToList());
            }
            result.AddRange(PassFilteredByStation);

            result = result.OrderBy(f => f.DATE).ToList();
            return Ok(result);
        }

        // GET: api/FPYLDM2PRO/AllByDate/dd-MM-yyyy/dd-MM-yyyy
        [HttpGet("AllByDate/{fromDate}/{toDate}")]
        public async Task<ActionResult> GetProFromLocalDB(string fromDate, string toDate)
        {
            DateTime FromDate = DateTime.ParseExact(fromDate, "dd-MM-yyyy", CultureInfo.InvariantCulture);
            DateTime ToDate = DateTime.ParseExact(toDate, "dd-MM-yyyy", CultureInfo.InvariantCulture);

            double diffDays = (ToDate - FromDate).TotalDays;
            if (!(diffDays > 0 && diffDays <= 30)) return BadRequest("Queries to the local database are only allowed for a maximum of 30 days");

            List<FPYLDM2PRO> result = await _context.ProducedUnitsLDM2FPY.Where(f => f.DIA >= FromDate && f.DIA <= ToDate).OrderBy(f => f.DIA).ToListAsync();
            return Ok(result);
        }

        // GET: api/FPYLDM2PRO/ByLineAndDate/LineID/dd-MM-yyyy/dd-MM-yyyy
        [HttpGet("ByLineAndDate/{LineID}/{fromDate}/{toDate}")]
        public async Task<ActionResult> GetProFromLocalDBByLineOLD(int LineID, string fromDate, string toDate)
        {
            DateTime FromDate = DateTime.ParseExact(fromDate, "dd-MM-yyyy", CultureInfo.InvariantCulture);
            DateTime ToDate = DateTime.ParseExact(toDate, "dd-MM-yyyy", CultureInfo.InvariantCulture);

            List<FPYLDM2PRO> result = new List<FPYLDM2PRO>();

            double diffDays = (ToDate - FromDate).TotalDays;
            if (!(diffDays > 0 && diffDays <= 30)) return BadRequest("Queries to the local database are only allowed for a maximum of 30 days");

            LineFPY line = await _context.LinesFPY.FindAsync(LineID);
            if (line == null) return NotFound("The line doesn`t exist.");


            List<ProcessFPY> processes = _context.ProcessesFPY.Where(l => l.LineID == line.Id).ToList();

            List<StationFPY> stations = new List<StationFPY>();
            foreach (ProcessFPY proces in processes)
            {
                stations.AddRange(_context.StationsFPY.Where(w => w.ProcessID == proces.Id).ToList());
            }

            List<ModelFPY> models = new List<ModelFPY>();
            foreach (StationFPY station in stations)
            {
                models.AddRange(_context.ModelsFPY.Where(s => s.StationID == station.Id).ToList());
            }

            List<FPYLDM2PRO> FaultsFilteredByStation = new List<FPYLDM2PRO>();
            foreach (StationFPY station in stations)
            {
                FaultsFilteredByStation.AddRange(_context.ProducedUnitsLDM2FPY.Where(f => f.ESTACION == station.Name && f.DIA >= FromDate && f.DIA <= ToDate).ToList());
            }

            List<ModelFPY> FaultsFilteredByModel = new List<ModelFPY>();
            foreach (FPYLDM2PRO falla in FaultsFilteredByStation)
            {
                FaultsFilteredByModel.AddRange(_context.ModelsFPY.Where(f => f.Name_Model == falla.MODELO).ToList());
            }
            result.AddRange(FaultsFilteredByStation);

            result = result.OrderBy(f => f.DIA).ToList();
            return Ok(result);
        }

        // GET: api/FPYLDM2PRO/ByProcessAndDate/LineID/dd-MM-yyyy/dd-MM-yyyy
        [HttpGet("ByProcessAndDate/{ProcessID}/{fromDate}/{toDate}")]
        public async Task<ActionResult> GetDataFromLocalDBbyProcessOLD(int ProcessID, string fromDate, string toDate)
        {
            DateTime FromDate = DateTime.ParseExact(fromDate, "dd-MM-yyyy", CultureInfo.InvariantCulture);
            DateTime ToDate = DateTime.ParseExact(toDate, "dd-MM-yyyy", CultureInfo.InvariantCulture);

            List<FPYLDM2PRO> result = new List<FPYLDM2PRO>();

            double diffDays = (ToDate - FromDate).TotalDays;
            if (!(diffDays > 0 && diffDays <= 30)) return BadRequest("Queries to the local database are only allowed for a maximum of 30 days");

            ProcessFPY Process = await _context.ProcessesFPY.FindAsync(ProcessID);
            if (Process == null) return NotFound("The Process doesn`t exist.");
            List<StationFPY> stations = _context.StationsFPY.Where(l => l.ProcessID == Process.Id).ToList();

            List<FPYLDM2PRO> FaultsFilteredByStation = new List<FPYLDM2PRO>();
            foreach (StationFPY station in stations)
            {
                FaultsFilteredByStation.AddRange(_context.ProducedUnitsLDM2FPY.Where(f => f.ESTACION == station.Name && f.DIA >= FromDate && f.DIA <= ToDate).ToList());
            }

            List<ModelFPY> FaultsFilteredByModel = new List<ModelFPY>();
            foreach (FPYLDM2PRO falla in FaultsFilteredByStation)
            {
                FaultsFilteredByModel.AddRange(_context.ModelsFPY.Where(f => f.Name_Model == falla.MODELO).ToList());
            }
            result.AddRange(FaultsFilteredByStation);

            result = result.OrderBy(f => f.DIA).ToList();
            return Ok(result);
        }

        // GET: api/FPYLDM2PRO/ByStationAndDate/LineID/dd-MM-yyyy/dd-MM-yyyy
        [HttpGet("ByStationAndDate/{StationID}/{fromDate}/{toDate}")]
        public async Task<ActionResult> GetDataFromLocalDBbyStationOLD(int StationID, string fromDate, string toDate)
        {
            DateTime FromDate = DateTime.ParseExact(fromDate, "dd-MM-yyyy", CultureInfo.InvariantCulture);
            DateTime ToDate = DateTime.ParseExact(toDate, "dd-MM-yyyy", CultureInfo.InvariantCulture);

            List<FPYLDM2PRO> result = new List<FPYLDM2PRO>();

            double diffDays = (ToDate - FromDate).TotalDays;
            if (!(diffDays > 0 && diffDays <= 30)) return BadRequest("Queries to the local database are only allowed for a maximum of 30 days");

            StationFPY station = await _context.StationsFPY.FindAsync(StationID);
            if (station == null) return NotFound("The Station doesn`t exist.");
            List<FPYLDM2PRO> PassFilteredByStation = _context.ProducedUnitsLDM2FPY.Where(l => l.ESTACION == station.Name && l.DIA >= FromDate && l.DIA <= ToDate).ToList();

            List<ModelFPY> FaultsFilteredByModel = new List<ModelFPY>();
            foreach (FPYLDM2PRO falla in PassFilteredByStation)
            {
                FaultsFilteredByModel.AddRange(_context.ModelsFPY.Where(f => f.Name_Model == falla.MODELO).ToList());
            }
            result.AddRange(PassFilteredByStation);

            result = result.OrderBy(f => f.DIA).ToList();
            return Ok(result);
        }

        // GET: api/FPYLDM2PRO/ByModelAndDate/LineID/dd-MM-yyyy/dd-MM-yyyy
        [HttpGet("ByModelAndDate/{modelName}/{fromDate}/{toDate}")]
        public async Task<ActionResult> GetDataFromLocalDBbyModelAndDate(string modelName, string fromDate, string toDate)
        {
            DateTime FromDate = DateTime.ParseExact(fromDate, "dd-MM-yyyy", CultureInfo.InvariantCulture);
            DateTime ToDate = DateTime.ParseExact(toDate, "dd-MM-yyyy", CultureInfo.InvariantCulture);

            List<FPYLDM2PRO> result = new List<FPYLDM2PRO>();

            double diffDays = (ToDate - FromDate).TotalDays;
            if (!(diffDays > 0 && diffDays <= 30)) return BadRequest("Queries to the local database are only allowed for a maximum of 30 days");

            List<FPYLDM2PRO> PassFilteredByModel = await _context.ProducedUnitsLDM2FPY
                .Where(l => l.MODELO == modelName && l.DIA >= FromDate && l.DIA <= ToDate)
                .ToListAsync();

            result.AddRange(PassFilteredByModel);
            result = result.OrderBy(f => f.DIA).ToList();

            return Ok(result);
        }

        // GET: api/FPYLDM2PRO
        [HttpGet]
        public async Task<ActionResult<IEnumerable<FPYLDM2PRO>>> GetProducedUnitsLDM2FPY()
        {
            return await _context.ProducedUnitsLDM2FPY.ToListAsync();
        }

        // GET: api/FPYLDM2PRO/5
        [HttpGet("{id}")]
        public async Task<ActionResult<FPYLDM2PRO>> GetFPYLDM2PRO(string id)
        {
            var fPYLDM2PRO = await _context.ProducedUnitsLDM2FPY.FindAsync(id);

            if (fPYLDM2PRO == null)
            {
                return NotFound();
            }

            return fPYLDM2PRO;
        }

        // PUT: api/FPYLDM2PRO/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutFPYLDM2PRO(string id, FPYLDM2PRO fPYLDM2PRO)
        {
            if (id != fPYLDM2PRO.ID)
            {
                return BadRequest();
            }

            _context.Entry(fPYLDM2PRO).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!FPYLDM2PROExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/FPYLDM2PRO
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<FPYLDM2PRO>> PostFPYLDM2PRO(FPYLDM2PRO fPYLDM2PRO)
        {
            _context.ProducedUnitsLDM2FPY.Add(fPYLDM2PRO);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (FPYLDM2PROExists(fPYLDM2PRO.ID))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetFPYLDM2PRO", new { id = fPYLDM2PRO.ID }, fPYLDM2PRO);
        }

        // DELETE: api/FPYLDM2PRO/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteFPYLDM2PRO(string id)
        {
            var fPYLDM2PRO = await _context.ProducedUnitsLDM2FPY.FindAsync(id);
            if (fPYLDM2PRO == null)
            {
                return NotFound();
            }

            _context.ProducedUnitsLDM2FPY.Remove(fPYLDM2PRO);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool FPYLDM2PROExists(string id)
        {
            return _context.ProducedUnitsLDM2FPY.Any(e => e.ID == id);
        }
    }
}
