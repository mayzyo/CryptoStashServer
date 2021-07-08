using CryptoStashStats.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CryptoStashStats.Models
{
    public class Worker : BaseEntity
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int WalletId { get; set; }
        public Wallet Wallet { get; set; }
        public MiningPool? MiningPool { get; set; }
    }
}
