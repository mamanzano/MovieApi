using ApiPeliculas.Data;
using ApiPeliculas.Models;
using ApiPeliculas.Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiPeliculas.Repository
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly AppicationDbContext _db;

        public CategoryRepository(AppicationDbContext db)
        {
            _db = db;
        }
        public bool CreateCategory(Category category)
        {
            category.CreateOn = DateTime.Now;
            _db.Category.Add(category);
            return Guardar();
        }

        public bool DeteteCategory(Category category)
        {
            _db.Category.Remove(category);
            return Guardar();
        }

        public bool ExistsCategory(string name)
        {
            return _db.Category.Any(c => c.Name.ToLower().Trim() == name.ToLower().Trim());
        }

        public bool ExistsCategory(int id)
        {
            return _db.Category.Any(c => c.Id == id);
        }

        public ICollection<Category> GetCategories()
        {
            return _db.Category.OrderBy(c => c.Name).ToList();
        }

        public Category GetCategory(int categoryId)
        {
            return _db.Category.FirstOrDefault(c => c.Id == categoryId);
        }

        public bool Guardar()
        {
            return _db.SaveChanges() >= 0 ? true : false;
        }

        public bool UpdateCategory(Category category)
        {
            category.CreateOn = DateTime.Now;
            _db.Category.Update(category);
            return Guardar();
        }
    }
}
