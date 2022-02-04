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
    [Route("MiningAccounts/{accountId}/Balances")]
    [ApiController]
    [Authorize("mining_audience")]
    public class MiningAccountBalancesController : ControllerBase
    {
        private readonly MiningContext context;

        private IQueryable<MiningAccountBalance> MiningAccountBalances
        {
            get
            {
                var owner = User.FindFirstValue(ClaimTypes.NameIdentifier);
                // Check if sub is assigned, lacking of is indictive of a client credential access; otherwise a code flow (user) access.
                var balances = owner != null
                    ? context.MiningAccountBalances.Where(e => e.MiningAccount.Owner == owner)
                    : context.MiningAccountBalances;
                return balances;
            }
        }

        public MiningAccountBalancesController(MiningContext context)
        {
            this.context = context;
        }

        // GET: /MiningAccounts/5/Balances?tokenId=&cursor=&size=
        [HttpGet]
        [Authorize("read_access")]
        public async Task<ActionResult<IEnumerable<MiningAccountBalance>>> GetMiningAccountBalances(
            int accountId,
            int? tokenId,
            int cursor = -1,
            int size = 10
            )
        {
            var miningAccountBalances = tokenId == null 
                ? MiningAccountBalances 
                : MiningAccountBalances.Where(e => e.Token.Id == tokenId);

            return await miningAccountBalances
                .Where(e => e.MiningAccount.Id == accountId)
                .Include(e => e.Token)
                .OrderByDescending(e => e.Created)
                .Pagination(cursor, size)
                .ToListAsync();
        }

        // GET /MiningAccounts/5/Balances/5
        [HttpGet("{id}")]
        [Authorize("read_access")]
        public async Task<ActionResult<MiningAccountBalance>> GetMiningAccountBalance(int accountId, int id)
        {
            var miningAccountBalance = await MiningAccountBalances
                .Include(e => e.MiningAccount)
                .FirstOrDefaultAsync(e => e.Id == id);

            if (miningAccountBalance == default(MiningAccountBalance) || miningAccountBalance.MiningAccount.Id != accountId)
            {
                return NotFound();
            }

            return miningAccountBalance;
        }

        // PUT: /MiningAccounts/5/Balances/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        [Authorize("manage_access")]
        public async Task<IActionResult> PutMiningAccountBalance(int accountId, int id, MiningAccountBalance miningAccountBalance)
        {
            if (id != miningAccountBalance.Id || accountId != miningAccountBalance.MiningAccount.Id)
            {
                return BadRequest();
            }

            context.Entry(miningAccountBalance).State = EntityState.Modified;

            try
            {
                await context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MiningAccountBalanceExists(id))
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

        // POST: /MiningAccounts/5/Balances
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        // Does not need owner verification because it's an internal API.
        [HttpPost]
        [Authorize("manage_access")]
        public async Task<ActionResult<MiningAccountBalance>> PostMiningAccountBalance(int accountId, MiningAccountBalance miningAccountBalance)
        {
            if (accountId != miningAccountBalance.MiningAccount.Id)
            {
                return BadRequest();
            }

            // Get existing account from database.
            miningAccountBalance.MiningAccount = await context.MiningAccounts
                .FindAsync(miningAccountBalance.MiningAccount.Id);

            // Get existing token from database.
            miningAccountBalance.Token = await context.Tokens
                .FindAsync(miningAccountBalance.Token.Id);

            if (miningAccountBalance.MiningAccount == null)
            {
                return BadRequest("Associated mining pool account not found");
            }
            else if (miningAccountBalance.Token == null)
            {
                return BadRequest("Associated token doesn't exist");
            }

            context.MiningAccountBalances.Add(miningAccountBalance);
            await context.SaveChangesAsync();

            return CreatedAtAction(
                "GetMiningAccountBalance",
                new { accountId, id = miningAccountBalance.Id },
                miningAccountBalance
                );
        }

        // DELETE: /MiningAccounts/5/Balances/5
        // Does not need owner verification because it's an internal API.
        [HttpDelete("{id}")]
        [Authorize("manage_access")]
        public async Task<IActionResult> DeleteMiningAccountBalance(int accountId, int id)
        {
            var miningAccountBalance = await context.MiningAccountBalances.FindAsync(id);
            if (miningAccountBalance == null || miningAccountBalance.MiningAccount.Id != accountId)
            {
                return NotFound();
            }

            context.MiningAccountBalances.Remove(miningAccountBalance);
            await context.SaveChangesAsync();

            return NoContent();
        }

        private bool MiningAccountBalanceExists(int id)
        {
            return context.MiningAccountBalances.Any(e => e.Id == id);
        }
    }
}
