using Xunit;
using ArletBank;

namespace ArletTests
{
    public class TestItems {
        public FileDatabase db;
        public AdminService adminService;
        public StaffService staffService;
        public CustomerService customerService;
        public AccountService accountService;
        public TransactionService transactionService;
        public static TestItems Prepare()
        {
            var items = new TestItems();
            items.db = new FileDatabase("./db-test.json");
            items.db.Clear();

            var adminModel = new Model<Admin>(items.db, "admins");
            var staffModel = new Model<Staff>(items.db, "staffs");
            var customerModel = new Model<Customer>(items.db, "customers");
            var accountModel = new Model<Account>(items.db, "accounts");
            var transactionModel = new Model<Transaction>(items.db, "transactions");

            items.adminService = new AdminService(adminModel);
            items.staffService = new StaffService(staffModel);
            items.customerService = new CustomerService(customerModel);
            items.accountService = new AccountService(accountModel);
            items.transactionService = new TransactionService(transactionModel);

            return items;
        }
    }

    public class AdminTests 
    {
        [Fact]
        public void LoginAsAdmin()
        {
            var items = TestItems.Prepare();

            var admin = items.adminService.CreateAdmin("johndoe", "johndoe", "John Doe", "");
            var login = items.adminService.Login(admin.Username, "johndoe");

            Assert.NotNull(login);
            Assert.True(login.Username == admin.Username);
        }

        [Fact]
        public void CreateAdmin()
        {
            var items = Prepare();

            items.adminService.CreateAdmin("johndoe", "johndoe", "John Doe", "superuser");
            var admin = items.adminService.GetAdmin("johndoe");

            Assert.True(1 == items.db.Collection("admins").Count);
            Assert.True("John Doe" == admin.Name);
        }
        
        [Fact]
        public void removeAdmin()
        {
            var items = Prepare();

            items.adminService.CreateAdmin("johndoe", "johndoe", "John Doe", "superuser");
            items.adminService.RemoveAdmin("johndoe");

            Assert.Empty(items.db.Collection("admins"));
        }

        [Fact]
        public void CreateStaff()
        {
            var items = Prepare();

            // create admin
            items.adminService.CreateAdmin("johndoe", "johndoe", "John Doe", "superuser");

            items.staffService.CreateStaff("dec@gmail.com", "Decker", "Doe", "johndoe", "johndoe");
            var staff = items.staffService.GetStaff("dec@gmail.com");

            Assert.True(1 == items.db.Collection("staffs").Count);
            Assert.True("johndoe" == staff.CreatedBy);
        }

        [Fact]
        public void RemoveStaff()
        {
            var items = Prepare();

            // create admin
            items.adminService.CreateAdmin("johndoe", "johndoe", "John Doe", "superuser");

            items.staffService.CreateStaff("dec@gmail.com", "Decker", "Doe", "johndoe", "johndoe");
            items.staffService.RemoveStaff("dec@gmail.com");

            Assert.Empty(items.db.Collection("staffs"));
        }
        
        [Fact]
        public void ListStaffs()
        {
            var items = Prepare();

            // create admin
            items.adminService.CreateAdmin("johndoe", "johndoe", "John Doe", "superuser");

            items.staffService.CreateStaff("dec1@gmail.com", "A", "Doe", "johndoea", "johndoea");
            items.staffService.CreateStaff("dec2@gmail.com", "B", "Doe", "johndoeb", "johndoeb");
            items.staffService.CreateStaff("dec@gmail.com", "C", "Doe", "johndoec", "johndoev");

            Assert.Equal<int>(3, items.db.Collection("staffs").Count);
        }

        public TestItems Prepare()
        {
            var items = new TestItems();
            items.db = new FileDatabase("./db-test.json");
            items.db.Clear();

            var adminModel = new Model<Admin>(items.db, "admins");
            var staffModel = new Model<Staff>(items.db, "staffs");

            items.adminService = new AdminService(adminModel);
            items.staffService = new StaffService(staffModel);

            return items;
        }
    }
}