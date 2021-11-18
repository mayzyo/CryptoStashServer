﻿using CryptoStashStats.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CryptoStashStats.Models
{
    public class Wallet : BaseEntity
    {
        public string Owner { get; set; }
        public string Address { get; set; }
        public double? Balance { get; set; }
        public Currency Currency { get; set; }
    }
}