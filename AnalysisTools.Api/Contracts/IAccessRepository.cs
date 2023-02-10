using analysistools.api.Models.Continental;

namespace analysistools.api.Contracts
{
    public interface IAccessRepository
    {
        List<LdmFlashUnit> GetData();

        List<IccidUnit> GetDataIccid();
        
    }
}
