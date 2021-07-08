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
    public class MiningPoolsController : ControllerBase
    {
        private readonly MinerContext context;

        public MiningPoolsController(MinerContext context)
        {
            this.context = context;
        }

        // GET: /MiningPools
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MiningPool>>> GetMiningPools()
        {
            return await context.MiningPool.ToListAsync();
        }

        // GET /MiningPools/5
        [HttpGet("{id}")]
        public async Task<ActionResult<MiningPool>> GetMiningPool(int id)
        {
            var miningPool = await context.MiningPool
                .Include(el => el.PoolBalances)
                .FirstOrDefaultAsync(el => el.Id == id);

            if (miningPool == default(MiningPool))
            {
                return NotFound();
            }

            return miningPool;
        }

        // PUT: /MiningPools/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutMiningPool(int id, MiningPool miningPool)
        {
            if (id != miningPool.Id)
            {
                return BadRequest();
            }

            context.Entry(miningPool).State = EntityState.Modified;

            try
            {
                await context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MiningPoolExists(id))
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

        // POST: /MiningPools
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<MiningPool>> PostMiningPool(MiningPool miningPool)
        {
            context.MiningPool.Add(miningPool);
            await context.SaveChangesAsync();

            return CreatedAtAction("GetMiningPool", new { id = miningPool.Id }, miningPool);
        }

        // DELETE: /MiningPools/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMiningPool(int id)
        {
            var miningPool = await context.MiningPool.FindAsync(id);
            if (miningPool == null)
            {
                return NotFound();
            }

            context.MiningPool.Remove(miningPool);
            await context.SaveChangesAsync();

            return NoContent();
        }

        private bool MiningPoolExists(int id)
        {
            return context.MiningPool.Any(e => e.Id == id);
        }
    }
}
