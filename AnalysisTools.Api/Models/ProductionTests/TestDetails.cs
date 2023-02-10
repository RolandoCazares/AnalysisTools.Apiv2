namespace analysistools.api.Models.ProductionTests
{
    /// <summary>
    /// Optical test details.
    /// </summary>
    public class TestDetails
    {
        public DateTime Date { get; set; }
        public string Model { get; set; }
        public string Number { get; set; }
        public string Description { get; set; }
        public string Value { get; set; }
        public string LSL { get; set; }
        public string USL { get; set; }
        public string Result { get; set; }
    }
}
