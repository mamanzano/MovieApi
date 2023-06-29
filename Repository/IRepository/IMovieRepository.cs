using ApiPeliculas.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiPeliculas.Repository.IRepository
{
    public interface IMovieRepository
    {
        ICollection<Movie> GetMovies();
        Movie GetMovie(int movieId);
        bool CreateMovie(Movie movie);
        bool UpdateMovie(Movie movie);
        bool DeleteMovie(Movie movie);
        bool ExistMovie(string name);
        bool ExistMovie(int movieId);

        ICollection<Movie> GetMovies(int categoryId);
        ICollection<Movie> GetMovies(string name);
        bool Save();
    }
}
