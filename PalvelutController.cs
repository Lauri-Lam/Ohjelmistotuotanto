using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VillageNewbiesReservationSystem.Data;
using VillageNewbiesReservationSystem.Models;

namespace VillageNewbiesReservationSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PalvelutController : ControllerBase
    {
        private readonly AppDbContext _context;

        public PalvelutController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Palvelu>>> GetPalvelut()
        {
            return await _context.Palvelut.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Palvelu>> GetPalvelu(int id)
        {
            var palvelu = await _context.Palvelut.FindAsync(id);
            if (palvelu == null) return NotFound();
            return palvelu;
        }

        [HttpPost]
        public async Task<ActionResult<Palvelu>> PostPalvelu(Palvelu palvelu)
        {
            _context.Palvelut.Add(palvelu);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetPalvelu), new { id = palvelu.PalveluId }, palvelu);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutPalvelu(int id, Palvelu palvelu)
        {
            if (id != palvelu.PalveluId) return BadRequest();
            _context.Entry(palvelu).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePalvelu(int id)
        {
            var palvelu = await _context.Palvelut.FindAsync(id);
            if (palvelu == null) return NotFound();
            _context.Palvelut.Remove(palvelu);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
