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
    public class PoolBalancesController : ControllerBase
    {
        private readonly MinerContext context;

        public PoolBalancesController(MinerContext context)
        {
            this.context = context;
        }

        // GET: /PoolBalances
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PoolBalance>>> GetPoolBalances()
        {
            return await context.PoolBalance.ToListAsync();
        }

        // GET /PoolBalances/5
        [HttpGet("{id}")]
        public async Task<ActionResult<PoolBalance>> GetPoolBalance(int id)
        {
            var poolBalance = await context.PoolBalance
                .Include(e => e.Wallet)
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
        public async Task<IActionResult> PutPoolBalance(int id, PoolBalance poolBalance)
        {
            if (id != poolBalance.Id)
            {
                return BadRequest();
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

        [HttpPut("{poolName}/{walletAddress}")]
        public async Task<IActionResult> PutPoolBalance(string poolName, string walletAddress, PoolBalance poolBalance)
        {
            if (poolName != poolBalance.MiningPool.Name || walletAddress != poolBalance.Wallet.Address)
            {
                return BadRequest();
            }

            PoolBalance existing;

            try
            {
                existing = await context.PoolBalance
                    .Include(e => e.MiningPool)
                    .Include(e => e.Wallet)
                    .FirstAsync(e => e.MiningPool.Name == poolName && e.Wallet.Address == walletAddress);
            }
            catch (InvalidOperationException)
            {
                return NotFound();
            }

            // TODO: Improve implementation.
            existing.Current = poolBalance.Current;

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

        // POST: /PoolBalances
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<PoolBalance>> PostPoolBalance(PoolBalance poolBalance)
        {
            var miningPool = await context.MiningPool
                .FirstOrDefaultAsync(e => e.Name == poolBalance.MiningPool.Name);

            if (miningPool != null)
            {
                poolBalance.MiningPool = miningPool;
            }

            var wallet = await context.Wallet
                .FirstOrDefaultAsync(e => e.Address == poolBalance.Wallet.Address);

            if (wallet != null)
            {
                poolBalance.Wallet = wallet;
            }

            context.PoolBalance.Add(poolBalance);
            await context.SaveChangesAsync();

            return CreatedAtAction("GetPoolBalance", new { id = poolBalance.Id }, poolBalance);
        }

        // DELETE: /PoolBalances/5
        [HttpDelete("{id}")]
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

        private bool PoolBalanceExists(string poolName, string walletAddress)
        {
            return context.PoolBalance
                .Include(e => e.MiningPool)
                .Include(e => e.Wallet)
                .Any(e => e.MiningPool.Name == poolName && e.Wallet.Address == walletAddress);
        }
    }
}
