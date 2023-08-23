using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace SharedLibrary.Services
{
    public static class SignService
    {
        public static SecurityKey GetSymmetricSecurityKey(string securitykey)
        {
            return new SymmetricSecurityKey(Encoding.UTF8.GetBytes(securitykey));
        }
    }
}
