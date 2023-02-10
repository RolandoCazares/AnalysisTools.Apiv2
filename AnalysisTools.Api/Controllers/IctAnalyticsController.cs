using analysistools.api.Contracts;
using analysistools.api.Models.ProductionTests;
using Microsoft.AspNetCore.Mvc;

namespace analysistools.api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class IctAnalyticsController : ControllerBase
    {
        private readonly IMesRepository _mesRepository;
        public IctAnalyticsController(IMesRepository mesRepository)
        {
            _mesRepository = mesRepository;
        }

        [HttpGet("ByComponent/{Model}/{Component}/{fromDate}/{toDate}")]
        public ActionResult GetIctTestByComponent(string Model, string Component, string fromDate, string toDate)
        {
            DateTime FromDate = DateTime.Parse(fromDate);
            DateTime ToDate = DateTime.Parse(toDate);
            List<ICTTest> tests = _mesRepository.GetICTTests(Model, Component, null, FromDate, ToDate);
            return Ok(tests);
        }
        [HttpGet("ByTestNumber/{Model}/{TestNumber}/{fromDate}/{toDate}")]
        public ActionResult GetIctTestByTestNumber(string Model, string TestNumber, string fromDate, string toDate)
        {
            DateTime FromDate = DateTime.Parse(fromDate);
            DateTime ToDate = DateTime.Parse(toDate);
            List<ICTTest> tests = _mesRepository.GetICTTests(Model, null, TestNumber, FromDate, ToDate);
            return Ok(tests);
        }
    }
}
