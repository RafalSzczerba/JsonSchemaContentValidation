using System.Text.Json.Serialization;

namespace JsonValidationProject.Model.Cards
{
    public class Address
    {
        [JsonPropertyName("city")]
        public string? City { get; set; }

        [JsonPropertyName("zipCode")]
        public string? ZipCode { get; set; }
    }

}
