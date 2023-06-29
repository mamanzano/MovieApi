using ApiPeliculas.Models;
using ApiPeliculas.Models.Dtos;
using ApiPeliculas.Repository.IRepository;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiPeliculas.Controllers
{
    [Route("api/Movies")]
    [ApiController]
    public class MoviesController : ControllerBase
    {
        private readonly IMovieRepository _mvRepo;
        private readonly IMapper _mapper;

        public MoviesController(IMovieRepository mvRepo, IMapper mapper)
        {
            _mvRepo = mvRepo;
            _mapper = mapper;
        }

        [AllowAnonymous]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult GetMovies()
        {
            var movies = _mvRepo.GetMovies();
            var moviesDto = new List<MovieDto>();

            foreach (var movie in movies)
            {
                moviesDto.Add(_mapper.Map<MovieDto>(movie));
            }
            return Ok(moviesDto);
        }

        [AllowAnonymous]
        [HttpGet("{movieId:int}", Name = "GetMovie")]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult GetCategory(int movieId)
        {
            var movie = _mvRepo.GetMovie(movieId);

            if (movie == null)
            {
                return NotFound();
            }

            var movieDto = _mapper.Map<MovieDto>(movie);

            return Ok(movieDto);
        }

        [Authorize(Roles = "admin")]
        [HttpPost]
        [ProducesResponseType(201, Type = typeof(CreateCategoryDto))]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult CreateMovie([FromBody] MovieDto movieDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (movieDto == null)
                return BadRequest(ModelState);

            if (_mvRepo.ExistMovie(movieDto.Name))
            {
                ModelState.AddModelError("", "La pelicula ya existe");
                return StatusCode(404, ModelState);
            }

            var movie = _mapper.Map<Movie>(movieDto);
            if (!_mvRepo.CreateMovie(movie))
            {
                ModelState.AddModelError("", $"Algo salio mal guardando el registro {movie.Name}");
                return StatusCode(500, ModelState);
            }

            return CreatedAtRoute("GetMovie", new { movieId = movie.Id }, movie);
        }

        [Authorize(Roles = "admin")]
        [HttpPatch("{movieId}", Name = "UpdatePatchMovie")]
        [ProducesResponseType(201, Type = typeof(MovieDto))]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult UpdatePatchCategory(int movieId, [FromBody] MovieDto movieDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (movieDto == null || movieDto.Id != movieId)
            {
                ModelState.AddModelError("", $"categoryId {movieId} y {movieDto.Id} no coinciden");
                return BadRequest(ModelState);
            }

            if (!_mvRepo.ExistMovie(movieDto.Id))
            {
                ModelState.AddModelError("", $"La categoría con el ID: {movieDto.Id}  no existe");
                return StatusCode(404, ModelState);
            }

            var movie = _mapper.Map<Movie>(movieDto);
            if (!_mvRepo.UpdateMovie(movie))
            {
                ModelState.AddModelError("", $"Algo salio mal actualizando el registro {movie.Name}");
                return StatusCode(500, ModelState);
            }

            return NoContent();
        }

        [Authorize(Roles = "admin")]
        [HttpDelete("{movieId}", Name = "DeleteMovie")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult DeleteCategory(int movieId)
        {

            if (!_mvRepo.ExistMovie(movieId))
            {
                ModelState.AddModelError("", $"La categoría con el ID: {movieId}  no existe");
                return StatusCode(404, ModelState);
            }

            var movie = _mvRepo.GetMovie(movieId);

            if (!_mvRepo.DeleteMovie(movie))
            {
                ModelState.AddModelError("", $"Algo salio mal borrando el registro {movie.Name}");
                return StatusCode(500, ModelState);
            }

            return NoContent();
        }

        [AllowAnonymous]
        [HttpGet("GetMoviesByCategoryId/{categoryId:int}")]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult GetMoviesByCategoryId(int categoryId)
        {
            var movies = _mvRepo.GetMovies(categoryId);
            var moviesDto = new List<MovieDto>();

            if(movies == null)
            {
                return NotFound();
            }

            foreach (var movie in movies)
            {
                moviesDto.Add(_mapper.Map<MovieDto>(movie));
            }
            return Ok(moviesDto);
        }

        [AllowAnonymous]
        [HttpGet("GetMoviesByCriteria")]
        public IActionResult GetMoviesByCriteria(string criteria)
        {

            try
            {
                if (!string.IsNullOrEmpty(criteria))
                {


                    var movies = _mvRepo.GetMovies(criteria.Trim());

                    if (movies.Any())
                    {
                        return Ok(movies);
                    }
                    return NotFound();
                }

                return StatusCode(StatusCodes.Status400BadRequest, "Debe mandar el criterio de busqueda");
            }
            catch(Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error recuperando datos");
            }
        }
    }
}
