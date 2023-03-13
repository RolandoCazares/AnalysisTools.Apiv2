using analysistools.api.Contracts;
using analysistools.api.Data;
using analysistools.api.Models.FPY;
using analysistools.api.Models.FPY.HELPERS;
using Microsoft.EntityFrameworkCore;

namespace analysistools.api.Helpers
{
    public class FiltersProducedByFamily : IFilterProducedByFamily
    {
        private readonly AppDbContext _context;

        public FiltersProducedByFamily(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<PRODUCEDMAX>> FilterProducedByFamily(int FamilyID, DateTime fromDate, DateTime toDate)
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


            List<PRODUCEDMAX> ProducedFilteredByLine = _context.PRODUCEDMAXes
            .Where(f => f.START_DATE >= fromDate && f.END_DATE <= toDate
                && stations.Select(s => s.Name).Contains(f.SUB_DEVICE))
            .AsEnumerable()
            .Where(f => stations.Any(s => s.ProcessName == f.PROCESS))
            .ToList();




            // Obtener una lista ordenada de estaciones por su nombre
            List<StationFPY> sortedStations = stations.OrderBy(s => s.ProcessID).ToList();

            // Obtener los valores de SUB_DEVICE ordenados por el orden en el que se encuentran las estaciones
            List<string> sortedSubDevices = sortedStations.Select(s => s.Name).ToList();

            // Ordenar la lista ProducedFilteredByLine por su valor de Id de manera ascendente utilizando los valores de SUB_DEVICE ordenados
            ProducedFilteredByLine = ProducedFilteredByLine.OrderBy(p => sortedSubDevices.IndexOf(p.SUB_DEVICE)).ToList();

            result.AddRange(ProducedFilteredByLine);
            return result;

        }

    }
}
