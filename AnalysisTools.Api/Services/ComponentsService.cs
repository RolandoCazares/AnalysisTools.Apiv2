using analysistools.api.Contracts;
using analysistools.api.Models.Continental;
using System.Xml;

namespace analysistools.api.Services
{
    /// <summary>
    /// The component services allows to search for components based on the product model.
    /// </summary>
    public class ComponentsService : IComponentsService
    {
        private string rcdFilesPath = @"\\NGMCS01A\Profiles\";
        private XmlDocument xmldoc = new XmlDocument();
        private XmlNodeList xmlnodes;
        /// <summary>
        /// Get all component in a product with specific part number.      
        /// </summary>
        /// <param name="ProductModel"></param>
        /// <param name="PartNumber"></param>
        /// <returns>Product with the list of searched components</returns>
        public Product GetAllComponents(string ProductModel, string PartNumber)
        {
            Product product = new Product() { Model = ProductModel };
            // All components are retrieved from xml files.
            xmldoc = new XmlDocument();
            try
            {
                FileStream fs = new FileStream(rcdFilesPath + ProductModel + ".rcd", FileMode.Open, FileAccess.Read);
                xmldoc.Load(fs);

                xmlnodes = xmldoc.GetElementsByTagName("item");

                foreach (XmlNode xmlnode in xmlnodes)
                {
                    string currentComponent = xmlnode.Attributes["position"].Value;
                    string currentComponentModel = xmlnode.Attributes["material"].Value;
                    string currentComponentDescription = xmlnode.Attributes["comment"].Value;
                    if (currentComponentModel == PartNumber)
                    {
                        product.Components.Add(new Component()
                        {
                            Name = currentComponent,
                            PartNumber = currentComponentModel,
                            Description = currentComponentDescription,
                        });
                    }
                }               
            }
            catch (FileNotFoundException)
            {
                product.Model = "Not Found";
                return product;
            }
            return product;
        }

        /// <summary>
        /// Gets a signle component bases on its name and the product model
        /// </summary>
        /// <param name="Name"></param>
        /// <param name="ProductModel"></param>
        /// <returns>A component with name, description and part number.</returns>
        public Component GetComponent(string Name, string ProductModel)
        {
            Component component = new Component() { Name = Name};
            xmldoc = new XmlDocument();          

            try
            {
                FileStream fs = new FileStream(rcdFilesPath + ProductModel + ".rcd", FileMode.Open, FileAccess.Read);
                xmldoc.Load(fs);

                xmlnodes = xmldoc.GetElementsByTagName("item");

                foreach (XmlNode xmlnode in xmlnodes)
                {
                    string currentComponent = xmlnode.Attributes["position"].Value;
                    if (currentComponent == Name)
                    {
                        component.PartNumber = xmlnode.Attributes["material"].Value;
                        component.Description = xmlnode.Attributes["comment"].Value;
                    }
                }
            }
            catch (FileNotFoundException)
            {
                component.Description = "Not Found";                
            }
            return component;
        }
    }
}
