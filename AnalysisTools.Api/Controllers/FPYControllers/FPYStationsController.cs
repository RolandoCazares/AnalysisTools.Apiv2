using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using analysistools.api.Data;
using analysistools.api.Models.FPY;

namespace analysistools.api.Controllers.FPYControllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FPYStationsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public FPYStationsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/FPYStations
        [HttpGet]
        public async Task<ActionResult<IEnumerable<StationFPY>>> GetStationsFPY()
        {
            return await _context.StationsFPY.ToListAsync();
        }

        // GET: api/FPYStations/5
        [HttpGet("{id}")]
        public async Task<ActionResult<StationFPY>> GetStationFPY(int id)
        {
            var stationFPY = await _context.StationsFPY.FindAsync(id);

            if (stationFPY == null)
            {
                return NotFound();
            }

            return stationFPY;
        }

        // PUT: api/FPYStations/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutStationFPY(int id, StationFPY stationFPY)
        {
            if (id != stationFPY.Id)
            {
                return BadRequest();
            }

            _context.Entry(stationFPY).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!StationFPYExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/FPYStations
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<StationFPY>> PostStationFPY(StationFPY stationFPY)
        {
            _context.StationsFPY.Add(stationFPY);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetStationFPY", new { id = stationFPY.Id }, stationFPY);
        }

        // DELETE: api/FPYStations/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteStationFPY(int id)
        {
            var stationFPY = await _context.StationsFPY.FindAsync(id);
            if (stationFPY == null)
            {
                return NotFound();
            }

            _context.StationsFPY.Remove(stationFPY);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool StationFPYExists(int id)
        {
            return _context.StationsFPY.Any(e => e.Id == id);
        }
    }
}
