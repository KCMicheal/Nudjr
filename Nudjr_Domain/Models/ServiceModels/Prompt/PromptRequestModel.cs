using System.Text.Json.Serialization;

namespace Nudjr_Domain.Models.ServiceModels.Prompt
{
    public class PromptRequestModel
    {
        [JsonPropertyName("contents")]
        public List<Content> contents { get; set; }
    }

    public class Content
    {
        [JsonPropertyName("parts")]
        public List<Part> parts { get; set; }
    }

    public class Part
    {
        [JsonPropertyName("text")]
        public string text { get; set; }
    }
}
