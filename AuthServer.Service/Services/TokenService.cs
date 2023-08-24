using AuthServer.Core.Configurations;
using AuthServer.Core.DTOs;
using AuthServer.Core.Models;
using AuthServer.Core.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SharedLibrary.Options;
using SharedLibrary.Services;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using JwtRegisteredClaimNames = System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames;

namespace AuthServer.Service.Services
{
    public class TokenService : ITokenService
    {
        private readonly CustomTokenOption _tokenOption;
        private readonly UserManager<AppUser> _userManager;

        public TokenService(IOptions<CustomTokenOption> options, UserManager<AppUser> userManager)
        {
            _tokenOption = options.Value;
            _userManager = userManager;
        }

        public TokenDto CreateToken(AppUser appUser)
        {
            var accessTokenExpiration = DateTime.Now.AddMinutes(_tokenOption.AccessTokenExpiration);
            var refreshTokenExpiration = DateTime.Now.AddMinutes(_tokenOption.RefreshTokenExpiration);
            var securityKey = SignService.GetSymmetricSecurityKey(_tokenOption.SecurityKey);
            SigningCredentials signingCredentials = new(securityKey, SecurityAlgorithms.HmacSha256Signature);
            JwtSecurityToken jwtSecurityToken = new
                (
                issuer: _tokenOption.Issuer,
                expires: accessTokenExpiration,
                notBefore: DateTime.Now,
                claims: GetClaims(appUser, _tokenOption.Audiences).Result,
                signingCredentials: signingCredentials
                );
            var handler = new JwtSecurityTokenHandler();
            var token = handler.WriteToken(jwtSecurityToken);
            var tokenDto = new TokenDto()
            {
                AccessToken = token,
                RefreshToken = CreateRefreshToken(),
                AccessTokenExpiration = accessTokenExpiration,
                RefreshTokenExpiration = refreshTokenExpiration
            };
            return tokenDto;
        }

        public ClientTokenDto CreateTokenByClient(Client client)
        {
            var accessTokenExpiration = DateTime.Now.AddMinutes(_tokenOption.AccessTokenExpiration);
            var securityKey = SignService.GetSymmetricSecurityKey(_tokenOption.SecurityKey);
            SigningCredentials signingCredentials = new(securityKey, SecurityAlgorithms.HmacSha256Signature);
            JwtSecurityToken jwtSecurityToken = new
                (
                issuer: _tokenOption.Issuer,
                expires: accessTokenExpiration,
                notBefore: DateTime.Now,
                claims: GetClaimsByClient(client),
                signingCredentials: signingCredentials
                );
            var handler = new JwtSecurityTokenHandler();
            var token = handler.WriteToken(jwtSecurityToken);
            var clientTokenDto = new ClientTokenDto()
            {
                AccessToken = token,
                AccessTokenExpiration = accessTokenExpiration
            };
            return clientTokenDto;
        }

        private static string CreateRefreshToken()
        {
            var numberByte = new byte[32];
            using var random = RandomNumberGenerator.Create();
            random.GetBytes(numberByte);
            return Convert.ToBase64String(numberByte);
        }

        private async Task<IEnumerable<Claim>> GetClaims(AppUser appUser, List<string> audiences)
        {
            var userList = new List<Claim>()
            {
                new Claim(ClaimTypes.NameIdentifier,appUser.Id),
                new Claim(JwtRegisteredClaimNames.Email,appUser.Email!),
                new Claim(ClaimTypes.Name,appUser.UserName!),
                new Claim(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString()),
                new Claim("City",appUser!.City!),
                new Claim("BirthDate",appUser.BirthDate!.Value.ToShortDateString())
            };
            var userRoles = await _userManager.GetRolesAsync(appUser);

            userList.AddRange(audiences.Select(x => new Claim(JwtRegisteredClaimNames.Aud, x)));
            userList.AddRange(userRoles.Select(x => new Claim(ClaimTypes.Role, x)));
            return userList;
        }

        private static IEnumerable<Claim> GetClaimsByClient(Client client)
        {
            var claims = new List<Claim>()
            {
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Sub, client.Id!.ToString())
            };

            claims.AddRange(client.Audiences!.Select(x => new Claim(JwtRegisteredClaimNames.Aud, x)));
            return claims;
        }
    }
}
