using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using analysistools.api.Data;
using analysistools.api.Models.FPY.PRODUCTS;
using analysistools.api.Models.FPY;
using System.Globalization;

namespace analysistools.api.Controllers.FPYControllers.PRODUCTOS.LDM2
{
    [Route("api/[controller]")]
    [ApiController]
    public class FPYLDM2FAILController : ControllerBase
    {
        private readonly AppDbContext _context;

        public FPYLDM2FAILController(AppDbContext context)
        {
            _context = context;
        }


        // GET: api/FPYLDM2FAIL/All/dd-MM-yyyy/dd-MM-yyyy
        [HttpGet("All/{fromDate}/{toDate}")]
        public async Task<ActionResult> GetFailFromLocalDB(string fromDate, string toDate)
        {
            DateTime FromDate = DateTime.ParseExact(fromDate, "dd-MM-yyyy", CultureInfo.InvariantCulture);
            DateTime ToDate = DateTime.ParseExact(toDate, "dd-MM-yyyy", CultureInfo.InvariantCulture);

            double diffDays = (ToDate - FromDate).TotalDays;
            if (!(diffDays > 0 && diffDays <= 30)) return BadRequest("Queries to the local database are only allowed for a maximum of 30 days");

            List<FPYLDM2FAIL> result = await _context.FailsUnitsLDM2FPY.Where(f => f.DATE >= FromDate && f.DATE <= ToDate && f.PROCESS != "SCRAP" && f.PROCESS != "ANALYSE_ICT" && f.PROCESS != "REPAIR"
            && f.PROCESS != "ANALYSE_EOL" && f.PROCESS != "ANALYSE_RTV").OrderBy(f => f.DATE).ToListAsync();
            result = result.OrderBy(f => f.DATE).ToList();
            return Ok(result);
        }

        // GET: api/FPYLDM2FAIL/AllWithProcesses/LineID/dd-MM-yyyy/dd-MM-yyyy
        [HttpGet("AllWithProcesses/{fromDate}/{toDate}")]
        public async Task<ActionResult> GetFailFromLocalDBbyProcess(string fromDate, string toDate)
        {
            DateTime FromDate = DateTime.ParseExact(fromDate, "dd-MM-yyyy", CultureInfo.InvariantCulture);
            DateTime ToDate = DateTime.ParseExact(toDate, "dd-MM-yyyy", CultureInfo.InvariantCulture);

            double diffDays = (ToDate - FromDate).TotalDays;
            if (!(diffDays > 0 && diffDays <= 30))
            {
                return BadRequest("Queries to the local database are only allowed for a maximum of 30 days");
            }

            List<FPYLDM2FAIL> UnitsFails = await _context.FailsUnitsLDM2FPY.Where(f => f.DATE >= FromDate && f.DATE <= ToDate).OrderBy(f => f.DATE).ToListAsync();

            UnitsFails = UnitsFails.OrderBy(f => f.DATE).ToList();
            int totalFailures = UnitsFails
                .Where(u => !string.IsNullOrEmpty(u.FAILURE) 
                && u.PROCESS != "SCRAP" 
                && u.PROCESS != "ANALYSE_ICT" 
                && u.PROCESS != "ANALYSE_FLASH" 
                && u.PROCESS != "ANALYSE_PCBA" 
                && u.PROCESS != "ANALYSE_RTV" 
                && u.PROCESS != "ANALYSE_DNG" 
                && u.PROCESS != "ANALYSE_PVA" 
                && u.PROCESS != "ANALYSE_SCREW" 
                && u.PROCESS != "ANALYSE_HOUSING" 
                && u.PROCESS != "ANALYSE_PRES" 
                && u.PROCESS != "ANALYSE_PINC" 
                && u.PROCESS != "ANALYSE_LABEL" 
                && u.PROCESS != "ANALYSE_EOL" 
                && u.PROCESS != "ANALYSE" 
                && u.PROCESS != "ANALYSE_SCREW"
                && u.PROCESS != "INTERCON-3"
                && u.PROCESS != "LDM_EVAP_2"
                && u.PROCESS != "REPAIR"
                ).Count();
            //CONTEO POR PROCESO
            int T_UMG = UnitsFails.Where(s => s.PROCESS == "UMG_PIN").Count();
            int T_ICT = UnitsFails.Where(s => s.PROCESS == "ICT" || s.PROCESS == "ICT_LDM3-4").Count();
            int T_FLASH = UnitsFails.Where(s => s.PROCESS == "FLASH").Count();
            int T_HSTAKE = UnitsFails.Where(s => s.PROCESS == "HSTAKE").Count();
            int T_LDM3_RTV = UnitsFails.Where(s => s.PROCESS == "LDM3_RTV").Count();
            int T_LDM3_GFILL = UnitsFails.Where(s => s.PROCESS == "LDM3_GFILL").Count();
            int T_LDM2_CRI = UnitsFails.Where(s => s.PROCESS == "LDM2_CRIMPING_ST1_1" || s.PROCESS == "LDM2_CRIMPING_ST3" || s.PROCESS == "LDM2_CRIMPING_ST1_2" || s.PROCESS == "LDM2_CRIMPING_ST2").Count();
            int T_SCREW = UnitsFails.Where(s => s.PROCESS == "SCREW").Count();
            int T_LDM2_FIN = UnitsFails.Where(s => s.PROCESS == "LDM2_FIN").Count();
            //Objeto guarda Conteo por proceso
            var TotalsByProcess = new
            {
                UMG = T_UMG,
                ICT = T_ICT,
                FLASH = T_FLASH,
                HSTAKE = T_HSTAKE,
                LDM3_RTV = T_LDM3_RTV,
                LDM3_GFILL = T_LDM3_GFILL,
                LDM2_CRI = T_LDM2_CRI,
                SCREW = T_SCREW,
                LDM2_FIN = T_LDM2_FIN,
            };
            //FILTRADO POR PROCESO
            var FailsByUMG = UnitsFails.Where(s => s.PROCESS == "UMG_PIN").ToList();
            var FailsByICT = UnitsFails.Where(s => s.PROCESS == "ICT" || s.PROCESS == "ICT_LDM3-4").ToList();
            var FailsByFLASH = UnitsFails.Where(s => s.PROCESS == "FLASH").ToList();
            var FailsByHSTAKE = UnitsFails.Where(s => s.PROCESS == "HSTAKE").ToList();
            var FailsByLDM3_RTV = UnitsFails.Where(s => s.PROCESS == "LDM3_RTV").ToList();
            var FailsByLDM3_GFILL = UnitsFails.Where(s => s.PROCESS == "LDM3_GFILL").ToList();
            var FailsByLDM2_CRI = UnitsFails.Where(s => s.PROCESS == "LDM2_CRIMPING_ST1_1" || s.PROCESS == "LDM2_CRIMPING_ST3" || s.PROCESS == "LDM2_CRIMPING_ST1_2" || s.PROCESS == "LDM2_CRIMPING_ST2").ToList();
            var FailsBySCREW = UnitsFails.Where(s => s.PROCESS == "SCREW").ToList();
            var FailsByLDM2_FIN = UnitsFails.Where(s => s.PROCESS == "LDM2_FIN").ToList();
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
                TotalFailures = totalFailures,
                TotalsByProcess,
                FailsByProcess = FailsByProcess,
                ListUnitsFails = UnitsFails
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
                TotalFailuresUMG = TotalFailuresUMG,
                TotalFailuresICT = TotalFailuresICT,
                TotalFailuresFLASH = TotalFailuresFLASH,
                TotalFailuresHSTAKE = TotalFailuresHSTAKE,
                TotalFailuresLDM3_RTV = TotalFailuresLDM3_RTV,
                TotalFailuresLDM3_GFILL = TotalFailuresLDM3_GFILL,
                TotalFailuresLDM2_CRI = TotalFailuresLDM2_CRI,
                TotalFailuresSCREW = TotalFailuresSCREW,
                TotalFailuresLDM2_FIN = TotalFailuresLDM2_FIN,
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
                TotalsByProcess = TotalsByProcess,
                FailsByProcess = FailsByProcess
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
                TotalFailuresUMG = TotalFailuresUMG,
                TotalFailuresICT = TotalFailuresICT,
                TotalFailuresFLASH = TotalFailuresFLASH,
                TotalFailuresHSTAKE = TotalFailuresHSTAKE,
                TotalFailuresLDM3_RTV = TotalFailuresLDM3_RTV,
                TotalFailuresLDM3_GFILL = TotalFailuresLDM3_GFILL,
                TotalFailuresLDM2_CRI = TotalFailuresLDM2_CRI,
                TotalFailuresSCREW = TotalFailuresSCREW,
                TotalFailuresLDM2_FIN = TotalFailuresLDM2_FIN,
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
                Process = Process.Name,
                ProcesID = Process.Id,
                TotalFailures = totalFailures,
                TotalsByProcess = TotalsByProcess,
                FailsByProcess = FailsByProcess
            };

            return Ok(response);
        }

        // GET: api/FPYLDM2FAIL/ByStationAndDate/LineID/dd-MM-yyyy/dd-MM-yyyy
        [HttpGet("ByStationAndDate/{StationID}/{fromDate}/{toDate}")]
        public async Task<ActionResult> GetDataFromLocalDBbyStation(int StationID, string fromDate, string toDate)
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

        // GET: api/FPYLDM2FAIL/ByModelAndDate/LineID/dd-MM-yyyy/dd-MM-yyyy
        [HttpGet("ByModelAndDate/{modelName}/{fromDate}/{toDate}")]
        public async Task<ActionResult> GetDataFromLocalDBbyModelAndDate(string modelName, string fromDate, string toDate)
        {
            DateTime FromDate = DateTime.ParseExact(fromDate, "dd-MM-yyyy", CultureInfo.InvariantCulture);
            DateTime ToDate = DateTime.ParseExact(toDate, "dd-MM-yyyy", CultureInfo.InvariantCulture);

            List<FPYLDM2FAIL> result = new List<FPYLDM2FAIL>();

            double diffDays = (ToDate - FromDate).TotalDays;
            if (!(diffDays > 0 && diffDays <= 30)) return BadRequest("Queries to the local database are only allowed for a maximum of 30 days");

            List<FPYLDM2FAIL> PassFilteredByModel = await _context.FailsUnitsLDM2FPY
                .Where(l => l.MATERIAL == modelName && l.DATE >= FromDate && l.DATE <= ToDate)
                .ToListAsync();

            result.AddRange(PassFilteredByModel);
            result = result.OrderBy(f => f.DATE).ToList();

            return Ok(result);
        }


        // GET: api/FPYLDM2FAIL
        [HttpGet]
        public async Task<ActionResult<IEnumerable<FPYLDM2FAIL>>> GetFPYLDM2FAIL()
        {
            return await _context.FailsUnitsLDM2FPY.ToListAsync();
        }

        // GET: api/FPYLDM2FAIL/5
        [HttpGet("{id}")]
        public async Task<ActionResult<FPYLDM2FAIL>> GetFPYLDM2FAIL(string id)
        {
            var fPYLDM2FAIL = await _context.FailsUnitsLDM2FPY.FindAsync(id);

            if (fPYLDM2FAIL == null)
            {
                return NotFound();
            }

            return fPYLDM2FAIL;
        }

        // PUT: api/FPYLDM2FAIL/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutFPYLDM2FAIL(string id, FPYLDM2FAIL fPYLDM2FAIL)
        {
            if (id != fPYLDM2FAIL.ID)
            {
                return BadRequest();
            }

            _context.Entry(fPYLDM2FAIL).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!FPYLDM2FAILExists(id))
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

        // POST: api/FPYLDM2FAIL
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<FPYLDM2FAIL>> PostFPYLDM2FAIL(FPYLDM2FAIL fPYLDM2FAIL)
        {
            _context.FailsUnitsLDM2FPY.Add(fPYLDM2FAIL);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (FPYLDM2FAILExists(fPYLDM2FAIL.ID))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetFPYLDM2FAIL", new { id = fPYLDM2FAIL.ID }, fPYLDM2FAIL);
        }

        // DELETE: api/FPYLDM2FAIL/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteFPYLDM2FAIL(string id)
        {
            var fPYLDM2FAIL = await _context.FailsUnitsLDM2FPY.FindAsync(id);
            if (fPYLDM2FAIL == null)
            {
                return NotFound();
            }

            _context.FailsUnitsLDM2FPY.Remove(fPYLDM2FAIL);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool FPYLDM2FAILExists(string id)
        {
            return _context.FailsUnitsLDM2FPY.Any(e => e.ID == id);
        }
    }
}
