//using analysistools.api.Contracts;
//using analysistools.api.Data;
//using analysistools.api.Models.FPY;
//using analysistools.api.Models.FPY.HELPERS;
//using Microsoft.EntityFrameworkCore;


//namespace analysistools.api.Helpers
//{
//    public class Filters : IFilters
//    {

//        private readonly AppDbContext _context;

//        public Filters(AppDbContext context)
//        {
//            _context = context;
//        }

//        public async Task<List<ProducedAndFilteredFPY>> FilterProducedByFamilyy(int FamilyID, DateTime fromDate, DateTime toDate)
//        {
//            List<ProducedAndFilteredFPY> result = new List<ProducedAndFilteredFPY>();

//            List<FamilyFPY> family = await _context.FamiliesFPY.Where(f => f.Id == FamilyID).ToListAsync();

//            List<LineFPY> lines = new List<LineFPY>();
//            foreach (FamilyFPY fami in family)
//            {
//                lines.AddRange(_context.LinesFPY.Where(f => f.FamilyId == fami.Id).ToList());
//            }

//            List<ProcessFPY> processes = new List<ProcessFPY>();
//            foreach (LineFPY line in lines)
//            {
//                processes.AddRange(_context.ProcessesFPY.Where(f => f.LineID == line.Id).ToList());
//            }

//            List<StationFPY> stations = new List<StationFPY>();
//            foreach (ProcessFPY proces in processes)
//            {
//                stations.AddRange(_context.StationsFPY.Where(w => w.ProcessID == proces.Id).ToList());
//            }

//            List<ModelFPY> Models = new List<ModelFPY>();
//            foreach (StationFPY station in stations)
//            {
//                Models.AddRange(_context.ModelsFPY.Where(w => w.StationID == station.Id).ToList());
//            }

//            List<string> uniqueModelNames = _context.ModelsFPY
//            .Where(m => stations.Select(s => s.Id).Contains(m.StationID))
//            .Select(m => m.Name_Model)
//            .Distinct()
//            .ToList();

//            List<ModelFPY> uniqueModels = _context.ModelsFPY
//                .Where(m => uniqueModelNames.Contains(m.Name_Model))
//                .ToList();

//            fromDate = fromDate.AddDays(-1);
//            // Obtener producciones filtradas por línea
//            List<ProducedAndFilteredFPY> ProducedFilteredByLine = _context.ProducedAndFilteredFPYs
//                .Where(f => f.Date >= fromDate && f.Date <= toDate
//                    && stations.Select(s => s.Name).Contains(f.Name)
//                    && uniqueModels.Select(m => m.Name_Model).Contains(f.Material))
//                .ToList();

//            result = ProducedFilteredByLine.Select(p => new ProducedAndFilteredFPY
//            {
//                ID = p.ID,
//                Material = p.Material,
//                Name = p.Name,
//                Var = p.Var,
//                IdType = p.IdType,
//                Date = p.Date,
//                Amount = p.Amount,
//            }).ToList();
//            return result;
//        }

//        public async Task<List<ProducedAndFilteredFPY>> FilterProducedByFamily(int FamilyID, DateTime fromDate, DateTime toDate)
//        {
//            List<ProducedAndFilteredFPY> result = new List<ProducedAndFilteredFPY>();
//            try
//            {
//                var family = await _context.FamiliesFPY.FirstOrDefaultAsync(f => f.Id == FamilyID);

//                if (family != null)
//                {
//                    var ProducedFilteredByFamily = _context.ProducedAndFilteredFPYs
//                        .Where(p => p.IdType == family.IdType && p.Date >= fromDate && p.Date <= toDate)
//                        .ToList();

//                    result = ProducedFilteredByFamily.Select(p => new ProducedAndFilteredFPY
//                    {
//                        ID = p.ID,
//                        Material = p.Material,
//                        Name = p.Name,
//                        Var = p.Var,
//                        IdType = p.IdType,
//                        Date = p.Date,
//                        Amount = p.Amount,
//                    }).ToList();
//                }
//            }
//            catch (Exception) { }
//            return result;
//        }

//        public async Task<List<ProducedAndFilteredFPY>> FilterProducedByLine(int LineID, DateTime fromDate, DateTime toDate)
//        {
//            List<ProducedAndFilteredFPY> result = new List<ProducedAndFilteredFPY>();

//            LineFPY line = await _context.LinesFPY.FindAsync(LineID);

//            List<ProcessFPY> processes = _context.ProcessesFPY.Where(l => l.LineID == line.Id).ToList();

//            List<StationFPY> stations = new List<StationFPY>();
//            foreach (ProcessFPY proces in processes)
//            {
//                stations.AddRange(_context.StationsFPY.Where(w => w.ProcessID == proces.Id).ToList());
//            }

//            List<ModelFPY> Models = new List<ModelFPY>();
//            foreach (StationFPY station in stations)
//            {
//                Models.AddRange(_context.ModelsFPY.Where(w => w.StationID == station.Id).ToList());
//            }

//            List<string> uniqueModelNames = _context.ModelsFPY
//            .Where(m => stations.Select(s => s.Id).Contains(m.StationID))
//            .Select(m => m.Name_Model)
//            .Distinct()
//            .ToList();

//            List<ModelFPY> uniqueModels = _context.ModelsFPY
//                .Where(m => uniqueModelNames.Contains(m.Name_Model))
//                .ToList();

//            fromDate = fromDate.AddDays(-1);
//            // Obtener producciones filtradas por línea
//            List<ProducedAndFilteredFPY> ProducedFilteredByLine = _context.ProducedAndFilteredFPYs
//                .Where(f => f.Date >= fromDate && f.Date <= toDate
//                    && stations.Select(s => s.Name).Contains(f.Name)
//                    && uniqueModels.Select(m => m.Name_Model).Contains(f.Material))
//                .ToList();

//            result = ProducedFilteredByLine.Select(p => new ProducedAndFilteredFPY
//            {
//                ID = p.ID,
//                Material = p.Material,
//                Name = p.Name,
//                Var = p.Var,
//                IdType = p.IdType,
//                Date = p.Date,
//                Amount = p.Amount,
//            }).ToList();
//            return result;
//        }

//        public async Task<List<ProducedAndFilteredFPY>> FilterProducedByProcess(int ProcessID, DateTime fromDate, DateTime toDate)
//        {
//            List<ProducedAndFilteredFPY> result = new List<ProducedAndFilteredFPY>();

//            ProcessFPY process = await _context.ProcessesFPY.FindAsync(ProcessID);

//            List<StationFPY> stations = _context.StationsFPY.Where(w => w.ProcessID == process.Id).ToList();

//            List<ModelFPY> Models = new List<ModelFPY>();
//            foreach (StationFPY station in stations)
//            {
//                Models.AddRange(_context.ModelsFPY.Where(w => w.StationID == station.Id).ToList());
//            }

//            List<string> uniqueModelNames = _context.ModelsFPY
//            .Where(m => stations.Select(s => s.Id).Contains(m.StationID))
//            .Select(m => m.Name_Model)
//            .Distinct()
//            .ToList();

//            List<ModelFPY> uniqueModels = _context.ModelsFPY
//                .Where(m => uniqueModelNames.Contains(m.Name_Model))
//                .ToList();

//            fromDate = fromDate.AddDays(-1);
//            // Obtener producciones filtradas por línea
//            List<ProducedAndFilteredFPY> ProducedFilteredByLine = _context.ProducedAndFilteredFPYs
//                .Where(f => f.Date >= fromDate && f.Date <= toDate
//                    && stations.Select(s => s.Name).Contains(f.Name)
//                    && uniqueModels.Select(m => m.Name_Model).Contains(f.Material))
//                .ToList();

//            result = ProducedFilteredByLine.Select(p => new ProducedAndFilteredFPY
//            {
//                ID = p.ID,
//                Material = p.Material,
//                Name = p.Name,
//                Var = p.Var,
//                IdType = p.IdType,
//                Date = p.Date,
//                Amount = p.Amount,
//            }).ToList();
//            return result;
//        }

//        public async Task<List<ProducedAndFilteredFPY>> FilterProducedByStation(int StationID, DateTime fromDate, DateTime toDate)
//        {
//            List<ProducedAndFilteredFPY> result = new List<ProducedAndFilteredFPY>();

//            StationFPY station = await _context.StationsFPY.FindAsync(StationID);

//            List<ModelFPY> models = _context.ModelsFPY.Where(m => m.StationID == station.Id).ToList();

//            List<ProducedAndFilteredFPY> producedFilteredByStation = new List<ProducedAndFilteredFPY>();
//            foreach (ModelFPY model in models)
//            {
//                fromDate = fromDate.AddDays(-1);
//                producedFilteredByStation.AddRange(_context.ProducedAndFilteredFPYs
//                    .Where(f => f.Date >= fromDate && f.Date <= toDate
//                        && f.Name == station.Name
//                        && f.Material == model.Name_Model)
//                    .ToList());
//            }

//            result = producedFilteredByStation.Select(p => new ProducedAndFilteredFPY
//            {
//                ID = p.ID,
//                Material = p.Material,
//                Name = p.Name,
//                Var = p.Var,
//                IdType = p.IdType,
//                Date = p.Date,
//                Amount = p.Amount,
//            }).ToList();

//            return result;
//        }

//        public async Task<List<ProducedAndFilteredFPY>> FilterProducedByModel(int ModelID, DateTime fromDate, DateTime toDate)
//        {
//            List<ProducedAndFilteredFPY> result = new List<ProducedAndFilteredFPY>();

//            var stations = await _context.StationsFPY.ToListAsync();
//            var models = await _context.ModelsFPY.ToListAsync();

//            var filteredStations = stations.Where(s => models.Any(m => m.Id == ModelID && m.StationID == s.Id)).ToList();

//            var producedFilteredByModel = new List<ProducedAndFilteredFPY>();
//            foreach (var station in filteredStations)
//            {
//                producedFilteredByModel.AddRange(_context.ProducedAndFilteredFPYs
//                    .Where(p => p.Date >= fromDate && p.Date <= toDate && p.Name == station.Name)
//                    .ToList());
//            }

//            result = producedFilteredByModel.Select(p => new ProducedAndFilteredFPY
//            {
//                ID = p.ID,
//                Material = p.Material,
//                Name = p.Name,
//                Var = p.Var,
//                IdType = p.IdType,
//                Date = p.Date,
//                Amount = p.Amount,
//            }).ToList();
//            return result;
//        }

//        //------------------------------------------------------------------------------------------------------//
//        //------------------------------------------------FAILS-----------------------------------------------//
//        //------------------------------------------------------------------------------------------------------//

//        public async Task<List<FailureFPY>> FilterFailsByFamily(int FamilyID, DateTime fromDate, DateTime toDate)
//        {
//            List<FailureFPY> result = new List<FailureFPY>();

//            List<FamilyFPY> family = await _context.FamiliesFPY.Where(f => f.Id == FamilyID).ToListAsync();

//            List<LineFPY> lines = new List<LineFPY>();
//            foreach (FamilyFPY fami in family)
//            {
//                lines.AddRange(_context.LinesFPY.Where(f => f.FamilyId == fami.Id).ToList());
//            }

//            List<ProcessFPY> processes = new List<ProcessFPY>();
//            foreach (LineFPY line in lines)
//            {
//                processes.AddRange(_context.ProcessesFPY.Where(f => f.LineID == line.Id).ToList());
//            }

//            List<StationFPY> stations = new List<StationFPY>();
//            foreach (ProcessFPY proces in processes)
//            {
//                stations.AddRange(_context.StationsFPY.Where(w => w.ProcessID == proces.Id).ToList());
//            }

//            List<ModelFPY> Models = new List<ModelFPY>();
//            foreach (StationFPY station in stations)
//            {
//                Models.AddRange(_context.ModelsFPY.Where(w => w.StationID == station.Id).ToList());
//            }

//            List<string> uniqueModelNames = _context.ModelsFPY
//            .Where(m => stations.Select(s => s.Id).Contains(m.StationID))
//            .Select(m => m.Name_Model)
//            .Distinct()
//            .ToList();

//            List<ModelFPY> uniqueModels = _context.ModelsFPY
//                .Where(m => uniqueModelNames.Contains(m.Name_Model))
//                .ToList();

//            fromDate = fromDate.AddDays(-1);
//            // Obtener producciones filtradas por línea
//            List<FailureFPY> FailsFilteredByLine = _context.FailuresFPY
//                .Where(f => f.DATE >= fromDate && f.DATE <= toDate
//                    && stations.Select(s => s.Name).Contains(f.NAME)
//                    && uniqueModels.Select(m => m.Name_Model).Contains(f.MATERIAL))
//                .ToList();

//            result = FailsFilteredByLine.Select(p => new FailureFPY
//            {
//                ID = p.ID,
//                SerialNumber = p.SerialNumber,
//                AUFTR = p.AUFTR,
//                STATE = p.STATE,
//                DATE = p.DATE,
//                MATERIAL = p.MATERIAL,
//                NAME = p.NAME,
//                VAR = p.VAR,
//                IDTYPE = p.IDTYPE,
//                BEZ = p.BEZ,
//            }).ToList();

//            return result;
//        }

//        public async Task<List<FailureFPY>> FilterFailsByLine(int LineID, DateTime fromDate, DateTime toDate)
//        {
//            List<FailureFPY> result = new List<FailureFPY>();

//            LineFPY line = await _context.LinesFPY.FindAsync(LineID);

//            List<ProcessFPY> processes = _context.ProcessesFPY.Where(l => l.LineID == line.Id).ToList();

//            List<StationFPY> stations = new List<StationFPY>();
//            foreach (ProcessFPY proces in processes)
//            {
//                stations.AddRange(_context.StationsFPY.Where(w => w.ProcessID == proces.Id).ToList());
//            }

//            List<ModelFPY> Models = new List<ModelFPY>();
//            foreach (StationFPY station in stations)
//            {
//                Models.AddRange(_context.ModelsFPY.Where(w => w.StationID == station.Id).ToList());
//            }

//            List<string> uniqueModelNames = _context.ModelsFPY
//            .Where(m => stations.Select(s => s.Id).Contains(m.StationID))
//            .Select(m => m.Name_Model)
//            .Distinct()
//            .ToList();

//            List<ModelFPY> uniqueModels = _context.ModelsFPY
//                .Where(m => uniqueModelNames.Contains(m.Name_Model))
//                .ToList();

//            fromDate = fromDate.AddDays(-1);
//            // Obtener producciones filtradas por línea
//            List<FailureFPY> FailsFilteredByLine = _context.FailuresFPY
//                .Where(f => f.DATE >= fromDate && f.DATE <= toDate
//                    && stations.Select(s => s.Name).Contains(f.NAME)
//                    && uniqueModels.Select(m => m.Name_Model).Contains(f.MATERIAL))
//                .ToList();

//            result = FailsFilteredByLine.Select(p => new FailureFPY
//            {
//                ID = p.ID,
//                SerialNumber = p.SerialNumber,
//                AUFTR = p.AUFTR,
//                STATE = p.STATE,
//                DATE = p.DATE,
//                MATERIAL = p.MATERIAL,
//                NAME = p.NAME,
//                VAR = p.VAR,
//                IDTYPE = p.IDTYPE,
//                BEZ = p.BEZ,
//            }).ToList();

//            return result;
//        }

//        public async Task<List<FailureFPY>> FilterFailsByProcess(int ProcessID, DateTime fromDate, DateTime toDate)
//        {
//            List<FailureFPY> result = new List<FailureFPY>();

//            ProcessFPY process = await _context.ProcessesFPY.FindAsync(ProcessID);

//            List<StationFPY> stations = _context.StationsFPY.Where(w => w.ProcessID == process.Id).ToList();

//            List<string> uniqueModelNames = _context.ModelsFPY
//            .Where(m => stations.Select(s => s.Id).Contains(m.StationID))
//            .Select(m => m.Name_Model)
//            .Distinct()
//            .ToList();

//            List<ModelFPY> uniqueModels = _context.ModelsFPY
//                .Where(m => uniqueModelNames.Contains(m.Name_Model))
//                .ToList();

//            fromDate = fromDate.AddDays(-1);
//            // Obtener producciones filtradas por línea
//            List<FailureFPY> FailsFilteredByProcess = _context.FailuresFPY
//                .Where(f => f.DATE >= fromDate && f.DATE <= toDate
//                    && stations.Select(s => s.Name).Contains(f.NAME)
//                    && uniqueModels.Select(m => m.Name_Model).Contains(f.MATERIAL))
//                .ToList();

//            result = FailsFilteredByProcess.Select(p => new FailureFPY
//            {
//                ID = p.ID,
//                SerialNumber = p.SerialNumber,
//                AUFTR = p.AUFTR,
//                STATE = p.STATE,
//                DATE = p.DATE,
//                MATERIAL = p.MATERIAL,
//                NAME = p.NAME,
//                VAR = p.VAR,
//                IDTYPE = p.IDTYPE,
//                BEZ = p.BEZ,
//            }).ToList();

//            return result;
//        }

//        public async Task<List<FailureFPY>> FilterFailsByStation(int StationID, DateTime fromDate, DateTime toDate)
//        {
//            List<FailureFPY> result = new List<FailureFPY>();

//            StationFPY station = await _context.StationsFPY.FindAsync(StationID);

//            List<ModelFPY> models = _context.ModelsFPY.Where(m => m.StationID == station.Id).ToList();

//            fromDate = fromDate.AddDays(-1);
//            // Obtener producciones filtradas por línea
//            List<FailureFPY> FailsFilteredByStation = _context.FailuresFPY
//            .Where(f => f.DATE >= fromDate && f.DATE <= toDate
//                && f.NAME == station.Name
//                && models.Select(m => m.Name_Model).Contains(f.MATERIAL))
//            .ToList();

//            result = FailsFilteredByStation.Select(p => new FailureFPY
//            {
//                ID = p.ID,
//                SerialNumber = p.SerialNumber,
//                AUFTR = p.AUFTR,
//                STATE = p.STATE,
//                DATE = p.DATE,
//                MATERIAL = p.MATERIAL,
//                NAME = p.NAME,
//                VAR = p.VAR,
//                IDTYPE = p.IDTYPE,
//                BEZ = p.BEZ,
//            }).ToList();

//            return result;
//        }

//        public async Task<List<FailureFPY>> FilterFailsByModel(int ModelID, DateTime fromDate, DateTime toDate)
//        {
//            List<FailureFPY> result = new List<FailureFPY>();

//            var stations = await _context.StationsFPY.ToListAsync();
//            var models = await _context.ModelsFPY.ToListAsync();

//            var filteredStations = stations.Where(s => models.Any(m => m.Id == ModelID && m.StationID == s.Id)).ToList();

//            var failsFilteredByModel = new List<FailureFPY>();
//            foreach (var station in filteredStations)
//            {
//                failsFilteredByModel.AddRange(_context.FailuresFPY
//                    .Where(f => f.DATE >= fromDate && f.DATE <= toDate && f.NAME == station.Name && f.MATERIAL == models.FirstOrDefault(m => m.Id == ModelID).Name_Model)
//                    .ToList());
//            }

//            result = failsFilteredByModel.Select(p => new FailureFPY
//            {
//                ID = p.ID,
//                MATERIAL = p.MATERIAL,
//                NAME = p.NAME,
//                VAR = p.VAR,
//                IDTYPE = p.IDTYPE,
//                DATE = p.DATE,
//                BEZ = p.BEZ,
//            }).ToList();

//            return result;
//        }

//        //--------------------------------Arbol------------------//


//        public async Task<Dictionary<string, List<object>>> GetFamilyTree(int FamilyID)
//        {
//            var family = await _context.FamiliesFPY.FirstOrDefaultAsync(f => f.Id == FamilyID);

//            if (family == null)
//            {
//                return null;
//            }

//            var lines = await _context.LinesFPY.Where(f => f.FamilyId == family.Id).ToListAsync();

//            List<ProcessFPY> processes = new List<ProcessFPY>();
//            foreach (LineFPY line in lines)
//            {
//                processes.AddRange(_context.ProcessesFPY.Where(f => f.LineID == line.Id).ToList());
//            }

//            List<StationFPY> stations = new List<StationFPY>();
//            foreach (ProcessFPY proces in processes)
//            {
//                stations.AddRange(_context.StationsFPY.Where(w => w.ProcessID == proces.Id).ToList());
//            }

//            var treeLines = new Dictionary<string, List<object>>();
//            foreach (LineFPY line in lines)
//            {
//                var treeProcesses = new Dictionary<string, List<object>>();
//                foreach (ProcessFPY process in processes.Where(w => w.LineID == line.Id))
//                {
//                    var treeStations = new List<object>();
//                    foreach (StationFPY station in stations.Where(w => w.ProcessID ==process.Id))
//                    {
//                        treeStations.Add(station.Name);
//                    }
//                    treeProcesses.Add(process.Name, treeStations);
//                }
//                treeLines.Add(line.Name, new List<object>() { treeProcesses });
//            }

//            var treesFPY = new Dictionary<string, List<object>>();

//            // Agregar la información de la familia
//            treesFPY.Add(family.Name, new List<object>() { treeLines });

//            return treesFPY;
//        }


//    }
//}









//var treeLines = new Dictionary<string, List<object>>();
//foreach (LineFPY line in lines)
//{
//    List<object> lineInfo = new List<object>();
//    foreach (ProcessFPY process in processes.Where(s => s.LineID == line.Id))
//    {
//        lineInfo.Add(new { process.Name });
//    }
//    treeLines.Add(line.Name, lineInfo);
//}




//public async Task<Dictionary<string, List<object>>> GetFamilyTree(int FamilyID)
//{
//    var family = await _context.FamiliesFPY.FirstOrDefaultAsync(f => f.Id == FamilyID);

//    var lines = await _context.LinesFPY.Where(f => f.FamilyId == family.Id).ToListAsync();

//    if (family == null)
//    {
//        return null;
//    }

//    var treeSFPY = new Dictionary<string, List<object>>();
//    foreach (var line in lines)
//    {
//        var processes = await _context.ProcessesFPY.Where(f => f.LineID == line.Id).ToListAsync();

//        var stations = new List<object>();
//        foreach (var process in processes)
//        {
//            var processStations = await _context.StationsFPY.Where(w => w.ProcessID == process.Id).Select(s => new
//            {
//                Name = s.Name
//            }).ToListAsync();

//            stations.AddRange(processStations);
//        }

//        treeSFPY.Add(line.Name, new List<object>()
//                {
//                    new { line = stations }
//                });

//    }

//    treeSFPY.Add(family.Name, new List<object>()
//            {
//                    new { Id = family.Id },
//                    new { Name = family.Name },
//                    new { Description = family.IdType },
//                    // Agregar otras propiedades de la entidad Family que desees incluir en el resultado.
//            });

//    return treeSFPY;
//}