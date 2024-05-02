namespace Nudjr_Domain.Entities
{
    public class NUDGE : BASE_ENTITY
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string? Content { get; set; }
        public string? Tone { get; set; }
    }
}
