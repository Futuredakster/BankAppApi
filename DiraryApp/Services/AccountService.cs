using DiaryApp.Data;
using DiaryApp.Models;
using Microsoft.EntityFrameworkCore;

namespace DiaryApp.Services
{
    public class AccountService
    {
        private readonly AppDbContext _db;

        // _db is the EF Core database context — equivalent to your Sequelize models object.
        // In Sequelize you'd do: const { Account, Customer } = require('../models')
        // Here we inject AppDbContext which gives us _db.Accounts and _db.Customers
        public AccountService(AppDbContext db)
        {
            _db = db;
        }

        public List<Account> GetAll()
        {
            // Sequelize: Account.findAll()
            // EF Core fetches all rows from the Accounts table and maps them to C# objects
            return _db.Accounts.ToList();
        }

        public Account? GetById(int id)
        {
            // Sequelize: Account.findOne({ where: { id } })
            // FirstOrDefault returns the first match or null — same as Sequelize returning null if not found
            return _db.Accounts.FirstOrDefault(a => a.Id == id);
        }

        public List<Account> SearchByName(string name)
        {
            // Step 1 — Sequelize: Customer.findAll({ where: { name: { [Op.like]: `%${name}%` } }, attributes: ['id'] })
            // Get the IDs of customers whose name contains the search string
            var matchingCustomerIds = _db.Customers
                .Where(c => c.Name.Contains(name))
                .Select(c => c.Id)
                .ToList();

            // Step 2 — Sequelize: Account.findAll({ where: { customerId: { [Op.in]: matchingCustomerIds } } })
            // Return all accounts that belong to those customers
            return _db.Accounts
                .Where(a => matchingCustomerIds.Contains(a.CustomerId))
                .ToList();
        }

        public Account? Add(Account account)
        {
            // Sequelize: Customer.findOne({ where: { id: account.customerId } })
            // Validate the customer exists before creating the account
            var customer = _db.Customers.FirstOrDefault(c => c.Id == account.CustomerId);
            if (customer == null) return null;

            // Sequelize: Account.create({ ...account })
            // Add stages the new row, SaveChanges() executes the INSERT statement
            _db.Accounts.Add(account);
            _db.SaveChanges();
            return account;
        }

        public Account? Update(int id, Account updated)
        {
            // Sequelize: Account.findOne({ where: { id } })
            var account = _db.Accounts.FirstOrDefault(a => a.Id == id);
            if (account == null) return null;

            // Sequelize: account.update({ accountNumber: ..., accountType: ..., balance: ... })
            // EF Core tracks changes on the fetched object — SaveChanges() runs the UPDATE
            account.AccountNumber = updated.AccountNumber;
            account.AccountType = updated.AccountType;
            account.Balance = updated.Balance;
            _db.SaveChanges();
            return account;
        }

        public bool Delete(int id)
        {
            // Sequelize: Account.findOne({ where: { id } })
            var account = _db.Accounts.FirstOrDefault(a => a.Id == id);
            if (account == null) return false;

            // Sequelize: await account.destroy()
            // Remove marks it for deletion, SaveChanges() executes the DELETE statement
            _db.Accounts.Remove(account);
            _db.SaveChanges();
            return true;
        }
    }
}