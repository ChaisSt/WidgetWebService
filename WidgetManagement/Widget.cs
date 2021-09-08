using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WidgetManagement
{
    public partial class Widget
    {
        [JsonProperty("Name")]
        public string Name { get; set; }

        [JsonProperty("Quantity")]
        public int Quantity { get; set; }

        [JsonProperty("Category")]
        public int Category { get; set; }
    }
}
