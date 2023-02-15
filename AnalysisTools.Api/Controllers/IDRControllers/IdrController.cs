using analysistools.api.Contracts;
using analysistools.api.Data;
using analysistools.api.Models.IDR;
using analysistools.api.Models.IDR.DTO;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Text;

namespace analysistools.api.Controllers.IDRControllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class IdrController : ControllerBase
    {
        private readonly IMesRepository _mesRepository;
        private readonly AppDbContext _context;

        public IdrController(IMesRepository mesRepository, AppDbContext context)
        {
            _mesRepository = mesRepository;
            _context = context;
        }

        [HttpGet("MES/failures/{fromDate}/{toDate}")]
        //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Administrator")]
        public async Task<ActionResult<List<Failure>>> GetFailuresFromMES(string fromDate, string toDate)
        {
            DateTime FromDate = DateTime.ParseExact(fromDate, "dd-MM-yyyy", CultureInfo.InvariantCulture);
            DateTime ToDate = DateTime.ParseExact(toDate, "dd-MM-yyyy", CultureInfo.InvariantCulture);
            double diffDays = (ToDate - FromDate).TotalDays;
            if (!(diffDays > 0 && diffDays <= 7)) return BadRequest("Solo se permite maximo 7 dias");

            List<Failure> failures = new List<Failure>();

            failures = _mesRepository.GetFailuresIDR(FromDate, ToDate);

            await _context.Failures.AddRangeAsync(failures);
            await _context.SaveChangesAsync();

            return Ok(failures);
        }

        [HttpGet("MES/produced/{familyId}/{fromDate}/{toDate}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Administrator")]
        public async Task<ActionResult<int[]>> GetProducedFromMES(int familyId, string fromDate, string toDate)
        {
            DateTime FromDate = DateTime.ParseExact(fromDate, "dd-MM-yyyy", CultureInfo.InvariantCulture);
            DateTime ToDate = DateTime.ParseExact(toDate, "dd-MM-yyyy", CultureInfo.InvariantCulture);
            double diffDays = (ToDate - FromDate).TotalDays;
            if (!(diffDays > 0 && diffDays <= 31)) return BadRequest("Only maximum 31 days are allowed");

            FamilyIDR family = await _context.FamiliesIDR.FindAsync(familyId);
            if (family == null) return NotFound("The family doesn´t exist");
            List<LineIDR> lines = _context.LinesIDR.Where(l => l.FamilyId == family.Id).ToList();

            List<StationIDR> stations = new List<StationIDR>();
            foreach (LineIDR line in lines)
            {
                stations.AddRange(_context.StationsIDR.Where(s => s.LineId == line.Id).ToList());
            }

            if (stations.Count == 0) return BadRequest("No hay estaciones ICT registradas.");

            StringBuilder stringBuilder = new StringBuilder();
            foreach (StationIDR station in stations)
            {
                stringBuilder.Append($"'{station.Name.Trim()}',");
            }
            string rawStations = stringBuilder.ToString();
            string familyICTs = rawStations.Remove(rawStations.Length - 1, 1);

            List<ProducedUnitsDTO> producedUnitsList = _mesRepository.GetAllProducedIDR(familyICTs, family.Id, FromDate, ToDate);
            foreach (ProducedUnitsDTO item in producedUnitsList)
            {
                await _context.ProducedUnits.AddAsync(new ProducedUnits()
                {
                    Date = item.Date,
                    Quantity = item.Quantity,
                    FamilyID = item.FamilyID,
                });
            }
            await _context.SaveChangesAsync();
            return Ok(producedUnitsList);
        }

        [HttpGet("failures/{FamilyId}/{fromDate}/{toDate}")]
        public async Task<ActionResult<List<Failure>>> GetFailuresFromLocalDB(int FamilyId, string fromDate, string toDate)
        {
            DateTime FromDate = DateTime.ParseExact(fromDate, "dd-MM-yyyy", CultureInfo.InvariantCulture);
            DateTime ToDate = DateTime.ParseExact(toDate, "dd-MM-yyyy", CultureInfo.InvariantCulture);

            List<Failure> resultado = new List<Failure>();

            double diffDays = (ToDate - FromDate).TotalDays;

            if (!(diffDays > 0 && diffDays <= 7)) return BadRequest("Solo se permite maximo 7 dias");

            FamilyIDR family = await _context.FamiliesIDR.FindAsync(FamilyId);
            if (family == null) return NotFound("La familia no existe.");
            List<LineIDR> lines = _context.LinesIDR.Where(l => l.FamilyId == family.Id).ToList();

            List<StationIDR> stations = new List<StationIDR>();
            foreach (LineIDR line in lines)
            {
                stations.AddRange(_context.StationsIDR.Where(s => s.LineId == line.Id).ToList());
            }

            List<Failure> fallasFiltradasPorStation = new List<Failure>();
            foreach (StationIDR station in stations)
            {
                // Condicion especial para ldm y epm (Se prueban en la misma maquina)
                // TODO: Implementar una mejor manera de diferencias este tipo de eventos
                if (family.Name.Contains("LDM"))
                {

                    fallasFiltradasPorStation.AddRange(_context.Failures.Where(f => !f.ID_TYPE.Contains("SMT") && !f.ID_TYPE.Contains("EPM") && f.StationID == station.Name && f.Date_Time >= FromDate && f.Date_Time <= ToDate).ToList());
                }
                else if (family.Name.Contains("EPM"))
                {
                    fallasFiltradasPorStation.AddRange(_context.Failures.Where(f => !f.ID_TYPE.Contains("LDM") && f.StationID == station.Name && f.Date_Time >= FromDate && f.Date_Time <= ToDate).ToList());
                }
                else
                {
                    fallasFiltradasPorStation.AddRange(_context.Failures.Where(f => f.StationID == station.Name && f.Date_Time >= FromDate && f.Date_Time <= ToDate).ToList());
                }
            }

            List<Failure> fallasSinStation = _context.Failures.Where(f => f.StationID == String.Empty && f.ID_TYPE != "SMT_FIN_TMP").ToList();

            string cleanIdType = family.IdType;
            cleanIdType = Regex.Replace(cleanIdType, @",+", ",").Trim();
            cleanIdType = Regex.Replace(cleanIdType, @"\s+", "");
            string[] idTypes = cleanIdType.Split(',');

            List<Failure> fallasFiltradasPorIdType = new List<Failure>();
            foreach (string idType in idTypes)
            {
                fallasFiltradasPorIdType.AddRange(fallasSinStation.Where(f => f.ID_TYPE == idType && f.Date_Time >= FromDate && f.Date_Time <= ToDate));
            }

            resultado.AddRange(fallasFiltradasPorStation);
            resultado.AddRange(fallasFiltradasPorIdType);

            resultado = resultado.OrderBy(f => f.Date_Time).ToList();

            // Solo valores unicos
            resultado = resultado.GroupBy(f => f.Serial_Number).Select(f => f.FirstOrDefault()).ToList();

            return Ok(resultado);
        }

        [HttpGet("produced/{FamilyId}/{fromDate}/{toDate}")]
        public async Task<ActionResult<List<ProducedUnits>>> GetProducedUnitsFromLocalDB(int FamilyId, string fromDate, string toDate)
        {
            DateTime FromDate = DateTime.ParseExact(fromDate, "dd-MM-yyyy", CultureInfo.InvariantCulture);
            DateTime ToDate = DateTime.ParseExact(toDate, "dd-MM-yyyy", CultureInfo.InvariantCulture);

            List<ProducedUnits> resultado = new List<ProducedUnits>();

            double diffDays = (ToDate - FromDate).TotalDays;
            if (!(diffDays > 0 && diffDays <= 7)) return BadRequest("Solo se permite maximo 7 dias");

            for (int i = 0; i < diffDays; i++)
            {
                DateTime Date = FromDate.AddDays(i);
                ProducedUnits producedUnits = _context.ProducedUnits.FirstOrDefault(p => p.Date == Date && p.FamilyID == FamilyId);
                if (producedUnits != null)
                {
                    resultado.Add(producedUnits);
                }
            }

            return Ok(resultado);
        }
    }
}
