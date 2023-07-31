using BankManagementDemo.Models;
using System.Threading.Tasks;

namespace BankManagementDemo.Services
{
    public interface ITransactionService
    {
        public Task Withdraw(long accountNumber, decimal amount);
        public Task Deposit(long accountNumber, decimal amount);
    }
}
