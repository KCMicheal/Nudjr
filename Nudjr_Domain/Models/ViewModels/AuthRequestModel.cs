using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nudjr_Domain.Models.ViewModels
{
    public class AuthRequestModel
    {
    }

    public class LoginRequest
    {
        public LoginRequest()
        {
        }
        [Required]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        public string LoggedInAs { get; set; } = "Member";


    }

    public class RenewJWTTokenModel
    {
        public string? token { get; set; }
        public string? refreshToken { get; set; }
    }
}
