namespace analysistools.api.Models.Continental
{
    /// <summary>
    /// Component from a product.
    /// For example: Capacitor, transistor, etc...
    /// </summary>
    public class Component
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string PartNumber { get; set; }
    }
}
