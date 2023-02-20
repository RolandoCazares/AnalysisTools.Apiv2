namespace analysistools.api.Models.Continental
{
    public class PiecesAnalyzed
    {
        public string ID { get; set; }
        public string SerialNumber { get; set; }

        public DateTime Date { get; set; }

        public string Component { get; set; }

        public string ComponentPartNumber { get; set; }

        public string AnalisisComment { get; set; }

        public string Carrier { get; set; }

        public string Equipment { get; set; }
    }
}
