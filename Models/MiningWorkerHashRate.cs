using CryptoStashServer.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CryptoStashServer.Models
{
    public class MiningWorkerHashRate : BaseEntity
    {
        public int Current { get; set; }
        public int Average { get; set; }
        public int Reported { get; set; }
        public MiningWorker MiningWorker { get; set; } // Many to One
    }
}
