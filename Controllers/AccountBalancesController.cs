//using CryptoStashStats.Data;
//using CryptoStashStats.Models;
//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Security.Claims;
//using System.Threading.Tasks;

//// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

//namespace CryptoStashStats.Controllers
//{
//    [Route("[controller]")]
//    [ApiController]
//    [Authorize("finance_audience")]
//    public class AccountBalancesController : ControllerBase
//    {
//        private readonly FinanceContext context;

//        public AccountBalancesController(FinanceContext context)
//        {
//            this.context = context;
//        }

//        // GET: /AccountBalances
//        // CHECK this route path has questionable logic
//        [HttpGet]
//        [Authorize("enumerate_access")]
//        public async Task<ActionResult<IEnumerable<AccountBalance>>> GetAccountBalances(int cursor = -1, int size = 10)
//        {
//            return await context.AccountBalances
//                .Include(e => e.Currency)
//                .Pagination(cursor, size)
//                .ToListAsync();
//        }

//        // GET /AccountBalances/5
//        [HttpGet("{id}")]
//        public async Task<ActionResult<AccountBalance>> GetAccountBalance(int id)
//        {
//            var accountBalance = await context.AccountBalances
//                .Include(e => e.Currency)
//                .FirstOrDefaultAsync(e => e.Id == id);

//            if (accountBalance == default(AccountBalance))
//            {
//                return NotFound();
//            }

//            return accountBalance;
//        }

//        // PUT: /AccountBalances/5
//        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
//        [HttpPut("{id}")]
//        [Authorize("manage_access")]
//        public async Task<IActionResult> PutAccountBalance(int id, AccountBalance accountBalance)
//        {
//            if (id != accountBalance.Id)
//            {
//                return BadRequest();
//            }

//            context.Entry(accountBalance).State = EntityState.Modified;

//            try
//            {
//                await context.SaveChangesAsync();
//            }
//            catch (DbUpdateConcurrencyException)
//            {
//                if (!AccountBalanceExists(id))
//                {
//                    return NotFound();
//                }
//                else
//                {
//                    throw;
//                }
//            }

//            return NoContent();
//        }

//        // POST: /AccountBalances
//        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
//        [HttpPost]
//        [Authorize("manage_access")]
//        public async Task<ActionResult<AccountBalance>> PostAccountBalance(AccountBalance accountBalance)
//        {
//            // Get existing currency from database.
//            accountBalance.Currency = await context.Currencies
//                .FirstOrDefaultAsync(e => e.Ticker == accountBalance.Currency.Ticker);

//            if (accountBalance.Currency == default(Currency))
//            {
//                return BadRequest("Associated currency doesn't exist");
//            }

//            context.AccountBalances.Add(accountBalance);
//            await context.SaveChangesAsync();
//            await context.SaveChangesAsync();

//            return CreatedAtAction("GetAccountBalance", new { id = accountBalance.Id }, accountBalance);
//        }

//        // DELETE: /AccountBalances/5
//        [HttpDelete("{id}")]
//        [Authorize("manage_access")]
//        public async Task<IActionResult> DeleteAccountBalance(int id)
//        {
//            var accountBalance = await context.AccountBalances.FindAsync(id);
//            if (accountBalance == null)
//            {
//                return NotFound();
//            }

//            context.AccountBalances.Remove(accountBalance);
//            await context.SaveChangesAsync();

//            return NoContent();
//        }

//        private bool AccountBalanceExists(int id)
//        {
//            return context.AccountBalances.Any(e => e.Id == id);
//        }
//    }
//}
