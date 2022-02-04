using CryptoStashServer.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CryptoStashServer.Models
{
    public class MiningAccount : BaseEntity
    {
        public string Owner { get; set; }
        public string Identifier { get; set; } // Usually the wallet address
        public int MiningPoolId { get; set; } // Unique FK need to be declared to use in DataContext.
        public MiningPool MiningPool { get; set; } // Many to One
        public ICollection<MiningWorker>? MiningWorkers { get; set; } // One to Many
        //public ICollection<MiningAccountBalance>? MiningAccountBalances { get; set; } // One to Many
    }
}