using analysistools.api.Models.Optical;

namespace analysistools.api.Models.ProductionTests
{
    /// <summary>
    /// Generic optical test with details and encrypted images if available.
    /// </summary>
    public class Test
    {
        public DateTime Date { get; set; }
        public string Result { get; set; }
        public string Station { get; set; }
        public List<Image> Images { get; set; }
        public List<TestDetails> Details { get; set; }
    }
}
