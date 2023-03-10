using analysistools.api.Models.Continental;
using analysistools.api.Models.FPY;
using analysistools.api.Models.IDR;
using analysistools.api.Models.IDR.DTO;
using analysistools.api.Models.Optical;
using analysistools.api.Models.ProductionTests;
using analysistools.api.Models.Screen;

namespace analysistools.api.Contracts
{
    public interface IMesRepository
    {
        bool IsGolden(string SerialNumber);
        OpticalProduct GetProduct(string SerialNumber);
        List<ICTTest> GetICTTests(string Model, string Component, string TestNumber, DateTime fromDate, DateTime toDate);
        List<ScrnDetails> GetSCRNprocess(string SerialNumber);
        List<Scrn2020Details> GetSCRN2020process(DateTime fromDate, DateTime toDate, string Station, string IdType);
        List<Failure> GetFailuresIDR(DateTime fromDate, DateTime toDate);
        List<ProducedUnitsDTO> GetAllProducedIDR(string FamilyICTs, int FamilyID, DateTime FromDate, DateTime ToDate);
        List<PRODUCEDMAX> GetProducedMAX(DateTime FromDate, DateTime ToDate);
        List<ProducedAndFilteredFPY> GetProducedAndFiltereds(DateTime FromDate, DateTime ToDate);
        List<ProducedRAWFPY> GetProducedRAWFPYs(DateTime FromDate, DateTime ToDate);
        List<FailureFPY> GetRAW_Fails(DateTime FromDate, DateTime ToDate);
        List<PiecesAnalyzed> GetPiecesAnalyzed(DateTime FromDate, DateTime ToDate);
    }
}
