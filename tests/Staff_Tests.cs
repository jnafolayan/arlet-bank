using Xunit;
using ArletBank;
using System.Collections.Generic;

namespace ArletTests
{
    public class StaffTests 
    {
    [Fact]
    public void LoginAsStaff()
    {
        var items = TestItems.Prepare();

        var staff = items.staffService.CreateStaff("dec1@gmail.com", "John", "Doe", "johndoe", "");
        var login = items.staffService.Login(staff.Username, "johndoe");

        Assert.NotNull(login);
        Assert.True(login.Email == staff.Email);
    }

        [Fact]
        public void ApproveCustomerRegistration()
        {
            var items = TestItems.Prepare();

            var customer = items.customerService.CreateCustomer("dek@gmail.com", "Random", "User", Account.Types.CURRENT_ACCOUNT);
            bool success = items.customerService.ApproveCustomer(customer.Email, "");

            var q = new Dictionary<string, object>();
            q.Add("Confirmed", false);
            var unconfirmedList = items.customerService.GetCustomers(q);

            Assert.True(success);
            Assert.Empty(unconfirmedList);
        }

        [Fact]
        public void RemoveCustomer()
        {
            var items = TestItems.Prepare();

            var customer = items.customerService.CreateCustomer("dek@gmail.com", "Izuku", "Midoriya", Account.Types.CURRENT_ACCOUNT);
            var previousCount = items.db.Collection("customers").Count;

            items.customerService.RemoveCustomer(customer.Email);
            var currentCount = items.db.Collection("customers").Count;
            
            Assert.NotEqual<int>(previousCount, currentCount);
            Assert.Equal<int>(0, currentCount);
        }

        [Fact]
        public void ListCustomers()
        {
            var items = TestItems.Prepare();

            items.customerService.CreateCustomer("dec1@gmail.com", "Doe", "Johndoea", Account.Types.CURRENT_ACCOUNT);
            items.customerService.CreateCustomer("dec2@gmail.com", "Doe", "Johndoeb", Account.Types.CURRENT_ACCOUNT);
            items.customerService.CreateCustomer("dec@gmail.com", "Doe", "Johndoec", Account.Types.CURRENT_ACCOUNT);

            Assert.Equal<int>(3, items.db.Collection("customers").Count);
        }
    }
}