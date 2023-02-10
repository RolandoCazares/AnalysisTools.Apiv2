using analysistools.api.Data;
using analysistools.api.Models.Optical;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace analysistools.api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Administrator")]
    public class OpticalStationsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public OpticalStationsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/OpticalStations
        [HttpGet]
        public async Task<ActionResult<IEnumerable<OpticalStation>>> GetOpticalStations()
        {
            return await _context.OpticalStations.ToListAsync();
        }

        // GET: api/OpticalStations/5
        [HttpGet("{id}")]
        public async Task<ActionResult<OpticalStation>> GetOpticalStation(int id)
        {
            var opticalStation = await _context.OpticalStations.FindAsync(id);

            if (opticalStation == null)
            {
                return NotFound();
            }

            return opticalStation;
        }

        // PUT: api/OpticalStations/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutOpticalStation(int id, OpticalStation opticalStation)
        {
            if (id != opticalStation.Id)
            {
                return BadRequest();
            }

            _context.Entry(opticalStation).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!OpticalStationExists(id))
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

        // POST: api/OpticalStations
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<OpticalStation>> PostOpticalStation(OpticalStation opticalStation)
        {
            _context.OpticalStations.Add(opticalStation);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetOpticalStation", new { id = opticalStation.Id }, opticalStation);
        }

        // DELETE: api/OpticalStations/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOpticalStation(int id)
        {
            var opticalStation = await _context.OpticalStations.FindAsync(id);
            if (opticalStation == null)
            {
                return NotFound();
            }

            _context.OpticalStations.Remove(opticalStation);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool OpticalStationExists(int id)
        {
            return _context.OpticalStations.Any(e => e.Id == id);
        }
    }
}
