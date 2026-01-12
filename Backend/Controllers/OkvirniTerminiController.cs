using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;
using eBarber.Data;
using eBarber.Models;

namespace eBarber.Controllers;

[Authorize(Roles = "Administrator")]
public class OkvirniTerminiController : Controller
{
    private readonly BarberContext _context;

    public OkvirniTerminiController(BarberContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var termini = await _context.OkvirniTermini
            .Include(o => o.Lokacija)
            .Include(o => o.Prijave)
            .OrderBy(o => o.ZacetekCasa)
            .ToListAsync();
        return View(termini);
    }

    public IActionResult Create()
    {
        ViewData["LokacijaId"] = new SelectList(_context.Lokacije, "Id", "Naslov");
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("ZacetekCasa,KonecCasa,LokacijaId,MaksimalnoUporabnikov,Opis")] OkvirniTermin okvirniTermin)
    {
        if (ModelState.IsValid)
        {
            okvirniTermin.JeAktiven = true;
            _context.Add(okvirniTermin);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        ViewData["LokacijaId"] = new SelectList(_context.Lokacije, "Id", "Naslov", okvirniTermin.LokacijaId);
        return View(okvirniTermin);
    }
}
