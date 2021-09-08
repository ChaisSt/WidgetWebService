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

            while (run)
            {
                Console.WriteLine(
                    "\n \nWhat would you like to do?\n" +
                    "(1) Display all widgets\n" +
                    "(2) Search widget by name\n" +
                    "(3) Search widget by category\n" +
                    "(4) Update widget\n" +
                    "(5) Add widget\n" +
                    "(6) Delete widget\n" +
                    "(0) Close\n\n"
                    );

                string input = Console.ReadLine();

                switch (input)
                {
                    //Show all widgets
                    case "1":
                        var response = GetAllWidgets();
                        Console.WriteLine(response);
                        output = "";
                        break;
                    //Search widget by name
                    case "2":
                        Console.WriteLine("Please enter a name: ");
                        input = Console.ReadLine();
                        output = GetWidgetByName(input);
                        break;
                    //Search widget by category
                    case "3":
                        Console.WriteLine("Please enter a category: ");
                        input = Console.ReadLine();
                        if (IsInt(input))
                        {
                            output = GetWidgetsByCategory(Convert.ToInt32(input));
                        }
                        break;
                    //Update existing widget
                    case "4":
                        Console.WriteLine("Please enter widget name: ");
                        input = Console.ReadLine();
                        output = UpdateWidget(input);
                        break;
                    //Add new widget
                    case "5":
                        Console.WriteLine("Please enter new widget name: ");
                        input = Console.ReadLine();
                        output = CreateWidget(input);
                        break;
                    //Delete existing widget
                    case "6":
                        Console.WriteLine("Please enter widget name: ");
                        input = Console.ReadLine();
                        output = DeleteWidget(input);
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
            return response;
        }

        //Search widget by name
        public static string GetWidgetByName(string name)
        {
            string path = $"https://localhost:44314/widget/name/{name}";
            var response = RequestAsync(path).Result;
            var widget = JsonConvert.DeserializeObject<Widget>(response.ToString());
            var output = $"\nName: {widget.Name} \nQuantity: {widget.Quantity} \nCategory: {widget.Category} \n";

            return output;
        }

        //Search widget by category
        public static string GetWidgetsByCategory(int category)
        {
            string path = $"https://localhost:44314/widget/category/{category}";
            var response = RequestAsync(path).Result;
            var widgets = JsonConvert.DeserializeObject<List<Widget>>(response.ToString());
            var output = "";
            foreach (var widget in widgets)
            {
                output += $"\nName: {widget.Name} \nQuantity: {widget.Quantity} \nCategory: {widget.Category} \n";
            }
            return output;
        }

        //Add new widget
        public static string CreateWidget(string name)
        {
            string output = "";
            string path = $"https://localhost:44314/widget/name/{name}";
            var response = RequestAsync(path).Result;

            if (response == "")
            {
                Console.WriteLine("Enter quantity: ");
                var quantity = Console.ReadLine();

                Console.WriteLine("Enter category: ");
                var category = Console.ReadLine();
                if (IsInt(quantity) && IsInt(category))
                {
                    Widget widget = new Widget { Name = name, Quantity = Convert.ToInt32(quantity), Category = Convert.ToInt32(category) };
                    path = $"https://localhost:44314/widget/update/{widget}";
                    var res = UpdateAsync(path, widget).Result;
                    output = $"\nWidget added! \nName: {widget.Name} \nQuantity: {widget.Quantity} \nCategory: {widget.Category}";
                }
                else
                {
                    output = "Must be a whole number.";
                }
            }
            else
            {
                output = "Name is already used.";
            }
            return output;
        }
        

        //Update existing widget
        public static string UpdateWidget(string name)
        {
            //Check if name input is an existing widget
            string path = $"https://localhost:44314/widget/name/{name}";
            var response = RequestAsync(path).Result;

            string input = "";
            string output = "";

            //If name exists in widget list, ask what to change
            if (response != "")
            {
                //Populate widget object and display existing widget information
                Widget widget = JsonConvert.DeserializeObject<Widget>(response.ToString());
                output = $"\nName: {widget.Name} \nQuantity: {widget.Quantity} \nCategory: {widget.Category} \n";

                Console.WriteLine($"{output}Change quantity?(Y/N)");
                input = Console.ReadLine();

                //If input is Y, allow user to enter new value
                if (input == "Y")
                {
                    Console.WriteLine("Enter new quantity: ");
                    input = Console.ReadLine();

                    //Check if input is int before posting to web service
                    if (IsInt(input))
                    {
                        //Update widget object
                        widget.Quantity = Convert.ToInt32(input);
                        path = $"https://localhost:44314/widget/update/{widget}";
                        var res = UpdateAsync(path, widget).Result;
                        output = $"\nWidget updated! \nName: {widget.Name} \nQuantity: {widget.Quantity} \nCategory: {widget.Category}";
                    }
                    else
                        output = "There was a problem, entry was not an integer.";
                }
                else
                {
                    Console.WriteLine("Change category?(Y/N)");
                    input = Console.ReadLine();

                    //If input is Y, allow user to enter new value
                    if (input == "Y")
                    {
                        Console.WriteLine("Enter new category: ");
                        input = Console.ReadLine();
                        if (IsInt(input))
                        {
                            //Update widget object
                            widget.Category = Convert.ToInt32(input);
                            path = $"https://localhost:44314/widget/update/{widget}";
                            var res = UpdateAsync(path, widget).Result;

                            output = $"\nWidget updated! \nName: {widget.Name} \nQuantity: {widget.Quantity} \nCategory: {widget.Category}";
                        }
                        else
                            output = "There was a problem, entry was not an integer.";
                    }
                    else
                        output = "No widget changed.";
                }
            }
            //If widget by given name doesn't exist, notify user and return to menu
            else
            {
                Console.WriteLine($"{name} is not an existing widget, please try again.");
                output = "No widget changed.";
            }
            return output;
        }

        //Delete existing widget
        public static string DeleteWidget(string name)
        {
            string output = "";
            string path = $"https://localhost:44314/widget/name/{name}";
            var response = RequestAsync(path).Result;
            Widget widget = JsonConvert.DeserializeObject<Widget>(response.ToString());

            if (response != "")
            {
                path = $"https://localhost:44314/widget/delete/{name}";
                var res = DeleteAsync(path).Result;
                output = "Widget Deleted.";
            }
            else
                output = "No widget exists by that name.";

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
        public static async Task<Widget> UpdateAsync(string path, Widget widget)
        {
            var json = JsonConvert.SerializeObject(widget);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await client.PostAsync(path, content);
            
            //doesn't work but file updates
            string data = await response.Content.ReadAsStringAsync();
            var test = JsonConvert.DeserializeObject<Widget>(data);
            return test;
        }

        //DELETE from web service
        public static async Task<string> DeleteAsync(string path)
        {
            var response = await client.DeleteAsync(path);
            string data = await response.Content.ReadAsStringAsync();
            return data;
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
