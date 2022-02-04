using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CryptoStashServer.Models
{
    public class ExchangeAccountBalance : AccountBalance
    {
        public ExchangeAccount ExchangeAccount { get; set; } // Many to One
    }
}