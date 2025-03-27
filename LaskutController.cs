using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VillageNewbiesReservationSystem.Data;
using VillageNewbiesReservationSystem.Models;

namespace VillageNewbiesReservationSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LaskutController : ControllerBase
    {
        private readonly AppDbContext _context;

        public LaskutController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Lasku>>> GetLaskut()
        {
            return await _context.Laskut
                .Include(l => l.Varaus)
                .ThenInclude(v => v.Asiakas)
                .Include(l => l.Varaus.Mokki)
                .ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Lasku>> GetLasku(int id)
        {
            var lasku = await _context.Laskut
                .Include(l => l.Varaus)
                .ThenInclude(v => v.Asiakas)
                .Include(l => l.Varaus.Mokki)
                .FirstOrDefaultAsync(l => l.LaskuId == id);
            if (lasku == null) return NotFound();
            return lasku;
        }

        [HttpPost]
        public async Task<ActionResult<Lasku>> PostLasku(Lasku lasku)
        {
            _context.Laskut.Add(lasku);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetLasku), new { id = lasku.LaskuId }, lasku);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutLasku(int id, Lasku lasku)
        {
            if (id != lasku.LaskuId) return BadRequest();
            _context.Entry(lasku).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteLasku(int id)
        {
            var lasku = await _context.Laskut.FindAsync(id);
            if (lasku == null) return NotFound();
            _context.Laskut.Remove(lasku);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
