using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace analysistools.api.Models.FPY
{
    public class LineFPY
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public int FamilyId { get; set; }
        [JsonIgnore]
        public FamilyFPY FamilyFPY { get; set; }
        [JsonIgnore]
        public List<ProcessFPY> ProcessesFPY { get; set; }
    }
}
