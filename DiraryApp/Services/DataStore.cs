using DiaryApp.Models;

namespace DiaryApp.Services
{
    public class DataStore
    {
        public static List<Customer> Customers { get; set; } = new();
        public static List<Account> Accounts { get; set; } = new();

        static DataStore()   
        {
            InitializeData();
        }

        public static void InitializeData()
        {
            Customers.Clear();
            Accounts.Clear();

            var customer1 = new Customer
            {
                Id = 1,
                Name = "John Doe",
                Email = "john.doe@example.com"
            };
            var account1 = new Account
            {
                Id = 1,
                AccountNumber = 1001,
                AccountType = AccountType.Savings,
                Balance = 5000,
                CustomerId = customer1.Id
            };
            customer1.Accounts.Add(account1);
            Customers.Add(customer1);
            Accounts.Add(account1);


            var customer2 = new Customer
            {
                Id = 2,
                Name = "Jane Smith",
                Email = "jane.smith@example.com"
            };
            var account2 = new Account
            {
                Id = 2,
                AccountNumber = 1002,
                AccountType = AccountType.Savings,
                Balance = 8000,
                CustomerId = customer2.Id
            };
            var account3 = new Account
            {
                Id = 3,
                AccountNumber = 1003,
                AccountType = AccountType.Checking,
                Balance = 4500,
                CustomerId = customer2.Id
            };
            customer2.Accounts.Add(account2);
            customer2.Accounts.Add(account3);
            Customers.Add(customer2);
            Accounts.Add(account2);
            Accounts.Add(account3);


            var customer3 = new Customer
            {
                Id = 3,
                Name = "Carlos Rivera",
                Email = "carlos.rivera@example.com"
            };
            var account4 = new Account
            {
                Id = 4,
                AccountNumber = 1004,
                AccountType = AccountType.Checking,
                Balance = 1200,
                CustomerId = customer3.Id
            };
            customer3.Accounts.Add(account4);
            Customers.Add(customer3);
            Accounts.Add(account4);


            var customer4 = new Customer
            {
                Id = 4,
                Name = "Emily Chen",
                Email = "emily.chen@example.com"
            };
            var account5 = new Account
            {
                Id = 5,
                AccountNumber = 1005,
                AccountType = AccountType.Savings,
                Balance = 15000,
                CustomerId = customer4.Id
            };
            customer4.Accounts.Add(account5);
            Customers.Add(customer4);
            Accounts.Add(account5);
        }
    }
}