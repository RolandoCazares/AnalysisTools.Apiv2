using analysistools.api.Contracts;
using analysistools.api.Models.Continental;
using Microsoft.AspNetCore.Mvc;

namespace analysistools.api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LdmFlashUnits : ControllerBase
    {
        private readonly IAccessRepository _repository;

        public LdmFlashUnits(IAccessRepository repository)
        {
            _repository = repository;
        }

        // GET: api/LdmFlashUnits
        [HttpGet]        
        public async Task<ActionResult<IEnumerable<LdmFlashUnit>>> GetLdmFlashUnits()
        {
            return  _repository.GetData().ToList();
        }
    }
}
