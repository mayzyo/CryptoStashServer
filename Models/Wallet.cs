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
        public int Id { get; set; }
        public int? UserId { get; set; }
        public Coin? Coin { get; set; }
        public string Address { get; set; }
        public double Balance { get; set; }
    }
}