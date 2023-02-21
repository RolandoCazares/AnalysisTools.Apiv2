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

        [HttpGet("MES/DataByDay")]
        //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Administrator")]
        public async Task<ActionResult<List<RAW_DATA>>> GetDATAFromMESByDay()
        {

            DateTime currentDate = DateTime.Now.Date;
            DateTime fromDate = currentDate.AddDays(-1);
            List<RAW_DATA> FPYData = _mesRepository.GetRAW_DATAs(fromDate, currentDate);

            // Agregar los datos a la tabla RAW_DATAs
            await _context.RAW_DATAs.AddRangeAsync(FPYData);
            await _context.SaveChangesAsync();

            // Agrupar los datos por día, material, var e idtype, y obtener la cantidad de elementos en cada grupo
            var groupedData = FPYData
                .GroupBy(data => new { data.DATE, data.MATERIAL, data.VAR, data.IDTYPE })
                .Select(group => new
                {
                    CantidadAgrupados = group.Count(),
                    Dia = group.Key.DATE,
                    Material = group.Key.MATERIAL,
                    Var = group.Key.VAR,
                    IDType = group.Key.IDTYPE
                })
                .ToList();

            List<RAW_DATAFILTER> FPYDataFiler = groupedData
            .Select(data => new RAW_DATAFILTER
            {
                CANTIDAD = data.CantidadAgrupados,
                DATE = data.Dia,
                MATERIAL = data.Material,
                VAR = data.Var,
                IDTYPE = data.IDType
            })
            .ToList();

            await _context.RAW_DATAsFilter.AddRangeAsync(FPYDataFiler);

            // Guardar los cambios en la base de datos
            await _context.SaveChangesAsync();

            return Ok(FPYData);
        }

        [HttpGet("MES/Data/{fromDate}/{toDate}")]
        //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Administrator")]
        public async Task<ActionResult<List<RAW_DATA>>> GetDATAFromMES(string fromDate, string toDate)
        {
            DateTime FromDate = DateTime.ParseExact(fromDate, "dd-MM-yyyy", CultureInfo.InvariantCulture);
            DateTime ToDate = DateTime.ParseExact(toDate, "dd-MM-yyyy", CultureInfo.InvariantCulture);
            double diffDays = (ToDate - FromDate).TotalDays;
            if (!(diffDays > 0 && diffDays <= 30)) return BadRequest("Solo se permite maximo 7 dias");
            List<RAW_DATA> FPYData = new List<RAW_DATA>();

            FPYData = _mesRepository.GetRAW_DATAs(FromDate, ToDate);
            await _context.RAW_DATAs.AddRangeAsync(FPYData);
            await _context.SaveChangesAsync();

            return Ok(FPYData);
        }

        [HttpGet("MES/Fails/{Producto}/{fromDate}/{toDate}")]
        //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Administrator")]
        public async Task<ActionResult<List<RAW_FAIL>>> GetFailsFromMES(string Producto, string fromDate, string toDate)
        {
            DateTime FromDate = DateTime.ParseExact(fromDate, "dd-MM-yyyy", CultureInfo.InvariantCulture);
            DateTime ToDate = DateTime.ParseExact(toDate, "dd-MM-yyyy", CultureInfo.InvariantCulture);
            double diffDays = (ToDate - FromDate).TotalDays;
            if (!(diffDays > 0 && diffDays <= 31)) return BadRequest("Only maximum 31 days are allowed");

            List<RAW_FAIL> FPYFail = new List<RAW_FAIL>();

            FPYFail = _mesRepository.GetRAW_Fails(Producto, FromDate, ToDate);

            await _context.RAW_FAILs.AddRangeAsync(FPYFail);
            await _context.SaveChangesAsync();

            return Ok(FPYFail);
        }

        [HttpGet("DataByFamilyID/{FamilyId}/{fromDate}/{toDate}")]
        public async Task<ActionResult> GetDataFromLocalDB(int FamilyId, string fromDate, string toDate)
        {
            DateTime FromDate = DateTime.ParseExact(fromDate, "dd-MM-yyyy", CultureInfo.InvariantCulture);
            DateTime ToDate = DateTime.ParseExact(toDate, "dd-MM-yyyy", CultureInfo.InvariantCulture);

            List<RAW_DATA> result = new List<RAW_DATA>();

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

            List<RAW_DATA> FaultsFilteredByStation = new List<RAW_DATA>();
            foreach(StationFPY station in stations)
            {   
                FaultsFilteredByStation.AddRange(_context.RAW_DATAs.Where(f => f.NAME== station.Name && f.DATE >= FromDate && f.DATE <= ToDate).ToList());
            }
            result.AddRange(FaultsFilteredByStation);

            result = result.OrderBy(f => f.DATE).ToList();
            return Ok(result);
        }


        [HttpGet("DataByProcess/{ProcessID}/{fromDate}/{toDate}")]
        public async Task<ActionResult> GetDataFromLocalDBbyProcess(int ProcessID, string fromDate, string toDate)
        {
            DateTime FromDate = DateTime.ParseExact(fromDate, "dd-MM-yyyy", CultureInfo.InvariantCulture);
            DateTime ToDate = DateTime.ParseExact(toDate, "dd-MM-yyyy", CultureInfo.InvariantCulture);

            List<RAW_DATA> result = new List<RAW_DATA>();

            ProcessFPY Process = await _context.ProcessesFPY.FindAsync(ProcessID);
            if (Process == null) return NotFound("The Process doesn`t exist.");
            List<StationFPY> stations = _context.StationsFPY.Where(l => l.ProcessID == Process.Id).ToList();

            List<RAW_DATA> FaultsFilteredByStation = new List<RAW_DATA>();
            foreach (StationFPY station in stations)
            {
                FaultsFilteredByStation.AddRange(_context.RAW_DATAs.Where(f => f.NAME == station.Name && f.DATE >= FromDate && f.DATE <= ToDate).ToList());
            }
            result.AddRange(FaultsFilteredByStation);

            result = result.OrderBy(f => f.DATE).ToList();
            return Ok(result);
        }

        [HttpGet("FailsByFamilyID/{FamilyId}/{fromDate}/{toDate}")]
        public async Task<ActionResult> GetFailsFromLocalDB(int FamilyId, string fromDate, string toDate)
        {
            DateTime FromDate = DateTime.ParseExact(fromDate, "dd-MM-yyyy", CultureInfo.InvariantCulture);
            DateTime ToDate = DateTime.ParseExact(toDate, "dd-MM-yyyy", CultureInfo.InvariantCulture);

            List<RAW_FAIL> result = new List<RAW_FAIL>();

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

            List<RAW_FAIL> FailsFilteredByStation = new List<RAW_FAIL>();
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

            List<RAW_FAIL> result = new List<RAW_FAIL>();

            ProcessFPY Process = await _context.ProcessesFPY.FindAsync(ProcessID);
            if (Process == null) return NotFound("The Process doesn`t exist.");
            List<StationFPY> stations = _context.StationsFPY.Where(l => l.ProcessID == Process.Id).ToList();

            List<RAW_FAIL> FailsFilteredByStation = new List<RAW_FAIL>();
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

