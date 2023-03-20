using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using WebApp.Contracts.Interfaces;
using WebApp.Entities.Models;
using WebApp.EntityFrameWork.Data;
using WebApp.Models;

namespace WebApp.Controllers
{
    public class MoviesController : Controller
    {
        private readonly WebAppContext _context;
        private readonly IMovieService _movieService;
        private readonly IWebHostEnvironment _hostEnvironment;
        private readonly ILogger<MoviesController> _logger;

        public MoviesController(WebAppContext context, IMovieService movieService, IWebHostEnvironment hostEnvironment, ILogger<MoviesController> logger)
        {
            _context = context;
            _movieService = movieService;
            _hostEnvironment = hostEnvironment;
            _hostEnvironment = hostEnvironment;
            _logger = logger;
        }

        // GET: Movies
        public async Task<IActionResult> Index(string searchKeyword = null)
        {
            /*return _context.Movie != null ? 
                        View(await _context.Movie.ToListAsync()) :
                        Problem("Entity set 'WebAppContext.Movie'  is null.");*/

            /*return View(await _movieService.GetAll());*/

            return View(await _movieService.GetAll(searchKeyword));

        }
        public async Task<IActionResult> Index2(string searchKeyword = null)
        {
            /*return _context.Movie != null ? 
                        View(await _context.Movie.ToListAsync()) :
                        Problem("Entity set 'WebAppContext.Movie'  is null.");*/

            /*return View(await _movieService.GetAll());*/

            return View(await _movieService.GetAll(searchKeyword));

        }

        public async Task<IActionResult> Search(string keyword)
        {
            var movies = await _context.Movie
         .Where(m => m.Title.Contains(keyword) || m.Director.Contains(keyword) || m.Actors.Contains(keyword))
         .ToListAsync();

            return View("index");
        }

        // GET: Movies/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            /*if (id == null || _context.Movie == null)
            {
                return NotFound();
            }

            var movie = await _context.Movie
                .FirstOrDefaultAsync(m => m.Id == id);
            if (movie == null)
            {
                return NotFound();
            }

            return View(movie);*/

            return View(await _movieService.GetDetails(id));
        }

        // GET: Movies/Create
        public IActionResult Create()
        {
            /*Movie movie = new();
            movie.Genres = PopulateGenres();*/
            return View();
        }

        // POST: Movies/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,Genre,ReleaseDate,Director,Actors,Rating,Runtime,Plot,PosterFile")] Movie movie)
        {
            if (ModelState.IsValid)
            {
                //Save image to wwwroot/uploads
                string wwwRootPath = _hostEnvironment.WebRootPath;
                string fileName = Path.GetFileNameWithoutExtension(movie.PosterFile.FileName);
                string extension = Path.GetExtension(movie.PosterFile.FileName);
                movie.PosterURL = fileName = fileName + DateTime.Now.ToString("yymmssfff") + extension;
                string path = Path.Combine(wwwRootPath + "/uploads/", fileName);
                if (movie.PosterFile!= null)
                {
                    using (var fileStream = new FileStream(path, FileMode.Create))
                    {
                        try
                        {
                            await movie.PosterFile.CopyToAsync(fileStream);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "Error uploading poster file");
                            throw;
                        }
                    } 
                }
                //insert record
                await _movieService.StoreMovie(movie);
                return RedirectToAction(nameof(Index));
            }
            return View(movie);
        }

        // GET: Movies/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            /*if (id == null || _context.Movie == null)
            {
                return NotFound();
            }

            var movie = await _context.Movie.FindAsync(id);
            if (movie == null)
            {
                return NotFound();
            }
            return View(movie);*/
            var movie = await _movieService.GetDetails(id);
            return View(movie);

        }

        // POST: Movies/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Genre,ReleaseDate,Director,Actors,Rating,Runtime,Plot,PosterFile")] Movie movie)
        {
            if (id != movie.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    /*_context.Update(movie);
                    await _context.SaveChangesAsync();*/

                    //Save image to wwwroot/uploads if there is image input
                    if (movie.PosterFile == null)
                    {
                        movie.PosterURL = _context.Movie.AsNoTracking().FirstOrDefault(m => m.Id == movie.Id)?.PosterURL;
                    }
                    else
                    {
                        //delete old image from uploads
                        var oldImage = _context.Movie.AsNoTracking().FirstOrDefault(m => m.Id == movie.Id)?.PosterURL;
                        string wwwRootPath = _hostEnvironment.WebRootPath;
                        string oldImagePath = Path.Combine(wwwRootPath, "uploads", oldImage);
                        if (System.IO.File.Exists(oldImagePath))
                        {
                            System.IO.File.Delete(oldImagePath);
                        }
                        //upload new image
                        string fileName = Path.GetFileNameWithoutExtension(movie.PosterFile.FileName);
                        string extension = Path.GetExtension(movie.PosterFile.FileName);
                        movie.PosterURL = fileName = fileName + DateTime.Now.ToString("yymmssfff") + extension;
                        string path = Path.Combine(wwwRootPath + "/uploads/", fileName);
                        using (var fileStream = new FileStream(path, FileMode.Create))
                        {
                            await movie.PosterFile.CopyToAsync(fileStream);
                        }
                    }
                    await _movieService.UpdateMovie(movie);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MovieExists(movie.Id))
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
            return View(movie);
        }

        // GET: Movies/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _movieService == null)
            {
                return NotFound();
            }

            var movie = await _movieService.GetDetails(id);

            if (movie == null)
            {
                return NotFound();
            }

            return View(movie);
        }

        // POST: Movies/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            /*if (_context.Movie == null)
            {
                return Problem("Entity set 'WebAppContext.Movie'  is null.");
            }
            var movie = await _context.Movie.FindAsync(id);
            if (movie != null)
            {
                _context.Movie.Remove(movie);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));*/

            await _movieService.DeleteConfirmed(id);

            var movie = await _context.Movie.FindAsync(id);
            if (movie != null)
            {
                //delete  image from uploads
                var Image = movie.PosterURL;
                string wwwRootPath = _hostEnvironment.WebRootPath;
                string oldImagePath = Path.Combine(wwwRootPath, "uploads", Image);
                if (System.IO.File.Exists(oldImagePath))
                {
                    System.IO.File.Delete(oldImagePath);
                }
            }
            return RedirectToAction(nameof(Index));
        }

        private bool MovieExists(int id)
        {
          return (_context.Movie?.Any(e => e.Id == id)).GetValueOrDefault();
        }

        private IEnumerable<SelectListItem> PopulateGenres()
        {
            var genres = from Genre g in Enum.GetValues(typeof(Genre))
                         select new { Id = g, Name = g.ToString() };

            return new SelectList(genres, "Id", "Name");
        }
    }
}
