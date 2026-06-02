using DiaryApp.Models;
using System.Reflection.Metadata;

namespace DiaryApp.Services
{
    public class CustomerService
    {
        public List<Customer> GetAll()
        {
            return DataStore.Customers;
        }

        public Customer GetById(int id)
        {
            foreach (var customer in DataStore.Customers)
            {
                if (customer.Id == id)
                {
                    return customer;
                }
            }
            return null;
        }

        public Customer SearchByName(string name)
        {
            foreach (var customer in DataStore.Customers)
            {
                if (customer.Name == name)
                {
                    return customer;
                }
            }
            return null;
        }

        public List<Customer> GetPremium()
        {
            List<Customer> list = new List<Customer>();
            foreach (var customer in DataStore.Customers)
            {
                foreach(var account in customer.Accounts)
                {
                    if(account.Balance> 10000)
                    {
                        list.Add(customer);
                        break;
                    }
                }
            }
            return list;
        }

        public void Add(Customer customer)
        {
            int newId;
            if (DataStore.Customers.Count > 0)
            {
                newId = DataStore.Customers.Max(c => c.Id) + 1;
            }
            else
            {
                newId = 1;
            }

            customer.Id = newId;
            DataStore.Customers.Add(customer);
        }

        public Customer? Update(int id, Customer updated)
        {
            var customer = DataStore.Customers.FirstOrDefault(c => c.Id == id);
            if (customer == null) return null;

            customer.Name = updated.Name;     
            customer.Email = updated.Email;
            return customer;
        }

        public bool Delete(int id)
        {
            var customer = DataStore.Customers.FirstOrDefault(c => c.Id == id);
            if (customer == null) return false;

      
            DataStore.Accounts.RemoveAll(a => a.CustomerId == id);
            DataStore.Customers.Remove(customer);
            return true;
        }
    }
}