namespace analysistools.api.Contracts
{
    public interface IImageService
    {
        FileStream GetImageByEncryptedPath(string EncryptedPath);
    }
}
