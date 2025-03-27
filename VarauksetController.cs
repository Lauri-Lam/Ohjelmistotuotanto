using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VillageNewbiesReservationSystem.Data;
using VillageNewbiesReservationSystem.Models;

namespace VillageNewbiesReservationSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VarauksetController : ControllerBase
    {
        private readonly AppDbContext _context;

        public VarauksetController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Varaus>>> GetVaraukset()
        {
            return await _context.Varaukset
                .Include(v => v.Asiakas)
                .Include(v => v.Mokki)
                .ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Varaus>> GetVaraus(int id)
        {
            var varaus = await _context.Varaukset
                .Include(v => v.Asiakas)
                .Include(v => v.Mokki)
                .FirstOrDefaultAsync(x => x.VarausId == id);
            if (varaus == null) return NotFound();
            return varaus;
        }

        [HttpPost]
        public async Task<ActionResult<Varaus>> PostVaraus(Varaus varaus)
        {
            _context.Varaukset.Add(varaus);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetVaraus), new { id = varaus.VarausId }, varaus);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutVaraus(int id, Varaus varaus)
        {
            if (id != varaus.VarausId) return BadRequest();
            _context.Entry(varaus).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteVaraus(int id)
        {
            var varaus = await _context.Varaukset.FindAsync(id);
            if (varaus == null) return NotFound();
            _context.Varaukset.Remove(varaus);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
