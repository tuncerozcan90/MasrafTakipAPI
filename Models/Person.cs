using MasrafTakipAPI.Models;
using System.Transactions;

namespace MasrafTakipAPI.Entity
{
    public class Person
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public User User { get; set; }
        public ICollection<Transaction> Transactions { get; set; }
    }
}
