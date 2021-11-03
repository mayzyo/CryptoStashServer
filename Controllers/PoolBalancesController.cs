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
    public class PoolBalancesController : ControllerBase
    {
        private readonly MinerContext context;

        private IQueryable<PoolBalance> PoolBalances
        {
            get
            {
                var owner = User.FindFirstValue(ClaimTypes.NameIdentifier);
                // Check if sub is assigned, lacking of is indictive of a client credential access; otherwise a code flow (user) access.
                var poolBalances = owner != null ? context.PoolBalance.Where(e => e.Owner == owner) : context.PoolBalance;
                return poolBalances;
            }
        }

        public PoolBalancesController(MinerContext context)
        {
            this.context = context;
        }

        // GET: /PoolBalances
        [HttpGet]
        [Authorize("enumerate_access")]
        public async Task<ActionResult<IEnumerable<PoolBalance>>> GetPoolBalances()
        {
            return await PoolBalances
                .Include(e => e.MiningPool)
                .ToListAsync();
        }

        // GET /PoolBalances/5
        [HttpGet("{id}")]
        public async Task<ActionResult<PoolBalance>> GetPoolBalance(int id)
        {
            var poolBalance = await PoolBalances
                .Include(e => e.MiningPool)
                .FirstOrDefaultAsync(e => e.Id == id);

            if (poolBalance == default(PoolBalance))
            {
                return NotFound();
            }

            return poolBalance;
        }

        // PUT: /PoolBalances/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        [Authorize("manage_access")]
        public async Task<IActionResult> PutPoolBalance(int id, PoolBalance poolBalance)
        {
            if (id != poolBalance.Id)
            {
                return BadRequest();
            }

            var owner = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (owner != null && poolBalance.Owner != owner)
            {
                return Forbid();
            }

            context.Entry(poolBalance).State = EntityState.Modified;

            try
            {
                await context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PoolBalanceExists(id))
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

        // PUT: /PoolBalances?address=&poolName=
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut]
        [Authorize("manage_access")]
        public async Task<IActionResult> PutPoolBalance(string address, string poolName, PoolBalance poolBalance)
        {
            if (poolName != poolBalance.MiningPool.Name || address != poolBalance.Address)
            {
                return BadRequest();
            }

            PoolBalance oldPoolBalance;

            // Get old pool balance.
            try
            {
                oldPoolBalance = await context.PoolBalance
                    .Include(e => e.MiningPool)
                    .FirstAsync(e => e.MiningPool.Name == poolName && e.Address == address);
            }
            catch (InvalidOperationException)
            {
                return NotFound();
            }

            // Update "current balance" in old pool balance.
            // TODO: Improve implementation.
            oldPoolBalance.Current = poolBalance.Current;

            context.Entry(oldPoolBalance).State = EntityState.Modified;

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

        // POST: /PoolBalances
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        [Authorize("manage_access")]
        public async Task<ActionResult<PoolBalance>> PostPoolBalance(PoolBalance poolBalance)
        {
            // Get child objects from database.
            var miningPool = await context.MiningPool
                .FirstOrDefaultAsync(e => e.Name == poolBalance.MiningPool.Name);

            // Attach child objects to the new element.
            if (miningPool != null)
            {
                poolBalance.MiningPool = miningPool;
            }

            context.PoolBalance.Add(poolBalance);
            await context.SaveChangesAsync();

            return CreatedAtAction("GetPoolBalance", new { id = poolBalance.Id }, poolBalance);
        }

        // DELETE: /PoolBalances/5
        [HttpDelete("{id}")]
        [Authorize("manage_access")]
        public async Task<IActionResult> DeletePoolBalance(int id)
        {
            var poolBalance = await context.PoolBalance.FindAsync(id);
            if (poolBalance == null)
            {
                return NotFound();
            }

            context.PoolBalance.Remove(poolBalance);
            await context.SaveChangesAsync();

            return NoContent();
        }

        private bool PoolBalanceExists(int id)
        {
            return context.PoolBalance.Any(e => e.Id == id);
        }

        private async Task<PoolBalance> FindPoolBalance(string poolName, string address)
        {
            return await context.PoolBalance
                .Include(e => e.MiningPool)
                .FirstAsync(e => e.MiningPool.Name == poolName && e.Address == address);
        }
    }
}
