using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using eBarber.Data;
using eBarber.Models;

namespace eBarber.Controllers.Api;

[Route("api/admin")]
[ApiController]
[Authorize(Roles = "Administrator")]
public class AdminApiController : ControllerBase
{
    private readonly BarberContext _context;

    public AdminApiController(BarberContext context)
    {
        _context = context;
    }


    [HttpGet("users")]
    public async Task<ActionResult<IEnumerable<object>>> GetUsers()
    {
        var users = await _context.Users
            .OrderByDescending(u => u.DateCreated)
            .Select(u => new
            {
                u.Id,
                u.Email,
                u.FirstName,
                u.LastName,
                u.TotalBookings,
                u.CompletedBookings,
                u.CancelledBookings,
                u.AverageRating
            })
            .ToListAsync();

        return Ok(users);
    }

    [HttpGet("users/{id}")]
    public async Task<ActionResult<object>> GetUserDetail(string id)
    {
        var user = await _context.Users.FindAsync(id);
        if (user == null) return NotFound();

        var prijave = await _context.PrijaveTerminov
            .Include(p => p.OkvirniTermin)
                .ThenInclude(ot => ot!.Lokacija)
            .Where(p => p.UprabnikId == id)
            .OrderByDescending(p => p.OkvirniTermin!.ZacetekCasa)
            .Select(p => new
            {
                p.Id,
                p.JePrispel,
                p.OcenaStranke,
                p.DatumPrijave,
                Termin = new
                {
                    p.OkvirniTermin!.ZacetekCasa,
                    p.OkvirniTermin.KonecCasa,
                    Lokacija = p.OkvirniTermin.Lokacija!.Naslov + ", " + p.OkvirniTermin.Lokacija.Mesto
                }
            })
            .ToListAsync();

        return Ok(new
        {
            User = new
            {
                user.Id,
                user.Email,
                user.FirstName,
                user.LastName,
                user.TotalBookings,
                user.CompletedBookings,
                user.CancelledBookings,
                user.AverageRating
            },
            Prijave = prijave
        });
    }
    [HttpPost("users/{id}/rating")]
    public async Task<ActionResult> SetUserRating(string id, [FromBody] RatingRequest req)
    {
        var user = await _context.Users.FindAsync(id);
        if (user == null) return NotFound();

        user.AverageRating = req.Rating;
        await _context.SaveChangesAsync();

        return Ok(new { message = "Ocena shranjena" });
    }
}

public class RatingRequest
{
    public decimal Rating { get; set; }
}