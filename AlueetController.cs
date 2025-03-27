using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VillageNewbiesReservationSystem.Data;
using VillageNewbiesReservationSystem.Models;

namespace VillageNewbiesReservationSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AlueetController : ControllerBase
    {
        private readonly AppDbContext _context;

        public AlueetController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Alue>>> GetAlueet()
        {
            return await _context.Alueet.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Alue>> GetAlue(int id)
        {
            var alue = await _context.Alueet.FindAsync(id);
            if (alue == null) return NotFound();
            return alue;
        }

        [HttpPost]
        public async Task<ActionResult<Alue>> PostAlue(Alue alue)
        {
            _context.Alueet.Add(alue);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetAlue), new { id = alue.AlueId }, alue);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutAlue(int id, Alue alue)
        {
            if (id != alue.AlueId) return BadRequest();
            _context.Entry(alue).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAlue(int id)
        {
            var alue = await _context.Alueet.FindAsync(id);
            if (alue == null) return NotFound();
            _context.Alueet.Remove(alue);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
