using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nudjr_Domain.Models.ServiceModels
{
    public class Jwt
    {
        public string AccessToken { get; set; }
        public DateTime Issued { get; set; }
        public DateTime Expires { get; set; }
        public string TokenIdentity { get; set; }
    }

    public class JwtWithRefreshToken : Jwt
    {
        public string? RefreshToken { get; set; }
    }
}
