using System.Text.Json.Serialization;

namespace analysistools.api.Models.IDR
{
    public class ProducedUnits
    {
        public int ID { get; set; }
        public DateTime Date { get; set; }
        public int Quantity { get; set; }
        public int FamilyID { get; set; }
        [JsonIgnore]
        public FamilyIDR FamilyIDR { get; set; }
    }
}
