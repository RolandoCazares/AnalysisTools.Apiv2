using analysistools.api.Contracts;
using analysistools.api.Models.Optical;
using Microsoft.AspNetCore.Mvc;

namespace analysistools.api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OpticalTestController : ControllerBase
    {
        private readonly IMesRepository _mesRepository;        
        public OpticalTestController(IMesRepository mesRepository)
        {
            _mesRepository = mesRepository;
        }


        [HttpGet("{SerialNumber}")]
        public ActionResult GetProduct(string SerialNumber)
        {
            OpticalProduct productTest = _mesRepository.GetProduct(SerialNumber);
            return Ok(productTest);
        }
    }
}
