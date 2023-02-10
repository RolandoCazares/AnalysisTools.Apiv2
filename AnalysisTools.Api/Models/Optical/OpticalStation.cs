using analysistools.api.Models.Authentication;
using analysistools.api.Models.Continental;
using System.Text.Json.Serialization;

namespace analysistools.api.Models.Optical
{
    /// <summary>
    /// Station that perform optical tests.
    /// </summary>
    public class OpticalStation
    {        
        public int Id { get; set; }
        public double TimeToleranceInHours { get; set; }
        public string Name { get; set; }
        public int FamilyId { get; set; }
        [JsonIgnore]
        public Family Family { get; set; }
        public string IpAddress { get; set; }
        public string MainImageDirectory { get; set; }
        public string ChildrenImageDirectories { get; set; }
        public int CredentialId { get; set; }
        [JsonIgnore]
        public WindowsCredential Credential { get; set; }

        public virtual List<Image> GetImages(string SerialNumber, DateTime TestDate)
        {
            return new List<Image>();
        }

        protected List<Image> SearchImagesInServer(string SerialNumber, string TargetImageDirectory, DateTime TestDate)
        {
            List<Image> Images = new List<Image>();
            if (Directory.Exists(TargetImageDirectory))
            {
                string[] searchedImages = Directory.GetFiles(TargetImageDirectory, SearchPattern(SerialNumber));
                foreach (string searchedImage in searchedImages)
                {
                    Image imagePath = new Image(searchedImage);
                    try
                    {
                        DateTime imageDate = DateTime.ParseExact(imagePath.Date, "dd/MMM/yyyy HH:mm:ss", null);
                        // Due to time difference between MES and the stations, it's necessary to retrieve images
                        // based on this difference. (2.5 hours) 
                        // TODO: Make this time a parameter for the administrator
                        double timeDifference = Math.Abs(imageDate.Subtract(TestDate).TotalHours);
                        if (timeDifference <= TimeToleranceInHours)
                        {
                            Images.Add(imagePath);
                        }
                    }
                    catch (Exception) { /* Do Nothing */ }
                }
            }
            return Images;
        }

        protected virtual string SearchPattern(string SerialNumber)
        {
            return $"*{SerialNumber}*";
        }
    }
}
