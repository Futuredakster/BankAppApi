using DiaryApp.Models;
using DiaryApp.Services;

namespace BankApp
{
    [Collection("Sequential")]
    public class CustomerServiceUnitTest : IDisposable
    {
        private readonly CustomerService _customerService;

        public CustomerServiceUnitTest()
        {
            _customerService = new CustomerService();
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
        public void GetAll_ReturnsAllCustomers_Pass()
        {
            // Act
            var result = _customerService.GetAll();

            // Assert
            Assert.NotNull(result);
            Assert.NotEmpty(result);
            Assert.Equal(DataStore.Customers.Count, result.Count);
        }

        [Fact]
        public void GetAll_WhenNoCustomers_ReturnsEmptyList_Fail()
        {
            // Arrange
            DataStore.Customers.Clear();

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
            // Arrange - Set all accounts to low balances in both DataStore and Customer objects
            foreach (var account in DataStore.Accounts)
            {
                account.Balance = 5000;
            }
            foreach (var customer in DataStore.Customers)
            {
                foreach (var account in customer.Accounts)
                {
                    account.Balance = 5000;
                }
            }

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
            int initialCount = DataStore.Customers.Count;

            // Act
            _customerService.Add(newCustomer);

            // Assert
            Assert.Equal(initialCount + 1, DataStore.Customers.Count);
            Assert.True(newCustomer.Id > 0);
            Assert.Contains(newCustomer, DataStore.Customers);
        }

        [Fact]
        public void Add_ToEmptyList_AssignsIdOne_Pass()
        {
            // Arrange
            DataStore.Customers.Clear();
            var newCustomer = new Customer
            {
                Name = "First Customer",
                Email = "first@example.com"
            };

            // Act
            _customerService.Add(newCustomer);

            // Assert
            Assert.Equal(1, newCustomer.Id);
            Assert.Single(DataStore.Customers);
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
            var customerAccountIds = DataStore.Accounts
                .Where(a => a.CustomerId == customerId)
                .Select(a => a.Id)
                .ToList();
            int initialCustomerCount = DataStore.Customers.Count;
            int initialAccountCount = DataStore.Accounts.Count;

            // Act
            var result = _customerService.Delete(customerId);

            // Assert
            Assert.True(result);
            Assert.Equal(initialCustomerCount - 1, DataStore.Customers.Count);
            Assert.Null(DataStore.Customers.FirstOrDefault(c => c.Id == customerId));
            // Verify customer's accounts are also deleted
            Assert.All(customerAccountIds, accountId =>
            {
                Assert.Null(DataStore.Accounts.FirstOrDefault(a => a.Id == accountId));
            });
        }

        [Fact]
        public void Delete_WithInvalidId_ReturnsFalse_Fail()
        {
            // Arrange
            int invalidId = 9999;
            int initialCustomerCount = DataStore.Customers.Count;
            int initialAccountCount = DataStore.Accounts.Count;

            // Act
            var result = _customerService.Delete(invalidId);

            // Assert
            Assert.False(result);
            Assert.Equal(initialCustomerCount, DataStore.Customers.Count);
            Assert.Equal(initialAccountCount, DataStore.Accounts.Count);
        }
        #endregion
    }
}
