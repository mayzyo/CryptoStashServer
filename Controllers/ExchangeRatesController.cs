using CryptoStashStats.Data;
using CryptoStashStats.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace CryptoStashStats.Controllers
{
    [Route("CurrencyExchanges/{exchangeId}/ExchangeRates")]
    [ApiController]
    public class ExchangeRatesController : ControllerBase
    {
        private readonly FinanceContext context;

        public ExchangeRatesController(FinanceContext context)
        {
            this.context = context;
        }

        // GET: /CurrencyExchanges/5/ExchangeRates?currencyId=&cursor=&size=
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ExchangeRate>>> GetExchangeRates(
            int exchangeId,
            int? currencyId,
            int cursor = -1,
            int size = 10
            )
        {
            var miningAccountBalances = currencyId == null
                ? context.ExchangeRates.Include(e => e.BuyerCurrency)
                : context.ExchangeRates.Where(e => e.BuyerCurrency.Id == currencyId);

            return await miningAccountBalances
                .Where(e => e.CurrencyExchange.Id == exchangeId)
                .Include(e => e.SellerCurrency)
                .OrderByDescending(e => e.Created)
                .Pagination(cursor, size)
                .ToListAsync();
        }

        // GET /CurrencyExchanges/5/ExchangeRates/5
        [HttpGet("{id}")]
        [Authorize("enumerate_access")]
        public async Task<ActionResult<ExchangeRate>> GetExchangeRate(int exchangeId, int id)
        {
            var exchangeRate = await context.ExchangeRates
                .Include(e => e.CurrencyExchange)
                .Include(e => e.BuyerCurrency)
                .Include(e => e.SellerCurrency)
                .FirstOrDefaultAsync(e => e.Id == id);

            if (exchangeRate == default(ExchangeRate) || exchangeId != exchangeRate.CurrencyExchange.Id)
            {
                return NotFound();
            }

            return exchangeRate;
        }

        // PUT: /CurrencyExchanges/5/ExchangeRates/5
        // Does not need owner verification because it's an internal API.
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        [Authorize("manage_access")]
        public async Task<IActionResult> PutExchangeRate(int exchangeId, int id, ExchangeRate exchangeRate)
        {
            if (id != exchangeRate.Id || exchangeId != exchangeRate.CurrencyExchange.Id)
            {
                return BadRequest();
            }

            context.Entry(exchangeRate).State = EntityState.Modified;

            try
            {
                await context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ExchangeRateExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: /CurrencyExchanges/5/ExchangeRates
        // Does not need owner verification because it's an internal API.
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        [Authorize("manage_access")]
        public async Task<ActionResult<ExchangeRate>> PostExchangeRate(int exchangeId, ExchangeRate exchangeRate)
        {
            if (exchangeId != exchangeRate.CurrencyExchange.Id)
            {
                return BadRequest();
            }

            // Get existing CurrencyExchange from database.
            exchangeRate.CurrencyExchange = await context.CurrencyExchanges
                .FindAsync(exchangeId);

            if (exchangeRate.CurrencyExchange == null)
            {
                return BadRequest();
            }

            exchangeRate.BuyerCurrency = await context.Currencies
                .FindAsync(exchangeRate.BuyerCurrency.Id);

            exchangeRate.SellerCurrency = await context.Currencies
                .FindAsync(exchangeRate.SellerCurrency.Id);

            context.ExchangeRates.Add(exchangeRate);
            await context.SaveChangesAsync();

            return CreatedAtAction(
                "GetExchangeRate",
                new { exchangeId, id = exchangeRate.Id },
                exchangeRate
                );
        }

        // DELETE: /CurrencyExchanges/5/ExchangeRates/5
        // Does not need owner verification because it's an internal API.
        [HttpDelete("{id}")]
        [Authorize("manage_access")]
        public async Task<IActionResult> DeleteExchangeRate(int exchangeId, int id)
        {
            var exchangeRate = await context.ExchangeRates.FindAsync(id);

            if (exchangeRate == null || exchangeId != exchangeRate.CurrencyExchange.Id)
            {
                return NotFound();
            }

            context.ExchangeRates.Remove(exchangeRate);
            await context.SaveChangesAsync();

            return NoContent();
        }

        private bool ExchangeRateExists(int id)
        {
            return context.ExchangeRates.Any(e => e.Id == id);
        }
    }
}
