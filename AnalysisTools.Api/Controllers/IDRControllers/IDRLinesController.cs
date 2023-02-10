using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using analysistools.api.Data;
using analysistools.api.Models.IDR;

//_context.Families.Add(family);

namespace analysistools.api.Controllers.IDRControllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Administrator")]
    public class IDRLinesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public IDRLinesController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/Lines
        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<LineIDR>>> GetLines()
        {
            return await _context.LinesIDR.ToListAsync();
        }

        // GET: api/Lines/5
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<LineIDR>> GetLine(int id)
        {
            var line = await _context.LinesIDR.FindAsync(id);

            if (line == null)
            {
                return NotFound();
            }

            return line;
        }

        // PUT: api/LinesIDR/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutLine(int id, LineIDR lineIDR)
        {
            if (id != lineIDR.Id)
            {
                return BadRequest();
            }

            _context.Entry(lineIDR).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!LineExists(id))
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

        // POST: api/LinesIDR
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<LineIDR>> PostLine(LineIDR lineIDR)
        {
            _context.LinesIDR.Add(lineIDR);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetLine", new { id = lineIDR.Id }, lineIDR);
        }

        // DELETE: api/LinesIDR/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteLine(int id)
        {
            var lineIDR = await _context.LinesIDR.FindAsync(id);
            if (lineIDR == null)
            {
                return NotFound();
            }

            _context.LinesIDR.Remove(lineIDR);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool LineExists(int id)
        {
            return _context.LinesIDR.Any(e => e.Id == id);
        }
    }
}
