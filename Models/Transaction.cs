namespace MasrafTakipAPI.Entity
{
    public class Transaction
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }
        public int PersonId { get; set; }
    }
}
