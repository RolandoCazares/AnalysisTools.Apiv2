namespace analysistools.api.Models.Continental
{
    public class Product
    {
        /// <summary>
        /// Continental product, that contains its model and a list of components.
        /// </summary>
        public string Model { get; set; }
        public List<Component> Components { get; set; }
        public Product()
        {
            Components = new List<Component>();
        }
    }
}
