using FutbolitoManager.Data;
using FutbolitoManager.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace FutbolitoManager.Controllers
{
    public class ScrumController : Controller
    {
        private readonly AppDbContext _context;
        public ScrumController(AppDbContext context)
        {
            _context = context;
        }

        // Helper para verificar sesión de admin
        private bool IsAdmin() =>
            HttpContext.Session.GetString("EsAdmin") == "true";

        // ———————————— USER STORIES ————————————

        // Backlog Board (opcionalmente filtrado por sprint)
        public async Task<IActionResult> Index(int? sprintId)
        {
            if (!IsAdmin())
                return RedirectToAction("Login", "Home");

            var query = _context.UserStories
                                .Include(us => us.Sprint)
                                .AsQueryable();

            if (sprintId.HasValue)
                query = query.Where(us => us.SprintId == sprintId.Value);

            var stories = await query.ToListAsync();
            ViewBag.Sprints = new SelectList(await _context.Sprints.ToListAsync(), "Id", "Nombre");
            ViewBag.SelectedSprint = sprintId;
            return View(stories);
        }

        // Crear historia (GET)
        public async Task<IActionResult> Create()
        {
            if (!IsAdmin())
                return RedirectToAction("Login", "Home");

            ViewBag.Sprints = new SelectList(await _context.Sprints.ToListAsync(), "Id", "Nombre");
            return View();
        }

        // Crear historia (POST)
        [HttpPost]
        public async Task<IActionResult> Create(UserStory story)
        {
            if (!IsAdmin())
                return RedirectToAction("Login", "Home");

            if (!ModelState.IsValid)
            {
                ViewBag.Sprints = new SelectList(await _context.Sprints.ToListAsync(), "Id", "Nombre", story.SprintId);
                return View(story);
            }

            _context.UserStories.Add(story);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // Editar historia (GET)
        public async Task<IActionResult> Edit(int id)
        {
            if (!IsAdmin())
                return RedirectToAction("Login", "Home");

            var story = await _context.UserStories.FindAsync(id);
            if (story == null) return NotFound();

            ViewBag.Sprints = new SelectList(await _context.Sprints.ToListAsync(), "Id", "Nombre", story.SprintId);
            return View(story);
        }

        // Editar historia (POST)
        [HttpPost]
        public async Task<IActionResult> Edit(UserStory story)
        {
            if (!IsAdmin())
                return RedirectToAction("Login", "Home");

            if (!ModelState.IsValid)
            {
                ViewBag.Sprints = new SelectList(await _context.Sprints.ToListAsync(), "Id", "Nombre", story.SprintId);
                return View(story);
            }

            _context.UserStories.Update(story);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // Eliminar historia
        [HttpPost]
        public async Task<IActionResult> DeleteStory(int id)
        {
            if (!IsAdmin())
                return RedirectToAction("Login", "Home");

            var story = await _context.UserStories.FindAsync(id);
            if (story != null)
            {
                _context.UserStories.Remove(story);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }


        // ———————————— SPRINTS ————————————

        // Listar Sprints
        public async Task<IActionResult> Sprints()
        {
            if (!IsAdmin())
                return RedirectToAction("Login", "Home");

            var sprints = await _context.Sprints.ToListAsync();
            return View(sprints);
        }

        // Crear Sprint (GET)
        public IActionResult CreateSprint()
        {
            if (!IsAdmin())
                return RedirectToAction("Login", "Home");

            return View();
        }

        // Crear Sprint (POST)
        [HttpPost]
        public async Task<IActionResult> CreateSprint(Sprint sprint)
        {
            if (!IsAdmin())
                return RedirectToAction("Login", "Home");

            if (!ModelState.IsValid)
                return View(sprint);

            _context.Sprints.Add(sprint);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Sprints));
        }

        // Editar Sprint (GET)
        public async Task<IActionResult> EditSprint(int id)
        {
            if (!IsAdmin())
                return RedirectToAction("Login", "Home");

            var sprint = await _context.Sprints.FindAsync(id);
            if (sprint == null) return NotFound();
            return View(sprint);
        }

        // Editar Sprint (POST)
        [HttpPost]
        public async Task<IActionResult> EditSprint(Sprint sprint)
        {
            if (!IsAdmin())
                return RedirectToAction("Login", "Home");

            if (!ModelState.IsValid)
                return View(sprint);

            _context.Sprints.Update(sprint);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Sprints));
        }

        // Eliminar Sprint
        [HttpPost]
        public async Task<IActionResult> DeleteSprint(int id)
        {
            if (!IsAdmin())
                return RedirectToAction("Login", "Home");

            var sprint = await _context.Sprints.FindAsync(id);
            if (sprint != null)
            {
                _context.Sprints.Remove(sprint);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Sprints));
        }
    }
}
