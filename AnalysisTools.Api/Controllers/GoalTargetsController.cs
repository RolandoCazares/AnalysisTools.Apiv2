using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using analysistools.api.Data;
using analysistools.api.Models.Continental;

namespace analysistools.api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GoalTargetsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public GoalTargetsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/GoalTargets
        [HttpGet]
        public async Task<ActionResult<IEnumerable<GoalTarget>>> GetGoalTargets()
        {
            return await _context.GoalTargets.ToListAsync();
        }

        // GET: api/GoalTargets/5
        [HttpGet("{id}")]
        public async Task<ActionResult<GoalTarget>> GetGoalTarget(int id)
        {
            var goalTarget = await _context.GoalTargets.FindAsync(id);

            if (goalTarget == null)
            {
                return NotFound();
            }

            return goalTarget;
        }

        // PUT: api/GoalTargets/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutGoalTarget(int id, GoalTarget goalTarget)
        {
            if (id != goalTarget.Id)
            {
                return BadRequest();
            }

            _context.Entry(goalTarget).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!GoalTargetExists(id))
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

        // POST: api/GoalTargets
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<GoalTarget>> PostGoalTarget(GoalTarget goalTarget)
        {
            _context.GoalTargets.Add(goalTarget);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetGoalTarget", new { id = goalTarget.Id }, goalTarget);
        }

        // DELETE: api/GoalTargets/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteGoalTarget(int id)
        {
            var goalTarget = await _context.GoalTargets.FindAsync(id);
            if (goalTarget == null)
            {
                return NotFound();
            }

            _context.GoalTargets.Remove(goalTarget);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool GoalTargetExists(int id)
        {
            return _context.GoalTargets.Any(e => e.Id == id);
        }
    }
}
