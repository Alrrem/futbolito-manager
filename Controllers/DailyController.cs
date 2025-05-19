using FutbolitoManager.Data;
using FutbolitoManager.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace FutbolitoManager.Controllers
{
    public class DailyController : Controller
    {
        private readonly AppDbContext _context;
        public DailyController(AppDbContext context) => _context = context;

        private bool IsAdmin() =>
            HttpContext.Session.GetString("EsAdmin") == "true";

        // Listado de logs (Daily Scrum)
        public async Task<IActionResult> Index(int? sprintId)
        {
            if (!IsAdmin())
                return RedirectToAction("Login", "Home");

            var q = _context.DailyLogs
                            .Include(dl => dl.Sprint)
                            .AsQueryable();

            if (sprintId.HasValue)
                q = q.Where(dl => dl.SprintId == sprintId);

            ViewBag.Sprints = new SelectList(
                await _context.Sprints.ToListAsync(),
                "Id",
                "Nombre"
            );
            ViewBag.SelectedSprint = sprintId;

            return View(await q.OrderByDescending(dl => dl.Fecha).ToListAsync());
        }

        // GET: Crear nuevo log
        public async Task<IActionResult> Create()
        {
            if (!IsAdmin())
                return RedirectToAction("Login", "Home");

            ViewBag.Sprints = new SelectList(
                await _context.Sprints.ToListAsync(),
                "Id",
                "Nombre"
            );
            return View();
        }

        // POST: Crear nuevo log
        [HttpPost]
        public async Task<IActionResult> Create(DailyLog log)
        {
            if (!IsAdmin())
                return RedirectToAction("Login", "Home");

            if (!ModelState.IsValid)
            {
                ViewBag.Sprints = new SelectList(
                    await _context.Sprints.ToListAsync(),
                    "Id",
                    "Nombre",
                    log.SprintId
                );
                return View(log);
            }

            log.Fecha = DateTime.Now;
            log.CreatedBy = HttpContext.Session.GetString("AdminEmail") ?? "Desconocido";  // ← Asignación de usuario

            _context.DailyLogs.Add(log);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: Editar log
        public async Task<IActionResult> Edit(int id)
        {
            if (!IsAdmin())
                return RedirectToAction("Login", "Home");

            var log = await _context.DailyLogs.FindAsync(id);
            if (log == null)
                return NotFound();

            ViewBag.Sprints = new SelectList(
                await _context.Sprints.ToListAsync(),
                "Id",
                "Nombre",
                log.SprintId
            );
            return View(log);
        }

        // POST: Editar log
        [HttpPost]
        public async Task<IActionResult> Edit(DailyLog log)
        {
            if (!IsAdmin())
                return RedirectToAction("Login", "Home");

            if (!ModelState.IsValid)
            {
                ViewBag.Sprints = new SelectList(
                    await _context.Sprints.ToListAsync(),
                    "Id",
                    "Nombre",
                    log.SprintId
                );
                return View(log);
            }

            _context.DailyLogs.Update(log);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // POST: Eliminar log
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            if (!IsAdmin())
                return RedirectToAction("Login", "Home");

            var log = await _context.DailyLogs.FindAsync(id);
            if (log != null)
            {
                _context.DailyLogs.Remove(log);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
