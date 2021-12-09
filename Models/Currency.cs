using CryptoStashStats.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CryptoStashStats.Models
{
    public class Currency
    {
        public int Id { get; set; }
        public string Ticker { get; set; }
        public string Name { get; set; }
        public ICollection<MiningPool>? MiningPools { get; set; } // Many to Many, mapped only in mining context
        public ICollection<Wallet>? Wallets { get; set; } // Many to Many, mapped only in finance context
        public ICollection<ExchangeAccount>? ExchangeAccounts { get; set; } //Many to Many, mapped only in finance context
    }
}
