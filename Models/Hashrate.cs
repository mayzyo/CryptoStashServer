using CryptoStashStats.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CryptoStashStats.Models
{
    public class Hashrate : BaseEntity
    {
        public int Id { get; set; }
        public int Current { get; set; }
        public int Average { get; set; }
        public int Reported { get; set; }
        public Worker Worker { get; set; }
    }
}
