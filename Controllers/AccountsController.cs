using CryptoStashStats.Data;
using CryptoStashStats.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace CryptoStashStats.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [Authorize("finance_audience")]
    public class AccountsController : ControllerBase
    {
        private readonly FinanceContext context;

        public AccountsController(FinanceContext context)
        {
            this.context = context;
        }

        // GET: /Accounts
        [HttpGet]
        [Authorize("enumerate_access")]
        public async Task<ActionResult<IEnumerable<Account>>> GetAccounts()
        {
            return await context.Account
                .Include(e => e.Provider)
                .ToListAsync();
        }

        // GET /Accounts/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Account>> GetAccount(int id)
        {
            var account = await context.Account
                .Include(e => e.Provider)
                .Include(e => e.AccountBalances)
                .FirstOrDefaultAsync(e => e.Id == id);

            if (account == default(Account))
            {
                return NotFound();
            }

            return account;
        }

        // PUT: /Accounts/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        [Authorize("manage_access")]
        public async Task<IActionResult> PutAccount(int id, Account account)
        {
            if (id != account.Id || !ValidateJson(account.AuthJson))
            {
                return BadRequest();
            }

            context.Entry(account).State = EntityState.Modified;

            try
            {
                await context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AccountExists(id))
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

        // POST: /Accounts
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        [Authorize("manage_access")]
        public async Task<ActionResult<Account>> PostAccount(Account account)
        {
            if (!ValidateJson(account.AuthJson))
            {
                return BadRequest();
            }

            context.Account.Add(account);
            await context.SaveChangesAsync();

            return CreatedAtAction("GetAccount", new { id = account.Id }, account);
        }

        // DELETE: /Accounts/5
        [HttpDelete("{id}")]
        [Authorize("manage_access")]
        public async Task<IActionResult> DeleteAccount(int id)
        {
            var account = await context.Account.FindAsync(id);
            if (account == null)
            {
                return NotFound();
            }

            context.Account.Remove(account);
            await context.SaveChangesAsync();

            return NoContent();
        }

        private bool AccountExists(int id)
        {
            return context.Account.Any(e => e.Id == id);
        }

        private static bool ValidateJson(string json)
        {
            if(json != null)
            {
                try { return JsonDocument.Parse(json) != null; } catch { }
            }

            return true;
        }
    }
}
