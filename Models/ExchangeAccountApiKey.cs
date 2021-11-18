using CryptoStashStats.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CryptoStashStats.Models
{
    public class ExchangeAccountApiKey : BaseEntity
    {
        public string PublicKey { get; set; }
        public string PrivateKey { get; set; }
        public int ExchangeAccountId { get; set; }
        public ExchangeAccount? ExchangeAccount { get; set; } // One to One
    }
}
