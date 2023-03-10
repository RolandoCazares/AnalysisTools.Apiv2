using analysistools.api.Contracts;
using analysistools.api.Data;
using analysistools.api.Models.FPY;
using analysistools.api.Models.FPY.HELPERS;
using Microsoft.EntityFrameworkCore;


namespace analysistools.api.Helpers
{
    public class Filters : IFilters
    {

        private readonly AppDbContext _context;

        public Filters(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<ProducedAndFilteredFPY>> FilterProducedByFamilyy(int FamilyID, DateTime fromDate, DateTime toDate)
        {
            var result = new List<ProducedAndFilteredFPY>();

            var family = await _context.FamiliesFPY
                .Where(f => f.Id == FamilyID)
                .Include(f => f.LinesFPY)
                    .ThenInclude(l => l.ProcessesFPY)
                        .ThenInclude(p => p.StationsFPY)
                            .ThenInclude(s => s.ModelsFPY)
                .FirstOrDefaultAsync();

            if (family != null)
            {
                var uniqueModelNames = family.LinesFPY
                    .SelectMany(l => l.ProcessesFPY)
                    .SelectMany(p => p.StationsFPY)
                    .SelectMany(s => s.ModelsFPY)
                    .Select(m => m.Name_Model)
                    .Distinct()
                    .ToList();

                fromDate = fromDate.AddDays(-1);

                var producedFilteredByLine = await _context.ProducedAndFilteredFPYs
                    .Where(f => f.Date >= fromDate && f.Date <= toDate
                        && family.LinesFPY.Select(l => l.Name).Contains(f.Name)
                        && uniqueModelNames.Contains(f.Material))
                    .ToListAsync();

                result = producedFilteredByLine.Select(p => new ProducedAndFilteredFPY
                {
                    ID = p.ID,
                    Material = p.Material,
                    Name = p.Name,
                    Var = p.Var,
                    IdType = p.IdType,
                    Date = p.Date,
                    Amount = p.Amount,
                }).ToList();
            }

            return result;
        }

        public async Task<List<FailureFPY>> FilterFailsByFamily(int FamilyID, DateTime fromDate, DateTime toDate)
        {
            var result = new List<FailureFPY>();

            var family = await _context.FamiliesFPY
                .Where(f => f.Id == FamilyID)
                .Include(f => f.LinesFPY)
                    .ThenInclude(l => l.ProcessesFPY)
                        .ThenInclude(p => p.StationsFPY)
                            .ThenInclude(s => s.ModelsFPY)
                .FirstOrDefaultAsync();

            if (family != null)
            {
                var uniqueModelNames = family.LinesFPY
                    .SelectMany(l => l.ProcessesFPY)
                    .SelectMany(p => p.StationsFPY)
                    .SelectMany(s => s.ModelsFPY)
                    .Select(m => m.Name_Model)
                    .Distinct()
                    .ToList();

                fromDate = fromDate.AddDays(-1);

                var failsFilteredByLine = await _context.FailuresFPY
                    .Where(f => f.DATE >= fromDate && f.DATE <= toDate
                        && family.LinesFPY.Select(l => l.Name).Contains(f.NAME)
                        && uniqueModelNames.Contains(f.MATERIAL))
                    .ToListAsync();

                result = failsFilteredByLine.Select(p => new FailureFPY
                {
                    ID = p.ID,
                    SerialNumber = p.SerialNumber,
                    AUFTR = p.AUFTR,
                    STATE = p.STATE,
                    DATE = p.DATE,
                    MATERIAL = p.MATERIAL,
                    NAME = p.NAME,
                    VAR = p.VAR,
                    IDTYPE = p.IDTYPE,
                    BEZ = p.BEZ,
                }).ToList();
            }

            return result;
        }

        public async Task<Dictionary<string, List<object>>> GetFamilyTree(int FamilyID)
        {
            var treesFPY = new Dictionary<string, List<object>>();

            var family = await _context.FamiliesFPY
                .Where(f => f.Id == FamilyID)
                .Include(f => f.LinesFPY)
                    .ThenInclude(l => l.ProcessesFPY)
                        .ThenInclude(p => p.StationsFPY)
                .FirstOrDefaultAsync();

            if (family != null)
            {
                var treeLines = new Dictionary<string, List<object>>();
                foreach (LineFPY line in family.LinesFPY)
                {
                    var treeProcesses = new Dictionary<string, List<StationFPY>>();
                    foreach (ProcessFPY process in line.ProcessesFPY)
                    {
                        var treeStations = new List<StationFPY>();
                        foreach (StationFPY station in process.StationsFPY)
                        {
                            treeStations.Add(station);
                        }
                        treeProcesses.Add(process.Name, treeStations);
                    }
                    treeLines.Add(line.Name, new List<object>() { treeProcesses });
                }

                treesFPY.Add(family.Name, new List<object>() { treeLines });
            }
            return treesFPY;
        }
    }
}