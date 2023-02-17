using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace analysistools.api.Models.FPY
{
    public class StationFPY
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public int ProcessID { get; set; }
        [JsonIgnore]
        public ProcessFPY ProcessFPY { get; set; }
    }
}
