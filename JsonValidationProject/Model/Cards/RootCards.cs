using System.Text.Json.Serialization;

namespace JsonValidationProject.Model.Cards
{
    public class RootCards
    {
        [JsonPropertyName("cards")]
        public List<Card> Cards { get; set; }
        public RootCards()
        {
            Cards = new List<Card>();
        }
    }
}
