using analysistools.api.Contracts;
using analysistools.api.Models.Continental;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Globalization;

namespace analysistools.api.Controllers
{
    [Route("api")]
    [ApiController]
    public class AnalisiController : ControllerBase
    {
        private readonly IMesRepository _mesRepository;
        public AnalisiController(IMesRepository mesRepository)
        {
            _mesRepository = mesRepository;
        }


        [HttpGet("MES/PiecesAnalyzed/{fromDate}/{toDate}")]
        public ActionResult GetPiecesAnalyzedFromMES(string fromDate, string toDate)
        {

            DateTime FromDate = DateTime.ParseExact(fromDate, "dd-MM-yyyy", CultureInfo.InvariantCulture);
            DateTime ToDate = DateTime.ParseExact(toDate, "dd-MM-yyyy", CultureInfo.InvariantCulture);

            List<PiecesAnalyzed> failures = _mesRepository.GetPiecesAnalyzed(FromDate, ToDate);
            return Ok(failures);
        }
    }
}