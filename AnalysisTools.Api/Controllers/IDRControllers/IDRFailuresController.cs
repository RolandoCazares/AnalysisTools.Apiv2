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
    public class IDRFailuresController : ControllerBase
    {
        private readonly AppDbContext _context;

        public IDRFailuresController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/IDRFailures
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Failure>>> GetFailures()
        {
            return await _context.Failures.ToListAsync();
        }

        // GET: api/IDRFailures/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Failure>> GetFailure(int id)
        {
            var failure = await _context.Failures.FindAsync(id);

            if (failure == null)
            {
                return NotFound();
            }

            return failure;
        }

        // PUT: api/IDRFailures/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutFailure(int id, Failure failure)
        {
            if (id != failure.Id)
            {
                return BadRequest();
            }

            _context.Entry(failure).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!FailureExists(id))
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

        // POST: api/IDRFailures
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Failure>> PostFailure(Failure failure)
        {
            _context.Failures.Add(failure);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetFailure", new { id = failure.Id }, failure);
        }

        // DELETE: api/IDRFailures/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteFailure(int id)
        {
            var failure = await _context.Failures.FindAsync(id);
            if (failure == null)
            {
                return NotFound();
            }

            _context.Failures.Remove(failure);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool FailureExists(int id)
        {
            return _context.Failures.Any(e => e.Id == id);
        }
    }
}
