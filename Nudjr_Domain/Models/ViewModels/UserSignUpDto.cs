using Nudjr_Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nudjr_Domain.Models.ViewModels
{
    public class UserSignUpDto
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? OtherName { get; set; }
        [Required]
        public required string EmailAddress { get; set; }
        public string? PhoneNumber { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public PersonType PersonType { get; set; }
        public Gender Gender { get; set; }
        [Required]
        public required string Username { get; set; }
        [Required]
        public required string Password { get; set; }
        public string? Address { get; set; }
        public string? Nationality { get; set; }
    }


    public class UserSignInDto
    {
        [Required]
        public required string UserNameOrEmail { get; set; }
        [Required]
        public required string Password { get; set; }
        [Required]
        public required PersonType PersonType { get; set; }
    }
}
