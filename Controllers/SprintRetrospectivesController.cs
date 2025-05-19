using FutbolitoManager.Data;
using FutbolitoManager.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace FutbolitoManager.Controllers
{
    public class SprintRetrospectivesController : Controller
    {
        private readonly AppDbContext _context;
        public SprintRetrospectivesController(AppDbContext context) => _context = context;

        // Comprueba si es administrador
        private bool IsAdmin() =>
            HttpContext.Session.GetString("EsAdmin") == "true";

        // GET: SprintRetrospectives
        public async Task<IActionResult> Index()
        {
            var list = await _context.SprintRetrospectives
                                     .Include(r => r.Sprint)
                                     .ToListAsync();
            return View(list);
        }

        // GET: SprintRetrospectives/Create
        public async Task<IActionResult> Create()
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Home");
            ViewBag.Sprints = new SelectList(
                await _context.Sprints.ToListAsync(), "Id", "Nombre");
            return View();
        }

        // POST: SprintRetrospectives/Create
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SprintRetrospective retrospective)
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Home");
            if (ModelState.IsValid)
            {
                _context.Add(retrospective);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Sprints = new SelectList(
                await _context.Sprints.ToListAsync(), "Id", "Nombre", retrospective.SprintId);
            return View(retrospective);
        }

        // GET: SprintRetrospectives/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Home");
            var retrospective = await _context.SprintRetrospectives.FindAsync(id);
            if (retrospective == null) return NotFound();
            ViewBag.Sprints = new SelectList(
                await _context.Sprints.ToListAsync(), "Id", "Nombre", retrospective.SprintId);
            return View(retrospective);
        }

        // POST: SprintRetrospectives/Edit/5
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(SprintRetrospective retrospective)
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Home");
            if (ModelState.IsValid)
            {
                _context.Update(retrospective);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Sprints = new SelectList(
                await _context.Sprints.ToListAsync(), "Id", "Nombre", retrospective.SprintId);
            return View(retrospective);
        }
    }
}
