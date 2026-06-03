using DiaryApp.Data;
using DiaryApp.Models;
using DiaryApp.Services;
using Microsoft.EntityFrameworkCore;

namespace BankApp
{
    public class CustomerServiceUnitTest : IDisposable
    {
        private readonly CustomerService _customerService;
        private readonly AppDbContext _db;

        public CustomerServiceUnitTest()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            _db = new AppDbContext(options);
            SeedData();
            _customerService = new CustomerService(_db);
        }

        private void SeedData()
        {
            var customer1 = new Customer { Id = 1, Name = "John Doe", Email = "john.doe@example.com" };
            var customer2 = new Customer { Id = 2, Name = "Jane Smith", Email = "jane.smith@example.com" };
            var customer3 = new Customer { Id = 3, Name = "Carlos Rivera", Email = "carlos.rivera@example.com" };
            var customer4 = new Customer { Id = 4, Name = "Emily Chen", Email = "emily.chen@example.com" };
            _db.Customers.AddRange(customer1, customer2, customer3, customer4);
            _db.Accounts.AddRange(
                new Account { Id = 1, AccountNumber = 1001, AccountType = AccountType.Savings, Balance = 5000, CustomerId = 1 },
                new Account { Id = 2, AccountNumber = 1002, AccountType = AccountType.Savings, Balance = 8000, CustomerId = 2 },
                new Account { Id = 3, AccountNumber = 1003, AccountType = AccountType.Checking, Balance = 4500, CustomerId = 2 },
                new Account { Id = 4, AccountNumber = 1004, AccountType = AccountType.Checking, Balance = 1200, CustomerId = 3 },
                new Account { Id = 5, AccountNumber = 1005, AccountType = AccountType.Savings, Balance = 15000, CustomerId = 4 }
            );
            _db.SaveChanges();
        }

        public void Dispose()
        {
            _db.Dispose();
        }

        #region GetAll Tests
        [Fact]
        public void GetAll_ReturnsAllCustomers_Pass()
        {
            // Act
            var result = _customerService.GetAll();

            // Assert
            Assert.NotNull(result);
            Assert.NotEmpty(result);
            Assert.Equal(_db.Customers.Count(), result.Count);
        }

        [Fact]
        public void GetAll_WhenNoCustomers_ReturnsEmptyList_Fail()
        {
            // Arrange
            _db.Accounts.RemoveRange(_db.Accounts);
            _db.Customers.RemoveRange(_db.Customers);
            _db.SaveChanges();

            // Act
            var result = _customerService.GetAll();

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }
        #endregion

        #region GetById Tests
        [Fact]
        public void GetById_WithValidId_ReturnsCustomer_Pass()
        {
            // Arrange
            int validId = 1;

            // Act
            var result = _customerService.GetById(validId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(validId, result.Id);
            Assert.Equal("John Doe", result.Name);
        }

        [Fact]
        public void GetById_WithInvalidId_ReturnsNull_Fail()
        {
            // Arrange
            int invalidId = 9999;

            // Act
            var result = _customerService.GetById(invalidId);

            // Assert
            Assert.Null(result);
        }
        #endregion

        #region SearchByName Tests
        [Fact]
        public void SearchByName_WithExactMatch_ReturnsCustomer_Pass()
        {
            // Arrange
            string customerName = "John Doe";

            // Act
            var result = _customerService.SearchByName(customerName);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(customerName, result.Name);
        }

        [Fact]
        public void SearchByName_WithNonExistentName_ReturnsNull_Fail()
        {
            // Arrange
            string nonExistentName = "NonExistent Person";

            // Act
            var result = _customerService.SearchByName(nonExistentName);

            // Assert
            Assert.Null(result);
        }
        #endregion

        #region GetPremium Tests
        [Fact]
        public void GetPremium_ReturnsCustomersWithBalanceOver10000_Pass()
        {
            // Act
            var result = _customerService.GetPremium();

            // Assert
            Assert.NotNull(result);
            Assert.NotEmpty(result);
            Assert.All(result, customer =>
            {
                Assert.True(customer.Accounts.Any(a => a.Balance > 10000));
            });
        }

        [Fact]
        public void GetPremium_WhenNoCustomersQualify_ReturnsEmptyList_Fail()
        {
            // Arrange - Set all accounts to low balances
            foreach (var account in _db.Accounts)
            {
                account.Balance = 5000;
            }
            _db.SaveChanges();

            // Act
            var result = _customerService.GetPremium();

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }
        #endregion

        #region Add Tests
        [Fact]
        public void Add_WithNewCustomer_AddsCustomer_Pass()
        {
            // Arrange
            var newCustomer = new Customer
            {
                Name = "New Customer",
                Email = "new.customer@example.com"
            };
            int initialCount = _db.Customers.Count();

            // Act
            _customerService.Add(newCustomer);

            // Assert
            Assert.Equal(initialCount + 1, _db.Customers.Count());
            Assert.True(newCustomer.Id > 0);
            Assert.NotNull(_db.Customers.FirstOrDefault(c => c.Id == newCustomer.Id));
        }

        [Fact]
        public void Add_ToEmptyList_AssignsIdOne_Pass()
        {
            // Arrange
            _db.Accounts.RemoveRange(_db.Accounts);
            _db.Customers.RemoveRange(_db.Customers);
            _db.SaveChanges();
            var newCustomer = new Customer
            {
                Name = "First Customer",
                Email = "first@example.com"
            };

            // Act
            _customerService.Add(newCustomer);

            // Assert
            Assert.Equal(1, newCustomer.Id);
            Assert.Equal(1, _db.Customers.Count());
        }
        #endregion

        #region Update Tests
        [Fact]
        public void Update_WithValidId_UpdatesCustomer_Pass()
        {
            // Arrange
            int customerId = 1;
            var updatedCustomer = new Customer
            {
                Name = "Updated Name",
                Email = "updated.email@example.com"
            };

            // Act
            var result = _customerService.Update(customerId, updatedCustomer);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(customerId, result.Id);
            Assert.Equal(updatedCustomer.Name, result.Name);
            Assert.Equal(updatedCustomer.Email, result.Email);
        }

        [Fact]
        public void Update_WithInvalidId_ReturnsNull_Fail()
        {
            // Arrange
            int invalidId = 9999;
            var updatedCustomer = new Customer
            {
                Name = "Updated Name",
                Email = "updated.email@example.com"
            };

            // Act
            var result = _customerService.Update(invalidId, updatedCustomer);

            // Assert
            Assert.Null(result);
        }
        #endregion

        #region Delete Tests
        [Fact]
        public void Delete_WithValidId_DeletesCustomerAndAccounts_Pass()
        {
            // Arrange
            int customerId = 1;
            var customerAccountIds = _db.Accounts
                .Where(a => a.CustomerId == customerId)
                .Select(a => a.Id)
                .ToList();
            int initialCustomerCount = _db.Customers.Count();
            int initialAccountCount = _db.Accounts.Count();

            // Act
            var result = _customerService.Delete(customerId);

            // Assert
            Assert.True(result);
            Assert.Equal(initialCustomerCount - 1, _db.Customers.Count());
            Assert.Null(_db.Customers.FirstOrDefault(c => c.Id == customerId));
            // Verify customer's accounts are also deleted
            Assert.All(customerAccountIds, accountId =>
            {
                Assert.Null(_db.Accounts.FirstOrDefault(a => a.Id == accountId));
            });
        }

        [Fact]
        public void Delete_WithInvalidId_ReturnsFalse_Fail()
        {
            // Arrange
            int invalidId = 9999;
            int initialCustomerCount = _db.Customers.Count();
            int initialAccountCount = _db.Accounts.Count();

            // Act
            var result = _customerService.Delete(invalidId);

            // Assert
            Assert.False(result);
            Assert.Equal(initialCustomerCount, _db.Customers.Count());
            Assert.Equal(initialAccountCount, _db.Accounts.Count());
        }
        #endregion
    }
}
