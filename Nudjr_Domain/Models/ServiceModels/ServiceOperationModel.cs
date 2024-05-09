namespace Nudjr_Domain.Models.ServiceModels
{
    public class ServiceOperationModel<T>
    {
        public T? Data { get; set; }
        public bool? IsSuccess { get; set; }
        public string? SuccessMessage { get; set; }
    }
}
