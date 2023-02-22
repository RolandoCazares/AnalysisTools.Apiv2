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



        [HttpGet("DataByFamilyID/{FamilyId}/{fromDate}/{toDate}")]
        public async Task<ActionResult> GetDataFromLocalDB(int FamilyId, string fromDate, string toDate)
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

            List<ProducedAndFilteredFPY> FaultsFilteredByStation = new List<ProducedAndFilteredFPY>();
            foreach(StationFPY station in stations)
            {   
                FaultsFilteredByStation.AddRange(_context.ProducedAndFilteredFPYs.Where(f => f.Name== station.Name && f.Date >= FromDate && f.Date <= ToDate).ToList());
            }
            result.AddRange(FaultsFilteredByStation);

            result = result.OrderBy(f => f.Date).ToList();
            return Ok(result);
        }


        [HttpGet("DataByProcess/{ProcessID}/{fromDate}/{toDate}")]
        public async Task<ActionResult> GetDataFromLocalDBbyProcess(int ProcessID, string fromDate, string toDate)
        {
            DateTime FromDate = DateTime.ParseExact(fromDate, "dd-MM-yyyy", CultureInfo.InvariantCulture);
            DateTime ToDate = DateTime.ParseExact(toDate, "dd-MM-yyyy", CultureInfo.InvariantCulture);

            List<ProducedAndFilteredFPY> result = new List<ProducedAndFilteredFPY>();

            ProcessFPY Process = await _context.ProcessesFPY.FindAsync(ProcessID);
            if (Process == null) return NotFound("The Process doesn`t exist.");
            List<StationFPY> stations = _context.StationsFPY.Where(l => l.ProcessID == Process.Id).ToList();

            List<ProducedAndFilteredFPY> FaultsFilteredByStation = new List<ProducedAndFilteredFPY>();
            foreach (StationFPY station in stations)
            {
                FaultsFilteredByStation.AddRange(_context.ProducedAndFilteredFPYs.Where(f => f.Name == station.Name && f.Date >= FromDate && f.Date <= ToDate).ToList());
            }
            result.AddRange(FaultsFilteredByStation);

            result = result.OrderBy(f => f.Date).ToList();
            return Ok(result);
        }

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

