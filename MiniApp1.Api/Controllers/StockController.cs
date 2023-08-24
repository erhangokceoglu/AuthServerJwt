using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace MiniApp1.Api.Controllers
{


    [Route("api/[controller]/[action]")]
    [ApiController]
    public class StockController : ControllerBase
    {
        [Authorize(Policy = "AgePolicy")]
        [Authorize(Roles = "Admin,Manager", Policy = "AnkaraPolicy")]
        [HttpGet]
        public Task<IActionResult> GetStock()
        {
            var username = User.Identity!.Name;
            var userIdClaim = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier);
            return Task.FromResult<IActionResult>(Ok($"Stok İşlemleri => UserName : {username} - UserId: {userIdClaim!.Value}"));
        }
    }
}
