using CryptoStashStats.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CryptoStashStats.Models
{
    public class PoolBalance : BaseEntity
    {
        public int Id { get; set; }
        public double Current { get; set; }
        public int MiningPoolId { get; set; }
        public MiningPool MiningPool { get; set; }
        public string Address { get; set; }
    }
}