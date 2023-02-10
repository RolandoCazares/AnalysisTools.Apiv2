namespace analysistools.api.Models.ProductionTests
{
    /// <summary>
    /// ICT Test with all required information.
    /// </summary>
    public class ICTTest
    {
        public string SerialNumber { get; set; }
        public string RunState { get; set; }
        public DateTime RunDate { get; set; }
        public string StationId { get; set; }
        public string Model { get; set; }
        public string TestNumber { get; set; }
        public string Description { get; set; }
        public string Value { get; set; }
        public string Units { get; set; }
        public string LSL { get; set; }
        public string USL { get; set; }

    }
}
