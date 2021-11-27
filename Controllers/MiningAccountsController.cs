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
    [Authorize("mining_audience")]
    public class MiningAccountsController : ControllerBase
    {
        private readonly MiningContext context;

        private IQueryable<MiningAccount> MiningAccounts
        {
            get
            {
                var owner = User.FindFirstValue(ClaimTypes.NameIdentifier);
                // Check if sub is assigned, lacking of is indictive of a client credential access; otherwise a code flow (user) access.
                var miningAccounts = owner != null
                    ? context.MiningAccounts.Where(e => e.Owner == owner)
                    : context.MiningAccounts;
                return miningAccounts;
            }
        }

        public MiningAccountsController(MiningContext context)
        {
            this.context = context;
        }

        // GET: /MiningAccounts
        [HttpGet]
        [Authorize("read_access")]
        public async Task<ActionResult<IEnumerable<MiningAccount>>> GetMiningAccounts()
        {
            return await MiningAccounts
                .Include(e => e.MiningPool)
                .ToListAsync();
        }

        // GET /MiningAccounts/5
        [HttpGet("{id}")]
        [Authorize("read_access")]
        public async Task<ActionResult<MiningAccount>> GetMiningAccount(int id)
        {
            var miningAccounts = await MiningAccounts
                .Include(e => e.MiningPool)
                .Include(e => e.MiningWorkers)
                .FirstOrDefaultAsync(e => e.Id == id);

            if (miningAccounts == default(MiningAccount))
            {
                return NotFound();
            }

            return miningAccounts;
        }

        // PUT: /MiningAccounts/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        [Authorize("manage_access")]
        public async Task<IActionResult> PutMiningAccount(int id, MiningAccount miningAccount)
        {
            if (id != miningAccount.Id)
            {
                return BadRequest();
            }

            if (await NotMiningAccountOwner(id))
            {
                return Forbid();
            }

            if(miningAccount.MiningPool != null)
            {
                miningAccount.MiningPool = await context.MiningPools
                    .FindAsync(miningAccount.MiningPool.Id);
            }

            context.Entry(miningAccount).State = EntityState.Modified;

            try
            {
                await context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MiningAccountExists(id))
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

        // POST: /MiningAccounts
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        [Authorize("write_access")]
        public async Task<ActionResult<MiningAccount>> PostMiningAccount(MiningAccount miningAccount)
        {
            if (NotMiningAccountOwner(miningAccount))
            {
                return Forbid();
            }

            // MiningAccount don't necessarily have to be associated with a Mining Pool. It could be solo mining.
            if (miningAccount.MiningPool != default(MiningPool))
            {
                // Use existing MiningPool to avoid changes. Changes should be made using MiningPoolController.
                miningAccount.MiningPool = await context.MiningPools.FindAsync(miningAccount.MiningPool.Id);
                if(miningAccount.MiningPool == default(MiningPool))
                {
                    return BadRequest("Associated mining pool not found!");
                }
            }

            context.MiningAccounts.Add(miningAccount);
            await context.SaveChangesAsync();

            return CreatedAtAction("GetMiningAccount", new { id = miningAccount.Id }, miningAccount);
        }

        // DELETE: /MiningAccounts/5
        [HttpDelete("{id}")]
        [Authorize("write_access")]
        public async Task<IActionResult> DeleteMiningAccount(int id)
        {
            var miningAccounts = await context.MiningAccounts.FindAsync(id);
            if (miningAccounts == null)
            {
                return NotFound();
            }

            if (NotMiningAccountOwner(miningAccounts))
            {
                return Forbid();
            }

            context.MiningAccounts.Remove(miningAccounts);
            await context.SaveChangesAsync();

            return NoContent();
        }

        private bool MiningAccountExists(int id)
        {
            return context.MiningAccounts.Any(e => e.Id == id);
        }

        private bool NotMiningAccountOwner(MiningAccount miningAccount)
        {
            var owner = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return owner != null 
                && User.FindFirstValue(ClaimTypes.NameIdentifier) != miningAccount.Owner;
        }

        private async Task<bool> NotMiningAccountOwner(int id)
        {
            var miningAccount = await context.MiningAccounts.FindAsync(id);
            context.Entry(miningAccount).State = EntityState.Detached;
            return NotMiningAccountOwner(miningAccount);
        }
    }
}
