using analysistools.api.Helpers;

namespace analysistools.api.Models.Optical
{
    public class PinCheckerDCU : OpticalStation
    {
        public PinCheckerDCU(OpticalStation opticalStation)
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
            string Model = SerialNumber.Substring(18, 13);
            string month = MonthConverter.EngToEsp(TestDate.ToString("MMM").ToUpper());
            string[] childrenDirectories = ChildrenImageDirectories.Split(',');
            foreach (string ChildDirectory in childrenDirectories)
            {
                foreach (DateTime SearchDay in searchDays)
                {
                    string targetImageDirectory = string.Format(@"\\{0}\{1}\{2}\{3}\{4}\{5}",
                        IpAddress,
                        MainImageDirectory,
                        Model,
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
