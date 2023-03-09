using analysistools.api.Contracts;
using analysistools.api.Data;
using analysistools.api.Models.FPY;
using analysistools.api.Models.FPY.HELPERS;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Globalization;

namespace analysistools.api.Controllers.FPYControllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TreeController : ControllerBase
    {
        private readonly IFilters _filter;

        public TreeController(IFilters filter)
        {
            _filter = filter;
        }

        [HttpGet("ByFamilyID/{FamilyID}")]
        public async Task<ActionResult<Dictionary<string, List<object>>>> GetTreeByFamilyID(int FamilyID)
        {
            var tree = await _filter.GetFamilyTree(FamilyID);
            if (tree == null)
            {
                return NotFound();
            }
            return Ok(tree);
        }

    }
}

        

        