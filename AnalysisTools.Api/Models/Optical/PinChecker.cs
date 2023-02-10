namespace analysistools.api.Models.Optical
{
    /// <summary>
    /// Specific optical station for the GBCM family.
    /// </summary>
    public class PinChecker : OpticalStation
    {
        public PinChecker(OpticalStation opticalStation)
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
            List<Image> Images = new List<Image>();
            DateTime fromDay = TestDate.AddHours(-TimeToleranceInHours);
            DateTime toDay = TestDate.AddHours(TimeToleranceInHours);
            DateTime[] searchDays = SearchDays(fromDay, toDay);
            string[] childrenDirectories = ChildrenImageDirectories.Split(',');
            foreach (string ChildDirectory in childrenDirectories)
            {
                foreach (DateTime SearchDay in searchDays)
                {
                    string targetImageDirectory = string.Format(@"\\{0}\{1}\{2}\{3}",
                          IpAddress,
                          MainImageDirectory,
                          ChildDirectory,
                          TestDate.ToString("M-d-yyyy"));
                    Images.AddRange(SearchImagesInServer(SerialNumber, targetImageDirectory, TestDate));
                }
            }
            return Images;
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
