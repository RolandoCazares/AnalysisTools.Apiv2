using analysistools.api.Data;
using analysistools.api.Models;
using analysistools.api.Models.Tickets;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace analysistools.api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Administrator")]
    public class TicketServersController : ControllerBase
    {
        private readonly AppDbContext _context;

        public TicketServersController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/TicketServers
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TicketServer>>> GetTicketServers()
        {
            return await _context.TicketServers.ToListAsync();
        }

        // GET: api/TicketServers/5
        [HttpGet("{id}")]
        public async Task<ActionResult<TicketServer>> GetTicketServer(int id)
        {
            var ticketServer = await _context.TicketServers.FindAsync(id);

            if (ticketServer == null)
            {
                return NotFound();
            }

            return ticketServer;
        }

        // PUT: api/TicketServers/5        
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTicketServer(int id, TicketServer ticketServer)
        {
            if (id != ticketServer.Id)
            {
                return BadRequest();
            }

            _context.Entry(ticketServer).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TicketServerExists(id))
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

        // POST: api/TicketServers        
        [HttpPost]
        public async Task<ActionResult<TicketServer>> PostTicketServer(TicketServer ticketServer)
        {
            _context.TicketServers.Add(ticketServer);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetTicketServer", new { id = ticketServer.Id }, ticketServer);
        }

        // DELETE: api/TicketServers/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTicketServer(int id)
        {
            var ticketServer = await _context.TicketServers.FindAsync(id);
            if (ticketServer == null)
            {
                return NotFound();
            }

            _context.TicketServers.Remove(ticketServer);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool TicketServerExists(int id)
        {
            return _context.TicketServers.Any(e => e.Id == id);
        }
    }
}
