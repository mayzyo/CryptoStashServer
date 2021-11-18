using CryptoStashStats.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CryptoStashStats.Models
{
    public class MiningPool : Provider
    {
        public ICollection<Currency>? Currencies { get; set; } // Many to Many
    }
}