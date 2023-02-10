using analysistools.api.Contracts;
using analysistools.api.Models.Continental;
using Microsoft.AspNetCore.Mvc;
namespace analysistools.api.Controllers
{
    [Route("api")]
    [ApiController]
    public class ComponentsController : ControllerBase
    {
        private readonly IComponentsService _componentsService;
        public ComponentsController(IComponentsService componentsService)
        {
            _componentsService = componentsService;
        }

        [HttpGet("component/{name}/{productmodel}")]
        public ActionResult GetComponent(string name, string productmodel)
        {
            Component component = _componentsService.GetComponent(name, productmodel);
            return Ok(component);
        }

        [HttpGet("components/{model}/{partnumber}")]
        public ActionResult GetComponents(string model, string partnumber)
        {
            Product product = _componentsService.GetAllComponents(model, partnumber);
            return Ok(product);
        }
    }
}
