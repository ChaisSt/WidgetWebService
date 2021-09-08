using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace WidgetWebService
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
    public partial class WidgetsArray
    {
        [JsonProperty("widgets")]
        public Widget[] Widgets { get; set; }
    }
}
