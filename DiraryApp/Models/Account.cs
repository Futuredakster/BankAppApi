namespace DiaryApp.Models
{
    public enum AccountType
    {
        Savings,
        Checking
    }

    public class Account
    {
        public int Id { get; set; }
        public int AccountNumber { get; set; }
        public AccountType AccountType { get; set; }

        public int CustomerId { get; set; }
        public int Balance { get; set; }
    }
}