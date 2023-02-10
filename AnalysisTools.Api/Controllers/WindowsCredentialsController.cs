using analysistools.api.Data;
using analysistools.api.Models.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace analysistools.api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Administrator")]
    public class WindowsCredentialsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public WindowsCredentialsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/WindowsCredentials
        [HttpGet]
        public async Task<ActionResult<IEnumerable<WindowsCredential>>> GetWindowsCredentials()
        {
            return await _context.WindowsCredentials.ToListAsync();
        }

        // GET: api/WindowsCredentials/5
        [HttpGet("{id}")]
        public async Task<ActionResult<WindowsCredential>> GetWindowsCredential(int id)
        {
            var windowsCredential = await _context.WindowsCredentials.FindAsync(id);

            if (windowsCredential == null)
            {
                return NotFound();
            }

            return windowsCredential;
        }

        // PUT: api/WindowsCredentials/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutWindowsCredential(int id, WindowsCredential windowsCredential)
        {
            if (id != windowsCredential.Id)
            {
                return BadRequest();
            }

            _context.Entry(windowsCredential).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!WindowsCredentialExists(id))
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

        // POST: api/WindowsCredentials
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<WindowsCredential>> PostWindowsCredential(WindowsCredential windowsCredential)
        {
            _context.WindowsCredentials.Add(windowsCredential);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetWindowsCredential", new { id = windowsCredential.Id }, windowsCredential);
        }

        // DELETE: api/WindowsCredentials/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteWindowsCredential(int id)
        {
            var windowsCredential = await _context.WindowsCredentials.FindAsync(id);
            if (windowsCredential == null)
            {
                return NotFound();
            }

            _context.WindowsCredentials.Remove(windowsCredential);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool WindowsCredentialExists(int id)
        {
            return _context.WindowsCredentials.Any(e => e.Id == id);
        }
    }
}
