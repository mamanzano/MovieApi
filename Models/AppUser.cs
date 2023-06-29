using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiPeliculas.Models
{
    public class AppUser : IdentityUser
    {
        //Añadir campos personalizados
        public string Name { get; set; }
    }
}
