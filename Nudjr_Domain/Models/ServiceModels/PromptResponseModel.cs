﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nudjr_Domain.Models.ServiceModels
{
    public class PromptResponseModel
    {
        [JsonProperty("candidates")]
        public List<Candidate> Candidates { get; set; }
    }

    public class Candidate
    {
        [JsonProperty("content")]
        public Content Content { get; set; }

        [JsonProperty("finishReason")]
        public string FinishReason { get; set; }

        [JsonProperty("index")]
        public int Index { get; set; }

        [JsonProperty("safetyRatings")]
        public List<SafetyRating> SafetyRatings { get; set; }
    }

    public class Content
    {
        [JsonProperty("parts")]
        public List<Part> Parts { get; set; }

        [JsonProperty("role")]
        public string Role { get; set; }
    }

    public class Part
    {
        [JsonProperty("text")]
        public string Text { get; set; }
    }

    public class SafetyRating
    {
        [JsonProperty("category")]
        public string Category { get; set; }

        [JsonProperty("probability")]
        public string Probability { get; set; }
    }
}
