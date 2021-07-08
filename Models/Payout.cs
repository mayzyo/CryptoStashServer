using CryptoStashStats.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CryptoStashStats.Models
{
    public class Payout : BaseEntity
    {
        public int Id { get; set; }
        public int MiningPoolId { get; set; }
        public MiningPool MiningPool { get; set; }
        public int WalletId { get; set; }
        public Wallet Wallet { get; set; }
        public string TXHash { get; set; }
        public double Amount { get; set; }
        public DateTime Confirmed { get; set; }
        public bool IsConfirmed { get; set; }
        public int ConfirmAttempts { get; set; }
    }
}
