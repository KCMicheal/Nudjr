using Nudjr_Domain.Enums;

namespace Nudjr_Domain.Entities
{
    public class USER : BASE_ENTITY
    {
        public Guid Id { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? EmailAddress { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public PersonType PersonType { get; set; }
        public Gender Gender { get; set; }
        public NumberOfNudges NumberOfNudges { get; set; } = NumberOfNudges.Three;
        public PersonalityType PersonalityType { get; set; }
        public NotificationPreference NotificationPreference { get; set; }
        public string? Username { get; set; }
        public string? Password { get; set; }
        public string? Nationality { get; set; }
        public required string IdentityUserId { get; set; }
        public string? ImageURL { get; set; }

        public virtual USER_SETTING? UserSettings { get; set; }
        public virtual ICollection<ALARM>? Alarms { get; set; }
        public virtual ICollection<EVENT>? Events { get; set; }
        public virtual ICollection<USER_ACTIVITY_LOG>? UserActivities { get; set; }
    }
}
