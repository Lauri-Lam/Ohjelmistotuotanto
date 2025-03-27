using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VillageNewbiesReservationSystem.Data;
using VillageNewbiesReservationSystem.Models;

namespace VillageNewbiesReservationSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MokitController : ControllerBase
    {
        private readonly AppDbContext _context;

        public MokitController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Mokki>>> GetMokit()
        {
            return await _context.Mokit.Include(m => m.Alue).ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Mokki>> GetMokki(int id)
        {
            var mokki = await _context.Mokit.Include(m => m.Alue).FirstOrDefaultAsync(m => m.MokkiId == id);
            if (mokki == null) return NotFound();
            return mokki;
        }

        [HttpPost]
        public async Task<ActionResult<Mokki>> PostMokki(Mokki mokki)
        {
            _context.Mokit.Add(mokki);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetMokki), new { id = mokki.MokkiId }, mokki);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutMokki(int id, Mokki mokki)
        {
            if (id != mokki.MokkiId) return BadRequest();
            _context.Entry(mokki).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMokki(int id)
        {
            var mokki = await _context.Mokit.FindAsync(id);
            if (mokki == null) return NotFound();
            _context.Mokit.Remove(mokki);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
