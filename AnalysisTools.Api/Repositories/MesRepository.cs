using analysistools.api.Contracts;
using analysistools.api.Data;
using analysistools.api.Helpers;
using analysistools.api.Models;
using analysistools.api.Models.Screen;
using analysistools.api.Models.Optical;
using analysistools.api.Models.ProductionTests;
using analysistools.api.Services;
using System.Data;
using System.Text.RegularExpressions;
using analysistools.api.Models.IDR.DTO;
using analysistools.api.Models.IDR;
using analysistools.api.Models.FPY;
using analysistools.api.Models.Continental;

namespace analysistools.api.Repositories
{
    /// <summary>
    /// The MES repository allows to search data from Continental's MES Database.
    /// </summary>
    public class MesRepository : IMesRepository
    {
        private AppDbContext _context;
        public MesRepository(AppDbContext context)
        {
            _context = context;
        }

        private static IDbContext dbContext = OracleDbContext.Instance;        
        private static Regex serialNumberPattern = new Regex(@"^[A-Z0-9_.-]*$");
        private static double MinuteTolerance = 0.2;

        /// <summary>
        /// Specifies if the given serial number corresponds to a golden unit.
        /// </summary>
        /// <param name="SerialNumber"></param>
        /// <returns>True if the product is golden, otherwise false</returns>
        public bool IsGolden(string SerialNumber)
        {
            try
            {
                string Query = MesQueryFabric.IsGoldenQuery(SerialNumber);
                DataTable result = dbContext.RunQuery(Query);

                string order = result.Rows[0]["PDK_AUFTR"].ToString().ToUpper();

                return order.Contains("GOLDEN");
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Constructs an optical product object with the given serial number.
        /// Optical products are those who have the pin fakra or pin checker processes.
        /// </summary>
        /// <param name="SerialNumber"></param>
        /// <returns>Optical product containing golden information, and a list of optical tests if they are available.</returns>
        public OpticalProduct GetProduct(string SerialNumber)
        {
            OpticalProduct product = new OpticalProduct();
            string cleanSerialNumber = Regex.Replace(SerialNumber, @"\s+", string.Empty);
            if (serialNumberPattern.IsMatch(cleanSerialNumber))
            {
                try
                {
                    bool isGolden = IsGolden(SerialNumber);
                    string Query = MesQueryFabric.ProductQuery(SerialNumber, isGolden);
                    DataTable result = dbContext.RunQuery(Query);
                    List<Test> tests = DataTableHelper.DataTableToTestList(result);
                    FillTestsDetails(SerialNumber, ref tests);
                    product = new OpticalProduct(SerialNumber, isGolden, tests);
                }
                catch (Exception)
                {
                    return product;
                }
            }
            return product;
        }

        public List<ScrnDetails> GetSCRNprocess(string SerialNumber)
        {
            List<ScrnDetails> test = new List<ScrnDetails>();
            try
            {
                string screenProcessQuery = MesQueryFabric.QueryForScreenLDMProcess(SerialNumber);
                DataTable queryResult = dbContext.RunQuery(screenProcessQuery);
                test = DataTableHelper.DataTableToScreenProcess(queryResult);
                
            }
            catch (Exception) 
            { 
                return test;
            }
            return test;
        }

        public List<Scrn2020Details> GetSCRN2020process(DateTime fromDate, DateTime toDate, string Station, string IdType)
        {
            List<Scrn2020Details> test = new List<Scrn2020Details>();
            try
            {
                string screenProcessQuery = MesQueryFabric.QueryForScreenLDM2020(fromDate, toDate, Station, IdType);
                DataTable queryResult = dbContext.RunQuery(screenProcessQuery);
                test = DataTableHelper.DataTableToScreenProcess2020(queryResult);

            }
            catch (Exception)
            {
                return test;
            }
            return test;
        }

        /// <summary>
        /// Fills optical test details.
        /// For example: a product from GBCM failt at pin J1_12.
        /// </summary>
        /// <param name="SerialNumber"></param>
        /// <param name="tests"></param>
        private void FillTestsDetails(string SerialNumber, ref List<Test> tests)
        {
            foreach (Test test in tests)
            {
                test.Images = GetImagePaths(SerialNumber, test.Date, test.Station);
                if (test.Result == "F")
                {
                    test.Details = GetTestDetailsFromMES(SerialNumber, test.Date);
                }
            }
        }

        /// <summary>
        /// Get test details from Continental MES database.
        /// </summary>
        /// <param name="SerialNumber"></param>
        /// <param name="TestDate"></param>
        /// <returns>A list of test details.</returns>
        private List<TestDetails> GetTestDetailsFromMES(string SerialNumber, DateTime TestDate)
        {
            List<TestDetails> testDetails = new List<TestDetails>();
            string Query = MesQueryFabric.TestDetailsQuery(SerialNumber, MinuteTolerance, TestDate);
            DataTable queryResult = dbContext.RunQuery(Query);
            testDetails = DataTableHelper.DataTableToTestDetailsList(queryResult);
            return testDetails;
        }

        /// <summary>
        /// Get optical test image paths, images that where used to perform the optical test.
        /// </summary>
        /// <param name="SerialNumber"></param>
        /// <param name="TestDate"></param>
        /// <param name="Station"></param>
        /// <returns>List of images if they are available.</returns>
        public List<Image> GetImagePaths(string SerialNumber, DateTime TestDate, string Station)
        {
            List<Image> images = new List<Image>();
            var opticalStation = _context.OpticalStations.FirstOrDefault(station => station.Name == Station);
            if (opticalStation == null) return images;
            opticalStation.Family = _context.Families.FirstOrDefault(f => f.Id == opticalStation.FamilyId);
            opticalStation.Credential = _context.WindowsCredentials.FirstOrDefault(c => c.Id == opticalStation.CredentialId);

            if (opticalStation.Family.Name == "GBCM")
            {
                opticalStation = new PinChecker(opticalStation);
            }
            else if (opticalStation.Family.Name == "DCU")
            {
                opticalStation = new PinCheckerDCU(opticalStation);
            }
            else
            {
                opticalStation = new PinFakra(opticalStation);
            }

            images = opticalStation.GetImages(SerialNumber, TestDate);
            images = EncryptImagePaths(images);
            return images;
        }

        /// <summary>
        /// Encrypt image path for security.
        /// </summary>
        /// <param name="ImagePaths"></param>
        /// <returns>A list of images with encrypted paths.</returns>
        private List<Image> EncryptImagePaths(List<Image> ImagePaths)
        {
            List<Image> encryptedPaths = ImagePaths;
            foreach (Image imagePath in encryptedPaths)
            {
                imagePath.Path = EncryptionService.EncryptString(imagePath.Path);
            }
            return encryptedPaths;
        }

        /// <summary>
        /// Get ICT tests from MES from a specific component and product model between two dates (3 days maximum).
        /// </summary>
        /// <param name="Model"></param>
        /// <param name="Component"></param>
        /// <param name="TestNumber"></param>
        /// <param name="fromDate"></param>
        /// <param name="toDate"></param>
        /// <returns>List of ICT tests</returns>
        public List<ICTTest> GetICTTests(string Model, string Component, string TestNumber, DateTime fromDate, DateTime toDate)
        {
            List<ICTTest> tests = new List<ICTTest>();
            try
            {
                string ictAnalyticsQuery = MesQueryFabric.ICTAnalytics(Model, Component, TestNumber, fromDate, toDate);
                DataTable queryResult = dbContext.RunQuery(ictAnalyticsQuery);
                tests = DataTableHelper.DataTableToICTTests(queryResult);
            }
            catch (Exception) {}
            return tests;
        }

        /// <summary>
        /// Get testsProcess from MES from a specific unit.
        /// </summary>
        /// <param name="SerialNumber"></param>
        /// 
        public List<ProducedUnitsDTO> GetAllProducedIDR(string FamilyICTs, int FamilyID, DateTime FromDate, DateTime ToDate)
        {
            List<ProducedUnitsDTO> result = new List<ProducedUnitsDTO>();
            double diffDays = (ToDate - FromDate).TotalDays;
            if (!(diffDays > 0 && diffDays <= 31)) throw new ArgumentException("Solo se permite maximo 31 dias");

            for (int i = 0; i < diffDays; i++)
            {
                DateTime _FromDate = FromDate.AddDays(i);
                DateTime _ToDate = FromDate.AddDays(i + 1);
                string fromDate = _FromDate.ToString("dd-MM-yyyy HH:mm:ss");
                string toDate = _ToDate.ToString("dd-MM-yyyy HH:mm:ss");
                Console.WriteLine($"Produced from Family with ID {FamilyID} from: {fromDate} to {toDate}");
                string producedIDRQuery = MesQueryFabric.QueryForProducedIDR(FamilyICTs, fromDate, toDate);
                DataTable queryResult = dbContext.RunQuery(producedIDRQuery);


                result.Add(new ProducedUnitsDTO()
                {
                    Quantity = int.Parse(queryResult.Rows[0]["PRODUCED"].ToString()),
                    Date = _FromDate,
                    FamilyID = FamilyID
                });
            }
            return result;
        }

        public List<Failure> GetFailuresIDR(DateTime FromDate, DateTime ToDate)
        {
            
            List<Failure> Resultado = new List<Failure>();
            try
            {
                string FailsQuery = MesQueryFabric.QueryFails(FromDate, ToDate);
                DataTable queryResult = dbContext.RunQuery(FailsQuery);
                Resultado = DataTableHelper.DataTableToFailure(queryResult);

            }
            catch (Exception) { }

            Resultado = Resultado.GroupBy(f => f.Serial_Number).Select(f => f.First()).ToList();

            return Resultado;
        }

        public List<ProducedAndFilteredFPY> GetProducedAndFiltereds (DateTime FromDate, DateTime ToDate)
        {

            List<ProducedAndFilteredFPY> Result = new List<ProducedAndFilteredFPY>();
            try
            {
                    string DataQueryProducedFiltered = MesQueryFabric.QueryForProducedMAXSINAGRUPAR(FromDate, ToDate);
                    DataTable queryResult = dbContext.RunQuery(DataQueryProducedFiltered);
                    List<ProducedAndFilteredFPY> filteredData = DataTableHelper.DataTableToProducedAndFilter(queryResult);
                    Result.AddRange(filteredData);

            }
            catch (Exception) { }
            return Result;
        }

        public List<PRODUCEDMAX> GetProducedMAX(DateTime FromDate, DateTime ToDate)
        {

            List<PRODUCEDMAX> Result = new List<PRODUCEDMAX>();
            try
            {
                    string DataQueryProducedFiltered = MesQueryFabric.QueryForProducedMax(FromDate, ToDate);
                    DataTable queryResult = dbContext.RunQuery(DataQueryProducedFiltered);
                    List<PRODUCEDMAX> filteredData = DataTableHelper.DataTableToProducedMAX(queryResult);
                    Result.AddRange(filteredData);
            }
            catch (Exception) { }
            return Result;
        }

        public List<ProducedRAWFPY> GetProducedRAWFPYs(DateTime FromDate, DateTime ToDate)
        {

            List<ProducedRAWFPY> Result = new List<ProducedRAWFPY>();
            try
            {
                List<FamilyFPY> families = _context.FamiliesFPY.ToList();
                foreach (FamilyFPY family in families)
                {
                    string Product = family.IdType;
                    string DataQueryFPYDATAs = MesQueryFabric.QueryForProducedRAWFPY(Product, FromDate, ToDate);
                    DataTable queryResult = dbContext.RunQuery(DataQueryFPYDATAs);
                    List<ProducedRAWFPY> filteredData = DataTableHelper.DataTableToProducedRAW(queryResult);
                    //.GroupBy(d => d.SerialNumber)
                    //.Select(g => g.OrderBy(d => d.DATE).First())
                    //.ToList();
                    Result.AddRange(filteredData);
                }


            }
            catch (Exception) { }
            return Result;
        }

        public List<FailureFPY> GetRAW_Fails(DateTime FromDate, DateTime ToDate)
        {

            List<FailureFPY> Result = new List<FailureFPY>();
            try
            {
                List<FamilyFPY> families = _context.FamiliesFPY.ToList();
                foreach (FamilyFPY family in families)
                {
                    string Product = family.IdType;
                    string DataQueryFPYDATAs = MesQueryFabric.QueryForProducedRAWFPY(Product, FromDate, ToDate);
                    DataTable queryResult = dbContext.RunQuery(DataQueryFPYDATAs);
                    List<FailureFPY> filteredData = DataTableHelper.DataTableToFailFPY(queryResult);
                    //.GroupBy(d => d.SerialNumber)
                    //.Select(g => g.OrderBy(d => d.DATE).First())
                    //.ToList();
                    Result.AddRange(filteredData);
                }

            }
            catch (Exception) { }
            Result = Result.GroupBy(f => f.SerialNumber).Select(f => f.First()).ToList();
            return Result;
        }

        public List<PiecesAnalyzed> GetPiecesAnalyzed(DateTime FromDate, DateTime ToDate)
        {
            List<PiecesAnalyzed> ann = new List<PiecesAnalyzed>();
            try
            {
                string PiecesAnalyzedQuery = MesQueryFabric.QueryForPTWA(FromDate, ToDate);
                DataTable queryResult = dbContext.RunQuery(PiecesAnalyzedQuery);
               ann = DataTableHelper.DataTableToPiecesAnalyzed(queryResult);
            }
            catch (Exception)
            {

                return ann;
            }
            return ann;
        }
    }
}
