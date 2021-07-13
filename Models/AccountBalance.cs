using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CryptoStashStats.Models
{
    public class AccountBalance
    {
        public int Id { get; set; }
        public double Current { get; set; }
        public int AccountId { get; set; }
        public Account Account { get; set; }
        public int CoinId { get; set; }
        public Coin Coin { get; set; }
    }
}
