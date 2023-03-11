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

        public async Task<List<PRODUCEDMAX>> FilterProducedByFamilyy(int FamilyID, DateTime fromDate, DateTime toDate)
        {
            //List<ProducedAndFilteredFPY> result = new List<ProducedAndFilteredFPY>();
            List<PRODUCEDMAX> result = new List<PRODUCEDMAX>();

            List<FamilyFPY> family = await _context.FamiliesFPY.Where(f => f.Id == FamilyID).ToListAsync();

            List<LineFPY> lines = new List<LineFPY>();
            foreach (FamilyFPY fami in family)
            {
                lines.AddRange(_context.LinesFPY.Where(f => f.FamilyId == fami.Id).ToList());
            }

            List<ProcessFPY> processes = new List<ProcessFPY>();
            foreach (LineFPY line in lines)
            {
                processes.AddRange(_context.ProcessesFPY.Where(f => f.LineID == line.Id).ToList());
            }

            List<StationFPY> stations = new List<StationFPY>();
            foreach (ProcessFPY proces in processes)
            {
                stations.AddRange(_context.StationsFPY.Where(w => w.ProcessID == proces.Id).ToList());
            }


            fromDate = fromDate.AddDays(-1);

            //List<ProducedAndFilteredFPY> ProducedFilteredByLine = _context.ProducedAndFilteredFPYs
            //    .Where(f => f.Date >= fromDate && f.Date <= toDate
            //        && stations.Select(s => s.Name).Contains(f.Name)
            //        && uniqueModels.Select(m => m.Name_Model).Contains(f.Material))
            //    .ToList();

            List<PRODUCEDMAX> ProducedFilteredByLine = _context.PRODUCEDMAXes
                .Where(f => f.START_DATE >= fromDate && f.END_DATE <= toDate
                    && stations.Select(s => s.Name).Contains(f.SUB_DEVICE))
                .ToList();

            result.AddRange(ProducedFilteredByLine);
            return result;
        }

        public async Task<List<FailureFPY>> FilterFailsByFamily(int FamilyID, DateTime fromDate, DateTime toDate)
        {
            List<FailureFPY> result = new List<FailureFPY>();

            List<FamilyFPY> family = await _context.FamiliesFPY.Where(f => f.Id == FamilyID).ToListAsync();

            List<LineFPY> lines = new List<LineFPY>();
            foreach (FamilyFPY fami in family)
            {
                lines.AddRange(_context.LinesFPY.Where(f => f.FamilyId == fami.Id).ToList());
            }

            List<ProcessFPY> processes = new List<ProcessFPY>();
            foreach (LineFPY line in lines)
            {
                processes.AddRange(_context.ProcessesFPY.Where(f => f.LineID == line.Id).ToList());
            }

            List<StationFPY> stations = new List<StationFPY>();
            foreach (ProcessFPY proces in processes)
            {
                stations.AddRange(_context.StationsFPY.Where(w => w.ProcessID == proces.Id).ToList());
            }

            List<ModelFPY> Models = new List<ModelFPY>();
            foreach (StationFPY station in stations)
            {
                Models.AddRange(_context.ModelsFPY.Where(w => w.StationID == station.Id).ToList());
            }

            List<string> uniqueModelNames = _context.ModelsFPY
            .Where(m => stations.Select(s => s.Id).Contains(m.StationID))
            .Select(m => m.Name_Model)
            .Distinct()
            .ToList();

            List<ModelFPY> uniqueModels = _context.ModelsFPY
                .Where(m => uniqueModelNames.Contains(m.Name_Model))
                .ToList();

            fromDate = fromDate.AddDays(-1);
            //Obtener producciones filtradas por línea
            List<FailureFPY> FailsFilteredByLine = _context.FailuresFPY
                .Where(f => f.DATE >= fromDate && f.DATE <= toDate
                    && stations.Select(s => s.Name).Contains(f.NAME)
                    && uniqueModels.Select(m => m.Name_Model).Contains(f.MATERIAL))
                .ToList();

            result = FailsFilteredByLine.Select(p => new FailureFPY
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

            return result;
        }

        //--------------------------------Arbol------------------//


        public async Task<Dictionary<string, List<object>>> GetFamilyTree(int FamilyID)
        {
            var family = await _context.FamiliesFPY.FirstOrDefaultAsync(f => f.Id == FamilyID);

            if (family == null)
            {
                return null;
            }

            var lines = await _context.LinesFPY.Where(f => f.FamilyId == family.Id).ToListAsync();

            List<ProcessFPY> processes = new List<ProcessFPY>();
            foreach (LineFPY line in lines)
            {
                processes.AddRange(_context.ProcessesFPY.Where(f => f.LineID == line.Id).ToList());
            }

            List<StationFPY> stations = new List<StationFPY>();
            foreach (ProcessFPY proces in processes)
            {
                stations.AddRange(_context.StationsFPY.Where(w => w.ProcessID == proces.Id).ToList());
            }

            var treeLines = new Dictionary<string, List<object>>();
            foreach (LineFPY line in lines)
            {
                var treeProcesses = new Dictionary<string, List<object>>();
                foreach (ProcessFPY process in processes.Where(w => w.LineID == line.Id))
                {
                    var treeStations = new List<object>();
                    foreach (StationFPY station in stations.Where(w => w.ProcessID == process.Id))
                    {
                        treeStations.Add(station.Name);
                    }
                    treeProcesses.Add(process.Name, treeStations);
                }
                treeLines.Add(line.Name, new List<object>() { treeProcesses });
            }

            var treesFPY = new Dictionary<string, List<object>>();

            // Agregar la información de la familia
            treesFPY.Add(family.Name, new List<object>() { treeLines });

            return treesFPY;
        }

}
}