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
        public Currency BuyerCurrency { get; set; } // Many to One
        public Currency SellerCurrency { get; set; } // Many to One
        public CurrencyExchange CurrencyExchange { get; set; } // Many to One
    }
}
