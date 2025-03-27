using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VillageNewbiesReservationSystem.Data;
using VillageNewbiesReservationSystem.Models;

namespace VillageNewbiesReservationSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AsiakkaatController : ControllerBase
    {
        private readonly AppDbContext _context;

        public AsiakkaatController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Asiakas>>> GetAsiakkaat()
        {
            return await _context.Asiakkaat.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Asiakas>> GetAsiakas(int id)
        {
            var asiakas = await _context.Asiakkaat.FindAsync(id);
            if (asiakas == null) return NotFound();
            return asiakas;
        }

        [HttpPost]
        public async Task<ActionResult<Asiakas>> PostAsiakas(Asiakas asiakas)
        {
            _context.Asiakkaat.Add(asiakas);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetAsiakas), new { id = asiakas.AsiakasId }, asiakas);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutAsiakas(int id, Asiakas asiakas)
        {
            if (id != asiakas.AsiakasId) return BadRequest();
            _context.Entry(asiakas).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsiakas(int id)
        {
            var asiakas = await _context.Asiakkaat.FindAsync(id);
            if (asiakas == null) return NotFound();
            _context.Asiakkaat.Remove(asiakas);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
