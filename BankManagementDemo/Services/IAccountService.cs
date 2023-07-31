using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using BankManagementDemo.Models;

namespace BankManagementDemo.Services
{
    public interface IAccountService
    {
        public Task<IEnumerable<Account>> GetAccountsByCustomerId(int id);

        public Task<Account> GetAccountByAccountNumber(long accountNumber);
        public Task<bool> CheckIfAccountExists(BaseAccount account);
        public Task CreateAccount(Account account);
        public Task DeleteAccount(long accountNumber);
        //public Task WithdrawMoney(Account account, decimal amount);
        //public Task Deposit(Account account, decimal amount);
    }
}
