using analysistools.api.Models.ProductionTests;

namespace analysistools.api.Models.Optical
{
    /// <summary>
    /// Optical product (A product that has pin checker or pin fakra process).
    /// </summary>
    public class OpticalProduct
    {
        public string SerialNumber { get; set; }
        public bool IsGolden { get; set; }
        public List<Test> Tests { get; set; }

        public OpticalProduct(string SerialNumber, bool IsGolden, List<Test> Tests)
        {
            this.SerialNumber = SerialNumber;
            this.IsGolden = IsGolden;
            this.Tests = Tests;
        }

        public OpticalProduct()
        {
            this.SerialNumber = string.Empty;
            this.IsGolden = false;
            this.Tests = new List<Test>();
        }
    }
}
