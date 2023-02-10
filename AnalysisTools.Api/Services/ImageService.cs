using analysistools.api.Contracts;

namespace analysistools.api.Services
{
    /// <summary>
    /// The single purpose of this service is to get images from an encrypted path.
    /// </summary>
    public class ImageService : IImageService
    {        
        public FileStream GetImageByEncryptedPath(string EncryptedPath)
        {
            string path = EncryptionService.DecryptString(EncryptedPath);
            var image = File.OpenRead(path);
            return image;
        }
    }
}
