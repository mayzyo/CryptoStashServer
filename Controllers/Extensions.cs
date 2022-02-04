using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CryptoStashServer.Utilities;

namespace CryptoStashServer.Controllers
{
    public static class Extensions
    {
        public static IQueryable<T> Pagination<T>(this IQueryable<T> orderedList, int cursor, int size) where T : IBaseEntity
        {
            return cursor == -1
                ? orderedList
                    .Take(size)
                : orderedList
                    .SkipWhile(e => e.Id != cursor)
                    .Skip(size)
                    .Take(size);
        }
    }
}
