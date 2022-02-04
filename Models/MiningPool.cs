using CryptoStashServer.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CryptoStashServer.Models
{
    public class MiningPool : Provider
    {
        public ICollection<Token>? Tokens { get; set; } // Many to Many
    }
}