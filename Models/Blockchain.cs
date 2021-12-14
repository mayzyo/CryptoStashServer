using CryptoStashStats.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CryptoStashStats.Models
{
    public class Blockchain
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int? NativeCurrencyId { get; set; } // Ambiguity on dependent side
        public Currency? NativeCurrency { get; set; } // One to One
        public ICollection<Currency>? Currencies { get; set; } // Many to Many
        public ICollection<Wallet>? Wallets { get; set; } // One to Many

    }
}
