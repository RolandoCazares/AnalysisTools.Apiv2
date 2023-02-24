using analysistools.api.Contracts;
using analysistools.api.Data;
using analysistools.api.Models.Continental;
using analysistools.api.Models.FPY;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NuGet.Packaging;
using System.Diagnostics;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace analysistools.api.Helpers
{
    public class FiltersDbContex
    {
       
        private readonly AppDbContext _context;

        public FiltersDbContex(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<ProducedAndFilteredFPY>> FilterProducedByFamily(int FamilyID, DateTime fromDate, DateTime toDate)
        {
            var family = await _context.FamiliesFPY.FindAsync(FamilyID);
            if (family == null) throw new ArgumentException("The family doesn`t exist.");

            List<ProducedAndFilteredFPY> result = new List<ProducedAndFilteredFPY>();

            List<LineFPY> lines = _context.LinesFPY.Where(l => l.FamilyId == family.Id).ToList();

            List<ProcessFPY> processesOfLine = new List<ProcessFPY>();
            foreach (LineFPY line in lines)
            {
                processesOfLine.AddRange(_context.ProcessesFPY.Where(w => w.LineID == line.Id).ToList());
            }

            List<StationFPY> stationsOfLine = new List<StationFPY>();
            foreach (ProcessFPY process in processesOfLine)
            {
                stationsOfLine.AddRange(_context.StationsFPY.Where(w => w.ProcessID == process.Id).ToList());
            }

            List<ModelFPY> modelsOfTheStationsInTheLine = new List<ModelFPY>();
            foreach (StationFPY station in stationsOfLine)
            {
                modelsOfTheStationsInTheLine.AddRange(_context.ModelsFPY.Where(w => w.StationID == station.Id).ToList());
            }

            List<ProducedAndFilteredFPY> ProducedFiltereByModelInAStationInALine = new List<ProducedAndFilteredFPY>();
            foreach (ModelFPY model in modelsOfTheStationsInTheLine)
            {
                ProducedFiltereByModelInAStationInALine.AddRange(_context.ProducedAndFilteredFPYs.Where(f => f.Name == model.Name_Model && f.Date >= fromDate && f.Date <= toDate).ToList());
            }
            result.AddRange(ProducedFiltereByModelInAStationInALine);
            result = result.OrderBy(f => f.Date).ToList();
            return result;
        }

        public async Task<List<ProducedAndFilteredFPY>> FilterProducedByLine(int LineID, DateTime fromDate, DateTime toDate)
        {
            var line = await _context.LinesFPY.FindAsync(LineID);
            if (line == null) throw new ArgumentException("The line doesn`t exist.");

            List<ProducedAndFilteredFPY> result = new List<ProducedAndFilteredFPY>();
            List<ProcessFPY> processesOfLine = _context.ProcessesFPY.Where(l => l.LineID == line.Id).ToList();

            List<StationFPY> stationsOfLine = new List<StationFPY>();
            foreach (ProcessFPY process in processesOfLine)
            {
                stationsOfLine.AddRange(_context.StationsFPY.Where(w => w.ProcessID == process.Id).ToList());
            }

            List<ModelFPY> modelsOfTheStationsInTheLine= new List<ModelFPY>();
            foreach(StationFPY station in stationsOfLine)
            {
                modelsOfTheStationsInTheLine.AddRange(_context.ModelsFPY.Where(w => w.StationID == station.Id).ToList());
            }

            List<ProducedAndFilteredFPY> ProducedFiltereByModelInAStationInALine = new List<ProducedAndFilteredFPY>();
            foreach (ModelFPY model in modelsOfTheStationsInTheLine)
            {
                ProducedFiltereByModelInAStationInALine.AddRange(_context.ProducedAndFilteredFPYs.Where(f => f.Name == model.Name_Model && f.Date >= fromDate && f.Date <= toDate).ToList());
            }
            result.AddRange(ProducedFiltereByModelInAStationInALine);
            result = result.OrderBy(f => f.Date).ToList();
            return result;

        }
    }
}
