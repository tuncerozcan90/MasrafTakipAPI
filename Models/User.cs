using MasrafTakipAPI.Entity;

namespace MasrafTakipAPI.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }  // Şifrelenmiş halde saklanmalıdır

        // Relationship
        public int PersonId { get; set; }
        public Person Person { get; set; }
    }
}
