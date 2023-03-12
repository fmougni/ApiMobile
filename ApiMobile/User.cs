namespace ApiMobile
{
    public class User
    {
        public int id { get; set; }
        public string mail { get; set; }
        public string password { get; set; }
        public string token_Auth_API { get; set; }

    }
    public class UserLoginModel
    {
        public string Mail { get; set; }
        public string Password { get; set; }
    }

}
