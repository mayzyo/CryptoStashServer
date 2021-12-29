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
    [Route("[controller]")]
    [ApiController]
    [Authorize("finance_audience")]
    public class ExchangeAccountsController : ControllerBase
    {
        private readonly FinanceContext context;

        private IQueryable<ExchangeAccount> ExchangeAccounts
        {
            get
            {
                var owner = User.FindFirstValue(ClaimTypes.NameIdentifier);
                // Check if sub is assigned, lacking of is indictive of a client credential access; otherwise a code flow (user) access.
                var exchangeAccounts = owner != null
                    ? context.ExchangeAccounts.Where(e => e.Owner == owner)
                    : context.ExchangeAccounts;
                return exchangeAccounts;
            }
        }

        private IQueryable<ExchangeAccountApiKey> ExchangeAccountApiKeys
        {
            get
            {
                var owner = User.FindFirstValue(ClaimTypes.NameIdentifier);
                // Check if sub is assigned, lacking of is indictive of a client credential access; otherwise a code flow (user) access.
                var exchangeAccountApiKeys = owner != null
                    ? context.ExchangeAccountApiKeys.Where(e => e.ExchangeAccount!.Owner == owner)
                    : context.ExchangeAccountApiKeys;
                return exchangeAccountApiKeys;
            }
        }

        public ExchangeAccountsController(FinanceContext context)
        {
            this.context = context;
        }

        // GET: /ExchangeAccounts
        [HttpGet]
        [Authorize("read_access")]
        public async Task<ActionResult<IEnumerable<ExchangeAccount>>> GetExchangeAccounts()
        {
            return await ExchangeAccounts
                .Include(e => e.CurrencyExchange)
                .ToListAsync();
        }

        // GET /ExchangeAccounts/5
        [HttpGet("{id}")]
        [Authorize("read_access")]
        public async Task<ActionResult<ExchangeAccount>> GetExchangeAccount(int id)
        {
            var exchangeAccounts = await ExchangeAccounts
                .Include(e => e.CurrencyExchange)
                .Include(e => e.ExchangeAccountApiKey)
                .Include(e => e.Tokens)
                .FirstOrDefaultAsync(e => e.Id == id);

            if (exchangeAccounts == default(ExchangeAccount))
            {
                return NotFound();
            }

            return exchangeAccounts;
        }

        // PUT: /ExchangeAccounts/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        [Authorize("manage_access")]
        public async Task<IActionResult> PutExchangeAccount(int id, ExchangeAccount exchangeAccount)
        {
            if (id != exchangeAccount.Id)
            {
                return BadRequest();
            }

            if (await NotExchangeAccountOwner(id))
            {
                return Forbid();
            }

            context.Entry(exchangeAccount).State = EntityState.Modified;

            try
            {
                await context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ExchangeAccountExists(id))
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

        // POST: /ExchangeAccounts
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        [Authorize("write_access")]
        public async Task<ActionResult<ExchangeAccount>> PostExchangeAccount(ExchangeAccount exchangeAccount)
        {
            if (NotExchangeAccountOwner(exchangeAccount))
            {
                return Forbid();
            }

            // Use existing MiningPool to avoid changes. Changes should be made using CurrencyExchangesController.
            exchangeAccount.CurrencyExchange = await context.CurrencyExchanges
                .FindAsync(exchangeAccount.CurrencyExchange.Id);

            if (exchangeAccount.CurrencyExchange == default(CurrencyExchange))
            {
                return BadRequest("Associated currency exchange not found!");
            }

            // Use existing Currency to avoid changes. Changes should be made using CurrencyController.
            if (exchangeAccount.Tokens != null)
            {
                exchangeAccount.Tokens = await context.Tokens
                    .Where(e => exchangeAccount.Tokens.Contains(e))
                    .ToListAsync();
            }

            context.ExchangeAccountApiKeys.Add(exchangeAccount.ExchangeAccountApiKey);

            context.ExchangeAccounts.Add(exchangeAccount);
            await context.SaveChangesAsync();

            return CreatedAtAction("GetExchangeAccount", new { id = exchangeAccount.Id }, exchangeAccount);
        }

        // DELETE: /ExchangeAccounts/5
        [HttpDelete("{id}")]
        [Authorize("write_access")]
        public async Task<IActionResult> DeleteExchangeAccount(int id)
        {
            var exchangeAccounts = await context.ExchangeAccounts
                .Include(e => e.ExchangeAccountApiKey)
                .FirstOrDefaultAsync(e => e.Id == id);

            if (exchangeAccounts == null)
            {
                return NotFound();
            }

            if (NotExchangeAccountOwner(exchangeAccounts))
            {
                return Forbid();
            }

            //context.ExchangeAccounts.Remove(exchangeAccounts);
            context.Remove(exchangeAccounts);
            await context.SaveChangesAsync();

            return NoContent();
        }

        // GET: /ExchangeAccounts/ApiKeys
        [HttpGet("ApiKeys")]
        [Authorize("manage_access")]
        public async Task<ActionResult<IEnumerable<ExchangeAccountApiKey>>> GetExchangeAccountsApiKeys()
        {
            return await ExchangeAccountApiKeys
                .Include(e => e.ExchangeAccount)
                .ThenInclude(e => e.CurrencyExchange)
                .ToListAsync();
        }

        // PUT: /ExchangeAccounts/5/ApiKeys
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}/ApiKeys")]
        [Authorize("write_access")]
        public async Task<IActionResult> PutExchangeAccountApiKey(int id, ExchangeAccountApiKey exchangeAccountApiKey)
        {
            var exchangeAccount = await ExchangeAccounts
                .Include(e => e.ExchangeAccountApiKey)
                .FirstOrDefaultAsync(e => e.Id == id);

            if (exchangeAccount == default(ExchangeAccount))
            {
                return NotFound();
            }

            if (NotExchangeAccountOwner(exchangeAccount))
            {
                return Forbid();
            }

            exchangeAccountApiKey.ExchangeAccount = exchangeAccount;
            exchangeAccount.ExchangeAccountApiKey = exchangeAccountApiKey;

            context.Entry(exchangeAccount).State = EntityState.Modified;

            try
            {
                await context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ExchangeAccountExists(id))
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

        // PUT: /ExchangeAccounts/5/Tokens
        [HttpPut("{id}/Tokens")]
        [Authorize("manage_access")]
        public async Task<IActionResult> PutExchangeAccountCurrencies(int id, ICollection<Token> tokens)
        {
            var exchangeAccount = await context.ExchangeAccounts
                .Include(e => e.Tokens)
                .FirstAsync(e => e.Id == id);

            exchangeAccount.Tokens = await context.Tokens
                .Where(e => tokens.Contains(e))
                .ToListAsync();

            context.Entry(exchangeAccount).State = EntityState.Modified;

            try
            {
                await context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ExchangeAccountExists(id))
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


        private bool ExchangeAccountExists(int id)
        {
            return context.ExchangeAccounts.Any(e => e.Id == id);
        }

        private bool NotExchangeAccountOwner(ExchangeAccount exchangeAccount)
        {
            var owner = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return owner != null 
                && User.FindFirstValue(ClaimTypes.NameIdentifier) != exchangeAccount.Owner;
        }

        private async Task<bool> NotExchangeAccountOwner(int id)
        {
            var exchangeAccount = await context.ExchangeAccounts.FindAsync(id);
            context.Entry(exchangeAccount).State = EntityState.Detached;
            return NotExchangeAccountOwner(exchangeAccount);
        }
    }
}
