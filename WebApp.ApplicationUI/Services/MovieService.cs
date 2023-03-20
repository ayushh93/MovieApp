using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApp.Contracts.Interfaces;
using WebApp.Entities.Models;
using WebApp.EntityFrameWork.Data;
using WebApp.Models;


namespace WebApp.ApplicationUI.Services
{
    public class MovieService : IMovieService
    {
        private readonly WebAppContext _context;


        public MovieService(WebAppContext context)
        {
            _context = context;
        }
        public async Task<List<Movie>> GetAll(string searchKeyword)
        {
            IQueryable<Movie> movies = _context.Movie;

            if (!string.IsNullOrWhiteSpace(searchKeyword))
            {
                movies = movies.Where(m =>
                    m.Title.Contains(searchKeyword) ||
                    m.Director.Contains(searchKeyword) ||
                    m.Actors.Contains(searchKeyword));
            }
            return await movies.ToListAsync();

        }

        public async Task<Movie> StoreMovie(Movie movie)
        {
            
             _context.Movie.Add(movie);
            await _context.SaveChangesAsync();
            return null;
        }

        public async Task<Movie> GetDetails(int? id)
        {
            var movie = await _context.Movie
                .FirstOrDefaultAsync(m => m.Id == id);

            return movie;
        }

        public async Task<Movie> UpdateMovie(Movie movie)
        {
            _context.Movie.Update(movie);
            await _context.SaveChangesAsync();
            return null;
        }
        public async Task<Movie> DeleteConfirmed(int id)
        {

            var movie = await _context.Movie.FindAsync(id);
            if (movie != null)
            {
                _context.Movie.Remove(movie);
               /* //delete old image from uploads
                var oldImage = movie.PosterURL;
                string wwwRootPath = _hostEnvironment.WebRootPath;
                string oldImagePath = Path.Combine(wwwRootPath, "uploads", oldImage);
                if (System.IO.File.Exists(oldImagePath))
                {
                    System.IO.File.Delete(oldImagePath);
                }*/
            }

            await _context.SaveChangesAsync();
            return null;
        }

    }
}
