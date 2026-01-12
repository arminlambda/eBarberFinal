using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using eBarber.Data;
using eBarber.Models;
using System.Security.Claims;

namespace eBarber.Controllers
{
    [Authorize(Roles = "Administrator,User")]
    public class TerminiController : Controller
    {
        private readonly BarberContext _context;

        public TerminiController(BarberContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var isAdmin = User.IsInRole("Administrator");

            IQueryable<Termin> terminiQuery = _context.Termini
                .Include(t => t.Lokacija)
                .Include(t => t.Stranka);

            if (!isAdmin)
            {
                terminiQuery = terminiQuery.Where(t => t.StrankaId == userId);
            }

            return View(await terminiQuery.ToListAsync());
        }

        // GET: Termini/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var termin = await _context.Termini
                .Include(t => t.Lokacija)
                .Include(t => t.Stranka)
                .FirstOrDefaultAsync(m => m.Id == id);
            
            if (termin == null)
            {
                return NotFound();
            }

            // Preveri dostop - samo lastnik ali admin
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (termin.StrankaId != userId && !User.IsInRole("Administrator"))
            {
                return Forbid();
            }

            return View(termin);
        }

        public IActionResult Create()
        {
            ViewData["LokacijaId"] = new SelectList(_context.Lokacije, "Id", "Naslov");
            
            if (User.IsInRole("Administrator"))
            {
                ViewData["StrankaId"] = new SelectList(_context.Users, "Id", "Email");
            }
            
            return View();
        }

        // POST: Termini/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,DatumInUra,Status,Opombe,StrankaId,LokacijaId")] Termin termin)
        {
            if (!User.IsInRole("Administrator"))
            {
                termin.StrankaId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
                termin.Status = "Čaka";
            }

            if (ModelState.IsValid)
            {
                _context.Add(termin);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            
            ViewData["LokacijaId"] = new SelectList(_context.Lokacije, "Id", "Naslov", termin.LokacijaId);
            
            if (User.IsInRole("Administrator"))
            {
                ViewData["StrankaId"] = new SelectList(_context.Users, "Id", "Email", termin.StrankaId);
            }
            
            return View(termin);
        }

        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var termin = await _context.Termini.FindAsync(id);
            if (termin == null)
            {
                return NotFound();
            }
            ViewData["LokacijaId"] = new SelectList(_context.Lokacije, "Id", "Naslov", termin.LokacijaId);
            ViewData["StrankaId"] = new SelectList(_context.Users, "Id", "Email", termin.StrankaId);
            return View(termin);
        }

        // POST: Termini/Edit/5 - samo admin
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,DatumInUra,Status,Opombe,StrankaId,LokacijaId")] Termin termin)
        {
            if (id != termin.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(termin);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TerminExists(termin.Id))
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
            ViewData["LokacijaId"] = new SelectList(_context.Lokacije, "Id", "Naslov", termin.LokacijaId);
            ViewData["StrankaId"] = new SelectList(_context.Users, "Id", "Email", termin.StrankaId);
            return View(termin);
        }

        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var termin = await _context.Termini
                .Include(t => t.Lokacija)
                .Include(t => t.Stranka)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (termin == null)
            {
                return NotFound();
            }

            return View(termin);
        }

        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var termin = await _context.Termini.FindAsync(id);
            if (termin != null)
            {
                _context.Termini.Remove(termin);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TerminExists(int id)
        {
            return _context.Termini.Any(e => e.Id == id);
        }
    }
}