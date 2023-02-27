using analysistools.api.Models.FPY;
using analysistools.api.Models.FPY.HELPERS;
using System.Data;

namespace analysistools.api.Contracts
{
    public interface IAccommodate
    {
        Task<List<Response>> DataTableAccommodate(List<ProducedAndFilteredFPY> producedAndFilteredList, List<FailureFPY> FailuresAndFilter);
    }
}
