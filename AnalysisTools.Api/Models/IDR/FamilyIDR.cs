using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace analysistools.api.Models.IDR
{
    /// <summary>
    /// Product family (GBCM, LDM, FordGen3, ...)
    /// </summary>
    public class FamilyIDR
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string IdType { get; set; }
        [JsonIgnore]
        public List<LineIDR> LinesIDR { get; set; }
        [JsonIgnore]
        public List<ProducedUnits> ProducedUnitsIDR { get; set; }
    }
}
