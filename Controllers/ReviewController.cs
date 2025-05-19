using FutbolitoManager.Data;
using FutbolitoManager.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace FutbolitoManager.Controllers
{
    public class ReviewController : Controller
    {
        private readonly AppDbContext _context;
        public ReviewController(AppDbContext context) => _context = context;

        // Verifica sesión de admin
        private bool IsAdmin() =>
            HttpContext.Session.GetString("EsAdmin") == "true";

        // Listado de Sprint Reviews (opcionalmente filtrado por Sprint)
        public async Task<IActionResult> Index(int? sprintId)
        {
            if (!IsAdmin())
                return RedirectToAction("Login", "Home");

            var q = _context.SprintReviews
                            .Include(r => r.Sprint)
                            .AsQueryable();
            if (sprintId.HasValue)
                q = q.Where(r => r.SprintId == sprintId);

            ViewBag.Sprints = new SelectList(
                await _context.Sprints.ToListAsync(),
                "Id", "Nombre"
            );
            ViewBag.SelectedSprint = sprintId;
            return View(await q.ToListAsync());
        }

        // GET: Crear nueva Review
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

        // POST: Crear nueva Review
        [HttpPost]
        public async Task<IActionResult> Create(SprintReview review)
        {
            if (!IsAdmin())
                return RedirectToAction("Login", "Home");

            if (!ModelState.IsValid)
            {
                ViewBag.Sprints = new SelectList(
                    await _context.Sprints.ToListAsync(),
                    "Id", "Nombre",
                    review.SprintId
                );
                return View(review);
            }

            review.Fecha = DateTime.Now;
            _context.SprintReviews.Add(review);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: Editar Review
        public async Task<IActionResult> Edit(int id)
        {
            if (!IsAdmin())
                return RedirectToAction("Login", "Home");

            var review = await _context.SprintReviews.FindAsync(id);
            if (review == null) return NotFound();

            ViewBag.Sprints = new SelectList(
                await _context.Sprints.ToListAsync(),
                "Id", "Nombre",
                review.SprintId
            );
            return View(review);
        }

        // POST: Editar Review
        [HttpPost]
        public async Task<IActionResult> Edit(SprintReview review)
        {
            if (!IsAdmin())
                return RedirectToAction("Login", "Home");

            if (!ModelState.IsValid)
            {
                ViewBag.Sprints = new SelectList(
                    await _context.Sprints.ToListAsync(),
                    "Id", "Nombre",
                    review.SprintId
                );
                return View(review);
            }

            _context.SprintReviews.Update(review);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
