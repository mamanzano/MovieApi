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
    [ApiController]
    [Route("api/[Controller]")]
    //[Route("api/[categories]")]
    public class CategoriesController : ControllerBase
    {
        private readonly ICategoryRepository _ctRepo;
        private readonly IMapper _mapper;

        public CategoriesController(ICategoryRepository ctRepo, IMapper mapper)
        {
            _ctRepo = ctRepo;
            _mapper = mapper;
        }

        [AllowAnonymous]
        [HttpGet]
        //[ResponseCache(Duration = 20)]
        [ResponseCache(CacheProfileName = "DefaultTwentySeconds")]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult GetCategories()
        {
            var categories = _ctRepo.GetCategories();
            var categoriesDto = new List<CategoryDto>();

            foreach (var category in categories)
            {
                categoriesDto.Add(_mapper.Map<CategoryDto>(category));
            }
            return Ok(categoriesDto);
        }

        [AllowAnonymous]
        [HttpGet("{categoryId:int}", Name = "GetCategory")]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult GetCategory(int categoryId)
        {
            var category = _ctRepo.GetCategory(categoryId);

            if(category == null)
            {
                return NotFound();
            }

            var categoryDto = _mapper.Map<CategoryDto>(category);

            return Ok(categoryDto);
        }

        [Authorize(Roles = "admin")]
        [HttpPost]
        [ProducesResponseType(201, Type = typeof(CreateCategoryDto))]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult CreateCategory([FromBody] CreateCategoryDto categoryDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if(categoryDto == null)
                return BadRequest(ModelState);

            if (_ctRepo.ExistsCategory(categoryDto.Name))
            {
                ModelState.AddModelError("", "La categoría ya existe");
                return StatusCode(404, ModelState);
            }

            var category = _mapper.Map<Category>(categoryDto);
            if (!_ctRepo.CreateCategory(category))
            {
                ModelState.AddModelError("", $"Algo salio mal guardando el registro {category.Name}");
                return StatusCode(500, ModelState);
            }

            return CreatedAtRoute("GetCategory", new { categoryId = category.Id }, category);
        }

        [Authorize(Roles = "admin")]
        [HttpPatch("{categoryId}", Name = "UpdatePatchCategory")]
        [ProducesResponseType(201, Type = typeof(CategoryDto))]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult UpdatePatchCategory(int categoryId,[FromBody] CategoryDto categoryDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (categoryDto == null || categoryDto.Id != categoryId)
            {
                ModelState.AddModelError("", $"categoryId {categoryId} y {categoryDto.Id} no coinciden");
                return BadRequest(ModelState);
            }

            if (!_ctRepo.ExistsCategory(categoryDto.Id))
            {
                ModelState.AddModelError("", $"La categoría con el ID: {categoryDto.Id}  no existe");
                return StatusCode(404, ModelState);
            }

            var category = _mapper.Map<Category>(categoryDto);
            if (!_ctRepo.UpdateCategory(category))
            {
                ModelState.AddModelError("", $"Algo salio mal actualizando el registro {category.Name}");
                return StatusCode(500, ModelState);
            }

            return NoContent();
        }

        [Authorize(Roles = "admin")]
        [HttpDelete("{categoryId}", Name = "DeleteCategory")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult DeleteCategory(int categoryId)
        {

            if (!_ctRepo.ExistsCategory(categoryId))
            {
                ModelState.AddModelError("", $"La categoría con el ID: {categoryId}  no existe");
                return StatusCode(404, ModelState);
            }

            var category = _ctRepo.GetCategory(categoryId);

            if (!_ctRepo.DeteteCategory(category))
            {
                ModelState.AddModelError("", $"Algo salio mal borrando el registro {category.Name}");
                return StatusCode(500, ModelState);
            }

            return NoContent();
        }
    }
}
