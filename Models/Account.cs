using CryptoStashStats.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CryptoStashStats.Models
{
    // Consider renaming to FintechAccount
    public class Account : BaseEntity
    {
        //public string Owner { get; set; }
        //public string identifier { get; set; }
        // DEPRECATED
        public int? UserId { get; set; }
        // Move to separate Object
        public string? AuthJson { get; set; }
        public Provider Provider { get; set; }
        public ICollection<AccountBalance>? AccountBalances { get; set; }
    }
}
