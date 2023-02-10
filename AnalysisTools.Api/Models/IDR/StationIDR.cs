using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace analysistools.api.Models.IDR
{
    public class StationIDR
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public int LineId { get; set; }
        [JsonIgnore]
        public LineIDR LineIDR { get; set; }
    }
}
