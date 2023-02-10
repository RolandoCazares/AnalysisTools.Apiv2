using analysistools.api.Data;
using analysistools.api.Models.Continental;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace analysistools.api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Administrator")]
    public class FamiliesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public FamiliesController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/Families
        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<Family>>> GetFamilies()
        {
            return await _context.Families.ToListAsync();
        }

        // GET: api/Families/5
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<Family>> GetFamily(int id)
        {
            var family = await _context.Families.FindAsync(id);

            if (family == null)
            {
                return NotFound();
            }

            return family;
        }

        // PUT: api/Families/5        
        [HttpPut("{id}")]        
        public async Task<IActionResult> PutFamily(int id, Family family)
        {
            if (id != family.Id)
            {
                return BadRequest();
            }

            _context.Entry(family).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!FamilyExists(id))
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

        // POST: api/Families        
        [HttpPost]        
        public async Task<ActionResult<Family>> PostFamily(Family family)
        {
            _context.Families.Add(family);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetFamily", new { id = family.Id }, family);
        }

        // DELETE: api/Families/5
        [HttpDelete("{id}")]        
        public async Task<IActionResult> DeleteFamily(int id)
        {
            var family = await _context.Families.FindAsync(id);
            if (family == null)
            {
                return NotFound();
            }

            _context.Families.Remove(family);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool FamilyExists(int id)
        {
            return _context.Families.Any(e => e.Id == id);
        }
    }
}
