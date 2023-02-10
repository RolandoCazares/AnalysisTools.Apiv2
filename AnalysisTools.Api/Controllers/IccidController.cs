using analysistools.api.Contracts;
using analysistools.api.Models.Continental;
using Microsoft.AspNetCore.Mvc;

namespace analysistools.api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class IccidController : ControllerBase
    {
        private readonly IAccessRepository _repository;

        public IccidController(IAccessRepository repository)
        {
            _repository = repository;
        }

        // GET: api/IccidUnit
        [HttpGet]        
        public async Task<ActionResult<IEnumerable<IccidUnit>>> GetIccidUnits()
        {
            return  _repository.GetDataIccid().ToList();
        }

    }
}
