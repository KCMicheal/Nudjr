using Nudjr_Domain.Enums;

namespace Nudjr_Domain.Entities
{
    public class USER : BASE_ENTITY
    {
        public Guid Id { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? OtherName { get; set; }
        public string? EmailAddress { get; set; }
        public string? PhoneNumber { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public PersonType PersonType { get; set; }
        public Gender Gender { get; set; }
        public string? Username { get; set; }
        public string? Password { get; set; }
        public string? Address { get; set; }
        public string? Nationality { get; set; }
        public required string IdentityUserId { get; set; }
        public string? ImageURL { get; set; }
    }
}
