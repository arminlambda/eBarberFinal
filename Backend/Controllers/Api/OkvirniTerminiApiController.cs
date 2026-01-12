using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using eBarber.Data;
using eBarber.Models;

namespace eBarber.Controllers.Api;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class OkvirniTerminiApiController : ControllerBase
{
    private readonly BarberContext _context;

    public OkvirniTerminiApiController(BarberContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<object>>> GetOkvirniTermini()
    {
        var termini = await _context.OkvirniTermini
            .Include(ot => ot.Lokacija)
            .Include(ot => ot.Prijave)
            .Where(ot => ot.JeAktiven && ot.ZacetekCasa > DateTime.Now)
            .OrderBy(ot => ot.ZacetekCasa)
            .Select(ot => new
            {
                ot.Id,
                ot.ZacetekCasa,
                ot.KonecCasa,
                ot.Opis,
                ot.MaksimalnoUporabnikov,
                Lokacija = new
                {
                    ot.Lokacija!.Id,
                    ot.Lokacija.Naslov,
                    ot.Lokacija.Mesto
                },
                SteviloPrijav = ot.Prijave!.Count(p => p.Status == "Potrjena"),
                JePolno = ot.Prijave!.Count(p => p.Status == "Potrjena") >= ot.MaksimalnoUporabnikov
            })
            .ToListAsync();

        return Ok(termini);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<object>> GetOkvirniTermin(int id)
    {
        var termin = await _context.OkvirniTermini
            .Include(ot => ot.Lokacija)
            .Include(ot => ot.Prijave)!
                .ThenInclude(p => p.Uporabnik)
            .Where(ot => ot.Id == id)
            .Select(ot => new
            {
                ot.Id,
                ot.ZacetekCasa,
                ot.KonecCasa,
                ot.Opis,
                ot.MaksimalnoUporabnikov,
                Lokacija = new
                {
                    ot.Lokacija!.Naslov,
                    ot.Lokacija.Mesto
                },
                Prijave = ot.Prijave!.Where(p => p.Status == "Potrjena").Select(p => new
                {
                    p.Id,
                    Uporabnik = p.Uporabnik!.Email,
                    p.DatumPrijave
                }).ToList()
            })
            .FirstOrDefaultAsync();

        if (termin == null)
        {
            return NotFound();
        }

        return Ok(termin);
    }

[HttpPost("{id}/prijava")]
public async Task<ActionResult> PrijaviSe(int id, [FromBody] PrijavaDto dto)
{
    var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

    var okvirniTermin = await _context.OkvirniTermini
        .Include(ot => ot.Prijave)
        .FirstOrDefaultAsync(ot => ot.Id == id);

    if (okvirniTermin == null)
    {
        return NotFound(new { message = "Okvirni termin ne obstaja" });
    }


    var obstojeca = await _context.PrijaveTerminov
        .AnyAsync(pt => pt.OkvirniTerminId == id && pt.UprabnikId == userId && pt.Status == "Potrjena");

    if (obstojeca)
    {
        return BadRequest(new { message = "Ste že prijavljeni na ta termin" });
    }

    var potrjenePrijave = okvirniTermin.Prijave!.Count(p => p.Status == "Potrjena");
    
    if (potrjenePrijave >= okvirniTermin.MaksimalnoUporabnikov)
    {
        var uporabnik = await _context.Users.FindAsync(userId);
        if (uporabnik == null) return BadRequest(new { message = "Uporabnik ne obstaja" });

        var novPriorityScore = CalculatePriorityScore(uporabnik);

        var najslabsaPrijava = await _context.PrijaveTerminov
            .Include(p => p.Uporabnik)
            .Where(p => p.OkvirniTerminId == id && p.Status == "Potrjena")
            .OrderBy(p => CalculatePriorityScore(p.Uporabnik!))
            .FirstOrDefaultAsync();

        if (najslabsaPrijava != null)
        {
            var najslabsiScore = CalculatePriorityScore(najslabsaPrijava.Uporabnik!);

            if (novPriorityScore > najslabsiScore)
            {
                najslabsaPrijava.Status = "Preklicana";
                
                var novaPrijava = new PrijavaTermina
                {
                    OkvirniTerminId = id,
                    UprabnikId = userId!,
                    Opombe = dto.Opombe + " (Avtomatsko razporejen - priority score)",
                    Status = "Potrjena"
                };

                _context.PrijaveTerminov.Add(novaPrijava);
                await _context.SaveChangesAsync();

                return Ok(new { 
                    message = "Prijava uspešna! Zaradi višjega priority score-a ste dobili mesto.",
                    priorityScore = novPriorityScore,
                    replacedUser = najslabsaPrijava.Uporabnik!.Email
                });
            }
            else
            {
                return BadRequest(new { 
                    message = "Termin je poln. Vaš priority score ni dovolj visok za avtomatsko razporeditev.",
                    yourScore = novPriorityScore,
                    requiredScore = najslabsiScore
                });
            }
        }
    }

    var prijava = new PrijavaTermina
    {
        OkvirniTerminId = id,
        UprabnikId = userId!,
        Opombe = dto.Opombe,
        Status = "Potrjena"
    };

    if (potrjenePrijave == 0)
    {
        prijava.Opombe += " (Prvi prijavljen)";
    }

    _context.PrijaveTerminov.Add(prijava);
    
    var user = await _context.Users.FindAsync(userId);
    if (user != null)
    {
        user.TotalBookings++;
    }

    await _context.SaveChangesAsync();

    return Ok(new { message = "Uspešno ste se prijavili na termin" });
}

private decimal CalculatePriorityScore(ApplicationUser user)
{
    decimal ratingScore = user.AverageRating / 5m;
    decimal reliabilityScore = user.ReliabilityScore;
    decimal visitScore = Math.Min(user.TotalBookings / 10m, 1.0m);

    decimal priorityScore = 
        (ratingScore * 0.4m) + 
        (reliabilityScore * 0.4m) + 
        (visitScore * 0.2m);

    return Math.Round(priorityScore, 3);
}

    [HttpDelete("prijave/{prijavaId}")]
    public async Task<ActionResult> OdjaviSe(int prijavaId)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        var prijava = await _context.PrijaveTerminov
            .FirstOrDefaultAsync(pt => pt.Id == prijavaId && pt.UprabnikId == userId);

        if (prijava == null)
        {
            return NotFound(new { message = "Prijava ne obstaja" });
        }

        prijava.Status = "Preklicana";
        await _context.SaveChangesAsync();

        return Ok(new { message = "Uspešno ste se odjavili" });
    }

    [HttpGet("moje-prijave")]
    public async Task<ActionResult<IEnumerable<object>>> GetMojePrijave()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        var prijave = await _context.PrijaveTerminov
            .Include(pt => pt.OkvirniTermin)
                .ThenInclude(ot => ot!.Lokacija)
            .Where(pt => pt.UprabnikId == userId && pt.Status == "Potrjena")
            .OrderBy(pt => pt.OkvirniTermin!.ZacetekCasa)
            .Select(pt => new
            {
                pt.Id,
                pt.DatumPrijave,
                pt.Opombe,
                OkvirniTermin = new
                {
                    pt.OkvirniTermin!.Id,
                    pt.OkvirniTermin.ZacetekCasa,
                    pt.OkvirniTermin.KonecCasa,
                    pt.OkvirniTermin.Opis,
                    Lokacija = new
                    {
                        pt.OkvirniTermin.Lokacija!.Naslov,
                        pt.OkvirniTermin.Lokacija.Mesto
                    }
                }
            })
            .ToListAsync();

        return Ok(prijave);
    }

    [HttpPost("test-dodaj")]
    public async Task<ActionResult> DodajTestniTermin()
    {
        var lokacija = await _context.Lokacije.FirstOrDefaultAsync();
        if (lokacija == null)
        {
            return BadRequest(new { message = "Ni lokacij" });
        }

        var termin = new OkvirniTermin
        {
            ZacetekCasa = DateTime.Now.AddDays(5).Date.AddHours(17),
            KonecCasa = DateTime.Now.AddDays(5).Date.AddHours(20),
            LokacijaId = lokacija.Id,
            MaksimalnoUporabnikov = 5,
            Opis = "TEST okvirni termin - Frankolovo",
            JeAktiven = true
        };

        _context.OkvirniTermini.Add(termin);
        await _context.SaveChangesAsync();

        return Ok(new { message = "Testni termin dodan!", id = termin.Id });
    }

    [HttpGet("admin/all")]
    [Authorize(Roles = "Administrator")]
    public async Task<ActionResult<IEnumerable<object>>> GetAllTerminiAdmin()
    {
        var termini = await _context.OkvirniTermini
            .Include(ot => ot.Lokacija)
            .Include(ot => ot.Prijave)!
                .ThenInclude(p => p.Uporabnik)
            .Where(ot => ot.JeAktiven)
            .OrderBy(ot => ot.ZacetekCasa)
            .Select(ot => new
            {
                ot.Id,
                ot.ZacetekCasa,
                ot.KonecCasa,
                ot.Opis,
                ot.MaksimalnoUporabnikov,
                Lokacija = new
                {
                    ot.Lokacija!.Naslov,
                    ot.Lokacija.Mesto
                },
                Prijave = ot.Prijave!.Where(p => p.Status == "Potrjena").Select(p => new
                {
                    p.Id,
                    p.JePrispel,
                    p.OcenaStranke,
                    p.Opombe,
                    Uporabnik = new
                    {
                        p.Uporabnik!.FirstName,
                        p.Uporabnik.LastName,
                        p.Uporabnik.Email,
                        p.Uporabnik.AverageRating
                    }
                }).ToList()
            })
            .ToListAsync();

        return Ok(termini);
    }

    [HttpPost("admin/checkin/{prijavaId}")]
    [Authorize(Roles = "Administrator")]
    public async Task<ActionResult> AdminCheckIn(int prijavaId)
    {
        var prijava = await _context.PrijaveTerminov
            .Include(p => p.Uporabnik)
            .FirstOrDefaultAsync(p => p.Id == prijavaId);

        if (prijava == null) return NotFound();

        prijava.JePrispel = true;
        
        if (prijava.Uporabnik != null)
        {
            prijava.Uporabnik.CompletedBookings++;
        }

        await _context.SaveChangesAsync();
        return Ok(new { message = "Check-in uspešen" });
    }

    [HttpPost("admin/rate/{prijavaId}")]
    [Authorize(Roles = "Administrator")]
    public async Task<ActionResult> AdminRateUser(int prijavaId, [FromBody] RatingDto dto)
    {
        if (dto.Rating < 1 || dto.Rating > 5) 
            return BadRequest(new { message = "Ocena mora biti med 1 in 5" });

        var prijava = await _context.PrijaveTerminov
            .Include(p => p.Uporabnik)
            .FirstOrDefaultAsync(p => p.Id == prijavaId);

        if (prijava == null) return NotFound();

        prijava.OcenaStranke = dto.Rating;

        if (prijava.Uporabnik != null)
        {
            var ocene = await _context.PrijaveTerminov
                .Where(p => p.UprabnikId == prijava.UprabnikId && p.OcenaStranke.HasValue)
                .Select(p => p.OcenaStranke!.Value)
                .ToListAsync();

            prijava.Uporabnik.AverageRating = ocene.Any() ? (decimal)ocene.Average() : 0;
        }

        await _context.SaveChangesAsync();
        return Ok(new { message = "Ocena shranjena" });
    }

    [HttpPost("admin/create")]
    [Authorize(Roles = "Administrator")]
    public async Task<ActionResult> AdminCreateTermin([FromBody] CreateTerminDto dto)
    {
        var termin = new OkvirniTermin
        {
            ZacetekCasa = dto.ZacetekCasa,
            KonecCasa = dto.KonecCasa,
            LokacijaId = dto.LokacijaId,
            MaksimalnoUporabnikov = dto.MaksimalnoUporabnikov,
            Opis = dto.Opis,
            JeAktiven = true
        };

        _context.OkvirniTermini.Add(termin);
        await _context.SaveChangesAsync();

        return Ok(new { message = "Termin ustvarjen", id = termin.Id });
    }
}

public class RatingDto
{
    public int Rating { get; set; }
}

public class PrijavaDto
{
    public string? Opombe { get; set; }
}

public class CreateTerminDto
{
    public DateTime ZacetekCasa { get; set; }
    public DateTime KonecCasa { get; set; }
    public int LokacijaId { get; set; }
    public int MaksimalnoUporabnikov { get; set; }
    public string? Opis { get; set; }
}