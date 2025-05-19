using FutbolitoManager.Data;
using FutbolitoManager.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace FutbolitoManager.Controllers
{
    public class RetrospectiveController : Controller
    {
        private readonly AppDbContext _context;
        public RetrospectiveController(AppDbContext context) => _context = context;

        // Solo admin
        private bool IsAdmin() =>
            HttpContext.Session.GetString("EsAdmin") == "true";

        // Listado de Retrospectives (filtrable por sprint)
        public async Task<IActionResult> Index(int? sprintId)
        {
            if (!IsAdmin())
                return RedirectToAction("Login", "Home");

            var q = _context.SprintRetrospectives
                            .Include(r => r.Sprint)
                            .AsQueryable();

            if (sprintId.HasValue)
                q = q.Where(r => r.SprintId == sprintId.Value);

            ViewBag.Sprints = new SelectList(
                await _context.Sprints.ToListAsync(),
                "Id", "Nombre"
            );
            ViewBag.SelectedSprint = sprintId;
            return View(await q.ToListAsync());
        }

        // GET: Crear nueva Retrospective
        public async Task<IActionResult> Create()
        {
            if (!IsAdmin())
                return RedirectToAction("Login", "Home");

            ViewBag.Sprints = new SelectList(
                await _context.Sprints.ToListAsync(),
                "Id", "Nombre"
            );
            return View();
        }

        // POST: Crear nueva Retrospective
        [HttpPost]
        public async Task<IActionResult> Create(SprintRetrospective ret)
        {
            if (!IsAdmin())
                return RedirectToAction("Login", "Home");

            if (!ModelState.IsValid)
            {
                ViewBag.Sprints = new SelectList(
                    await _context.Sprints.ToListAsync(),
                    "Id", "Nombre",
                    ret.SprintId
                );
                return View(ret);
            }

            ret.Fecha = DateTime.Now;
            _context.SprintRetrospectives.Add(ret);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: Editar Retrospective
        public async Task<IActionResult> Edit(int id)
        {
            if (!IsAdmin())
                return RedirectToAction("Login", "Home");

            var ret = await _context.SprintRetrospectives.FindAsync(id);
            if (ret == null) return NotFound();

            ViewBag.Sprints = new SelectList(
                await _context.Sprints.ToListAsync(),
                "Id", "Nombre",
                ret.SprintId
            );
            return View(ret);
        }

        // POST: Editar Retrospective
        [HttpPost]
        public async Task<IActionResult> Edit(SprintRetrospective ret)
        {
            if (!IsAdmin())
                return RedirectToAction("Login", "Home");

            if (!ModelState.IsValid)
            {
                ViewBag.Sprints = new SelectList(
                    await _context.Sprints.ToListAsync(),
                    "Id", "Nombre",
                    ret.SprintId
                );
                return View(ret);
            }

            _context.SprintRetrospectives.Update(ret);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
