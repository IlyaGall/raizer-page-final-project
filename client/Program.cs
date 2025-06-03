using System.Text;

namespace client
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            var url = "https://localhost:7018/api/auth/login";
            var requestBody = new
            {
                username = "string",
                password = "string"
            };

            using (var httpClient = new HttpClient())
            {
                try
                {
                    // Сериализация тела запроса в JSON
                    var json = Newtonsoft.Json.JsonConvert.SerializeObject(requestBody);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");

                    // Выполнение POST-запроса
                    var response = await httpClient.PostAsync(url, content);

                    // Проверка статуса ответа
                    if (response.IsSuccessStatusCode)
                    {
                        var responseContent = await response.Content.ReadAsStringAsync();
                        Console.WriteLine($"Успешный ответ: {responseContent}");
                    }
                    else
                    {
                        Console.WriteLine($"Ошибка: {response.StatusCode} - {response.ReasonPhrase}");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Исключение: {ex.Message}");
                }
            }
            Console.ReadKey();
        }
    }
}
