using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CryptoStashStats.Utilities
{
    public interface IBaseEntity
    {
        int Id { get; set; }
        DateTime Created { get; set; }
        DateTime LastModified { get; set; }
    }

    public class BaseEntity : IBaseEntity
    {
        public int Id { get; set; }
        public DateTime Created { get; set; }
        public DateTime LastModified { get; set; }
    }
}
