using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RelationsNaN.Data;
using RelationsNaN.Models;

namespace RelationsNaN.Controllers
{
    public class GamesController : Controller
    {
        private readonly RelationsNaNContext _context;

        public GamesController(RelationsNaNContext context)
        {
            _context = context;
        }

        // GET: Games
        public async Task<IActionResult> Index()
        {
            var relationsNaNContext = _context.Game.Include(g => g.Genre).Include(g => g.Platforms);
            return View(await relationsNaNContext.ToListAsync());
        }

        // GET: Games/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var game = await _context.Game
                .Include(g => g.Genre)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (game == null)
            {
                return NotFound();
            }

            return View(game);
        }

        // GET: Games/Create
        public IActionResult Create()
        {
            ViewData["GenreId"] = new SelectList(_context.Genre, "Id", "Name");
            return View();
        }

        // POST: Games/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Image,ReleaseYear,GenreId")] Game game)
        {
            if (ModelState.IsValid)
            {
                _context.Add(game);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["GenreId"] = new SelectList(_context.Genre, "Id", "Name", game.GenreId);
            return View(game);
        }

        // GET: Games/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var game = await _context.Game.Include(g => g.Platforms).FirstOrDefaultAsync(p => p.Id == id);
            if (game == null)
            {
                return NotFound();
            }

            // Add the available platforms to the ViewBag
            var allPlatforms = await _context.Platform.ToListAsync();
            ViewBag.Platforms = new SelectList(allPlatforms, "Id", "Name");

            ViewData["GenreId"] = new SelectList(_context.Genre, "Id", "Name", game.GenreId);
            ViewData["PlatformId"] = new SelectList(_context.Platform, "Id", "Name", game.Platforms.Select(p => p.Id).ToList());

            return View(game);
        }

        // POST: Games/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Image,ReleaseYear,GenreId")] Game game)
        {
            if (id != game.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(game);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!GameExists(game.Id))
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
            ViewData["GenreId"] = new SelectList(_context.Genre, "Id", "Name", game.GenreId);
            ViewData["PlatformId"] = new SelectList(_context.Platform, "Id", "Name", game.Platforms.Select(p => p.Id).ToList());

            return View(game);
        }

        // GET: Games/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var game = await _context.Game
                .Include(g => g.Genre)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (game == null)
            {
                return NotFound();
            }

            return View(game);
        }

        // POST: Games/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var game = await _context.Game.FindAsync(id);
            if (game != null)
            {
                _context.Game.Remove(game);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool GameExists(int id)
        {
            return _context.Game.Any(e => e.Id == id);
        }
        // GET: Games/AddPlatform/5
        public async Task<IActionResult> AddPlatform(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var game = await _context.Game.Include(g => g.Platforms).FirstOrDefaultAsync(g => g.Id == id);
            if (game == null)
            {
                return NotFound();
            }

            // Get all platforms that are not already associated with this game
            var allPlatforms = await _context.Platform.ToListAsync();
            var availablePlatforms = allPlatforms.Where(p => !game.Platforms.Contains(p)).ToList();

            // Populate the ViewData with available platforms
            ViewData["PlatformId"] = new SelectList(availablePlatforms, "Id", "Name");

            return View(game);
        }



        // POST: Games/AddPlatform/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddPlatform(int id, int platformId)
        {
            // Find the game by id and include its associated platforms
            var game = await _context.Game.Include(g => g.Platforms).FirstOrDefaultAsync(g => g.Id == id);
            if (game == null)
            {
                return NotFound();
            }

            // Find the platform by its id
            var platform = await _context.Platform.FindAsync(platformId);
            if (platform == null)
            {
                return NotFound();
            }

            // Add the platform to the game's Platforms collection
            game.Platforms.Add(platform);

            // Save the changes to the database
            await _context.SaveChangesAsync();

            // Redirect back to the game's details page (you can also redirect to the Index if you prefer)
            return RedirectToAction(nameof(Index));
        }


        // GET: Games/RemovePlatform/5
        public async Task<IActionResult> RemovePlatform(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // Get the game including its platforms
            var game = await _context.Game.Include(g => g.Platforms).FirstOrDefaultAsync(g => g.Id == id);
            if (game == null)
            {
                return NotFound();
            }

            // Populate the list of platforms associated with the game
            ViewData["PlatformId"] = new SelectList(game.Platforms, "Id", "Name");

            return View(game);
        }


        // POST: Games/RemovePlatform/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemovePlatform(int id, int platformId)
        {
            // Find the game by id and include its associated platforms
            var game = await _context.Game.Include(g => g.Platforms).FirstOrDefaultAsync(g => g.Id == id);
            if (game == null)
            {
                return NotFound();
            }

            // Find the platform by its id
            var platform = await _context.Platform.FindAsync(platformId);
            if (platform == null)
            {
                return NotFound();
            }

            // Add the platform to the game's Platforms collection
            game.Platforms.Remove(platform);

            // Save the changes to the database
            await _context.SaveChangesAsync();

            // Redirect back to the game's details page (you can also redirect to the Index if you prefer)
            return RedirectToAction(nameof(Index));
        }

    }
}
