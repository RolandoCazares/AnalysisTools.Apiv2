using System;
using System.Globalization;
using System.Text.RegularExpressions;

namespace analysistools.api.Models.Optical
{
    /// <summary>
    /// Image from Optical tests
    /// </summary>
    public class Image
    {
        public string Path { get; set; }
        public string Date { get; set; }
        public string Description { get; set; }

        public Image(string path)
        {
            this.Path = path;

            DateTime date;
            string dateInfoFromPath = string.Empty;
            bool isParsed = false;

            string[] dividedPath = Path.Split("\\");

            if (dividedPath.Length > 3)
            {
                this.Description = dividedPath[4];
            }

            string fileNameWithExtension = dividedPath[dividedPath.Length - 1];

            string[] filnameWithExtensionParts = fileNameWithExtension.Split(".");
            string fileNameWithoutExtension = filnameWithExtensionParts[0];

            if (IsGlobalPath(fileNameWithoutExtension))
            {
                dateInfoFromPath = fileNameWithoutExtension.Substring(fileNameWithoutExtension.Length - 20, 20);
                double unixTimeStamp = double.Parse(dateInfoFromPath);
                date = UnixTimeStampToDateTime(unixTimeStamp);
                isParsed = true;
            }
            else
            {
                dateInfoFromPath = fileNameWithoutExtension.Substring(fileNameWithoutExtension.Length - 15, 15);
                isParsed = DateTime.TryParseExact(dateInfoFromPath, "yyyyMMdd_HHmmss",
                  CultureInfo.InvariantCulture,
                  DateTimeStyles.None,
                  out date);
            }

            if (isParsed)
            {
                this.Date = date.ToString("dd/MMM/yyyy HH:mm:ss");
            }
            else
            {
                this.Date = "Cannot get date";
            }
        }

        private bool IsGlobalPath(string FileName)
        {
            return Regex.Match(FileName, @"\d{20}$").Success;
        }


        /// <summary>
        /// Converts unix time stamp to regular DateTime object.
        /// Pin chekcers from GBCM store their test dates with this format.
        /// </summary>
        /// <param name="unixTimeStamp"></param>
        /// <returns>DateTime from the given unix timestamp.</returns>
        private static DateTime UnixTimeStampToDateTime(double unixTimeStamp)
        {
            // Unix timestamp is seconds past epoch
            DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            unixTimeStamp /= 1000000000;
            dateTime = dateTime.AddSeconds(unixTimeStamp).ToLocalTime();
            dateTime = dateTime.AddYears(-1969);
            dateTime = dateTime.AddHours(7);
            return dateTime;
        }
    }
}
