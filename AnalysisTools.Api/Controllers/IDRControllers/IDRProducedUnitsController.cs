using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using analysistools.api.Data;
using analysistools.api.Models.IDR;

namespace analysistools.api.Controllers.IDRControllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class IDRProducedUnitsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public IDRProducedUnitsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/IDRProducedUnits
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProducedUnits>>> GetProducedUnits()
        {
            return await _context.ProducedUnits.ToListAsync();
        }

        // GET: api/IDRProducedUnits/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ProducedUnits>> GetProducedUnits(int id)
        {
            var producedUnits = await _context.ProducedUnits.FindAsync(id);

            if (producedUnits == null)
            {
                return NotFound();
            }

            return producedUnits;
        }

        // PUT: api/IDRProducedUnits/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutProducedUnits(int id, ProducedUnits producedUnits)
        {
            if (id != producedUnits.ID)
            {
                return BadRequest();
            }

            _context.Entry(producedUnits).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProducedUnitsExists(id))
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

        // POST: api/IDRProducedUnits
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<ProducedUnits>> PostProducedUnits(ProducedUnits producedUnits)
        {
            _context.ProducedUnits.Add(producedUnits);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetProducedUnits", new { id = producedUnits.ID }, producedUnits);
        }

        // DELETE: api/IDRProducedUnits/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProducedUnits(int id)
        {
            var producedUnits = await _context.ProducedUnits.FindAsync(id);
            if (producedUnits == null)
            {
                return NotFound();
            }

            _context.ProducedUnits.Remove(producedUnits);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ProducedUnitsExists(int id)
        {
            return _context.ProducedUnits.Any(e => e.ID == id);
        }
    }
}
