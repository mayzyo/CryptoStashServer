using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CryptoStashStats.Models
{
    public class Account
    {
        public int Id { get; set; }
        public int? UserId { get; set; }
        public Provider Provider { get; set; }
        public ICollection<AccountBalance>? AccountBalances { get; set; }
    }
}
