using analysistools.api.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using analysistools.api.Models.IDR;


namespace analysistools.api.Controllers.IDRControllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Administrator")]
    public class IDRStationsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public IDRStationsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/StationsIDR
        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<StationIDR>>> GetStationsIDR()
        {
            return await _context.StationsIDR.ToListAsync();
        }

        // GET: api/StationsIDR/5
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<StationIDR>> GetStationIDR(int id)
        {
            var stationIDR = await _context.StationsIDR.FindAsync(id);

            if (stationIDR == null)
            {
                return NotFound();
            }

            return stationIDR;
        }

        // PUT: api/StationsIDR/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutStationIDR(int id, StationIDR stationIDR)
        {
            if (id != stationIDR.Id)
            {
                return BadRequest();
            }

            _context.Entry(stationIDR).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!StationExists(id))
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

        // POST: api/StationsIDR
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<StationIDR>> PostStationIDR(StationIDR stationIDR)
        {
            _context.StationsIDR.Add(stationIDR);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetStationIDR", new { id = stationIDR.Id }, stationIDR);
        }

        // DELETE: api/StationsIDR/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteStationIDR(int id)
        {
            var stationIDR = await _context.StationsIDR.FindAsync(id);
            if (stationIDR == null)
            {
                return NotFound();
            }

            _context.StationsIDR.Remove(stationIDR);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool StationExists(int id)
        {
            return _context.StationsIDR.Any(e => e.Id == id);
        }
    }
}
