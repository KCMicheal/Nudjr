using Nudjr_Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace Nudjr_Domain.Models.ViewModels
{
    public class AdminSignUpDto
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? OtherName { get; set; }
        [Required]
        public required string EmailAddress { get; set; }
        public string? PhoneNumber { get; set; }
        public PersonType PersonType { get; set; }
        public Gender Gender { get; set; }
        [Required]
        public required string Username { get; set; }
        [Required]
        public required string Password { get; set; }
        public string? Nationality { get; set; }
    }

    public class AdminSignInDto
    {
        public required string UserNameOrEmailAddress { get; set; }
        public required string Password { get; set; }
    }
}
