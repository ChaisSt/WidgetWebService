using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using System.Text;
using Newtonsoft.Json.Linq;

namespace WidgetWebService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WidgetController : ControllerBase
    {
        private readonly IWebHostEnvironment _hostingEnvironment;
        private string dataPath = "";
        Widgets widgets;

        public WidgetController(IWebHostEnvironment webHostEnvironment)
        {
            //Initialize object for accessing the json file
            _hostingEnvironment = webHostEnvironment;
            dataPath = Path.Combine(_hostingEnvironment.ContentRootPath, "widgets.json");

            //Populate a widget array for later use
            PopulateWidgetsArray();
        }

        // GET: /widget - Get all widgets
        [HttpGet]
        public List<Widget> Get()
        {
            var widgetList = widgets.WidgetsA.ToList();
            return widgetList;
        }

        // GET /widget/name/{Name} - Get widget by name
        [HttpGet("name/{name}")]
        public Widget Get(string name)
        {
            //Search widgets array for matching name(not case sensitive)
            var widget = widgets.WidgetsA.Where(w => w.Name.Equals(name, StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
            return widget;
        }

        // GET /widget/category/{Category} - Get widget by category
        [HttpGet("category/{category}")]
        public List<Widget> Get(int category)
        {
            //Search widgets array for matching category number
            var widgetList = widgets.WidgetsA.Where(w => w.Category.Equals(category)).ToList();
            return widgetList;
        }

        // POST /widget/update/{Widget} - Add new widget
        [HttpPost("update/{widget}")]
        public void Post([FromBody] Widget widget)
        {
            //Check for existing widget. If widget exists, edit. If not, add.
            Widget existing = Get(widget.Name);
            if (existing != null)
                Edit(widget);
            else
                Add(widget);
        }

        // DELETE /widget/delete/{Name}
        [HttpDelete("delete/{name}")]
        public void Delete(string name)
        {
            var json = System.IO.File.ReadAllText(dataPath);
            var jsonObj = JObject.Parse(json);
            var widgetArray = jsonObj.GetValue("widgets") as JArray;

            var deleteWidget = widgetArray.FirstOrDefault(obj => obj["Name"].Value<string>().Equals(name,StringComparison.OrdinalIgnoreCase));
            widgetArray.Remove(deleteWidget);

            string newJson = JsonConvert.SerializeObject(jsonObj, Formatting.Indented);
            System.IO.File.WriteAllText("widgets.json", newJson);
            
            //Update widgets array
            PopulateWidgetsArray();
        }

        public void Edit(Widget widget)
        {
            dynamic jObject = JsonConvert.DeserializeObject(System.IO.File.ReadAllText(dataPath));
            int i = 0;
            //Iterate through each widget in list to find position of matching widget
            foreach (var existing in widgets.WidgetsA)
            {
                if (existing.Name != widget.Name)
                    i++;
                else
                    break;
            }
            //Gets quantity and category of existing widget and updates with new values
            jObject["widgets"][i]["Quantity"] = widget.Quantity;
            jObject["widgets"][i]["Category"] = widget.Category;

            //Update json file
            string updatedJson = JsonConvert.SerializeObject(jObject, Formatting.Indented);
            System.IO.File.WriteAllText("widgets.json", updatedJson);

        }

        public void Add(Widget widget)
        {
            var json = System.IO.File.ReadAllText(dataPath);
            var jsonObj = JObject.Parse(json);
            var widgetArray = jsonObj.GetValue("widgets") as JArray;

            //Create a string for the new widget entry
            string newWidget = "{ 'Name': '" + widget.Name + "', 'Quantity': " + widget.Quantity + ", 'Category': " + widget.Category + "}";
            JObject newWidgetObj = JObject.Parse(newWidget);
            widgetArray.Add(newWidgetObj);

            jsonObj["widgets"] = widgetArray;
            string newJson = JsonConvert.SerializeObject(jsonObj, Formatting.Indented);
            System.IO.File.WriteAllText("widgets.json", newJson);
        }

        public void PopulateWidgetsArray()
        {
            //Populate widgets array with current values
            widgets = JsonConvert.DeserializeObject<Widgets>(System.IO.File.ReadAllText(dataPath));
        }
    }
}
