using System.Data;
using System.Security.Claims;
using System.Threading.Tasks;

public class UserService
{
    // Временная заглушка - в реальном приложении замените на запросы к БД
    public async Task<User> AuthenticateAsync(string username, string password)
    {
        // Пример проверки учетных данных
        if (username == "string" && password == "string")
        {
            return await Task.FromResult(new User
            {
                Id = 1,
                Username = "admin",
                FullName = "Administrator",
                Email = "admin@example.com",
                Roles = new List<string> { "Admin" }
            });
        }

        return null;
    }
}

public class User
{
    public int Id { get; set; }
    public string Username { get; set; }
    public string FullName { get; set; }
    
    public string NikeName { get; set; }
    public string Email { get; set; }
    public List<string> Roles { get; set; } = new List<string>();

    public void TestIngData() 
    {

        Id = 1;
        Username = "ИВАН";
        FullName = "АДЯ";
        NikeName = "IvanAGI";
        Email = "wwa@fas.tu";
        Roles = new List<string>() { "Adminn", "USERS_IZE" };
    }
}