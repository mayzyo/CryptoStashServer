using CryptoStashServer.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CryptoStashServer.Models
{
    public class MiningAccountWithdrawal : BaseEntity
    {
        public string? Address { get; set; }
        public string TXHash { get; set; }
        public double Amount { get; set; }
        public DateTime Confirmed { get; set; }
        public bool IsConfirmed { get; set; }
        public int ConfirmAttempts { get; set; }

        public MiningAccount MiningAccount { get; set; }
    }
}
