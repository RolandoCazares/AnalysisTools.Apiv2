using analysistools.api.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace analysistools.api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImageController : ControllerBase
    {
        private readonly IImageService _imageService;
        public ImageController(IImageService imageService)
        {
            _imageService = imageService;
        }
        [HttpGet]
        public ActionResult GetImage()
        {
            try
            {
                string EncryptedPath = Request.Headers["encryptedpath"];
                var image = _imageService.GetImageByEncryptedPath(EncryptedPath);
                return File(image, "image/jpeg");
            }
            catch (Exception)
            {
                return BadRequest(new { message = "Image not found" });
            }
        }
    }
}
