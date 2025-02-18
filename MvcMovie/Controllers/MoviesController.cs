using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MvcMovie.Data;
using MvcMovie.Models;

namespace MvcMovie.Controllers
{
    public class MoviesController(MvcMovieContext c) : Controller
    {
        private readonly MvcMovieContext db = c;

        // GET: Movies
        public async Task<IActionResult> Index(string movieGenre, string searchString)
        {
            if (db.Movie == null) return Problem("Entity set 'MvcMovieContext.Movie'  is null.");

            // Use LINQ to get list of genres.
            IQueryable<string> genreQuery = from m in db.Movie orderby m.Genre select m.Genre;
            var movies = from m in db.Movie select m;

            if (!string.IsNullOrEmpty(searchString)) movies = movies.Where(s => s.Title!.ToUpper().Contains(searchString.ToUpper()));

            if (!string.IsNullOrEmpty(movieGenre)) movies = movies.Where(x => x.Genre == movieGenre);

            var movieGenreVM = new MovieGenreViewModel
            {
                Genres = new SelectList(await genreQuery.Distinct().ToListAsync()),
                Movies = await movies.ToListAsync()
            };

            return View(movieGenreVM);
        }

        [HttpPost]
        public string Index(string searchString, bool notUsed) => "From [HttpPost]Index: filter on " + searchString;
        

        // GET: Movies/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var movie = await db.Movie
                .FindAsync(id);
            return movie == null ? NotFound() : View(movie);
        }

        // GET: Movies/Create
        public IActionResult Create() => View();

        // POST: Movies/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost] [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,ReleaseDate,Genre,Price,Rating")] Movie movie)
        {
            if ( ! ModelState.IsValid) return View(movie);
            db.Add(movie);
            await db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));

        }

        // GET: Movies/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var movie = await db.Movie.FindAsync(id);
            return movie == null ? NotFound() : View(movie);
        }

        // POST: Movies/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost] [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,ReleaseDate,Genre,Price,Rating")] Movie movie)
        {
            if (id != movie.Id) return NotFound();

            if ( ! ModelState.IsValid) return View(movie);
            db.Update(movie);
            await db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: Movies/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var movie = await db.Movie.FindAsync(id);
            return movie == null ? NotFound() : View(movie);
        }

        // POST: Movies/Delete/5
        [HttpPost, ActionName("Delete")] [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var movie = await db.Movie.FindAsync(id);
            if (movie != null) db.Movie.Remove(movie);
            await db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
