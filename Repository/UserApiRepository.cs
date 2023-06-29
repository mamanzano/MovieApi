using ApiPeliculas.Data;
using ApiPeliculas.Models;
using ApiPeliculas.Models.Dtos;
using ApiPeliculas.Repository.IRepository;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using XSystem.Security.Cryptography;

namespace ApiPeliculas.Repository
{
    public class UserApiRepository : IUserApiRepository
    {
        private readonly AppicationDbContext _db;
        private string keySecret;
        private readonly UserManager<AppUser> _userManeger;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IMapper _mapper;

        public UserApiRepository(AppicationDbContext db, IConfiguration config,
            UserManager<AppUser> userManeger, RoleManager<IdentityRole> roleManager, IMapper mapper)
        {
            _db = db;
            keySecret = config.GetValue<string>("ApiSettings:Secret");
            _userManeger = userManeger;
            _roleManager = roleManager;
            _mapper = mapper;
        }
        public AppUser GetUser(string userId)
        {
            return _db.AppUser.FirstOrDefault(u => u.Id == userId);
        }

        public ICollection<AppUser> GetUsers()
        {
            return _db.AppUser.OrderBy(u => u.Name).ToList();
        }

        public bool IsUniqueUSer(string userName)
        {
            var userDb = _db.AppUser.FirstOrDefault(u => u.UserName == userName);
            if (userDb == null)
                return true;
            else
                return false; 
        }

        public async Task<UserApiLoginResponseDto> Login(UserApiLoginDto userApiLoginDto)
        {
            /*var passwordEncrypt = getMd5(userApiLoginDto.Password);

            var user = _db.UserApi.FirstOrDefault(
                    u => u.UserName == userApiLoginDto.UserName
                    && u.Password == passwordEncrypt
                );*/

            var user = _db.AppUser.FirstOrDefault(
                   u => u.UserName == userApiLoginDto.UserName
               );

            bool isValid = await _userManeger.CheckPasswordAsync(user, userApiLoginDto.Password);

            if (user == null || isValid == false)
            {
                return new UserApiLoginResponseDto()
                {
                    Token = "",
                    User = null
                };
            }

            var roles = await _userManeger.GetRolesAsync(user);

            var manageToken = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(keySecret);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, user.UserName.ToString()),
                    new Claim(ClaimTypes.Role, roles.FirstOrDefault())
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new (new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = manageToken.CreateToken(tokenDescriptor);

            UserApiLoginResponseDto userApiLoginResponseDto = new UserApiLoginResponseDto()
            {
                Token = manageToken.WriteToken(token),
                User = _mapper.Map<UserDataDto>(user)
            };
            return userApiLoginResponseDto;
        }

        public async Task<UserDataDto> UserRegister(UserApiRegisterDto userApiRegisterDto)
        {
            //var passwordEncrypt = getMd5(userApiRegisterDto.Password);

            AppUser user = new()
            {
                Name = userApiRegisterDto.Name,
                UserName = userApiRegisterDto.UserName,
                Email = userApiRegisterDto.UserName,
                NormalizedEmail = userApiRegisterDto.UserName.ToUpper(),
                //Password = passwordEncrypt,
                //Role = userApiRegisterDto.Role
            };

            //_db.UserApi.Add(user);
            //await _db.SaveChangesAsync();
            //user.Password = passwordEncrypt;
            //return user;

            var result = await _userManeger.CreateAsync(user, userApiRegisterDto.Password);
            if(result.Succeeded)
            {
                if (!_roleManager.RoleExistsAsync("admin").GetAwaiter().GetResult())
                {
                    await _roleManager.CreateAsync(new IdentityRole("admin"));
                    await _roleManager.CreateAsync(new IdentityRole("registrado"));
                    await _userManeger.AddToRoleAsync(user, "admin");
                }
                else
                    await _userManeger.AddToRoleAsync(user, "registrado");

                var userResult = _db.AppUser.FirstOrDefault(u => u.UserName == userApiRegisterDto.UserName);

                return _mapper.Map<UserDataDto>(userResult);
            }

            return new UserDataDto();

        }

        public static string getMd5(string value)
        {
            MD5CryptoServiceProvider x = new MD5CryptoServiceProvider();
            byte[] data = System.Text.Encoding.UTF8.GetBytes(value);
            data = x.ComputeHash(data);
            string response = "";

            for (int i = 0; i < data.Length; i++)
                response += data[i].ToString("x2").ToLower();

            return response;
        }
    }
}
