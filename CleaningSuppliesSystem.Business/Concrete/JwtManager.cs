using CleaningSuppliesSystem.Business.Abstract;
using CleaningSuppliesSystem.Business.Configurations;
using CleaningSuppliesSystem.DTO.DTOs.LoginDtos;
using CleaningSuppliesSystem.DTO.DTOs.TokenDtos;
using CleaningSuppliesSystem.Entity.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace CleaningSuppliesSystem.Business.Concrete
{
    public class JwtManager : IJwtService
    {
        private readonly JwtTokenOptions _jwtTokenOptions;
        private readonly UserManager<AppUser> _userManager;

        public JwtManager(IOptions<JwtTokenOptions> jwtTokenOptions, UserManager<AppUser> userManager)
        {
            _jwtTokenOptions = jwtTokenOptions.Value;
            _userManager = userManager;
        }

        public async Task<LoginResponseDto> CreateTokenAsync(AppUser user)
        {
            var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtTokenOptions.Key));
            var userRoles = await _userManager.GetRolesAsync(user);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim("fullName", user.FirstName + " " + user.LastName),
                new Claim(ClaimTypes.Role, userRoles.FirstOrDefault() ?? "")
            };

            var jwtSecurityToken = new JwtSecurityToken(
                issuer: _jwtTokenOptions.Issuer,
                audience: _jwtTokenOptions.Audience,
                claims: claims,
                notBefore: DateTime.UtcNow,
                expires: DateTime.UtcNow.AddMinutes(_jwtTokenOptions.ExpireInMinutes),
                signingCredentials: new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256)
            );

            var handler = new JwtSecurityTokenHandler();
            var tokenString = handler.WriteToken(jwtSecurityToken);

            // Tek seviye token için
            var theme = "light"; // sabit tema

            return new LoginResponseDto
            {
                Token = tokenString,
                ExpireDate = jwtSecurityToken.ValidTo,
                IsActive = true,
                Message = "Giriş başarılı",
                Theme = theme
            };
        }


    }
}
