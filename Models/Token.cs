using CryptoStashServer.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CryptoStashServer.Models
{
    public class Token
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? Ticker { get; set; }
        public string? Address { get; set; }
        public Blockchain? NativeBlockchain { get; set; } // One to One, mapped only in finance context
        public ICollection<Blockchain>? Blockchains { get; set; } // Many to Many, mapped only in finance context
        public ICollection<MiningPool>? MiningPools { get; set; } // Many to Many, mapped only in mining context
        public ICollection<Wallet>? Wallets { get; set; } // Many to Many, mapped only in finance context
        public ICollection<ExchangeAccount>? ExchangeAccounts { get; set; } //Many to Many, mapped only in finance context
    }
}
