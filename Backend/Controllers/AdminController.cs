using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using eBarber.Data;

namespace eBarber.Controllers;

[Authorize(Roles = "Administrator")]
public class AdminController : Controller
{
    private readonly BarberContext _context;

    public AdminController(BarberContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var stats = new
        {
            TotalUsers = await _context.Users.CountAsync(),
            TodaysAppointments = await _context.PrijaveTerminov
                .Include(p => p.OkvirniTermin)
                .Where(p => p.OkvirniTermin!.ZacetekCasa.Date == DateTime.Today)
                .CountAsync(),
            PendingMessages = await _context.Messages
                .Where(m => !m.IsRead && m.ReceiverId == null)
                .CountAsync(),
            AverageRating = await _context.Users
                .Where(u => u.AverageRating > 0)
                .AverageAsync(u => (double?)u.AverageRating) ?? 0
        };
        
        return View(stats);
    }

    public async Task<IActionResult> Users()
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
                u.AverageRating,
                ReliabilityScore = u.TotalBookings > 0 ? 
                    (decimal)u.CompletedBookings / u.TotalBookings : 0
            })
            .ToListAsync();
        
        return View(users);
    }

    public async Task<IActionResult> UserDetails(string id)
    {
        var user = await _context.Users.FindAsync(id);
        if (user == null) return NotFound();

        var prijave = await _context.PrijaveTerminov
            .Include(p => p.OkvirniTermin)
                .ThenInclude(ot => ot!.Lokacija)
            .Where(p => p.UprabnikId == id)
            .OrderByDescending(p => p.OkvirniTermin!.ZacetekCasa)
            .ToListAsync();

        ViewBag.User = user;
        ViewBag.Prijave = prijave;
        
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> CheckIn(int prijavaId)
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
        return RedirectToAction(nameof(UserDetails), new { id = prijava.UprabnikId });
    }

    [HttpPost]
    public async Task<IActionResult> RateUser(int prijavaId, int rating)
    {
        if (rating < 1 || rating > 5) return BadRequest("Ocena mora biti med 1 in 5");

        var prijava = await _context.PrijaveTerminov
            .Include(p => p.Uporabnik)
            .FirstOrDefaultAsync(p => p.Id == prijavaId);

        if (prijava == null) return NotFound();

        prijava.OcenaStranke = rating;

        if (prijava.Uporabnik != null)
        {
            var ocene = await _context.PrijaveTerminov
                .Where(p => p.UprabnikId == prijava.UprabnikId && p.OcenaStranke.HasValue)
                .Select(p => p.OcenaStranke!.Value)
                .ToListAsync();

            prijava.Uporabnik.AverageRating = ocene.Any() ? 
                (decimal)ocene.Average() : 0;
        }

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(UserDetails), new { id = prijava.UprabnikId });
    }

    public async Task<IActionResult> Messages()
    {
        var messages = await _context.Messages
            .Include(m => m.Sender)
            .Where(m => m.ReceiverId == null)
            .OrderByDescending(m => m.SentAt)
            .ToListAsync();

        return View(messages);
    }

    [HttpPost]
    public async Task<IActionResult> MarkAsRead(int messageId)
    {
        var message = await _context.Messages.FindAsync(messageId);
        if (message != null)
        {
            message.IsRead = true;
            await _context.SaveChangesAsync();
        }
        return RedirectToAction(nameof(Messages));
    }

    [HttpPost]
    public async Task<IActionResult> SetUserRating(string userId, decimal rating)
    {
        if (rating < 0 || rating > 5) return BadRequest("Ocena mora biti med 0 in 5");

        var user = await _context.Users.FindAsync(userId);
        if (user == null) return NotFound();

        user.AverageRating = rating;
        await _context.SaveChangesAsync();

        TempData["Success"] = "Ocena uspešno shranjena!";
        return RedirectToAction(nameof(UserDetails), new { id = userId });
    }

    [HttpPost]
    public async Task<IActionResult> UpdateUserStats(string userId, int totalBookings, int completedBookings, int cancelledBookings)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user == null) return NotFound();

        user.TotalBookings = totalBookings;
        user.CompletedBookings = completedBookings;
        user.CancelledBookings = cancelledBookings;

        await _context.SaveChangesAsync();

        TempData["Success"] = "Statistika posodobljena!";
        return RedirectToAction(nameof(UserDetails), new { id = userId });
    }
}