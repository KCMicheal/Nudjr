using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Nudjr_Domain.Models.ServiceModels.Prompt
{
    public class PromptRequestModel
    {
        [JsonPropertyName("contents")]
        public List<Content> Contents { get; set; }
    }

    public class Content
    {
        [JsonPropertyName("parts")]
        public List<Part> Parts { get; set; }
    }

    public class Part
    {
        [JsonPropertyName("Text")]
        public string Text { get; set; }
    }
}
