using CryptoStashStats.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CryptoStashStats.Models
{
    public class Coin : BaseEntity
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string Ticker { get; set; }
    }
}
