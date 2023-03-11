namespace analysistools.api.Models.FPY
{
    public class ProducedAndFilteredFPY
    {
        public int ID { get; set; }

        public string UNIT_ID_IN { get; set; }

        public int PASS { get; set; }

        public int FAIL { get; set; }

        public string PRODUCT_DEFINITION { get; set; }

        public string SUB_DEVICE { get; set; }

        public string DST_EQUIPMENT { get; set; }

        public string UNIT_ID_IN_TYPE { get; set; }

        public DateTime START_DATE { get; set; }

        public DateTime END_DATE { get; set; }

    }
}
