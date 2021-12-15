using CryptoStashStats.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CryptoStashStats.Models
{
    public class ExchangeRate : BaseEntity
    {
        public double Current { get; set; }
        public Token BuyerToken { get; set; } // Many to One
        public Token SellerToken { get; set; } // Many to One
        public CurrencyExchange CurrencyExchange { get; set; } // Many to One
    }
}
