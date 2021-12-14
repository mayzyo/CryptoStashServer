using CryptoStashStats.Data;
using CryptoStashStats.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace CryptoStashStats.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class CurrenciesController : ControllerBase
    {
        private readonly FinanceContext financeContext;
        private readonly MiningContext miningContext;

        public CurrenciesController(FinanceContext financeContext, MiningContext miningContext)
        {
            this.financeContext = financeContext;
            this.miningContext = miningContext;
        }

        // GET: /Currencies
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Currency>>> GetCurrencies()
        {
            return await financeContext.Currencies.ToListAsync();
        }

        // GET /Currencies/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Currency>> GetCurrency(int id)
        {
            var currency = await financeContext.Currencies.FindAsync(id);

            if (currency == default(Currency))
            {
                return NotFound();
            }

            return currency;
        }

        // PUT: /Currencies/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        [Authorize("manage_access")]
        public async Task<IActionResult> PutCurrency(int id, Currency currency)
        {
            if (id != currency.Id)
            {
                return BadRequest();
            }

            var status = await PutCurrency(id, currency, financeContext);
            if(status != await PutCurrency(id, currency, miningContext))
            {
                throw new Exception("Out of Sync!");
            }

            if(status == 1)
            {
                return BadRequest();
            }
            else if(status == 2)
            {
                return NotFound();
            }

            return NoContent();
        }
        public async Task<int> PutCurrency<T>(int id, Currency currency, T context) where T : DbContext, ICurrencyContext
        {
            if (id != currency.Id)
            {
                return 1; // BadRequest();
            }

            context.Entry(currency).State = EntityState.Modified;

            try
            {
                await context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CurrencyExists(id, context))
                {
                    return 2; // NotFound();
                }
                else
                {
                    throw;
                }
            }

            return 0; // NoContent();
        }

        // POST: /Currencies
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        [Authorize("manage_access")]
        public async Task<ActionResult<Currency>> PostCurrency(Currency currency)
        {
            await PostCurrency(currency, financeContext);
            await PostCurrency(currency, miningContext);

            return CreatedAtAction("GetCurrency", new { id = currency.Id }, currency);
        }

        public async Task PostCurrency<T>(Currency currency, T context) where T : DbContext, ICurrencyContext
        {
            context.Currencies.Add(currency);
            await context.SaveChangesAsync();
        }

        // DELETE: /Currencies/5
        [HttpDelete("{id}")]
        [Authorize("manage_access")]
        public async Task<IActionResult> DeleteCurrency(int id)
        {
            var status = await DeleteCurrency(id, financeContext);
            if (status != await DeleteCurrency(id, miningContext))
            {
                throw new Exception("Out of Sync!");
            }

            if (status == 1)
            {
                return NotFound();
            }

            return NoContent();
        }

        public async Task<int> DeleteCurrency<T>(int id, T context) where T : DbContext, ICurrencyContext
        {
            var Currency = await context.Currencies.FindAsync(id);
            if (Currency == null)
            {
                return 1;
            }

            context.Currencies.Remove(Currency);
            await context.SaveChangesAsync();

            return 0;
        }

        // GET: /Currencies/Contexts/MiningContext
        // For debuggin only
        [HttpGet("contexts/{contextId}")]
        [Authorize("manage_access")]
        public async Task<ActionResult<IEnumerable<Currency>>> GetContextCurrencies(string contextId)
        {
            if (contextId == "MiningContext")
            {
                return await miningContext.Currencies.ToListAsync();
            }
            else if (contextId == "FinanceContext")
            {
                return await financeContext.Currencies.ToListAsync();
            }

            return NotFound();
        }

        //// GET /Currencies/5/Wallets?cursor=&size=
        //[HttpGet("{id}/Wallets")]
        //public async Task<ActionResult<IEnumerable<Wallet>>> GetCurrencyWallets(int id, int cursor = -1, int size = 10)
        //{
        //    var currency = await financeContext.Currencies.FindAsync(id);

        //    if (currency == default(Currency))
        //    {
        //        return NotFound();
        //    }

        //    var wallets = await financeContext.Wallets
        //        .Where(e => e.Currencies.Contains(currency))
        //        .Pagination(cursor, size)
        //        .ToListAsync();

        //    return wallets;
        //}

        private static bool CurrencyExists(int id, ICurrencyContext context)
        {
            return context.Currencies.Any(e => e.Id == id);
        }
    }
}
