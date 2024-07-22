﻿using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using MongoDB_Empty_Pattern.Models;
using MongoDB_Empty_Pattern.Models.Dtos;
using MongoDB_Empty_Pattern.MongoRepository.GenericRepository;
using MongoDB_Empty_Pattern.Services;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Text;

namespace MongoDB_Empty_Pattern.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors("_myAllowSpecificOrigins")]
    public class UsersController : ControllerBase
    {
        private readonly IMongoRepository<Users> _UsersCollection;
        private readonly IConfiguration _configuration;
        private readonly IUserService _userService;
        public UsersController(IMongoRepository<Users> dbRepository, IConfiguration configuration, IUserService userRepository) 
        {
            _UsersCollection = dbRepository;
            _configuration = configuration;
            _userService = userRepository;
        }

        [HttpPost("Login")]
        public async Task<ActionResult> Get(LoginDto login)
        {
            try
            {
                // Подготовьте лямбда-выражение для фильтрации
                Expression<Func<Users, bool>> filterExpression = u => u.username == login.username;
                // Вызовите метод FindOne с этим фильтром
                var user = await _UsersCollection.FindOne(filterExpression);
                // Если пользователь не найден, верните сообщение об ошибке
                if (user == null)
                {
                    return NotFound("User not found");
                }
                // Проверяем пароль
                if (!_userService.VerifyPasswordHash(login.password, user.PasswordHash, user.PasswordSalt))
                {
                    return Unauthorized("Invalid password");
                }

                var claims = new List<Claim>
                {
                      new Claim("UserId", user.Id.ToString()),
                      new Claim(ClaimTypes.Name.ToString() , user.Id)
                };

                var refreshclaim = new List<Claim>
                {
                      new Claim("Username", user.username.ToString())
                };



                var encodedJwt = GenerateAccessToken(claims);
                var refreshJwt = GenerateRefreshToken(refreshclaim);
                var response = new
                {
                    encodedJwt = encodedJwt,
                    refreshJwt = refreshJwt
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPost("Registration")]
        public async Task<ActionResult> Post(LoginDto obj)
        {
            try
            {
                // Подготовьте лямбда-выражение для фильтрации
                Expression<Func<Users, bool>> filterExpression = u => u.username == obj.username;
                // Вызовите метод FindOne с этим фильтром
                var user = await _UsersCollection.FindOne(filterExpression);
                if (user != null)
                {
                    return BadRequest("This username already exist, please choose another one");
                }
                
                _userService.CreatePasswordHash(obj.password, out byte[] passwordHash, out byte[] passwordSalt);

                var users = new Users
                {
                    Id = "",
                    username = obj.username,
                    PasswordHash = passwordHash,
                    PasswordSalt = passwordSalt,
                
                };
                _UsersCollection.InsertOne(users);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        
        private string GenerateAccessToken(List<Claim> claims)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtSettings:Key"]));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expires = DateTime.UtcNow.Add(TimeSpan.FromHours(1)); // Устанавливаем срок действия refresh token

            var token = new JwtSecurityToken(
                issuer: _configuration["JwtSettings:Issuer"],
                audience: _configuration["JwtSettings:Audience"],
                claims: claims,
                expires: expires,
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private string GenerateRefreshToken(List<Claim> claims)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtSettings:Key"]));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expires = DateTime.UtcNow.Add(TimeSpan.FromDays(30)); // Устанавливаем срок действия refresh token

            var token = new JwtSecurityToken(
                issuer: _configuration["JwtSettings:Issuer"],
                audience: _configuration["JwtSettings:Audience"],
                claims: claims,
                expires: expires,
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
