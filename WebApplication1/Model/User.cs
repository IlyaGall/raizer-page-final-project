namespace Model.User
{
    public class User
    {
        /// <summary>
        /// Id пользователя
        /// </summary>
        public long UserID { get; set; }

        /// <summary>
        /// Имя пользователя
        /// </summary>
        public string UserName { get; set; } = string.Empty;

        /// <summary>
        /// Ник ней пользователя
        /// </summary>
        public string NikeName { get; set; } = string.Empty;

        /// <summary>
        /// Полное имя пользователя
        /// </summary>
        public string FullName { get; set; } = string.Empty;
        
        /// <summary>
        /// Email пользователя
        /// </summary>
        public string Email { get; set; } = string.Empty ;
        /// <summary>
        /// 
        /// </summary>
        public List<string> Role { get; set; } = new List<string>();


        public void test() 
        {
            UserID = 1;
            UserName = "ИВАН";
            FullName = "АДЯ";
            NikeName = "IvanAGI";
            Email = "wwa@fas.tu";
            Role = new List<string>() { "Adminn","USERS_IZE"  };
        }
    }
}
