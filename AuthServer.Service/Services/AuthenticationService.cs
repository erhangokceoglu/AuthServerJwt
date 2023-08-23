using AuthServer.Core.Configurations;
using AuthServer.Core.DTOs;
using AuthServer.Core.Models;
using AuthServer.Core.Repositories;
using AuthServer.Core.Services;
using AuthServer.Core.UnitOfWork;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SharedLibrary.DTOs;

namespace AuthServer.Service.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly ITokenService _tokenService;
        private readonly List<Client> _clients;
        private readonly UserManager<AppUser> _userManager;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IGenericRepository<AppUserRefreshToken> _userRefreshTokenService;

        public AuthenticationService(IOptions<List<Client>> options, ITokenService tokenService, UserManager<AppUser> userManager, IUnitOfWork unitOfWork, IGenericRepository<AppUserRefreshToken> userRefreshTokenService)
        {
            _clients = options.Value;
            _tokenService = tokenService;
            _userManager = userManager;
            _unitOfWork = unitOfWork;
            _userRefreshTokenService = userRefreshTokenService;
        }

        public async Task<Response<TokenDto>> CrateTokenByRefreshTokenAsync(string refreshToken)
        {
            var existRefreshToken = await _userRefreshTokenService.Where(x => x.Code == refreshToken).SingleOrDefaultAsync();
            if (existRefreshToken == null)
            {
                return Response<TokenDto>.Fail("Refresh Token not found.", 404, true);
            }
            var currentUser = await _userManager.FindByIdAsync(existRefreshToken!.AppUserId);

            if (currentUser == null)
            {
                return Response<TokenDto>.Fail("User Id not found.", 404, true);
            }

            var tokenDto = _tokenService.CreateToken(currentUser);
            existRefreshToken.Code = tokenDto.RefreshToken!;
            existRefreshToken.Expiration = tokenDto.RefreshTokenExpiration;
            await _unitOfWork.CommitAsync();
            return Response<TokenDto>.Success(tokenDto, 200);
        }

        public async Task<Response<TokenDto>> CreateTokenAsync(LoginDto loginDto)
        {
            if (loginDto == null)
            {
                throw new ArgumentNullException(nameof(loginDto));
            }

            var currentUser = await _userManager.FindByEmailAsync(loginDto.Email);

            if (currentUser == null)
            {
                return Response<TokenDto>.Fail("Email or Password is wrong", 400, true);
            }

            if (!await _userManager.CheckPasswordAsync(currentUser, loginDto.Password))
            {
                return Response<TokenDto>.Fail("Email or Password is wrong", 400, true);
            }

            var token = _tokenService.CreateToken(currentUser);
            var userRefreshToken = await _userRefreshTokenService.Where(x => x.AppUserId == currentUser.Id).SingleOrDefaultAsync();
            if (userRefreshToken == null)
            {
                await _userRefreshTokenService.AddAsync(
                    new AppUserRefreshToken()
                    {
                        AppUserId = currentUser.Id,
                        Code = token.RefreshToken!,
                        Expiration = token.RefreshTokenExpiration
                    });

            }
            else
            {
                userRefreshToken.Code = token.RefreshToken!;
                userRefreshToken.Expiration = token.RefreshTokenExpiration;
            }

            await _unitOfWork.CommitAsync();
            return Response<TokenDto>.Success(token, 200);
        }

        public Task<Response<ClientTokenDto>> CreateTokenByClientAsync(ClientLoginDto clientLoginDto)
        {
            var client = _clients.SingleOrDefault(x => x.Id == clientLoginDto.Id && x.Secret == clientLoginDto.Secret);
            if (client == null)
            {
                return Task.FromResult(Response<ClientTokenDto>.Fail("Client Id or Secret not found.", 404, true));
            }

            var token = _tokenService.CreateTokenByClient(client);
            return Task.FromResult(Response<ClientTokenDto>.Success(token, 200));
        }

        public async Task<Response<NoDataDto>> RevokeRefreshTokenAsync(string refreshToken)
        {
            var existRefreshToken = await _userRefreshTokenService.Where(x => x.Code == refreshToken).SingleOrDefaultAsync();
            if (existRefreshToken == null)
            {
                return Response<NoDataDto>.Fail("Refresh Token not found.", 404, true);
            }
            _userRefreshTokenService.Remove(existRefreshToken);
            await _unitOfWork.CommitAsync();
            return Response<NoDataDto>.Success(200);
        }
    }
}
