using DiaryApp.Data;
using DiaryApp.Models;
using DiaryApp.Services;
using Microsoft.EntityFrameworkCore;

namespace BankApp
{
    public class AccountServiceUnitTest : IDisposable
    {
        private readonly AccountService _accountService;
        private readonly AppDbContext _db;

        public AccountServiceUnitTest()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            _db = new AppDbContext(options);
            SeedData();
            _accountService = new AccountService(_db);
        }

        private void SeedData()
        {
            _db.Customers.AddRange(
                new Customer { Id = 1, Name = "John Doe", Email = "john.doe@example.com" },
                new Customer { Id = 2, Name = "Jane Smith", Email = "jane.smith@example.com" },
                new Customer { Id = 3, Name = "Carlos Rivera", Email = "carlos.rivera@example.com" },
                new Customer { Id = 4, Name = "Emily Chen", Email = "emily.chen@example.com" }
            );
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
        public void GetAll_ReturnsAllAccounts_Pass()
        {
            // Act
            var result = _accountService.GetAll();

            // Assert
            Assert.NotNull(result);
            Assert.NotEmpty(result);
            Assert.Equal(_db.Accounts.Count(), result.Count);
        }

        [Fact]
        public void GetAll_WhenNoAccounts_ReturnsEmptyList_Fail()
        {
            // Arrange
            _db.Accounts.RemoveRange(_db.Accounts);
            _db.SaveChanges();

            // Act
            var result = _accountService.GetAll();

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }
        #endregion

        #region GetById Tests
        [Fact]
        public void GetById_WithValidId_ReturnsAccount_Pass()
        {
            // Arrange
            int validId = 1;

            // Act
            var result = _accountService.GetById(validId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(validId, result.Id);
        }

        [Fact]
        public void GetById_WithInvalidId_ReturnsNull_Fail()
        {
            // Arrange
            int invalidId = 9999;

            // Act
            var result = _accountService.GetById(invalidId);

            // Assert
            Assert.Null(result);
        }
        #endregion

        #region SearchByName Tests
        [Fact]
        public void SearchByName_WithValidCustomerName_ReturnsAccounts_Pass()
        {
            // Arrange
            string customerName = "John";

            // Act
            var result = _accountService.SearchByName(customerName);

            // Assert
            Assert.NotNull(result);
            Assert.NotEmpty(result);
            Assert.All(result, account =>
            {
                var customer = _db.Customers.First(c => c.Id == account.CustomerId);
                Assert.Contains(customerName, customer.Name, StringComparison.OrdinalIgnoreCase);
            });
        }

        [Fact]
        public void SearchByName_WithNonExistentName_ReturnsEmptyList_Fail()
        {
            // Arrange
            string nonExistentName = "NonExistentCustomer";

            // Act
            var result = _accountService.SearchByName(nonExistentName);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }
        #endregion

        #region Add Tests
        [Fact]
        public void Add_WithValidCustomerId_AddsAccount_Pass()
        {
            // Arrange
            var newAccount = new Account
            {
                AccountNumber = 2000,
                AccountType = AccountType.Savings,
                Balance = 3000,
                CustomerId = 1
            };
            int initialCount = _db.Accounts.Count();

            // Act
            var result = _accountService.Add(newAccount);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(initialCount + 1, _db.Accounts.Count());
            Assert.True(result.Id > 0);
            Assert.NotNull(_db.Accounts.FirstOrDefault(a => a.Id == result.Id));
        }

        [Fact]
        public void Add_WithInvalidCustomerId_ReturnsNull_Fail()
        {
            // Arrange
            var newAccount = new Account
            {
                AccountNumber = 2000,
                AccountType = AccountType.Savings,
                Balance = 3000,
                CustomerId = 9999
            };
            int initialCount = _db.Accounts.Count();

            // Act
            var result = _accountService.Add(newAccount);

            // Assert
            Assert.Null(result);
            Assert.Equal(initialCount, _db.Accounts.Count());
        }
        #endregion

        #region Update Tests
        [Fact]
        public void Update_WithValidId_UpdatesAccount_Pass()
        {
            // Arrange
            int accountId = 1;
            var updatedAccount = new Account
            {
                AccountNumber = 5555,
                AccountType = AccountType.Checking,
                Balance = 10000
            };

            // Act
            var result = _accountService.Update(accountId, updatedAccount);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(accountId, result.Id);
            Assert.Equal(updatedAccount.AccountNumber, result.AccountNumber);
            Assert.Equal(updatedAccount.AccountType, result.AccountType);
            Assert.Equal(updatedAccount.Balance, result.Balance);
        }

        [Fact]
        public void Update_WithInvalidId_ReturnsNull_Fail()
        {
            // Arrange
            int invalidId = 9999;
            var updatedAccount = new Account
            {
                AccountNumber = 5555,
                AccountType = AccountType.Checking,
                Balance = 10000
            };

            // Act
            var result = _accountService.Update(invalidId, updatedAccount);

            // Assert
            Assert.Null(result);
        }
        #endregion

        #region Delete Tests
        [Fact]
        public void Delete_WithValidId_DeletesAccount_Pass()
        {
            // Arrange
            int accountId = 1;
            int initialCount = _db.Accounts.Count();

            // Act
            var result = _accountService.Delete(accountId);

            // Assert
            Assert.True(result);
            Assert.Equal(initialCount - 1, _db.Accounts.Count());
            Assert.Null(_db.Accounts.FirstOrDefault(a => a.Id == accountId));
        }

        [Fact]
        public void Delete_WithInvalidId_ReturnsFalse_Fail()
        {
            // Arrange
            int invalidId = 9999;
            int initialCount = _db.Accounts.Count();

            // Act
            var result = _accountService.Delete(invalidId);

            // Assert
            Assert.False(result);
            Assert.Equal(initialCount, _db.Accounts.Count());
        }
        #endregion
    }
}
