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
    public class Fpy01MESController : ControllerBase
    {
        private readonly IMesRepository _mesRepository;
        private readonly AppDbContext _context;

        public Fpy01MESController(IMesRepository mesRepository, AppDbContext context)
        {
            _mesRepository = mesRepository;
            _context = context;
        }


        [HttpGet("ProducedByCurrentDay")]
        public async Task<ActionResult<List<ProducedAndFilteredFPY>>> GetProducedByCurrentDay()
        {

            DateTime currentDate = DateTime.Now.Date;
            DateTime fromDate = currentDate.AddDays(-1);

            List<ProducedAndFilteredFPY> FPYData = _mesRepository.GetProducedAndFiltereds(fromDate, currentDate);

            await _context.ProducedAndFilteredFPYs.AddRangeAsync(FPYData);
            await _context.SaveChangesAsync();

            return Ok(FPYData);
        }

        [HttpGet("ProducedByCurrentDayMax")]
        public async Task<ActionResult<List<PRODUCEDMAX>>> GetProducedByCurrentDayMax()
        {

            DateTime currentDate = DateTime.Now.Date;
            DateTime fromDate = currentDate.AddDays(-1);

            List<PRODUCEDMAX> FPYData = _mesRepository.GetProducedMAX(fromDate, currentDate);

            await _context.PRODUCEDMAXes.AddRangeAsync(FPYData);
            await _context.SaveChangesAsync();

            return Ok(FPYData);
        }


        [HttpGet("ProducedRAWByCurrentDay")]
        public async Task<ActionResult<List<ProducedRAWFPY>>> GetProducedRAWByCurrentDay()
        {

            DateTime currentDate = DateTime.Now.Date;
            DateTime fromDate = currentDate.AddDays(-1);

            List<ProducedRAWFPY> FPYData = _mesRepository.GetProducedRAWFPYs(fromDate, currentDate);

            await _context.ProducedRAWFPY.AddRangeAsync(FPYData);
            await _context.SaveChangesAsync();

            return Ok(FPYData);
        }

        [HttpGet("Fails")]
        public async Task<ActionResult<List<FailureFPY>>> GetFailsFromMES()
        {
            DateTime currentDate = DateTime.Now.Date;
            DateTime fromDate = currentDate.AddDays(-1);

            List<FailureFPY> FPYFail = _mesRepository.GetRAW_Fails(fromDate, currentDate);

            await _context.FailuresFPY.AddRangeAsync(FPYFail);
            await _context.SaveChangesAsync();

            return Ok(FPYFail);
        }
    }
}

