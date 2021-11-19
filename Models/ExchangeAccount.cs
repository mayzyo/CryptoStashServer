using CryptoStashStats.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CryptoStashStats.Models
{
    public class ExchangeAccount : BaseEntity
    {
        public string Owner { get; set; }
        public string Name { get; set; }
        public ExchangeAccountApiKey ExchangeAccountApiKey { get; set; } // One to One
        public int CurrencyExchangeId { get; set; }
        public CurrencyExchange CurrencyExchange { get; set; } // Many to One
        public ICollection<Currency>? Currencies { get; set; } // Many to Many
    }
}