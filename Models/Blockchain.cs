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
        public int? NativeTokenId { get; set; } // Unique FK need to be declared to use in DataContext.
        public Token? NativeToken { get; set; } // One to One
        public ICollection<Token>? Tokens { get; set; } // Many to Many
        public ICollection<Wallet>? Wallets { get; set; } // One to Many

    }
}
