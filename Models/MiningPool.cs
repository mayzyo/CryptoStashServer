using CryptoStashStats.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CryptoStashStats.Models
{
    public class MiningPool : BaseEntity
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public ICollection<PoolBalance>? PoolBalances { get; set; }
        public ICollection<Worker>? Workers { get; set; }
    }
}
