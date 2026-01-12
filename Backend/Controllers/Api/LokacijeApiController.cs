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

namespace eBarber.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class LokacijeApiController : ControllerBase
    {
        private readonly BarberContext _context;

        public LokacijeApiController(BarberContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<object>>> GetLokacije()
        {
            var lokacije = await _context.Lokacije
                .Select(l => new
                {
                    l.Id,
                    l.Naslov,
                    l.Mesto,
                    DelovniCasOd = l.DelovniCasOd.HasValue ? l.DelovniCasOd.Value.ToString(@"hh\:mm") : null,
                    DelovniCasDo = l.DelovniCasDo.HasValue ? l.DelovniCasDo.Value.ToString(@"hh\:mm") : null
                })
                .ToListAsync();

            return Ok(lokacije);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Lokacija>> GetLokacija(int id)
        {
            var lokacija = await _context.Lokacije.FindAsync(id);

            if (lokacija == null)
            {
                return NotFound();
            }

            return lokacija;
        }
    }
}