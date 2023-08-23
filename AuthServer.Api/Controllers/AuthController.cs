using AuthServer.Core.DTOs;
using AuthServer.Core.Services;
using Microsoft.AspNetCore.Mvc;

namespace AuthServer.Api.Controllers
{
    public class AuthController : CustomBaseController
    {
        private readonly IAuthenticationService _authenticationService;

        public AuthController(IAuthenticationService authenticationService)
        {
            _authenticationService = authenticationService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateToken(LoginDto loginDto)
        {
            return ActionResultInstance(await _authenticationService.CreateTokenAsync(loginDto));
        }

        [HttpPost]
        public async Task<IActionResult> CreateTokenByClient(ClientLoginDto clientLoginDto)
        {
            return ActionResultInstance(await _authenticationService.CreateTokenByClientAsync(clientLoginDto));
        }

        [HttpPost]
        public async Task<IActionResult> RevokeToken(RefreshTokenDto refreshTokenDto)
        {
            return ActionResultInstance(await _authenticationService.RevokeRefreshTokenAsync(refreshTokenDto.RefreshToken));
        }

        [HttpPost]
        public async Task<IActionResult> CreateTokenByRefreshToken(RefreshTokenDto refreshTokenDto)
        {
            return ActionResultInstance(await _authenticationService.CrateTokenByRefreshTokenAsync(refreshTokenDto.RefreshToken));
        }
    }
}
