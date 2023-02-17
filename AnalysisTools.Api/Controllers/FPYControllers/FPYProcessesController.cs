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
    public class FPYProcessesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public FPYProcessesController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/FPYProcess
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProcessFPY>>> GetProcessesFPY()
        {
            return await _context.ProcessesFPY.ToListAsync();
        }

        // GET: api/FPYProcess/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ProcessFPY>> GetProcessFPY(int id)
        {
            var processFPY = await _context.ProcessesFPY.FindAsync(id);

            if (processFPY == null)
            {
                return NotFound();
            }

            return processFPY;
        }

        // PUT: api/FPYProcess/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutProcessFPY(int id, ProcessFPY processFPY)
        {
            if (id != processFPY.Id)
            {
                return BadRequest();
            }

            _context.Entry(processFPY).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProcessFPYExists(id))
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

        // POST: api/FPYProcess
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<ProcessFPY>> PostProcessFPY(ProcessFPY processFPY)
        {
            _context.ProcessesFPY.Add(processFPY);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetProcessFPY", new { id = processFPY.Id }, processFPY);
        }

        // DELETE: api/FPYProcess/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProcessFPY(int id)
        {
            var processFPY = await _context.ProcessesFPY.FindAsync(id);
            if (processFPY == null)
            {
                return NotFound();
            }

            _context.ProcessesFPY.Remove(processFPY);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ProcessFPYExists(int id)
        {
            return _context.ProcessesFPY.Any(e => e.Id == id);
        }
    }
}
