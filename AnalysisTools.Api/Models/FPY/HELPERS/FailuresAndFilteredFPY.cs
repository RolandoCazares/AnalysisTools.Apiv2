using analysistools.api.Models.FPY.PRODUCTS;

namespace analysistools.api.Models.FPY.HELPERS
{
    public class FailuresAndFilteredFPY
    {
        public int ID { get; set; }

        public string SerialNumber { get; set; }

        public string AUFTR { get; set; }

        public string STATE { get; set; }

        public DateTime DATE { get; set; }

        public string MATERIAL { get; set; }

        public string NAME { get; set; }

        public string VAR { get; set; }

        public string IDTYPE { get; set; }

        public string NUM { get; set; }

        public string BEZ { get; set; }

        public int AMOUNT { get; set; } 
    }
}
