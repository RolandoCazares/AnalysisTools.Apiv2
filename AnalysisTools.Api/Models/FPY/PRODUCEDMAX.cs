namespace analysistools.api.Models.FPY
{
    public class PRODUCEDMAX
    {
        public int ID { get; set; }

        

        public string PRODUCT_DEFINITION { get; set; }

        public string SUB_DEVICE { get; set; }

        public string PROCESS { get; set; }

        public DateTime START_DATE { get; set; }

        public DateTime END_DATE { get; set; }

        public int TOTAL_PASS { get; set; }

        public int TOTAL_FAIL { get; set; }

        public int TOTAL { get; set; }

        public double FPY { get; set; }

    }
}
