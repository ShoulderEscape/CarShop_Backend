using Entites;

namespace CarShopAPI
{
    public class UserDto
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public List<int> CarIds { get; set; }

    }
}
