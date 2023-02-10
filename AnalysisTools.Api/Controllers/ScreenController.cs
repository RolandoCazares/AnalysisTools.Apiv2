using analysistools.api.Contracts;
using analysistools.api.Models.Optical;
using analysistools.api.Models.Screen;
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
    public class ScreenController : ControllerBase
    {
        private readonly IMesRepository _mesRepository;
        public ScreenController(IMesRepository mesRepository)
        {
            _mesRepository = mesRepository;
        }

        [HttpGet("MES/screenUnit/{SerialNumber}")]
        public ActionResult GetProcessFromMES(string SerialNumber)
        {
            List<ScrnDetails> failures = _mesRepository.GetSCRNprocess(SerialNumber);
            return Ok(failures); 
        }

        [HttpGet("MES/screenUnit2020/{fromDate}/{toDate}/{Station}/{IdType}")]
        public ActionResult GetProcess2020FromMES(DateTime fromDate, DateTime toDate, string Station, string IdType)
        {
            List<Scrn2020Details> failures = _mesRepository.GetSCRN2020process(fromDate, toDate, Station, IdType);
            return Ok(failures);
        }
    }
}


