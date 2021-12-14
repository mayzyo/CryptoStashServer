using CryptoStashStats.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CryptoStashStats.Models
{
    public class Wallet : BaseEntity
    {
        public string Owner { get; set; }
        public string Address { get; set; }
        public int BlockchainId { get; set; } // Unique FK need to be declared to use in DataContext.
        public Blockchain Blockchain { get; set; } // Many to One
        public ICollection<Currency>? Currencies { get; set; } // Many to Many
    }
}