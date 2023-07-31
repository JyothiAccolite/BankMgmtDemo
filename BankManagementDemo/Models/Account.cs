using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BankManagementDemo.Models
{

    public class Account
    {        
        public int CustomerId { get; set; }

        public long AccountNumber { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string AccountType { get; set; } = string.Empty;
        public decimal Balance { get; set; } 
        public DateTime LastUpdatedDate { get; set; } = DateTime.UtcNow;
    }

    public class BaseAccount
    {
        public int CustomerId { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public decimal Balance { get; set; } = decimal.MinValue;
        public string AccountType { get; set; } = string.Empty;
    }
    
}

