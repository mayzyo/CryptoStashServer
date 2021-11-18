using CryptoStashStats.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CryptoStashStats.Models
{
    public abstract class AccountBalance : BaseEntity
    {
        public double Savings { get; set; }
        public int CurrencyId { get; set; } // Unique FK need to be declared to use in DataContext.
        public Currency Currency { get; set; } // Many to One
    }
}