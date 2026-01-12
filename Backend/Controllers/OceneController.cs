using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authorization;

using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using eBarber.Data;
using eBarber.Models;

namespace eBarber.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class OceneController : Controller
    {
        private readonly BarberContext _context;

        public OceneController(BarberContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var barberContext = _context.Ocene.Include(o => o.Stranka).Include(o => o.Termin);
            return View(await barberContext.ToListAsync());
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ocena = await _context.Ocene
                .Include(o => o.Stranka)
                .Include(o => o.Termin)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (ocena == null)
            {
                return NotFound();
            }

            return View(ocena);
        }

        public IActionResult Create()
        {
            ViewData["StrankaId"] = new SelectList(_context.Users, "Id", "Id");
            ViewData["TerminId"] = new SelectList(_context.Termini, "Id", "Status");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Stevilka,Komentar,DatumOcene,TerminId,StrankaId")] Ocena ocena)
        {
            if (ModelState.IsValid)
            {
                _context.Add(ocena);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["StrankaId"] = new SelectList(_context.Users, "Id", "Id", ocena.StrankaId);
            ViewData["TerminId"] = new SelectList(_context.Termini, "Id", "Status", ocena.TerminId);
            return View(ocena);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ocena = await _context.Ocene.FindAsync(id);
            if (ocena == null)
            {
                return NotFound();
            }
            ViewData["StrankaId"] = new SelectList(_context.Users, "Id", "Id", ocena.StrankaId);
            ViewData["TerminId"] = new SelectList(_context.Termini, "Id", "Status", ocena.TerminId);
            return View(ocena);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Stevilka,Komentar,DatumOcene,TerminId,StrankaId")] Ocena ocena)
        {
            if (id != ocena.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(ocena);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OcenaExists(ocena.Id))
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
            ViewData["StrankaId"] = new SelectList(_context.Users, "Id", "Id", ocena.StrankaId);
            ViewData["TerminId"] = new SelectList(_context.Termini, "Id", "Status", ocena.TerminId);
            return View(ocena);
        }

        // GET: Ocene/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ocena = await _context.Ocene
                .Include(o => o.Stranka)
                .Include(o => o.Termin)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (ocena == null)
            {
                return NotFound();
            }

            return View(ocena);
        }

        // POST: Ocene/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var ocena = await _context.Ocene.FindAsync(id);
            if (ocena != null)
            {
                _context.Ocene.Remove(ocena);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool OcenaExists(int id)
        {
            return _context.Ocene.Any(e => e.Id == id);
        }
    }
}
