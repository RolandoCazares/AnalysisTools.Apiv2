using analysistools.api.Models.Continental;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace analysistools.api.Models.IDR
{
    public class LineIDR
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public int FamilyId { get; set; }
        [JsonIgnore]
        public FamilyIDR FamilyIDR { get; set; }
        [JsonIgnore]
        public List<StationIDR> StationsIDR { get; set; }

    }
}
