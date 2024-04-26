namespace Nudjr_Domain.Models.ServiceModels.NovuModels
{
    public class SubscriberDTO
    {

    }

    public class UpdateSubscriberDTO
    {
        public string? Email { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }

    }

    public record NovuOperationModel
    {
        public required bool Success { get; set; }
        public object? Result { get; set; }
    }
}
