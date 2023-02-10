using analysistools.api.Models.Continental;

namespace analysistools.api.Contracts
{
    public interface IComponentsService
    {
        Component GetComponent(string Name, string ProductModel);
        Product GetAllComponents(string ProductModel, string PartNumber);
    }
}
