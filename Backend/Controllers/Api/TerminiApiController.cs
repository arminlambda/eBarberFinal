using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using eBarber.Data;
using eBarber.Models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace eBarber.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class TerminiApiController : ControllerBase
    {
        private readonly BarberContext _context;

        public TerminiApiController(BarberContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<object>>> GetTermini()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var isAdmin = User.IsInRole("Administrator");

            IQueryable<Termin> query = _context.Termini
                .Include(t => t.Lokacija)
                .Include(t => t.Stranka);

            if (!isAdmin)
            {
                query = query.Where(t => t.StrankaId == userId);
            }

            var termini = await query
                .Select(t => new
                {
                    t.Id,
                    t.DatumInUra,
                    t.Status,
                    t.Opombe,
                    Lokacija = new
                    {
                        t.Lokacija!.Id,
                        t.Lokacija.Naslov,
                        t.Lokacija.Mesto
                    },
                    Stranka = new
                    {
                        t.Stranka!.Email,
                        t.Stranka.FirstName,
                        t.Stranka.LastName
                    }
                })
                .ToListAsync();

            return Ok(termini);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Termin>> GetTermin(int id)
        {
            var termin = await _context.Termini
                .Include(t => t.Lokacija)
                .Include(t => t.Stranka)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (termin == null)
            {
                return NotFound();
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (termin.StrankaId != userId && !User.IsInRole("Administrator"))
            {
                return Forbid();
            }

            return termin;
        }

        [HttpPost]
        public async Task<ActionResult<Termin>> PostTermin(TerminCreateDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var termin = new Termin
            {
                DatumInUra = dto.DatumInUra,
                Status = "Čaka",
                Opombe = dto.Opombe,
                StrankaId = userId!,
                LokacijaId = dto.LokacijaId
            };

            _context.Termini.Add(termin);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetTermin), new { id = termin.Id }, termin);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> DeleteTermin(int id)
        {
            var termin = await _context.Termini.FindAsync(id);
            if (termin == null)
            {
                return NotFound();
            }

            _context.Termini.Remove(termin);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool TerminExists(int id)
        {
            return _context.Termini.Any(e => e.Id == id);
        }
    }

    public class TerminCreateDto
    {
        public DateTime DatumInUra { get; set; }
        public string? Opombe { get; set; }
        public int LokacijaId { get; set; }
    }
}