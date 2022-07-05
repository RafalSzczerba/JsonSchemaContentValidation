using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JsonValidationProject.Model.Ranges
{
    public class Root
    {
        [JsonProperty("ranges")]
        public List<Range> Ranges { get; set; }
    }
}
