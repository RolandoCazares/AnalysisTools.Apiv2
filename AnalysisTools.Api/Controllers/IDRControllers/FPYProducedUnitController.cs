using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using analysistools.api.Data;
using analysistools.api.Models.FPY;

namespace analysistools.api.Controllers.IDRControllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FPYProducedUnitController : ControllerBase
    {
        private readonly AppDbContext _context;

        public FPYProducedUnitController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/FPYProducedUnit
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProducedUnitFPY>>> GetProducedUnitsFPY()
        {
            return await _context.ProducedUnitsFPY.ToListAsync();
        }

        // GET: api/FPYProducedUnit/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ProducedUnitFPY>> GetProducedUnitFPY(string id)
        {
            var producedUnitFPY = await _context.ProducedUnitsFPY.FindAsync(id);

            if (producedUnitFPY == null)
            {
                return NotFound();
            }

            return producedUnitFPY;
        }

        // PUT: api/FPYProducedUnit/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutProducedUnitFPY(string id, ProducedUnitFPY producedUnitFPY)
        {
            if (id != producedUnitFPY.ID)
            {
                return BadRequest();
            }

            _context.Entry(producedUnitFPY).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProducedUnitFPYExists(id))
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

        // POST: api/FPYProducedUnit
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<ProducedUnitFPY>> PostProducedUnitFPY(ProducedUnitFPY producedUnitFPY)
        {
            _context.ProducedUnitsFPY.Add(producedUnitFPY);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (ProducedUnitFPYExists(producedUnitFPY.ID))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetProducedUnitFPY", new { id = producedUnitFPY.ID }, producedUnitFPY);
        }

        // DELETE: api/FPYProducedUnit/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProducedUnitFPY(string id)
        {
            var producedUnitFPY = await _context.ProducedUnitsFPY.FindAsync(id);
            if (producedUnitFPY == null)
            {
                return NotFound();
            }

            _context.ProducedUnitsFPY.Remove(producedUnitFPY);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ProducedUnitFPYExists(string id)
        {
            var matchingIds = _context.ProducedUnitsFPY.Where(e => e.ID.Contains(id)).Select(e => e.ID);
            foreach (var matchingId in matchingIds)
            {
                if (IsSimilarEnough(id, matchingId))
                {
                    return true;
                }
            }
            return false;
        }

        private bool IsSimilarEnough(string id1, string id2)
        {
            int maxLength = Math.Max(id1.Length, id2.Length);
            int differenceCount = maxLength - id1.Intersect(id2).Count();
            return differenceCount <= 10; // Puede ajustar este umbral según sus necesidades
        }

    }
}
