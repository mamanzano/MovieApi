using ApiPeliculas.Data;
using ApiPeliculas.Models;
using ApiPeliculas.Repository.IRepository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiPeliculas.Repository
{
    public class MovieRepository : IMovieRepository

    {
        private readonly AppicationDbContext _db;

        public MovieRepository(AppicationDbContext db)
        {
            _db = db;
        }
        public bool CreateMovie(Movie movie)
        {
            movie.CreationOn = DateTime.Now;
            _db.Movie.Add(movie);
            return Save();
        }

        public bool DeleteMovie(Movie movie)
        {
            _db.Movie.Remove(movie);
            return Save();
        }

        public bool ExistMovie(string name)
        {
            return _db.Movie.Any(m => m.Name.ToLower().Trim() == name.ToLower().Trim());
        }

        public bool ExistMovie(int movieId)
        {
            return _db.Movie.Any(m => m.Id == movieId);
        }

        public Movie GetMovie(int movieId)
        {
            return _db.Movie.FirstOrDefault(m => m.Id == movieId);
        }

        public ICollection<Movie> GetMovies(string name)
        {
            IQueryable<Movie> query = _db.Movie;

            query = query.Where(m => m.Name.Contains(name) || m.Description.Contains(name));

            return query.ToList();
        }

        public ICollection<Movie> GetMovies()
        {
            return _db.Movie.OrderBy(c => c.Name).ToList();
        }

        public ICollection<Movie> GetMovies(int categoryId)
        {
            return _db.Movie.Include(c => c.Category).Where(ca => ca.CategoryId == categoryId).ToList();
        }

        public bool UpdateMovie(Movie movie)
        {
            movie.CreationOn = DateTime.Now;
            _db.Movie.Update(movie);
            return Save();
        }

        public bool Save()
        {
            return _db.SaveChanges() >= 0 ? true : false; throw new NotImplementedException();
        }
    }
}
