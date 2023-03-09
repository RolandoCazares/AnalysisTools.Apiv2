using analysistools.api.Models.FPY;
using analysistools.api.Models.FPY.HELPERS;

namespace analysistools.api.Contracts
{
    public interface IFilters
    {

        //-----------------------------ProducedFPY-------------------------//
        Task<List<ProducedAndFilteredFPY>> FilterProducedByFamilyy(int FamilyID, DateTime fromDate, DateTime toDate);
        //Task<List<ProducedAndFilteredFPY>> FilterProducedByLine(int LineID, DateTime fromDate, DateTime toDate);
        //Task<List<ProducedAndFilteredFPY>> FilterProducedByProcess(int ProcessID, DateTime fromDate, DateTime toDate);
        //Task<List<ProducedAndFilteredFPY>> FilterProducedByStation(int StationID, DateTime fromDate, DateTime toDate);
        //Task<List<ProducedAndFilteredFPY>> FilterProducedByModel(int ModelID, DateTime fromDate, DateTime toDate);
        ////-----------------------------FailuresFPY-------------------------//
        Task<List<FailureFPY>> FilterFailsByFamily(int FamilyID, DateTime fromDate, DateTime toDate);
        //Task<List<FailureFPY>> FilterFailsByLine(int LineID, DateTime fromDate, DateTime toDate);
        //Task<List<FailureFPY>> FilterFailsByProcess(int ProcessID, DateTime fromDate, DateTime toDate);
        //Task<List<FailureFPY>> FilterFailsByStation(int StationID, DateTime fromDate, DateTime toDate);
        //Task<List<FailureFPY>> FilterFailsByModel(int ModelID, DateTime fromDate, DateTime toDate);
        //-----------------------------Arbol--------------------------------//
        Task<Dictionary<string, List<object>>> GetFamilyTree(int FamilyID);
    }
}
