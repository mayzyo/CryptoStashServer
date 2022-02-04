using CryptoStashServer.Data;
using CryptoStashServer.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace CryptoStashServer.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class CurrencyExchangesController : ControllerBase
    {
        private readonly FinanceContext context;

        public CurrencyExchangesController(FinanceContext context)
        {
            this.context = context;
        }

        // GET: /CurrencyExchanges
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CurrencyExchange>>> GetCurrencyExchanges()
        {
            return await context.CurrencyExchanges.ToListAsync();
        }

        // GET /CurrencyExchanges/5
        [HttpGet("{id}")]
        public async Task<ActionResult<CurrencyExchange>> GetCurrencyExchange(int id)
        {
            var currencyExchange = await context.CurrencyExchanges
                .FirstOrDefaultAsync(e => e.Id == id);

            if (currencyExchange == default(CurrencyExchange))
            {
                return NotFound();
            }

            return currencyExchange;
        }

        // PUT: /CurrencyExchanges/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        [Authorize("manage_access")]
        public async Task<IActionResult> PutCurrencyExchange(int id, CurrencyExchange currencyExchange)
        {
            if (id != currencyExchange.Id)
            {
                return BadRequest();
            }

            context.Entry(currencyExchange).State = EntityState.Modified;

            try
            {
                await context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CurrencyExchangeExists(id))
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

        // POST: /CurrencyExchanges
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        [Authorize("manage_access")]
        public async Task<ActionResult<CurrencyExchange>> PostCurrencyExchange(CurrencyExchange currencyExchange)
        {
            context.CurrencyExchanges.Add(currencyExchange);
            await context.SaveChangesAsync();

            return CreatedAtAction("GetCurrencyExchange", new { id = currencyExchange.Id }, currencyExchange);
        }

        // DELETE: /CurrencyExchanges/5
        [HttpDelete("{id}")]
        [Authorize("manage_access")]
        public async Task<IActionResult> DeleteCurrencyExchange(int id)
        {
            var currencyExchange = await context.CurrencyExchanges.FindAsync(id);
            if (currencyExchange == null)
            {
                return NotFound();
            }

            context.CurrencyExchanges.Remove(currencyExchange);
            await context.SaveChangesAsync();

            return NoContent();
        }

        private bool CurrencyExchangeExists(int id)
        {
            return context.CurrencyExchanges.Any(e => e.Id == id);
        }
    }
}
