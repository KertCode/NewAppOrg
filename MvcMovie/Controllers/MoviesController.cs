using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using MvcMovie.Data;
using MvcMovieISA2.Domain;
using MvcMovieISA2.Infra;
using MvcMovieISA2.MvcMovie.Models;

namespace MvcMovie.Controllers
{
    public class MoviesController(MvcMovieContext c) : Controller
    {
        private readonly MvcMovieContext db = c;
        private readonly Repo<Movie> repo = new(c);

        public async Task<IActionResult> Index(string movieGenre, string searchString)
        {
            if (db.Movie == null) return Problem("Entity set 'MvcMovieContext.Movie'  is null.");

            IQueryable<string> genreQuery = from m in db.Movie orderby m.Genre select m.Genre;
            var movies = from m in db.Movie select m;

            if (!string.IsNullOrEmpty(searchString)) movies = movies.Where(s => s.Title!.ToUpper().Contains(searchString.ToUpper()));

            if (!string.IsNullOrEmpty(movieGenre)) movies = movies.Where(x => x.Genre == movieGenre);

            var movieGenreVM = new MovieGenreViewModel
            {
                Genres = new SelectList(await repo.Get()),
                Movies = await repo.Get()
            };

            return View(movieGenreVM);
        }

        [HttpPost]
        public string Index(string searchString, bool notUsed) => "From [HttpPost]Index: filter on " + searchString;

        public async Task<IActionResult> Details(int? id)
        {
            var movie = await repo.Get(id);
            return movie == null ? NotFound() : View(movie);
        }
        public IActionResult Create() => View();

        [HttpPost] [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,ReleaseDate,Genre,Price,Rating")] Movie movie)
        {
            if ( ! ModelState.IsValid) return View(movie);
            await repo.Add(movie);
            return RedirectToAction(nameof(Index));

        }
        public async Task<IActionResult> Edit(int? id)
        {
            var movie = await repo.Get(id);
            return movie == null ? NotFound() : View(movie);
        }
        [HttpPost] [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,ReleaseDate,Genre,Price,Rating")] Movie movie)
        {
            if (id != movie.Id) return NotFound();
            if ( ! ModelState.IsValid) return View(movie);
            await repo.Update(movie);
            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> Delete(int? id)
        {
            var movie = await repo.Get(id);
            return movie == null ? NotFound() : View(movie);
        }
        [HttpPost, ActionName("Delete")] [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await repo.Delete(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
