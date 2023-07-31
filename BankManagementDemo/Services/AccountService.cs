using System.Linq;
using System.Threading.Tasks;
using MongoDB.Driver;
using BankManagementDemo.Models;
using System.Collections.Generic;

namespace BankManagementDemo.Services
{
    public class AccountService : IAccountService
    {
        public async Task CreateAccount(Account account)
        {
            AccountRepository.accounts.Add(account);
        }

        public async Task DeleteAccount(long accountNumber)
        {
            AccountRepository.accounts.Remove(AccountRepository.accounts.Find(x => x.AccountNumber == accountNumber));
        }

        public async Task<IEnumerable<Account>> GetAccountsByCustomerId(int id)
        {
            return AccountRepository.accounts.Where(x => x.CustomerId == id);
        }

        public async Task<Account> GetAccountByAccountNumber(long accountNumber)
        {
            return AccountRepository.accounts.SingleOrDefault(x => x.AccountNumber == accountNumber);
        }

        public async Task<bool> CheckIfAccountExists(BaseAccount account)
        {
            var accnt= AccountRepository.accounts.FirstOrDefault(
                x => (x.CustomerId == account.CustomerId && x.AccountType.ToLower()==account.AccountType.ToLower()) || 
                (x.CustomerId==account.CustomerId && x.FirstName.ToLower()==account.FirstName.ToLower() && x.LastName.ToLower()==account.LastName.ToLower()));

            if (accnt != null)  return true;
                else return false;
        }

    }
}
