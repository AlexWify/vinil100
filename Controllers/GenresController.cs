using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VinylStore;        // если AppDbContext лежит в корне проекта
using VinylStore.Models; // если Genre лежит в Models

namespace VinylStore.Controllers
{
    public class GenresController : Controller
    {
        private readonly AppDbContext _context;

        public GenresController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Genres
        public async Task<IActionResult> Index()
        {
            var genres = await _context.Genres
                .Include(g => g.Records)
                .ToListAsync();

            return View(genres);
        }

        // GET: Genres/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Genres/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name")] Genre genre)
        {
            if (ModelState.IsValid)
            {
                _context.Add(genre);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = $"Жанр «{genre.Name}» успешно создан.";
                return RedirectToAction(nameof(Index));
            }
            return View(genre);
        }

        // GET: Genres/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var genre = await _context.Genres.FindAsync(id);
            if (genre == null)
            {
                return NotFound();
            }
            return View(genre);
        }

        // POST: Genres/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name")] Genre genre)
        {
            if (id != genre.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(genre);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = $"Жанр «{genre.Name}» успешно обновлен.";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Genres.Any(e => e.Id == genre.Id))
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
            return View(genre);
        }

        // POST: Genres/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var genre = await _context.Genres
                .Include(g => g.Records)
                .FirstOrDefaultAsync(g => g.Id == id);

            if (genre != null)
            {
                if (genre.Records != null && genre.Records.Any())
                {
                    TempData["ErrorMessage"] = $"Нельзя удалить «{genre.Name}»: к нему привязано пластинок: {genre.Records.Count}.";
                    return RedirectToAction(nameof(Index));
                }

                _context.Genres.Remove(genre);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = $"Жанр «{genre.Name}» успешно удален.";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}