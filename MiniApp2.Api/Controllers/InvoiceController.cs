using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace MiniApp2.Api.Controllers
{
    [Authorize]
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class InvoiceController : ControllerBase
    {
        [HttpGet]
        public Task<IActionResult> GetInVoices()
        {
            var username = User.Identity!.Name;
            var userIdClaim = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier);
            return Task.FromResult<IActionResult>(Ok($"Fatura İşlemleri => UserName : {username} - UserId: {userIdClaim!.Value}"));
        }
    }
}
