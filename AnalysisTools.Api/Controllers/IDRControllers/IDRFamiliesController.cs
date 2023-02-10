using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using analysistools.api.Data;
using analysistools.api.Models.IDR;

namespace analysistools.api.Controllers.IDRControllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Administrator")]
    public class IDRFamiliesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public IDRFamiliesController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/FamiliesIDR
        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<FamilyIDR>>> GetFamiliesIDR()
        {
            return await _context.FamiliesIDR.ToListAsync();
        }

        // GET: api/FamiliesIDR/5
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<FamilyIDR>> GetFamilyIDR(int id)
        {
            var family = await _context.FamiliesIDR.FindAsync(id);

            if (family == null)
            {
                return NotFound();
            }

            return family;
        }

        // PUT: api/FamiliesIDR/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutFamilyIDR(int id, FamilyIDR familyIDR)
        {
            if (id != familyIDR.Id)
            {
                return BadRequest();
            }

            _context.Entry(familyIDR).State = EntityState.Modified;

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

        // POST: api/FamiliesIDR
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<FamilyIDR>> PostFamily(FamilyIDR familyIDR)
        {
            _context.FamiliesIDR.Add(familyIDR);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetFamilyIDR", new { id = familyIDR.Id }, familyIDR);
        }

        // DELETE: api/FamiliesIDR/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteFamilyIDR(int id)
        {
            var familyIDR = await _context.FamiliesIDR.FindAsync(id);
            if (familyIDR == null)
            {
                return NotFound();
            }

            _context.FamiliesIDR.Remove(familyIDR);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool FamilyExists(int id)
        {
            return _context.FamiliesIDR.Any(e => e.Id == id);
        }
    }
}
