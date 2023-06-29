using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ApiPeliculas.Models.Dtos
{
    public class UserDataDto
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public string Name { get; set; }
    }
}
