using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiPeliculas.Models.Dtos
{
    public class UserApiLoginResponseDto
    {
        public UserDataDto User { get; set; }
        public string Token { get; set; }
    }
}
