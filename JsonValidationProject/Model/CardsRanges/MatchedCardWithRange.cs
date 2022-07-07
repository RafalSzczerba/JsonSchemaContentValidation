using System.Text.Json.Serialization;

namespace JsonValidationProject.Model.CardsRanges
{
    public class MatchedCardWithRange
    {
        [JsonPropertyName("track2")]
        public string? Track2 { get; set; }

        [JsonPropertyName("firstName")]
        public string? FirstName { get; set; }

        [JsonPropertyName("lastName")]
        public string? LastName { get; set; }       
        public string? Name { get; set; }
        [JsonPropertyName("name")]
        public string? Min { get; set; }
        public string? Max { get; set; }
        public int Matching { get; set; }
    }
}
