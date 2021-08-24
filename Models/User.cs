using CryptoStashStats.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CryptoStashStats.Models
{
    public class User : BaseEntity
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string? Apn { get; set; }
    }
}
