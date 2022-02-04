using CryptoStashServer.Data;
using CryptoStashServer.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace CryptoStashServer.Controllers
{
    [Route("ExchangeAccounts/{accountId}/Balances")]
    [ApiController]
    [Authorize("finance_audience")]
    public class ExchangeAccountBalancesController : ControllerBase
    {
        private readonly FinanceContext context;

        private IQueryable<ExchangeAccountBalance> ExchangeAccountBalances
        {
            get
            {
                var owner = User.FindFirstValue(ClaimTypes.NameIdentifier);
                // Check if sub is assigned, lacking of is indictive of a client credential access; otherwise a code flow (user) access.
                var balances = owner != null
                    ? context.ExchangeAccountBalances.Where(e => e.ExchangeAccount.Owner == owner)
                    : context.ExchangeAccountBalances;
                return balances;
            }
        }

        public ExchangeAccountBalancesController(FinanceContext context)
        {
            this.context = context;
        }

        // GET: /ExchangeAccounts/5/Balances?currencyId=&cursor=&size=
        [HttpGet]
        [Authorize("read_access")]
        public async Task<ActionResult<IEnumerable<ExchangeAccountBalance>>> GetExchangeAccountBalances(
            int accountId,
            int? currencyId,
            int cursor = -1,
            int size = 10
            )
        {
            var exchangeAccountBalances = currencyId == null
                ? ExchangeAccountBalances.Include(e => e.Token)
                : ExchangeAccountBalances.Where(e => e.Token.Id == currencyId);

            return await exchangeAccountBalances
                .Where(e => e.ExchangeAccount.Id == accountId)
                .OrderByDescending(e => e.Created)
                .Pagination(cursor, size)
                .ToListAsync();
        }

        // GET /ExchangeAccounts/5/Balances/5
        [HttpGet("{id}")]
        [Authorize("read_access")]
        public async Task<ActionResult<ExchangeAccountBalance>> GetExchangeAccountBalance(int accountId, int id)
        {
            var exchangeAccountBalance = await ExchangeAccountBalances
                .Include(e => e.ExchangeAccount)
                .FirstOrDefaultAsync(e => e.Id == id);

            if (exchangeAccountBalance == default(ExchangeAccountBalance) || exchangeAccountBalance.ExchangeAccount.Id != accountId)
            {
                return NotFound();
            }

            return exchangeAccountBalance;
        }

        // PUT: /ExchangeAccounts/5/Balances/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        [Authorize("manage_access")]
        public async Task<IActionResult> PutExchangeAccountBalance(int accountId, int id, ExchangeAccountBalance exchangeAccountBalance)
        {
            if (id != exchangeAccountBalance.Id || accountId != exchangeAccountBalance.ExchangeAccount.Id)
            {
                return BadRequest();
            }

            context.Entry(exchangeAccountBalance).State = EntityState.Modified;

            try
            {
                await context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ExchangeAccountBalanceExists(id))
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

        // POST: /ExchangeAccounts/5/Balances
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        // Does not need owner verification because it's an internal API.
        [HttpPost]
        [Authorize("manage_access")]
        public async Task<ActionResult<ExchangeAccountBalance>> PostExchangeAccountBalance(int accountId, ExchangeAccountBalance exchangeAccountBalance)
        {
            if (accountId != exchangeAccountBalance.ExchangeAccount.Id)
            {
                return BadRequest();
            }

            // Get existing account from database.
            exchangeAccountBalance.ExchangeAccount = await context.ExchangeAccounts
                .Include(e => e.Tokens)
                .FirstOrDefaultAsync(e => e.Id == exchangeAccountBalance.ExchangeAccount.Id);

            // Get existing currency from database.
            exchangeAccountBalance.Token = await context.Tokens
                .FindAsync(exchangeAccountBalance.Token.Id);

            if (exchangeAccountBalance.ExchangeAccount == null)
            {
                return BadRequest("Associated exchange account not found");
            }
            else if (exchangeAccountBalance.Token == null)
            {
                return BadRequest("Associated currency doesn't exist");
            }

            if (exchangeAccountBalance.ExchangeAccount.Tokens == null)
            {
                exchangeAccountBalance.ExchangeAccount.Tokens = new List<Token> { exchangeAccountBalance.Token };
            }
            else if (!exchangeAccountBalance.ExchangeAccount.Tokens.Contains(exchangeAccountBalance.Token))
            {
                exchangeAccountBalance.ExchangeAccount.Tokens.Add(exchangeAccountBalance.Token);
            }

            context.ExchangeAccountBalances.Add(exchangeAccountBalance);
            await context.SaveChangesAsync();

            return CreatedAtAction(
                "GetExchangeAccountBalance",
                new { accountId, id = exchangeAccountBalance.Id },
                exchangeAccountBalance
                );
        }

        // DELETE: /ExchangeAccounts/5/Balances/5
        // Does not need owner verification because it's an internal API.
        [HttpDelete("{id}")]
        [Authorize("manage_access")]
        public async Task<IActionResult> DeleteExchangeAccountBalance(int accountId, int id)
        {
            var exchangeAccountBalance = await context.ExchangeAccountBalances.FindAsync(id);
            if (exchangeAccountBalance == null || exchangeAccountBalance.ExchangeAccount.Id != accountId)
            {
                return NotFound();
            }

            context.ExchangeAccountBalances.Remove(exchangeAccountBalance);
            await context.SaveChangesAsync();

            return NoContent();
        }

        private bool ExchangeAccountBalanceExists(int id)
        {
            return context.ExchangeAccountBalances.Any(e => e.Id == id);
        }
    }
}
