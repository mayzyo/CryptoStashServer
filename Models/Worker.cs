﻿using CryptoStashStats.Utilities;
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
        public string Address { get; set; }
        public MiningPool? MiningPool { get; set; }
        public ICollection<Hashrate>? Hashrates { get; set; }
    }
}
