using analysistools.api.Contracts;
using analysistools.api.Data;
using analysistools.api.Models.FPY;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Globalization;

namespace analysistools.api.Controllers.FPYControllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class Fpy02DBController : ControllerBase
    {
        private readonly IFilters _filter;
        private readonly IAccommodate _accomodate;

        public Fpy02DBController(IFilters filter, IAccommodate accomodate)
        {
            _filter = filter;
            _accomodate = accomodate;
        }

        [HttpGet("ProducedByFamily/{FamilyId}/{fromDate}/{toDate}")]
        public async Task<ActionResult<List<PRODUCEDMAX>>> GetProducedByFamily(int FamilyId, string fromDate, string toDate)
        {
            DateTime FromDate = DateTime.ParseExact(fromDate, "dd-MM-yyyy", CultureInfo.InvariantCulture);
            DateTime ToDate = DateTime.ParseExact(toDate, "dd-MM-yyyy", CultureInfo.InvariantCulture);

            var ProducedAndFilter = await Task.Run(() => _filter.FilterProducedByFamilyy(FamilyId, FromDate, ToDate));
            var FailuresAndFilter = await Task.Run(() => _filter.FilterFailsByFamily(FamilyId, FromDate, ToDate));
            var Acomodar = await Task.Run(() => _accomodate.DataTableAccommodate(ProducedAndFilter, FailuresAndFilter));
            return Ok(Acomodar);
        }

        // GET: /api/Fpy02DB/ProducedByFamily/1/01-02-2023/15-02-2023
        //[HttpGet("ProducedByFamily/{FamilyId}/{fromDate}/{toDate}")]
        //public async Task<ActionResult<List<ProducedAndFilteredFPY>>> GetProducedByFamily(int FamilyId, string fromDate, string toDate)
        //{
        //    DateTime FromDate = DateTime.ParseExact(fromDate, "dd-MM-yyyy", CultureInfo.InvariantCulture);
        //    DateTime ToDate = DateTime.ParseExact(toDate, "dd-MM-yyyy", CultureInfo.InvariantCulture);

        //    var ProducedAndFilter = await Task.Run(() => _filter.FilterProducedByFamilyy(FamilyId, FromDate, ToDate));
        //    var FailuresAndFilter = await Task.Run(() => _filter.FilterFailsByFamily(FamilyId, FromDate, ToDate)); 
        //    var Acomodar = await Task.Run(() => _accomodate.DataTableAccommodate(ProducedAndFilter, FailuresAndFilter));
        //    return Ok(Acomodar);
        //}

        ////localhost:7202/api/Fpy02DB/ProducedByLine/1/01-02-2023/08-02-2023
        //// GET: ProducedByLine/LineID/dd-MM-yyyy/dd-MM-yyyy
        //[HttpGet("ProducedByLine/{LineID}/{fromDate}/{toDate}")]
        //public async Task<ActionResult> GetProducedByLine(int LineID, string fromDate, string toDate)
        //{
        //    DateTime FromDate = DateTime.ParseExact(fromDate, "dd-MM-yyyy", CultureInfo.InvariantCulture);
        //    DateTime ToDate = DateTime.ParseExact(toDate, "dd-MM-yyyy", CultureInfo.InvariantCulture);

        //    var ProducedAndFilter = await Task.Run(() => _filter.FilterProducedByLine(LineID, FromDate, ToDate));
        //    var FailuresAndFilter = await Task.Run(() => _filter.FilterFailsByLine(LineID, FromDate, ToDate));
        //    var Acomodar = await Task.Run(() => _accomodate.DataTableAccommodate(ProducedAndFilter, FailuresAndFilter));

        //    return Ok(Acomodar);
        //}

        //// GET: ProducedByProcess/ProcessID/dd-MM-yyyy/dd-MM-yyyy
        //[HttpGet("ProducedByProcess/{ProcessID}/{fromDate}/{toDate}")]
        //public async Task<ActionResult> GetProducedByProcess(int ProcessID, string fromDate, string toDate)
        //{
        //    DateTime FromDate = DateTime.ParseExact(fromDate, "dd-MM-yyyy", CultureInfo.InvariantCulture);
        //    DateTime ToDate = DateTime.ParseExact(toDate, "dd-MM-yyyy", CultureInfo.InvariantCulture);

        //    var ProducedAndFilter = await Task.Run(() => _filter.FilterProducedByProcess(ProcessID, FromDate, ToDate));
        //    var FailuresAndFilter = await Task.Run(() => _filter.FilterFailsByProcess(ProcessID, FromDate, ToDate));
        //    var Acomodar = await Task.Run(() => _accomodate.DataTableAccommodate(ProducedAndFilter, FailuresAndFilter));

        //    return Ok(Acomodar);
        //}

        //// GET: ProducedByStation/StationID/dd-MM-yyyy/dd-MM-yyyy
        //[HttpGet("ProducedByStation/{StationID}/{fromDate}/{toDate}")]
        //public async Task<ActionResult> GetProducedByStation(int StationID, string fromDate, string toDate)
        //{
        //    DateTime FromDate = DateTime.ParseExact(fromDate, "dd-MM-yyyy", CultureInfo.InvariantCulture);
        //    DateTime ToDate = DateTime.ParseExact(toDate, "dd-MM-yyyy", CultureInfo.InvariantCulture);

        //    var ProducedAndFilter = await Task.Run(() => _filter.FilterProducedByStation(StationID, FromDate, ToDate));
        //    var FailuresAndFilter = await Task.Run(() => _filter.FilterFailsByStation(StationID, FromDate, ToDate));
        //    var Acomodar = await Task.Run(() => _accomodate.DataTableAccommodate(ProducedAndFilter, FailuresAndFilter));

        //    return Ok(Acomodar);
        //}

        //// GET: ProducedByModel/StationID/dd-MM-yyyy/dd-MM-yyyy
        //[HttpGet("ProducedByModel/{StationID}/{fromDate}/{toDate}")]
        //public async Task<ActionResult> GetProducedByModel(int ModelID, string fromDate, string toDate)
        //{
        //    DateTime FromDate = DateTime.ParseExact(fromDate, "dd-MM-yyyy", CultureInfo.InvariantCulture);
        //    DateTime ToDate = DateTime.ParseExact(toDate, "dd-MM-yyyy", CultureInfo.InvariantCulture);

        //    var ProducedAndFilter = await Task.Run(() => _filter.FilterProducedByModel(ModelID, FromDate, ToDate));
        //    var FailuresAndFilter = await Task.Run(() => _filter.FilterFailsByModel(ModelID, FromDate, ToDate));
        //    var Acomodar = await Task.Run(() => _accomodate.DataTableAccommodate(ProducedAndFilter, FailuresAndFilter));

        //    return Ok(Acomodar);
        //}
    }
}
