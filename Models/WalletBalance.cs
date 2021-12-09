using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CryptoStashStats.Models
{
    public class WalletBalance : AccountBalance
    {
        public Wallet Wallet { get; set; } // Many to One
    }
}