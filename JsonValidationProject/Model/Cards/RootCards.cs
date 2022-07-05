using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace JsonValidationProject.Model.Cards
{
    public class RootCards
    {
        [JsonPropertyName("cards")]
        public List<Card> Cards { get; set; }
    }
}
