using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Npgsql;


namespace food_web.Pages
{
    public class IndexModel : PageModel
    {
        public List<string> RecipeName { get; set; } = new List<string>();

        private readonly HttpClient _httpClient;

        public IndexModel(HttpClient httpClient)
        {
            _httpClient = httpClient;
            RecipeName = new List<string>();
        }

        public async Task OnGetAsync()
        {
            string connString = "Server=::1;Port=5432;Database=recipe_database;User Id=postgres;Password=123;";
            using (NpgsqlConnection conn = new NpgsqlConnection(connString))
            {
                await conn.OpenAsync();

                string sql = "SELECT * FROM recipe";
                using (NpgsqlCommand cmd = new NpgsqlCommand(sql, conn))
                {
                    using (NpgsqlDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            RecipeName.Add(reader.GetString(1));
                            
                        }
                    }
                }
            }

            foreach (var name in RecipeName)
            {
                var content = new StringContent(name);

                if (_httpClient != null)
                {
                    var response = await _httpClient.PostAsync("https://localhost:7010", content);

                    if (response.IsSuccessStatusCode)
                    {
                        Console.WriteLine("Recipe name posted successfully.");
                    }
                    else
                    {
                        Console.WriteLine("Error posting recipe name: " + response.StatusCode);
                    }
                }
                else
                {
                    Console.WriteLine("_httpClient is null.");
                }
      
            }
           
        }

        public async Task<IActionResult> OnPostAsync(string dish_name, string cuisine, string category)
        {
            if (!string.IsNullOrEmpty(dish_name) && !string.IsNullOrEmpty(cuisine) && !string.IsNullOrEmpty(category))
            {

                string connString = "Server=::1;Port=5432;Database=recipe_database;User Id=postgres;Password=123;";
                using (NpgsqlConnection conn = new NpgsqlConnection(connString))
                {
                    await conn.OpenAsync();

                    string sql = "INSERT INTO recipe (dish_name, cuisine, category) VALUES (@dish_name, @cuisine, @category)";
                    using (NpgsqlCommand cmd = new NpgsqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@dish_name", dish_name);
                        RecipeName.Add(dish_name);
                        cmd.Parameters.AddWithValue("@cuisine", cuisine);
                        cmd.Parameters.AddWithValue("@category", category);
                        cmd.ExecuteNonQuery();  

                    }
                }


                var content = new StringContent($"Name: {dish_name}, cuisine: {cuisine}, category: {category}");

                if (_httpClient != null)
                {
                    var response = await _httpClient.PostAsync("https://localhost:7010", content);

                    if (response.IsSuccessStatusCode)
                    {
                        Console.WriteLine("Recipe data posted successfully.");
                    }
                    else
                    {
                        Console.WriteLine("Error posting recipe data: " + response.StatusCode);
                    }
                }
                else
                {
                    Console.WriteLine("_httpClient is null.");
                }
                
            }
            
            return RedirectToPage();   

        }

        public async Task<IActionResult> OnPostDeleteAsync(string dish_name)
        {
            string connString = "Server=::1;Port=5432;Database=recipe_database;User Id=postgres;Password=123;";

            using (NpgsqlConnection conn = new NpgsqlConnection(connString))
            {
                await conn.OpenAsync();

                string sql = "DELETE FROM recipe WHERE \"dish_name\" = @dish_name";

                using (NpgsqlCommand cmd = new NpgsqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@dish_name", dish_name);
                    RecipeName.Remove(dish_name);
                    cmd.ExecuteNonQuery();
                }
            }

            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostUpdateAsync(string dish_name, string old_cuisine, string new_cuisine, string category)
        {
            if (!string.IsNullOrEmpty(dish_name) && !string.IsNullOrEmpty(old_cuisine) && !string.IsNullOrEmpty(new_cuisine) && !string.IsNullOrEmpty(category))
            {
                string connString = "Server=::1;Port=5432;Database=recipe_database;User Id=postgres;Password=123;";

                using (NpgsqlConnection conn = new NpgsqlConnection(connString))
                {
                    await conn.OpenAsync();

                    string sql = "UPDATE recipe SET \"cuisine\" = @new_cuisine WHERE \"dish_name\" = @dish_name AND \"cuisine\" = @old_cuisine AND \"category\" = @category";

                    using (NpgsqlCommand cmd = new NpgsqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@dish_name", dish_name);
                        cmd.Parameters.AddWithValue("@old_cuisine", old_cuisine);
                        cmd.Parameters.AddWithValue("@new_cuisine", new_cuisine);
                        cmd.Parameters.AddWithValue("@category", category);
                        cmd.ExecuteNonQuery();

                    }
                }

                var content = new StringContent($"dish_name: {dish_name}, old_cuisine: {old_cuisine}, new_cuisine: {new_cuisine}, category: {category}");

                if (_httpClient != null)
                {
                    var response = await _httpClient.PostAsync("https://localhost:7010", content);

                    if (response.IsSuccessStatusCode)
                    {
                        Console.WriteLine("Recipe data posted successfully.");
                    }
                    else
                    {
                        Console.WriteLine("Error posting recipe data: " + response.StatusCode);
                    }
                }
                else
                {
                    Console.WriteLine("_httpClient is null.");
                }
            }

            return RedirectToPage();
        }



    }

}

