using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthServer.Core.Models
{
    public class AppUserRefreshToken
    {
        public string AppUserId { get; set; } = null!;
        public string Code { get; set; } = null!;
        public DateTime Expiration { get; set; }
    }
}
