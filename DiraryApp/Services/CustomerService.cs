using DiaryApp.Data;
using DiaryApp.Models;
using Microsoft.EntityFrameworkCore;

namespace DiaryApp.Services
{
    public class CustomerService
    {
        private readonly AppDbContext _db;

        // _db is the EF Core database context — equivalent to your Sequelize models object.
        // In Sequelize you'd do: const { Customer, Account } = require('../models')
        public CustomerService(AppDbContext db)
        {
            _db = db;
        }

        public List<Customer> GetAll()
        {
            // Sequelize: Customer.findAll({ include: [Account] })
            // Include(c => c.Accounts) is EF Core's eager loading — it JOINs the Accounts table
            // so each Customer comes back with its Accounts list already populated
            return _db.Customers.Include(c => c.Accounts).ToList();
        }

        public Customer? GetById(int id)
        {
            // Sequelize: Customer.findOne({ where: { id }, include: [Account] })
            return _db.Customers.Include(c => c.Accounts).FirstOrDefault(c => c.Id == id);
        }

        public Customer? SearchByName(string name)
        {
            // Sequelize: Customer.findOne({ where: { name }, include: [Account] })
            // Note: this is an exact match — use Contains(name) for a LIKE query
            return _db.Customers.Include(c => c.Accounts).FirstOrDefault(c => c.Name == name);
        }

        public List<Customer> GetPremium()
        {
            // Sequelize: Customer.findAll({ include: [{ model: Account, where: { balance: { [Op.gt]: 10000 } } }] })
            // Any() checks if at least one account has balance > 10000 — equivalent to Sequelize's Op.gt
            return _db.Customers.Include(c => c.Accounts)
                .Where(c => c.Accounts.Any(a => a.Balance > 10000))
                .ToList();
        }

        public void Add(Customer customer)
        {
            // Sequelize: Customer.create({ ...customer })
            // Add stages the INSERT, SaveChanges() executes it against the database
            _db.Customers.Add(customer);
            _db.SaveChanges();
        }

        public Customer? Update(int id, Customer updated)
        {
            // Sequelize: customer.update({ name: ..., email: ... })
            // EF Core change tracking detects the property changes and SaveChanges() runs the UPDATE
            var customer = _db.Customers.FirstOrDefault(c => c.Id == id);
            if (customer == null) return null;

            customer.Name = updated.Name;
            customer.Email = updated.Email;
            _db.SaveChanges();
            return customer;
        }

        public bool Delete(int id)
        {
            // Sequelize: await customer.destroy()
            // Because we set OnDelete: Cascade in AppDbContext, deleting a Customer
            // automatically DELETEs all their Accounts too — same as Sequelize's onDelete: 'CASCADE'
            var customer = _db.Customers.FirstOrDefault(c => c.Id == id);
            if (customer == null) return false;

            _db.Customers.Remove(customer);
            _db.SaveChanges();
            return true;
        }
    }
}