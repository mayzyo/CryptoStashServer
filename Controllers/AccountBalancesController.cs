using CryptoStashStats.Data;
using CryptoStashStats.Models;
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
    public class AccountBalancesController : ControllerBase
    {
        private readonly FinanceContext context;

        public AccountBalancesController(FinanceContext context)
        {
            this.context = context;
        }

        // GET: /AccountBalances
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AccountBalance>>> GetAccountBalances()
        {
            //return await context.AccountBalance.ToListAsync();
            return await context.AccountBalance
                .Include(e => e.Account)
                .Include(e => e.Coin)
                .ToListAsync();
        }

        // GET /AccountBalances/5
        [HttpGet("{id}")]
        public async Task<ActionResult<AccountBalance>> GetAccountBalance(int id)
        {
            var accountBalance = await context.AccountBalance
                .Include(e => e.Account)
                .Include(e => e.Coin)
                .FirstOrDefaultAsync(e => e.Id == id);

            if (accountBalance == default(AccountBalance))
            {
                return NotFound();
            }

            return accountBalance;
        }

        // PUT: /AccountBalances
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut]
        public async Task<IActionResult> PutAccountBalance(string coinTicker, int userId, AccountBalance accountBalance)
        {
            if (coinTicker != accountBalance.Coin.Ticker || userId != accountBalance.Account.UserId)
            {
                return BadRequest();
            }

            AccountBalance existing;

            try
            {
                existing = await context.AccountBalance
                    .Include(e => e.Account)
                    .Include(e => e.Coin)
                    .FirstAsync(e => e.Coin.Ticker == coinTicker && e.Account.UserId == userId);
            }
            catch (InvalidOperationException)
            {
                return NotFound();
            }

            // TODO: Improve implementation.
            existing.Current = accountBalance.Current;

            context.Entry(existing).State = EntityState.Modified;

            try
            {
                await context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }

            return NoContent();
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutAccountBalance(int id, AccountBalance accountBalance)
        {
            if (id != accountBalance.Id)
            {
                return BadRequest();
            }

            context.Entry(accountBalance).State = EntityState.Modified;

            try
            {
                await context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AccountBalanceExists(id))
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

        // POST: /AccountBalances
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<AccountBalance>> PostAccountBalance(AccountBalance accountBalance)
        {
            var account = await context.Account
                .FirstOrDefaultAsync(e => e.UserId == accountBalance.Account.UserId);
            var provider = await context.Provider
                .FirstOrDefaultAsync(e => e.Name == accountBalance.Account.Provider.Name);
            var coin = await context.Coin
                .FirstOrDefaultAsync(e => e.Ticker == accountBalance.Coin.Ticker);

            if (account != null)
            {
                accountBalance.Account = account;
            }

            if (provider != null)
            {
                accountBalance.Account.Provider = provider;
            }

            if (coin != null)
            {
                accountBalance.Coin = coin;
            }

            context.AccountBalance.Add(accountBalance);
            await context.SaveChangesAsync();

            return CreatedAtAction("GetAccountBalance", new { id = accountBalance.Id }, accountBalance);
        }

        // DELETE: /AccountBalances/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAccountBalance(int id)
        {
            var accountBalance = await context.AccountBalance.FindAsync(id);
            if (accountBalance == null)
            {
                return NotFound();
            }

            context.AccountBalance.Remove(accountBalance);
            await context.SaveChangesAsync();

            return NoContent();
        }

        private bool AccountBalanceExists(int id)
        {
            return context.AccountBalance.Any(e => e.Id == id);
        }
    }
}
