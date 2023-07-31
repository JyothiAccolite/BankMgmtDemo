using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Threading.Tasks;
using BankManagementDemo.Models;
using BankManagementDemo.Services;
using System.Linq;

namespace BankManagementDemo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : Controller
    {
        private readonly IAccountService _accountService;
        private readonly ITransactionService _transactionService;
        public AccountController(IAccountService accountService, ITransactionService transactionService)
        {
            _accountService = accountService;
            _transactionService= transactionService;
        }
        
        /// <summary>
        /// Get Account details based on CustomerId
        /// </summary>
        /// <param name="customerId"></param>
        /// <returns></returns>
        [HttpGet("{customerId}")]
        [SwaggerResponse(StatusCodes.Status200OK)]
        [SwaggerResponse(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetAccounts(int customerId)
        {
            var response = await _accountService.GetAccountsByCustomerId(customerId);

            if (response?.ToList().Count<1) return NotFound("Accounts Do not exist for the given Customer Id");

            return Ok(response);
        }

        
        /// <summary>
        /// Creates a bank account. Miminum opening balance $100
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [SwaggerResponse(StatusCodes.Status400BadRequest)]
        [SwaggerResponse(StatusCodes.Status200OK)]
        public async Task<IActionResult> CreateAccount(BaseAccount baseAccount)
        {
            if(baseAccount.Balance < 100)
                return new BadRequestObjectResult("Opening Balance cannot be less than $100");

            //check if the same Account is already existing based on CustomerId
            if (await _accountService.CheckIfAccountExists(baseAccount))
                return new BadRequestObjectResult("Account already existing with the same accountType or Name");           

            Account account=BuildAccount(baseAccount);

            //check if Account is already existing based on id
            await _accountService.CreateAccount(account);
            

            return NoContent();
        }
        
        /// <summary>
        /// Deletes an existing account based on AccountNumber
        /// </summary>
        /// <param name="accountNumber"></param>
        /// <returns></returns>
        [HttpDelete("{accountNumber}")]
        [SwaggerResponse(StatusCodes.Status400BadRequest)]
        [SwaggerResponse(StatusCodes.Status200OK)]
        public async Task<IActionResult> DeleteAccount(long accountNumber)
        {
            var account = await _accountService.GetAccountByAccountNumber(accountNumber);

            if (account is null)
                return NotFound("Account Does not exist");

            await _accountService.DeleteAccount(accountNumber);            
            return NoContent();
        }

        
        /// <summary>
        /// Deposits money to an existing account based on AccountNumber
        /// </summary>
        /// <param name="accountNumber"></param>
        /// <param name="amount"></param>
        /// <returns></returns>
        [HttpPut("Deposit/{accountNumber}")]
        [SwaggerResponse(StatusCodes.Status400BadRequest)]
        [SwaggerResponse(StatusCodes.Status200OK)]
        public async Task<IActionResult> DepositMoney(long accountNumber, decimal amount=0)
        {
            if (amount > 10000)
                return new BadRequestObjectResult("Cannot Deposit more than $10,000 in a single Transaction");

            var account = await _accountService.GetAccountByAccountNumber(accountNumber);

            if (account is null)
                return NotFound("Account Does not exist");

            await _transactionService.Deposit(accountNumber,amount);
            return NoContent();
        }

        
        /// <summary>
        /// Withdraws money from an existing account based on AccountNumber
        /// </summary>
        /// <param name="accountNumber"></param>
        /// <param name="amount"></param>
        /// <returns></returns>
        [HttpPut("Withdraw/{accountNumber}")]
        [SwaggerResponse(StatusCodes.Status400BadRequest)]
        [SwaggerResponse(StatusCodes.Status200OK)]
        public async Task<IActionResult> WithdrawMoney(long accountNumber, decimal amount=0)
        {
            
            var account = await _accountService.GetAccountByAccountNumber(accountNumber);
            if (account is null)
                return NotFound("Account Does not exist");

            var withdrawPercentage= (amount/account.Balance) * 100;
            if (withdrawPercentage > 90)
                return new BadRequestObjectResult("Cannot Withdraw more than 90% from your account");            

            await _transactionService.Withdraw(accountNumber, amount);
            return NoContent();
        }

        #region private methods
        private Account BuildAccount(BaseAccount baseAccount)
        {
            Account account = new Account();
            Random rnd = new Random();
            account.AccountNumber = rnd.Next();
            account.CustomerId = baseAccount.CustomerId;
            account.FirstName = baseAccount.FirstName;
            account.LastName = baseAccount.LastName;
            account.Balance = baseAccount.Balance;
            account.AccountType = baseAccount.AccountType;
            account.LastUpdatedDate = DateTime.UtcNow;

            return account;

        }
        #endregion
        
    }
}
