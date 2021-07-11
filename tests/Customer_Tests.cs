using Xunit;
using ArletBank;
using System.Collections.Generic;

namespace ArletTests
{
    public class CustomerTests
    {
        [Fact]
        public void LoginAsCustomer()
        {
            var items = TestItems.Prepare();
            var account = CreateTestAccount(items);
            var login = items.accountService.Login(account.Number, "0000");

            Assert.NotNull(login);
            Assert.True(login.Number == account.Number);
        }

        [Fact]
        public void ChangePIN()
        {
            var items = TestItems.Prepare();
            var account = CreateTestAccount(items);
            items.accountService.ChangeAccountPIN(account.Number, "1234");
            account = items.accountService.GetAccountByAccountNumber(account.Number);

            Assert.NotNull(account);
            Assert.True(PasswordHasher.Verify("1234", account.PIN));
        }

        [Fact]
        public void ViewBalance()
        {
            var items = TestItems.Prepare();
            var account = CreateTestAccount(items);
            Assert.True(account.Balance == 0);
        }
        
        [Fact]
        public void Deposit()
        {
            var items = TestItems.Prepare();
            var account = CreateTestAccount(items);
            items.accountService.Deposit(account.Number, 500);

            account = items.accountService.GetAccountByAccountNumber(account.Number);

            Assert.True(account.Balance == 500);
        }

        [Fact]
        public void Withdraw()
        {
            var items = TestItems.Prepare();
            var account = CreateTestAccount(items);
            items.accountService.Deposit(account.Number, 500);
            items.accountService.Withdraw(account.Number, 200);

            account = items.accountService.GetAccountByAccountNumber(account.Number);

            Assert.True(account.Balance == 300);
        }

        [Fact]
        public void Transfer()
        {
            var items = TestItems.Prepare();
            var sender = CreateTestAccount(items);
            var receiver = CreateTestAccount(items, "rachel@gmail.com", "1122334455");

            // deposit 500 first
            items.accountService.Deposit(sender.Number, 500);
            bool transferSuccess = items.accountService.Transfer(sender.Number, receiver.Number, 500);

            sender = items.accountService.GetAccountByAccountNumber(sender.Number);
            receiver = items.accountService.GetAccountByAccountNumber(receiver.Number);

            Assert.True(transferSuccess);
            Assert.True(sender.Balance == 0);
            Assert.True(receiver.Balance == 500);
        }

        [Fact]
        public void ListTransactions()
        {
            var items = TestItems.Prepare();
            var account = CreateTestAccount(items);

            items.transactionService.CreateTransaction(account.Number, (int)Transaction.Types.CREDIT, 500, "");
            items.transactionService.CreateTransaction(account.Number, (int)Transaction.Types.DEBIT, 100, "");

            var q = new Dictionary<string, object>();
            q.Add("Account", account.Number);
            var transactions = items.transactionService.GetTransactions(q);

            Assert.NotEmpty(transactions);
            Assert.True(transactions.Count == 2);
            Assert.True(transactions[0].Amount == 500);
        }

        public Account CreateTestAccount(TestItems items)
        {
            return CreateTestAccount(items, "johndoe@gmail.com", "1234567890");
        }

        public Account CreateTestAccount(TestItems items, string email, string accountNumber)
        {
            var customer = items.customerService.CreateCustomer(email, "John", "Doe");
            // approve registration and create account for login
            items.customerService.ApproveCustomer(customer.Email, "");
            var account = items.accountService.CreateAccount(customer.Email, accountNumber, "0000");

            return account;
        }
    }
}