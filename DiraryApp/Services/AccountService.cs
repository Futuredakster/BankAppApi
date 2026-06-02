using DiaryApp.Models;

namespace DiaryApp.Services
{
    public class AccountService
    {
        public List<Account> GetAll()
        {
            return DataStore.Accounts;
        }

        public Account? GetById(int id)
        {
            return DataStore.Accounts.FirstOrDefault(a => a.Id == id);
        }

        public List<Account> SearchByName(string name)
        {
            
            var matchingCustomerIds = DataStore.Customers
                .Where(c => c.Name.Contains(name, StringComparison.OrdinalIgnoreCase))
                .Select(c => c.Id)
                .ToList();

            return DataStore.Accounts
                .Where(a => matchingCustomerIds.Contains(a.CustomerId))
                .ToList();
        }

        public Account? Add(Account account)
        {
            
            var customer = DataStore.Customers.FirstOrDefault(c => c.Id == account.CustomerId);
            if (customer == null) return null;

            int newId;
            if (DataStore.Accounts.Count > 0)
                newId = DataStore.Accounts.Max(a => a.Id) + 1;
            else
                newId = 1;

            account.Id = newId;

            DataStore.Accounts.Add(account);   
            customer.Accounts.Add(account);   
            return account;
        }

        public Account? Update(int id, Account updated)
        {
            var account = DataStore.Accounts.FirstOrDefault(a => a.Id == id);
            if (account == null) return null;

            
            account.AccountNumber = updated.AccountNumber;
            account.AccountType = updated.AccountType;
            account.Balance = updated.Balance;
            return account;
        }

        public bool Delete(int id)
        {
            var account = DataStore.Accounts.FirstOrDefault(a => a.Id == id);
            if (account == null) return false;

            
            var customer = DataStore.Customers.FirstOrDefault(c => c.Id == account.CustomerId);
            customer?.Accounts.Remove(account);

            DataStore.Accounts.Remove(account);
            return true;
        }
    }
}