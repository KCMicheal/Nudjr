namespace Nudjr_Domain.Entities
{
    public class NOVUSUBSCRIBER : BASE_ENTITY
    {
        public Guid Id { get; set; }
        public string? SubscriberId { get; set; }
        public Guid UserId { get; set; }
    }
}
