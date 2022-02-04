using CryptoStashServer.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CryptoStashServer.Models
{
    public abstract class Provider : BaseEntity
    {
        public string Name { get; set; }
    }
}
