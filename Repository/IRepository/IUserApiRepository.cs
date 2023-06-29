using ApiPeliculas.Models;
using ApiPeliculas.Models.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiPeliculas.Repository.IRepository
{
    public interface IUserApiRepository
    {
        ICollection<AppUser> GetUsers();
        AppUser GetUser(string userId);
        Task<UserApiLoginResponseDto> Login(UserApiLoginDto userApiLoginDto);
        //Task<UserApi> UserRegister(UserApiRegisterDto userApiRegisterDto);
        Task<UserDataDto> UserRegister(UserApiRegisterDto userApiRegisterDto);
        bool IsUniqueUSer(string userName);

    }
}
