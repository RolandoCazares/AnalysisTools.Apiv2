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
    public class FPYFailuresController : ControllerBase
    {
        private readonly AppDbContext _context;

        public FPYFailuresController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/FPYFailures
        [HttpGet]
        public async Task<ActionResult<IEnumerable<RAW_FAIL>>> GetRAW_FAILs()
        {
            return await _context.RAW_FAILs.ToListAsync();
        }

        // GET: api/FPYFailures/5
        [HttpGet("{id}")]
        public async Task<ActionResult<RAW_FAIL>> GetRAW_FAIL(int id)
        {
            var rAW_FAIL = await _context.RAW_FAILs.FindAsync(id);

            if (rAW_FAIL == null)
            {
                return NotFound();
            }

            return rAW_FAIL;
        }

        // PUT: api/FPYFailures/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutRAW_FAIL(int id, RAW_FAIL rAW_FAIL)
        {
            if (id != rAW_FAIL.ID)
            {
                return BadRequest();
            }

            _context.Entry(rAW_FAIL).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RAW_FAILExists(id))
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

        // POST: api/FPYFailures
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<RAW_FAIL>> PostRAW_FAIL(RAW_FAIL rAW_FAIL)
        {
            _context.RAW_FAILs.Add(rAW_FAIL);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetRAW_FAIL", new { id = rAW_FAIL.ID }, rAW_FAIL);
        }

        // DELETE: api/FPYFailures/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRAW_FAIL(int id)
        {
            var rAW_FAIL = await _context.RAW_FAILs.FindAsync(id);
            if (rAW_FAIL == null)
            {
                return NotFound();
            }

            _context.RAW_FAILs.Remove(rAW_FAIL);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool RAW_FAILExists(int id)
        {
            return _context.RAW_FAILs.Any(e => e.ID == id);
        }
    }
}
