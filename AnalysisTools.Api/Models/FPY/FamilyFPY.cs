using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace analysistools.api.Models.FPY

{
    public class FamilyFPY
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string IdType { get; set; }
        [JsonIgnore]
        public List<LineFPY> LinesFPY { get; set; }

    }
}
