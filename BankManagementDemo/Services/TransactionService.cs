using BankManagementDemo.Models;
using System.Linq;
using System.Threading.Tasks;

namespace BankManagementDemo.Services
{
    public class TransactionService : ITransactionService
    {
        public async Task Withdraw(long accountNumber, decimal amount)
        {            
            AccountRepository.accounts.Where(w => w.AccountNumber == accountNumber).ToList().ForEach(i => i.Balance = i.Balance-amount);
        }

        public async Task Deposit(long accountNumber, decimal amount)
        {
            AccountRepository.accounts.Where(w => w.AccountNumber == accountNumber).ToList().ForEach(i => i.Balance = i.Balance + amount);
        }
    }
}
