using ApiPeliculas.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiPeliculas.Repository.IRepository
{
    public interface ICategoryRepository
    {
        ICollection<Category> GetCategories();
        Category GetCategory(int categoryId);
        bool ExistsCategory(string name);
        bool ExistsCategory(int id);
        bool CreateCategory(Category category);
        bool UpdateCategory(Category category);
        bool DeteteCategory(Category category);
        bool Guardar();

    }
}
