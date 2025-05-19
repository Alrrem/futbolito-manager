using FutbolitoManager.Data;
using FutbolitoManager.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace FutbolitoManager.Controllers
{
    public class SprintReviewsController : Controller
    {
        private readonly AppDbContext _context;
        public SprintReviewsController(AppDbContext context) => _context = context;

        private bool IsAdmin() =>
            HttpContext.Session.GetString("EsAdmin") == "true";

        // GET: SprintReviews
        public async Task<IActionResult> Index()
        {
            var list = await _context.SprintReviews
                                     .Include(r => r.Sprint)
                                     .ToListAsync();
            return View(list);
        }

        // GET: SprintReviews/Create
        public async Task<IActionResult> Create()
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Home");
            ViewBag.Sprints = new SelectList(
                await _context.Sprints.ToListAsync(), "Id", "Nombre");
            return View();
        }

        // POST: SprintReviews/Create
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SprintReview review)
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Home");
            if (ModelState.IsValid)
            {
                _context.Add(review);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Sprints = new SelectList(
                await _context.Sprints.ToListAsync(), "Id", "Nombre", review.SprintId);
            return View(review);
        }

        // GET: SprintReviews/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Home");
            var review = await _context.SprintReviews.FindAsync(id);
            if (review == null) return NotFound();
            ViewBag.Sprints = new SelectList(
                await _context.Sprints.ToListAsync(), "Id", "Nombre", review.SprintId);
            return View(review);
        }

        // POST: SprintReviews/Edit/5
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(SprintReview review)
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Home");
            if (ModelState.IsValid)
            {
                _context.Update(review);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Sprints = new SelectList(
                await _context.Sprints.ToListAsync(), "Id", "Nombre", review.SprintId);
            return View(review);
        }
    }
}
