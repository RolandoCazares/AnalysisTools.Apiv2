using analysistools.api.Models.FPY;

namespace analysistools.api.Contracts
{
    public interface IFilterProducedByFamily
    {
        Task<List<PRODUCEDMAX>> FilterProducedByFamily(int FamilyID, DateTime fromDate, DateTime toDate);
    }
}
