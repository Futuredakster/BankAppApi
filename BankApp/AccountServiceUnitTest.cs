using DiaryApp.Models;
using DiaryApp.Services;

namespace BankApp
{
    [Collection("Sequential")]
    public class AccountServiceUnitTest : IDisposable
    {
        private readonly AccountService _accountService;

        public AccountServiceUnitTest()
        {
            _accountService = new AccountService();
            // Reset data to initial state before each test
            DataStore.InitializeData();
        }

        public void Dispose()
        {
            // Reset data to initial state after each test
            DataStore.InitializeData();
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
            Assert.Equal(DataStore.Accounts.Count, result.Count);
        }

        [Fact]
        public void GetAll_WhenNoAccounts_ReturnsEmptyList_Fail()
        {
            // Arrange
            DataStore.Accounts.Clear();

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
                var customer = DataStore.Customers.First(c => c.Id == account.CustomerId);
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
            int initialCount = DataStore.Accounts.Count;

            // Act
            var result = _accountService.Add(newAccount);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(DataStore.Accounts.Count, initialCount + 1);
            Assert.True(result.Id > 0);
            Assert.Contains(result, DataStore.Accounts);
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
            int initialCount = DataStore.Accounts.Count;

            // Act
            var result = _accountService.Add(newAccount);

            // Assert
            Assert.Null(result);
            Assert.Equal(initialCount, DataStore.Accounts.Count);
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
            int initialCount = DataStore.Accounts.Count;

            // Act
            var result = _accountService.Delete(accountId);

            // Assert
            Assert.True(result);
            Assert.Equal(initialCount - 1, DataStore.Accounts.Count);
            Assert.Null(DataStore.Accounts.FirstOrDefault(a => a.Id == accountId));
        }

        [Fact]
        public void Delete_WithInvalidId_ReturnsFalse_Fail()
        {
            // Arrange
            int invalidId = 9999;
            int initialCount = DataStore.Accounts.Count;

            // Act
            var result = _accountService.Delete(invalidId);

            // Assert
            Assert.False(result);
            Assert.Equal(initialCount, DataStore.Accounts.Count);
        }
        #endregion
    }
}
