using analysistools.api.Models.FPY;


namespace analysistools.api.Contracts
{
    public interface IFilterFailsByFamily
    {
        Task<List<FailureFPY>> FailuresByFamily(int FamilyID, DateTime fromDate, DateTime toDate);
    }
}
