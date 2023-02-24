using analysistools.api.Contracts;
using analysistools.api.Data;
using analysistools.api.Models.FPY;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Text;
using analysistools.api.Models.FPY.PRODUCTS;
using NuGet.Packaging;

namespace analysistools.api.Controllers.FPYControllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FpyController : ControllerBase
    {
        private readonly IMesRepository _mesRepository;
        private readonly AppDbContext _context;

        public FpyController(IMesRepository mesRepository, AppDbContext context)
        {
            _mesRepository = mesRepository;
            _context = context;
        }


        [HttpGet("MES/ProducedAndFilteredByCurrentDay")]
        public async Task<ActionResult<List<ProducedAndFilteredFPY>>> GetProducedByCurrentDay()
        {

            DateTime currentDate = DateTime.Now.Date;
            DateTime fromDate = currentDate.AddDays(-1);

            List<ProducedAndFilteredFPY> FPYData = _mesRepository.GetProducedAndFiltereds(fromDate, currentDate);

            await _context.ProducedAndFilteredFPYs.AddRangeAsync(FPYData);
            await _context.SaveChangesAsync();

            return Ok(FPYData);
        }


        [HttpGet("MES/ProducedRawByCurrentDay")]
        public async Task<ActionResult<List<ProducedRAWFPY>>> GetProducedRawByCurrentDay()
        {

            DateTime currentDate = DateTime.Now.Date;
            DateTime fromDate = currentDate.AddDays(-1);

            List<ProducedRAWFPY> FPYData = _mesRepository.GetProducedRAWFPYs(fromDate, currentDate);

            await _context.ProducedRAWFPY.AddRangeAsync(FPYData);
            await _context.SaveChangesAsync();

            return Ok(FPYData);
        }


        [HttpGet("MES/Fails")]
        public async Task<ActionResult<List<FailureFPY>>> GetFailsFromMES()
        {
            DateTime currentDate = DateTime.Now.Date;
            DateTime fromDate = currentDate.AddDays(-1);

            List<FailureFPY> FPYFail = _mesRepository.GetRAW_Fails(fromDate, currentDate);

            await _context.RAW_FAILs.AddRangeAsync(FPYFail);
            await _context.SaveChangesAsync();

            return Ok(FPYFail);
        }


        [HttpGet("ProducedAndFilterByLocalDBByFamily/{FamilyId}/{fromDate}/{toDate}")]
        public async Task<ActionResult> GetProducedAndFilterByLocalDBByFamily(int FamilyId, string fromDate, string toDate)
        {
            DateTime FromDate = DateTime.ParseExact(fromDate, "dd-MM-yyyy", CultureInfo.InvariantCulture);
            DateTime ToDate = DateTime.ParseExact(toDate, "dd-MM-yyyy", CultureInfo.InvariantCulture);

            List<ProducedAndFilteredFPY> result = new List<ProducedAndFilteredFPY>();

            double diffDays = (ToDate - FromDate).TotalDays;
            if (!(diffDays > 0 && diffDays <= 7)) return BadRequest("Solo se permite maximo 7 dias");

            FamilyFPY family = await _context.FamiliesFPY.FindAsync(FamilyId);
            if (family == null) return NotFound("The family doesn`t exist.");
            List<LineFPY> lines = _context.LinesFPY.Where(l => l.FamilyId == family.Id).ToList();

            List<ProcessFPY> processes = new List<ProcessFPY>();
            foreach (LineFPY line in lines)
            {
                processes.AddRange(_context.ProcessesFPY.Where(w => w.LineID == line.Id).ToList());
            }

            List<StationFPY> stations = new List<StationFPY>();
            foreach (ProcessFPY process in processes)
            {
                stations.AddRange(_context.StationsFPY.Where(s => s.ProcessID == process.Id).ToList());
            }

            List<ModelFPY> models = new List<ModelFPY>();
            foreach (StationFPY station in stations)
            {
                models.AddRange(_context.ModelsFPY.Where(s => s.StationID == station.Id).ToList());
            }

            List<ProducedAndFilteredFPY> ProducedFilteredByStation = new List<ProducedAndFilteredFPY>();
            foreach (StationFPY station in stations)
            {
                ProducedFilteredByStation.AddRange(_context.ProducedAndFilteredFPYs.Where(f => f.Name == station.Name && f.Date >= FromDate && f.Date <= ToDate).ToList());
            }

            List<ModelFPY> ProducedFilteredByModel = new List<ModelFPY>();
            foreach (ProducedAndFilteredFPY falla in ProducedFilteredByStation)
            {
                ProducedFilteredByModel.AddRange(_context.ModelsFPY.Where(f => f.Name_Model == falla.Material).ToList());
            }
            result.AddRange(ProducedFilteredByStation);
            result = result.OrderBy(f => f.Date).ToList();

            //CONTADOR de piezas producidas
            int CANTIDADTOTAL = result.Sum(x => x.Amount);

            //CONTEO POR PROCESO
            int CountUMG = result.Where(s => s.Var == "UMG_PIN").Sum(x => x.Amount);
            int CountICT = result.Where(s => s.Var == "ICT" || s.Var == "ICT_LDM3-4").Sum(x => x.Amount);
            int CountFLASH = result.Where(s => s.Var == "FLASH").Sum(x => x.Amount);
            int CountHSTAKE = result.Where(s => s.Var == "HSTAKE").Sum(x => x.Amount);
            int CountLDM3_RTV = result.Where(s => s.Var == "LDM3_RTV").Sum(x => x.Amount);
            int CountLDM3_GFILL = result.Where(s => s.Var == "LDM3_GFILL").Sum(x => x.Amount);
            int CountLDM2_CRI = result.Where(s => s.Var == "LDM2_CRIMPING_ST1_1" || s.Var == "LDM2_CRIMPING_ST3" || s.Var == "LDM2_CRIMPING_ST1_2" || s.Var == "LDM2_CRIMPING_ST2").Sum(x => x.Amount);
            int CountSCREW = result.Where(s => s.Var == "SCREW").Sum(x => x.Amount);
            int CountLDM2_FIN = result.Where(s => s.Var == "LDM2_FIN").Sum(x => x.Amount);

            //Objeto guarda Conteo por proceso
            var TotalsCountByProcess = new
            {
                CountUMG,
                CountICT,
                CountFLASH,
                CountHSTAKE,
                CountLDM3_RTV,
                CountLDM3_GFILL,
                CountLDM2_CRI,
                CountSCREW,
                CountLDM2_FIN,
            };

            //FILTRADO POR PROCESO
            var UMG = result.Where(s => s.Var == "UMG_PIN").ToList();
            var ICT = result.Where(s => s.Var == "ICT" || s.Var == "ICT_LDM3-4").ToList();
            var FLASH = result.Where(s => s.Var == "FLASH").ToList();
            var HSTAKE = result.Where(s => s.Var == "HSTAKE").ToList();
            var LDM3_RTV = result.Where(s => s.Var == "LDM3_RTV").ToList();
            var LDM3_GFILL = result.Where(s => s.Var == "LDM3_GFILL").ToList();
            var LDM2_CRI = result.Where(s => s.Var == "LDM2_CRIMPING_ST1_1" || s.Var == "LDM2_CRIMPING_ST3" || s.Var == "LDM2_CRIMPING_ST1_2" || s.Var == "LDM2_CRIMPING_ST2").ToList();
            var SCREW = result.Where(s => s.Var == "SCREW").ToList();
            var LDM2_FIN = result.Where(s => s.Var == "LDM2_FIN").ToList();
            //Objeto guarda FILTRADO por proceso
            var ProducedByProcess = new
            {
                UMG = UMG,
                ICT = ICT,
                FLASH = FLASH,
                HSTAKE = HSTAKE,
                LDM3_RTV = LDM3_RTV,
                LDM3_GFILL = LDM3_GFILL,
                LDM2_CRI = LDM2_CRI,
                SCREW = SCREW,
                LDM2_FIN = LDM2_FIN,
            };

            //Objeto final piezas producidas
            var response = new
            {
                TotalProduced = CANTIDADTOTAL,
                TotalsCountByProcess = TotalsCountByProcess,
                ProducedByProcess = ProducedByProcess,
                Granularidad = result,
            };

            return Ok(response);
        }

        
        // GET: api/Fpy/ProducedAndFilterByLocalDBByLine/LineID/dd-MM-yyyy/dd-MM-yyyy
        [HttpGet("ProducedAndFilterByLocalDBByLine/{LineID}/{fromDate}/{toDate}")]
        public async Task<ActionResult> GetProducedAndFilterByLocalDBByLine(int LineID, string fromDate, string toDate)
        {
            DateTime FromDate = DateTime.ParseExact(fromDate, "dd-MM-yyyy", CultureInfo.InvariantCulture);
            DateTime ToDate = DateTime.ParseExact(toDate, "dd-MM-yyyy", CultureInfo.InvariantCulture);

            List<ProducedAndFilteredFPY> result = new List<ProducedAndFilteredFPY>();

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

            List<ProducedAndFilteredFPY> ProducedFilteredByStation = new List<ProducedAndFilteredFPY>();
            foreach (StationFPY station in stations)
            {
                ProducedFilteredByStation.AddRange(_context.ProducedAndFilteredFPYs.Where(f => f.Name == station.Name && f.Date >= FromDate && f.Date <= ToDate).ToList());
            }

            List<ModelFPY> ProducedFilteredByModel = new List<ModelFPY>();
            foreach (ProducedAndFilteredFPY falla in ProducedFilteredByStation)
            {
                ProducedFilteredByModel.AddRange(_context.ModelsFPY.Where(f => f.Name_Model == falla.Material).ToList());
            }
            result.AddRange(ProducedFilteredByStation);
            result = result.OrderBy(f => f.Date).ToList();

            //CONTADOR de piezas producidas
            int CANTIDADTOTAL = result.Sum(x => x.Amount);

            //CONTEO POR PROCESO
            int CountUMG = result.Where(s => s.Var == "UMG_PIN").Sum(x => x.Amount);
            int CountICT = result.Where(s => s.Var == "ICT" || s.Var == "ICT_LDM3-4").Sum(x => x.Amount);
            int CountFLASH = result.Where(s => s.Var == "FLASH").Sum(x => x.Amount);
            int CountHSTAKE = result.Where(s => s.Var == "HSTAKE").Sum(x => x.Amount);
            int CountLDM3_RTV = result.Where(s => s.Var == "LDM3_RTV").Sum(x => x.Amount);
            int CountLDM3_GFILL = result.Where(s => s.Var == "LDM3_GFILL").Sum(x => x.Amount);
            int CountLDM2_CRI = result.Where(s => s.Var == "LDM2_CRIMPING_ST1_1" || s.Var == "LDM2_CRIMPING_ST3" || s.Var == "LDM2_CRIMPING_ST1_2" || s.Var == "LDM2_CRIMPING_ST2").Sum(x => x.Amount);
            int CountSCREW = result.Where(s => s.Var == "SCREW").Sum(x => x.Amount);
            int CountLDM2_FIN = result.Where(s => s.Var == "LDM2_FIN").Sum(x => x.Amount);

            //Objeto guarda Conteo por proceso
            var TotalsCountByProcess = new
            {
                CountUMG,
                CountICT,
                CountFLASH,
                CountHSTAKE,
                CountLDM3_RTV,
                CountLDM3_GFILL,
                CountLDM2_CRI,
                CountSCREW,
                CountLDM2_FIN,
            };

            //FILTRADO POR PROCESO
            var UMG = result.Where(s => s.Var == "UMG_PIN").ToList();
            var ICT = result.Where(s => s.Var == "ICT" || s.Var == "ICT_LDM3-4").ToList();
            var FLASH = result.Where(s => s.Var == "FLASH").ToList();
            var HSTAKE = result.Where(s => s.Var == "HSTAKE").ToList();
            var LDM3_RTV = result.Where(s => s.Var == "LDM3_RTV").ToList();
            var LDM3_GFILL = result.Where(s => s.Var == "LDM3_GFILL").ToList();
            var LDM2_CRI = result.Where(s => s.Var == "LDM2_CRIMPING_ST1_1" || s.Var == "LDM2_CRIMPING_ST3" || s.Var == "LDM2_CRIMPING_ST1_2" || s.Var == "LDM2_CRIMPING_ST2").ToList();
            var SCREW = result.Where(s => s.Var == "SCREW").ToList();
            var LDM2_FIN = result.Where(s => s.Var == "LDM2_FIN").ToList();
            //Objeto guarda FILTRADO por proceso
            var ProducedByProcess = new
            {
                UMG = UMG,
                ICT = ICT,
                FLASH = FLASH,
                HSTAKE = HSTAKE,
                LDM3_RTV = LDM3_RTV,
                LDM3_GFILL = LDM3_GFILL,
                LDM2_CRI = LDM2_CRI,
                SCREW = SCREW,
                LDM2_FIN = LDM2_FIN,
            };

            //Objeto final piezas producidas
            var response = new
            {
                TotalProduced = CANTIDADTOTAL,
                TotalsCountByProcess = TotalsCountByProcess,
                ProducedByProcess= ProducedByProcess,
                Granularidad = result,
            };
            return Ok(response);
        }

        //
        //
        //
        //
        //
        //
        //
        //
        //
        //
        //
        //
        //
        //








        [HttpGet("FailsByFamilyID/{FamilyId}/{fromDate}/{toDate}")]
        public async Task<ActionResult> GetFailsFromLocalDB(int FamilyId, string fromDate, string toDate)
        {
            DateTime FromDate = DateTime.ParseExact(fromDate, "dd-MM-yyyy", CultureInfo.InvariantCulture);
            DateTime ToDate = DateTime.ParseExact(toDate, "dd-MM-yyyy", CultureInfo.InvariantCulture);

            List<FailureFPY> result = new List<FailureFPY>();

            double diffDays = (ToDate - FromDate).TotalDays;
            if (!(diffDays > 0 && diffDays <= 7)) return BadRequest("Solo se permite maximo 7 dias");

            FamilyFPY family = await _context.FamiliesFPY.FindAsync(FamilyId);
            if (family == null) return NotFound("The family doesn`t exist.");
            List<LineFPY> lines = _context.LinesFPY.Where(l => l.FamilyId == family.Id).ToList();

            List<ProcessFPY> processes = new List<ProcessFPY>();
            foreach (LineFPY line in lines)
            {
                processes.AddRange(_context.ProcessesFPY.Where(w => w.LineID == line.Id).ToList());
            }

            List<StationFPY> stations = new List<StationFPY>();
            foreach (ProcessFPY process in processes)
            {
                stations.AddRange(_context.StationsFPY.Where(s => s.ProcessID == process.Id).ToList());
            }

            List<FailureFPY> FailsFilteredByStation = new List<FailureFPY>();
            foreach (StationFPY station in stations)
            {
                FailsFilteredByStation.AddRange(_context.RAW_FAILs.Where(f => f.NAME == station.Name && f.DATE >= FromDate && f.DATE <= ToDate).ToList());
            }
            result.AddRange(FailsFilteredByStation);

            result = result.OrderBy(f => f.DATE).ToList();
            return Ok(result);
        }


        [HttpGet("FailsByProcess/{ProcessID}/{fromDate}/{toDate}")]
        public async Task<ActionResult> GetFailsFromLocalDBbyProcess(int ProcessID, string fromDate, string toDate)
        {
            DateTime FromDate = DateTime.ParseExact(fromDate, "dd-MM-yyyy", CultureInfo.InvariantCulture);
            DateTime ToDate = DateTime.ParseExact(toDate, "dd-MM-yyyy", CultureInfo.InvariantCulture);

            List<FailureFPY> result = new List<FailureFPY>();

            ProcessFPY Process = await _context.ProcessesFPY.FindAsync(ProcessID);
            if (Process == null) return NotFound("The Process doesn`t exist.");
            List<StationFPY> stations = _context.StationsFPY.Where(l => l.ProcessID == Process.Id).ToList();

            List<FailureFPY> FailsFilteredByStation = new List<FailureFPY>();
            foreach (StationFPY station in stations)
            {
                FailsFilteredByStation.AddRange(_context.RAW_FAILs.Where(f => f.NAME == station.Name && f.DATE >= FromDate && f.DATE <= ToDate).ToList());
            }
            result.AddRange(FailsFilteredByStation);

            result = result.OrderBy(f => f.DATE).ToList();
            return Ok(result);
        }


    }


}

