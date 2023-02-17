using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace analysistools.api.Models.FPY
{
    public class ProcessFPY
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public int LineID { get; set; }
        [JsonIgnore]
        public LineFPY LineFPY { get; set; }
        [JsonIgnore]
        public List<StationFPY> StationsFPY { get; set; }
    }
}
