using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApp.Models;

namespace WebApp.Contracts.Interfaces
{
    public interface IMovieService
    {
        Task<List<Movie>> GetAll(string searchKeyword);
        Task<Movie> StoreMovie(Movie movie);
        Task<Movie> GetDetails(int? id);
        Task<Movie> UpdateMovie(Movie movie);
        Task<Movie> DeleteConfirmed(int id);
    }
}
