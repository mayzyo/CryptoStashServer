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
    public class AccountBalancesController : ControllerBase
    {
        private readonly FinanceContext context;

        public AccountBalancesController(FinanceContext context)
        {
            this.context = context;
        }

        // GET: /AccountBalances
        [HttpGet]
        [Authorize("enumerate_access")]
        public async Task<ActionResult<IEnumerable<AccountBalance>>> GetAccountBalances()
        {
            return await context.AccountBalance
                .Include(e => e.Account)
                .Include(e => e.Coin)
                .ToListAsync();
        }

        // GET /AccountBalances/5/Secure
        [HttpGet("{id}/Secure")]
        [Authorize("enumerate_access")]
        public async Task<ActionResult<AccountBalance>> SecureGetAccountBalance(int id)
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

        // GET /AccountBalances/5
        [HttpGet("{id}")]
        public async Task<ActionResult<AccountBalance>> GetAccountBalance(int id)
        {
            var accountBalance = await SecureGetAccountBalance(id);

            if (User.FindFirstValue(ClaimTypes.NameIdentifier) != accountBalance.Value.Account.UserId.ToString())
            {
                return Forbid();
            }

            return accountBalance;
        }

        // PUT: /AccountBalances
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut]
        [Authorize("manage_access")]
        public async Task<IActionResult> PutAccountBalance(string coinTicker, int userId, AccountBalance accountBalance)
        {
            if (coinTicker != accountBalance.Coin.Ticker || userId != accountBalance.Account.UserId)
            {
                return BadRequest();
            }

            AccountBalance oldAccountBalance;

            // Get old account balance.
            try
            {
                oldAccountBalance = await context.AccountBalance
                    .Include(e => e.Account)
                    .Include(e => e.Coin)
                    .FirstAsync(e => e.Coin.Ticker == coinTicker && e.Account.UserId == userId);
            }
            catch (InvalidOperationException)
            {
                return NotFound();
            }

            // Update "current balance" in old account balance.
            // TODO: Improve implementation.
            oldAccountBalance.Current = accountBalance.Current;

            context.Entry(oldAccountBalance).State = EntityState.Modified;

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
        [Authorize("manage_access")]
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
        [Authorize("manage_access")]
        public async Task<ActionResult<AccountBalance>> PostAccountBalance(AccountBalance accountBalance)
        {
            // Get existing child objects from database.
            var account = await context.Account
                .FirstOrDefaultAsync(e => e.UserId == accountBalance.Account.UserId);
            var provider = await context.Provider
                .FirstOrDefaultAsync(e => e.Name == accountBalance.Account.Provider.Name);
            var coin = await context.Coin
                .FirstOrDefaultAsync(e => e.Ticker == accountBalance.Coin.Ticker);

            // Attach existing child objects to the new element.
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
        [Authorize("manage_access")]
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
