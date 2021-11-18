using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CryptoStashStats.Models
{
    public class MiningAccountBalance : AccountBalance
    {
        public MiningAccount MiningAccount { get; set; } // Many to One
    }
}