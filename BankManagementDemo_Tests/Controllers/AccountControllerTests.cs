using AutoFixture;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BankManagementDemo.Controllers;
using BankManagementDemo.Models;
using BankManagementDemo.Services;
using Xunit;

namespace BankManagementDemo_Tests.Controllers
{
    public class AccountControllerTests
    {
        private readonly Mock<IAccountService> _accountServiceMock;
        private readonly Mock<ITransactionService> _transactionServiceMock;
        private readonly AccountController _accountController;
        private readonly Fixture _fixture;

        public AccountControllerTests()
        {
            _fixture = new Fixture();

            _accountServiceMock = new Mock<IAccountService>();
            _transactionServiceMock = new Mock<ITransactionService>();

            _accountController = new AccountController(_accountServiceMock.Object, _transactionServiceMock.Object);
        }               

        [Fact]
        public async Task ShouldGetAccountsReturnAccountsByCustomerId()
        {
            //arrange
            var account = _fixture.Create<Account>();
            var accounts = _fixture.Create<IEnumerable<Account>>();
            var customerId = account.CustomerId;

            _accountServiceMock.Setup(x => x.GetAccountsByCustomerId(customerId)).Returns(Task.FromResult(accounts));

            //act
            var result = await _accountController.GetAccounts(customerId);

            //assert
            result.Should().NotBeNull();
            result.Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task ShouldGetAccountsReturnNotFoundWhenAccountsNotFound()
        {
            //arrange
            var accounts = _fixture.CreateMany<Account>(1);
            var customerId = accounts.FirstOrDefault().CustomerId;
            _accountServiceMock.Setup(x => x.GetAccountsByCustomerId(customerId)).Returns(Task.FromResult(accounts));

            //act
            var result = await _accountController.GetAccounts(_fixture.Create<int>());

            //assert
            result.Should().BeOfType<NotFoundObjectResult>();
        }

        

        [Fact]
        public async Task ShouldCreateAccountReturnNoContentResultOnAccountCreation()
        {
            //arrange
            var newAccount = _fixture.Create<BaseAccount>();
            newAccount.Balance =200;
            var account = _fixture.Create<Account>();

            _accountServiceMock.Setup(x => x.CreateAccount(It.IsAny<Account>())).Returns(Task.FromResult(account));

            //act
            var result = await _accountController.CreateAccount(newAccount);

            //assert
            
            result.Should().BeOfType<NoContentResult>();
        }

        [Fact]
        public async Task ShouldCreateAccountReturnBadObjectResultWithBalanceLessThanHundred()
        {
            //arrange
            var newAccount = _fixture.Create<BaseAccount>();
            newAccount.Balance = 90;
            var account = _fixture.Create<Account>();

            _accountServiceMock.Setup(x => x.CreateAccount(It.IsAny<Account>())).Returns(Task.FromResult(account));

            //act
            var result = await _accountController.CreateAccount(newAccount);

            //assert

            result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public async Task ShouldDeleteAccountReturnNoContentResultOnSuccessfulDelete()
        {
            //arrange
            var account = _fixture.Create<Account>();
            var accountNumber = account.AccountNumber;

            _accountServiceMock.Setup(x => x.GetAccountByAccountNumber(accountNumber)).ReturnsAsync(account);
            _accountServiceMock.Setup(x => x.DeleteAccount(accountNumber));
            //act
            var result = await _accountController.DeleteAccount(accountNumber);

            //assert
            result.Should().BeOfType<NoContentResult>();
        }

        [Fact]
        public async Task ShouldDeleteAccountReturnNotFoundResultOnDeleteFailure()
        {
            //arrange
            var account = _fixture.CreateMany<Account>(0).FirstOrDefault();
            var accountNumber = _fixture.Create<long>();

            _accountServiceMock.Setup(x => x.GetAccountByAccountNumber(accountNumber)).ReturnsAsync(account);
            _accountServiceMock.Setup(x => x.DeleteAccount(accountNumber));

            //act
            var result = await _accountController.DeleteAccount(accountNumber);

            //assert
            result.Should().BeOfType<NotFoundObjectResult>();
        }

        [Fact]
        public async Task ShouldDepositAmountReturnNoContentResultOnSuccessfulUpdate()
        {
            //arrange
            Fixture fixture = new Fixture();
            var updateAccount = fixture.Create<Account>();
            var amount = fixture.Create<int>();
            var accountNumber = updateAccount.AccountNumber;

            _accountServiceMock.Setup(x => x.GetAccountByAccountNumber(accountNumber)).ReturnsAsync(updateAccount);
            _transactionServiceMock.Setup(x => x.Deposit(accountNumber, amount));
            //act
            var result = await _accountController.DepositMoney(accountNumber, amount);

            //assert
            result.Should().NotBeNull();
            result.Should().BeOfType<NoContentResult>();
        }

        [Fact]
        public async Task ShouldWithdrawReturnNoContentResultOnSuccessfulWithdrawal()
        {
            //arrange
            Fixture fixture = new Fixture();
            var updateAccount = fixture.Create<Account>();
            var amount = updateAccount.Balance - ((updateAccount.Balance / 100) * 10);
            var accountNumber = updateAccount.AccountNumber;

            _accountServiceMock.Setup(x => x.GetAccountByAccountNumber(accountNumber)).ReturnsAsync(updateAccount);
            _transactionServiceMock.Setup(x => x.Withdraw(accountNumber, amount));
            //act
            var result = await _accountController.WithdrawMoney(accountNumber, amount);

            //assert
            result.Should().NotBeNull();
            result.Should().BeOfType<NoContentResult>();
        }

        [Fact]
        public async Task ShouldWithdrawReturnBadRequestObjectResultWithdrawalFailure()
        {
            //arrange
            Fixture fixture = new Fixture();
            var updateAccount = fixture.Create<Account>();
            updateAccount.Balance = 100;
            var amount = 95;
            var accountNumber = updateAccount.AccountNumber;

            _accountServiceMock.Setup(x => x.GetAccountByAccountNumber(accountNumber)).ReturnsAsync(updateAccount);
            _transactionServiceMock.Setup(x => x.Withdraw(accountNumber, amount));
            //act
            var result = await _accountController.WithdrawMoney(accountNumber, amount);

            //assert
            result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public async Task ShouldDepositReturnBadRequestObjectResultWithMorethanlimit()
        {
            //arrange
            Fixture fixture = new Fixture();
            var updateAccount = fixture.Create<Account>();
            var amount = 11000;
            var accountNumber = updateAccount.AccountNumber;

            _accountServiceMock.Setup(x => x.GetAccountByAccountNumber(accountNumber)).ReturnsAsync(updateAccount);
            _transactionServiceMock.Setup(x => x.Withdraw(accountNumber, amount));
            //act
            var result = await _accountController.WithdrawMoney(accountNumber, amount);

            //assert
            result.Should().BeOfType<BadRequestObjectResult>();
        }

    }
}
