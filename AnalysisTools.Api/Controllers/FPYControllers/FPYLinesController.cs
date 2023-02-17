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
    public class FPYLinesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public FPYLinesController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/FPYLines
        [HttpGet]
        public async Task<ActionResult<IEnumerable<LineFPY>>> GetLinesFPY()
        {
            return await _context.LinesFPY.ToListAsync();
        }

        // GET: api/FPYLines/5
        [HttpGet("{id}")]
        public async Task<ActionResult<LineFPY>> GetLineFPY(int id)
        {
            var lineFPY = await _context.LinesFPY.FindAsync(id);

            if (lineFPY == null)
            {
                return NotFound();
            }

            return lineFPY;
        }

        // PUT: api/FPYLines/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutLineFPY(int id, LineFPY lineFPY)
        {
            if (id != lineFPY.Id)
            {
                return BadRequest();
            }

            _context.Entry(lineFPY).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!LineFPYExists(id))
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

        // POST: api/FPYLines
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<LineFPY>> PostLineFPY(LineFPY lineFPY)
        {
            _context.LinesFPY.Add(lineFPY);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetLineFPY", new { id = lineFPY.Id }, lineFPY);
        }

        // DELETE: api/FPYLines/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteLineFPY(int id)
        {
            var lineFPY = await _context.LinesFPY.FindAsync(id);
            if (lineFPY == null)
            {
                return NotFound();
            }

            _context.LinesFPY.Remove(lineFPY);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool LineFPYExists(int id)
        {
            return _context.LinesFPY.Any(e => e.Id == id);
        }
    }
}
