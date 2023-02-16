using analysistools.api.Data;
using analysistools.api.Models.FPY;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace analysistools.api.Controllers.FPYControllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FPYDAtasController : ControllerBase
    {
        private readonly AppDbContext _context;

        public FPYDAtasController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/FPYDAtas
        [HttpGet]
        public async Task<ActionResult<IEnumerable<RAW_DATA>>> GetDataFailUnits()
        {
            return await _context.RAW_DATAs.ToListAsync();
        }

        // GET: api/FPYDAtas/5
        [HttpGet("{id}")]
        public async Task<ActionResult<RAW_DATA>> GetDataFailUnits(int id)
        {
            var dataFailUnits = await _context.RAW_DATAs.FindAsync(id);

            if (dataFailUnits == null)
            {
                return NotFound();
            }

            return dataFailUnits;
        }

        // PUT: api/FPYDAtas/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutDataFailUnits(int id, RAW_DATA dataFailUnits)
        {
            if (id != dataFailUnits.ID)
            {
                return BadRequest();
            }

            _context.Entry(dataFailUnits).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!FpyDataUnitsExists(id))
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

        // POST: api/FPYDAtas
        [HttpPost]
        public async Task<ActionResult<RAW_DATA>> PostDataFailUnits(RAW_DATA dataFailUnits)
        {
            _context.RAW_DATAs.Add(dataFailUnits);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetDataFailUnits", new { id = dataFailUnits.ID }, dataFailUnits);
        }

        // DELETE: api/FPYDAtas/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDataFailUnits(int id)
        {
            var dataFailUnits = await _context.RAW_DATAs.FindAsync(id);
            if (dataFailUnits == null)
            {
                return NotFound();
            }

            _context.RAW_DATAs.Remove(dataFailUnits);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool FpyDataUnitsExists(int id)
        {
            return _context.RAW_DATAs.Any(e => e.ID == id);
        }
    }
}
