using CryptoStashServer.Models;
using CryptoStashServer.Utilities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CryptoStashServer.Data
{
    public interface ITokenContext
    {
        DbSet<Token> Tokens { get; set; }
    }
}