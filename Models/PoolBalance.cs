using CryptoStashStats.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CryptoStashStats.Models
{
    // Consider renaming to PoolAccount and giving it an "AccountBalance" which should work for both this and ExchangeAccount (Account currently).
    public class PoolBalance
    {
        public int Id { get; set; }
        public string Owner { get; set; }
        public double Current { get; set; }
        public int MiningPoolId { get; set; }
        public MiningPool MiningPool { get; set; }
        public string Address { get; set; }
        // Wallet address is sometimes not the login account.
        public string? LoginAccount { get; set; }
    }
}