using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace analysistools.api.Models.FPY
{
    public class ModelFPY
    {
        public int Id { get; set; }
        [Required]
        public string Name_Model { get; set; }
        [Required]
        public int StationID { get; set; }
        [JsonIgnore]
        public StationFPY StationFPY { get; set; }
    }
}
