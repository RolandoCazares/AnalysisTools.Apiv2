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
    public class FPYFamiliesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public FPYFamiliesController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/FPYFamilies
        [HttpGet]
        public async Task<ActionResult<IEnumerable<FamilyFPY>>> GetFamiliesFPY()
        {
            return await _context.FamiliesFPY.ToListAsync();
        }

        // GET: api/FPYFamilies/5
        [HttpGet("{id}")]
        public async Task<ActionResult<FamilyFPY>> GetFamilyFPY(int id)
        {
            var familyFPY = await _context.FamiliesFPY.FindAsync(id);

            if (familyFPY == null)
            {
                return NotFound();
            }

            return familyFPY;
        }

        // PUT: api/FPYFamilies/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutFamilyFPY(int id, FamilyFPY familyFPY)
        {
            if (id != familyFPY.Id)
            {
                return BadRequest();
            }

            _context.Entry(familyFPY).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!FamilyFPYExists(id))
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

        // POST: api/FPYFamilies
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<FamilyFPY>> PostFamilyFPY(FamilyFPY familyFPY)
        {
            _context.FamiliesFPY.Add(familyFPY);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetFamilyFPY", new { id = familyFPY.Id }, familyFPY);
        }

        // DELETE: api/FPYFamilies/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteFamilyFPY(int id)
        {
            var familyFPY = await _context.FamiliesFPY.FindAsync(id);
            if (familyFPY == null)
            {
                return NotFound();
            }

            _context.FamiliesFPY.Remove(familyFPY);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool FamilyFPYExists(int id)
        {
            return _context.FamiliesFPY.Any(e => e.Id == id);
        }
    }
}
