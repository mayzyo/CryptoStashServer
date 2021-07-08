using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CryptoStashStats.Utilities
{
    public class BaseEntity
    {
        public DateTime Created { get; set; }
        public DateTime LastModified { get; set; }
    }
}
