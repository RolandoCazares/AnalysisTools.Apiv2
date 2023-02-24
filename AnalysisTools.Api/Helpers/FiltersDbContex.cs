using analysistools.api.Contracts;
using analysistools.api.Data;
using analysistools.api.Models.Continental;
using analysistools.api.Models.FPY;
using Microsoft.AspNetCore.Mvc;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace analysistools.api.Helpers
{
    public class FiltersDbContex : ControllerBase
    {
       
        private readonly AppDbContext _context;

        public FiltersDbContex(AppDbContext context)
        {
            _context = context;
        }

        public async List<ProducedAndFilteredFPY> FilterProducedByFamily(int FamilyID, DateTime fromDate, DateTime toDate)
        {
            List<ProducedAndFilteredFPY> result = new List<ProducedAndFilteredFPY>();

            FamilyFPY family = await _context.FamiliesFPY.FindAsync(FamilyID);
            if (family == null) return NotFound("The family doesn`t exist.");

            List<ProducedAndFilteredFPY> ProducedFilteredByFamily = _context.ProducedAndFilteredFPYs.ToList();
            foreach (ProducedAndFilteredFPY Product in ProducedFilteredByFamily)
            {
                ProducedFilteredByFamily.AddRange(_context.ProducedAndFilteredFPYs.Where(f => f.IdType == family.IdType && f.Date >= fromDate && f.Date <= toDate).ToList());
            }

            result.AddRange(ProducedFilteredByFamily);
            return Ok(result);
        }

        public async Task<IActionResult> FilterProducedByLine(int LineID, DateTime fromDate, DateTime toDate)
        {
            List<ProducedAndFilteredFPY> result = new List<ProducedAndFilteredFPY>();

            LineFPY line = await _context.LinesFPY.FindAsync(LineID);
            if (line == null) return NotFound("The line doesn`t exist in the LocalDatabase.");

            List<ProcessFPY> processes = _context.ProcessesFPY.Where(l => l.LineID == line.Id).ToList();

            List<StationFPY> stations = new List<StationFPY>();
            foreach (ProcessFPY proces in processes)
            {
                stations.AddRange(_context.StationsFPY.Where(w => w.ProcessID == proces.Id).ToList());
            }

            List<ProducedAndFilteredFPY> ProducedFilteredByLine = new List<ProducedAndFilteredFPY>();
            foreach (StationFPY station in stations)
            {
                ProducedFilteredByLine.AddRange(_context.ProducedAndFilteredFPYs.Where(f => f.Name == station.Name && f.Date >= fromDate && f.Date <= toDate).ToList());
            }

            result.AddRange(ProducedFilteredByLine);
            return Ok(result);
        }
    }
}
