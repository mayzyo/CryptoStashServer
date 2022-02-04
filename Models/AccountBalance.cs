using CryptoStashServer.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CryptoStashServer.Models
{
    public abstract class AccountBalance : BaseEntity
    {
        public double Savings { get; set; }
        public int TokenId { get; set; } // Unique FK need to be declared to use in DataContext.
        public Token Token { get; set; } // Many to One
    }
}