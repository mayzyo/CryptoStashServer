using CryptoStashStats.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CryptoStashStats.Models
{
    public class MiningWorker : BaseEntity
    {
        public string Name { get; set; }
        public int MiningAccountId { get; set; } // Unique FK need to be declared to use in DataContext.
        public MiningAccount MiningAccount { get; set; } // Many to One
        public ICollection<MiningWorkerHashRate>? MiningWorkerHashRates { get; set; } // One to Many
    }
}