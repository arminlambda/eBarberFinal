using System;
using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using eBarber.Data;
using eBarber.Models;

namespace eBarber.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class LokacijeController : Controller
    {
        private readonly BarberContext _context;

        public LokacijeController(BarberContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var barberContext = _context.Lokacije.Include(l => l.Frizer);
            return View(await barberContext.ToListAsync());
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var lokacija = await _context.Lokacije
                .Include(l => l.Frizer)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (lokacija == null)
            {
                return NotFound();
            }

            return View(lokacija);
        }

        public IActionResult Create()
        {
            ViewData["FrizerId"] = new SelectList(_context.Frizerji, "Id", "Ime");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Naslov,Mesto,DelovniCasOd,DelovniCasDo,FrizerId")] Lokacija lokacija)
        {
            if (ModelState.IsValid)
            {
                _context.Add(lokacija);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["FrizerId"] = new SelectList(_context.Frizerji, "Id", "Ime", lokacija.FrizerId);
            return View(lokacija);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var lokacija = await _context.Lokacije.FindAsync(id);
            if (lokacija == null)
            {
                return NotFound();
            }
            ViewData["FrizerId"] = new SelectList(_context.Frizerji, "Id", "Ime", lokacija.FrizerId);
            return View(lokacija);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Naslov,Mesto,DelovniCasOd,DelovniCasDo,FrizerId")] Lokacija lokacija)
        {
            if (id != lokacija.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(lokacija);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LokacijaExists(lokacija.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["FrizerId"] = new SelectList(_context.Frizerji, "Id", "Ime", lokacija.FrizerId);
            return View(lokacija);
        }

        // GET: Lokacije/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var lokacija = await _context.Lokacije
                .Include(l => l.Frizer)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (lokacija == null)
            {
                return NotFound();
            }

            return View(lokacija);
        }

        // POST: Lokacije/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var lokacija = await _context.Lokacije.FindAsync(id);
            if (lokacija != null)
            {
                _context.Lokacije.Remove(lokacija);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool LokacijaExists(int id)
        {
            return _context.Lokacije.Any(e => e.Id == id);
        }
    }
}
