using Newtonsoft.Json;

namespace Nudjr_Domain.Models.ServiceModels.NovuModels
{
    public record WelcomeMessage
    {
        [JsonProperty("firstname")]
        public required string FirstName { get; set; }
        [JsonProperty("lastname")]
        public required string LastName { get; set; }
    }
}
