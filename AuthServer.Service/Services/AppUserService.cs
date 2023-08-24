using AuthServer.Core.DTOs;
using AuthServer.Core.Models;
using AuthServer.Core.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using SharedLibrary.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthServer.Service.Services
{
    public class AppUserService : IAppUserService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AppUserService(UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<Response<AppUserDto>> CreateUserAsync(CreateUserDto createUserDto)
        {
            var appUser = new AppUser()
            {
                Email = createUserDto.Email,
                UserName = createUserDto.Username
            };

            var result = await _userManager.CreateAsync(appUser, createUserDto.Password);

            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(x => x.Description).ToList();
                return Response<AppUserDto>.Fail(new ErrorDto(errors, true), 400);
            }
            return Response<AppUserDto>.Success(ObjectMapper.Mapper.Map<AppUserDto>(appUser), 200);
        }

        public async Task<Response<NoDataDto>> CreateUserRolesAsync(string userName)
        {
            if (!await _roleManager.RoleExistsAsync("Admin"))
                await _roleManager.CreateAsync(new IdentityRole() { Name = "Admin" });
            if (!await _roleManager.RoleExistsAsync("Manager"))
                await _roleManager.CreateAsync(new IdentityRole() { Name = "Manager" });
            var appuser = await _userManager.FindByNameAsync(userName);
            await _userManager.AddToRoleAsync(appuser!, "Admin");
            await _userManager.AddToRoleAsync(appuser!, "Manager");
            return Response<NoDataDto>.Success(StatusCodes.Status204NoContent);
        }

        public async Task<Response<AppUserDto>> GetUserByNamesAsync(string userName)
        {
            var appUser = await _userManager.FindByNameAsync(userName);

            if (appUser == null)
            {
                return Response<AppUserDto>.Fail("Username not found.", 404, true);
            }
            return Response<AppUserDto>.Success(ObjectMapper.Mapper.Map<AppUserDto>(appUser), 200);
        }
    }
}
