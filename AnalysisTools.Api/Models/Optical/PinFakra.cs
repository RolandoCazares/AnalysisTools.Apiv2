using analysistools.api.Helpers;

namespace analysistools.api.Models.Optical
{
    /// <summary>
    /// Optical station that is used on FordGen3, Subaru and Nissan families.
    /// </summary>
    public class PinFakra : OpticalStation
    {
        public PinFakra(OpticalStation opticalStation)
        {
            this.Id = opticalStation.Id;
            this.TimeToleranceInHours = opticalStation.TimeToleranceInHours;
            this.Name = opticalStation.Name;
            this.FamilyId = opticalStation.FamilyId;
            this.Family = opticalStation.Family;
            this.IpAddress = opticalStation.IpAddress;
            this.MainImageDirectory = opticalStation.MainImageDirectory;
            this.ChildrenImageDirectories = opticalStation.ChildrenImageDirectories;
            this.CredentialId = opticalStation.CredentialId;
            this.Credential = opticalStation.Credential;
        }
        public override List<Image> GetImages(string SerialNumber, DateTime TestDate)
        {
            List<Image> ImagesPaths = new List<Image>();
            DateTime fromDay = TestDate.AddHours(-TimeToleranceInHours);
            DateTime toDay = TestDate.AddHours(TimeToleranceInHours);
            DateTime[] searchDays = SearchDays(fromDay, toDay);
            string month = MonthConverter.EngToEsp(TestDate.ToString("MMM").ToUpper());
            string[] childrenDirectories = ChildrenImageDirectories.Split(',');
            foreach (string ChildDirectory in childrenDirectories)
            {
                foreach (DateTime SearchDay in searchDays)
                {
                    string targetImageDirectory = string.Format(@"\\{0}\{1}\{2}\{3}\{4}\{5}",
                        IpAddress,
                        MainImageDirectory,
                        ChildDirectory,
                        TestDate.ToString("yyyy"),
                        month,
                        SearchDay.Day
                        );
                    ImagesPaths.AddRange(SearchImagesInServer(SerialNumber, targetImageDirectory, TestDate));
                }
            }
            return ImagesPaths;
        }

        private DateTime[] SearchDays(DateTime fromDay, DateTime toDay)
        {
            if (fromDay.Day == toDay.Day)
            {
                return new DateTime[] { fromDay };
            }
            return new DateTime[] { fromDay, toDay };
        }
    }
}
