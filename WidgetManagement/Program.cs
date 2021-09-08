using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace WidgetManagement
{
    class Program
    {
        //Client for working with the web service
        static HttpClient client;

        static void Main(string[] args)
        {
            client = new HttpClient();
            string output = "";
            bool run = true;
            //Keep app returning to menu until user wants to close.
            while (run)
            {
                Console.Write(
                    " What would you like to do?\n" +
                    " (1) Display all widgets\n" +
                    " (2) Search widget by name\n" +
                    " (3) Search widget by category\n" +
                    " (4) Update widget\n" +
                    " (5) Add widget\n" +
                    " (6) Delete widget\n" +
                    " (0) Close\n\n "
                    );

                string input = Console.ReadLine();
                int numInput;
                switch (input)
                {
                    //Show all widgets
                    case "1":
                        output = GetAllWidgets();
                        break;

                    //Search widget by name
                    case "2":
                        Console.Write("\n Please enter a name: ");
                        Widget widget = GetWidgetByName(Console.ReadLine());
                        output = $"\n Name: {widget.Name} \n Quantity: {widget.Quantity} \n Category: {widget.Category} \n";
                        break;

                    //Search widget by category
                    case "3":
                        Console.Write("\n Please enter a category: ");
                        try { 
                            numInput = Convert.ToInt32(Console.ReadLine()); 
                            output = GetWidgetsByCategory(Convert.ToInt32(numInput));
                        }
                        catch (Exception)
                        { output = "\n Category must be an integer."; }
                        break;

                    //Update existing widget
                    case "4":
                        Console.Write("\n Please enter widget name: ");
                        output = UpdateWidget(Console.ReadLine());
                        break;

                    //Add new widget
                    case "5":
                        Console.Write("\n Please enter new widget name: ");
                        output = CreateWidget(Console.ReadLine());
                        break;

                    //Delete existing widget
                    case "6":
                        Console.Write("\n Please enter widget name: ");
                        output = DeleteWidget(Console.ReadLine());
                        break;

                    //Close
                    case "0":
                        run = false;
                        break;
                }
                Console.WriteLine(output);
            }
        }

        //Show all widgets
        public static string GetAllWidgets()
        {
            string path = "https://localhost:44314/widget";
            var response = RequestAsync(path).Result;
            var widgets = JsonConvert.DeserializeObject<List<Widget>>(response);

            string output = "";
            foreach (var widget in widgets)
                output += $"\n Name: {widget.Name} \n Quantity: {widget.Quantity} \n Category: {widget.Category} \n"; 

            return output;
        }

        //Search widget by name
        public static Widget GetWidgetByName(string name)
        {
            string path = $"https://localhost:44314/widget/name/{name}";
            var response = RequestAsync(path).Result;
            Widget widget = JsonConvert.DeserializeObject<Widget>(response);

            return widget;
        }

        //Search widget by category
        public static string GetWidgetsByCategory(int category)
        {
            string path = $"https://localhost:44314/widget/category/{category}";
            var response = RequestAsync(path).Result;
            var widgets = JsonConvert.DeserializeObject<List<Widget>>(response);
            
            string output = "";
            foreach (var widget in widgets)
                output += $"\n Name: {widget.Name} \n Quantity: {widget.Quantity} \n Category: {widget.Category} \n";

            return output;
        }

        //Update existing widget
        public static string UpdateWidget(string name)
        {
            string output, path = "";
            if (Exists(name))
            {
                //Populate widget object and display existing information
                Widget widget = GetWidgetByName(name);
                output = $" Name: {widget.Name} \n Quantity: {widget.Quantity} \n Category: {widget.Category} \n";

                Console.Write($"{output}\n Update quantity?(Y/N) ");
                if (Console.ReadLine().Equals("Y", StringComparison.OrdinalIgnoreCase))
                {
                    Console.Write(" New quantity: ");
                    try
                    { widget.Quantity = Convert.ToInt32(Console.ReadLine()); }
                    catch (Exception)
                    { output = " Quantity must be an integer."; }
                }

                Console.Write("\n Update category?(Y/N) ");
                if (Console.ReadLine().Equals("Y", StringComparison.OrdinalIgnoreCase))
                {
                    Console.Write(" New category: ");
                    try
                    { widget.Category = Convert.ToInt32(Console.ReadLine()); }
                    catch (Exception)
                    { output = " Cagegory must be an integer."; }
                }
                //Update widget records
                path = $"https://localhost:44314/widget/update/{widget}";
                var res = UpdateAsync(path, widget).Result;
                output = $"\n Widget information \n Name: {widget.Name} \n Quantity: {widget.Quantity} \n Category: {widget.Category}\n";
            }
            else 
                output = $"\n {name} is not an existing widget, please try again. No change made."; 

            return output;
        }

        //Add new widget
        public static string CreateWidget(string name)
        {
            string output = "";
            if (!Exists(name))
            {
                Console.Write(" Enter quantity: ");
                var quantity = Console.ReadLine();
                Console.Write(" Enter category: ");
                var category = Console.ReadLine();

                if (IsInt(quantity) && IsInt(category))
                {
                    //Populate a new widget object and add to widget records
                    Widget widget = new Widget { Name = name, Quantity = Convert.ToInt32(quantity), Category = Convert.ToInt32(category) };
                    string path = $"https://localhost:44314/widget/update/{widget}";
                    var res = UpdateAsync(path, widget).Result;

                    output = $"\n Widget added! \n Name: {widget.Name} \n Quantity: {widget.Quantity} \n Category: {widget.Category}\n";
                }
                else
                    output = " Category and quantity must be integers. Please try again.\n";
            }
            else
                output = " Name is already used.\n";

            return output;
        }

        //Delete existing widget
        public static string DeleteWidget(string name)
        {
            string output = "";
            if (Exists(name))
            {
                string path = $"https://localhost:44314/widget/delete/{name}";
                var res = DeleteAsync(path).Result;
                output = $" {name} was deleted!\n";
            }
            else
                output = $" {name} doesn't exist. Please try again.\n";

            return output;
        }

        //GET from web service
        public static async Task<string> RequestAsync(string path)
        {
            var response = await client.GetAsync(path);
            string data = await response.Content.ReadAsStringAsync();
            return data;
        }

        //POST to web service
        public static async Task<string> UpdateAsync(string path, Widget widget)
        {
            var json = JsonConvert.SerializeObject(widget);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await client.PostAsync(path, content);
            string data = response.ToString();
            return data;
        }

        //DELETE from web service
        public static async Task<string> DeleteAsync(string path)
        {
            var response = await client.DeleteAsync(path);
            string data = await response.Content.ReadAsStringAsync();
            return data;
        }

        //Check if name exists
        private static bool Exists(string name)
        {
            string path = $"https://localhost:44314/widget/name/{name}";
            var response = RequestAsync(path).Result;

            if (response == "")
                return false;
            else
                return true;
        }

        //Check if entry is an int
        private static bool IsInt(string number)
        {
            try
            {
                Convert.ToInt32(number);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
