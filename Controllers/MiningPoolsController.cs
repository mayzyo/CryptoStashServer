using CryptoStashStats.Data;
using CryptoStashStats.Models;
using Microsoft.AspNetCore.Authorization;
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
        private readonly MiningContext context;

        public MiningPoolsController(MiningContext context)
        {
            this.context = context;
        }

        // GET: /MiningPools
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MiningPool>>> GetMiningPools()
        {
            return await context.MiningPools
                .Include(e => e.Tokens)
                .ToListAsync();
        }

        // GET /MiningPools/5
        [HttpGet("{id}")]
        [Authorize("manage_access")]
        public async Task<ActionResult<MiningPool>> GetMiningPool(int id)
        {
            var miningPool = await context.MiningPools
                .Include(e => e.Tokens)
                .FirstOrDefaultAsync(e => e.Id == id);

            if (miningPool == default(MiningPool))
            {
                return NotFound();
            }

            return miningPool;
        }

        // PUT: /MiningPools/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        [Authorize("manage_access")]
        public async Task<IActionResult> PutMiningPool(int id, MiningPool miningPool)
        {
            if (id != miningPool.Id)
            {
                return BadRequest();
            }

            //if (miningPool.Currencies != null)
            //{
            //    if (miningPool.Currencies.Count == 0)
            //    {
            //        var currencies = miningPool.Currencies;
            //        // Assign an existing Currency and attach entity in order to edit many to many relationship
            //        miningPool.Currencies = new List<Currency> { await context.Currencies.FirstAsync() };
            //        context.Attach(miningPool);
            //        miningPool.Currencies = currencies;
            //    }
            //    else
            //    {
            //        miningPool.Currencies = await context.Currencies
            //            .Where(e => miningPool.Currencies.Contains(e))
            //            .ToListAsync();

            //        context.Attach(miningPool);
            //    }
            //}

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
        [Authorize("manage_access")]
        public async Task<ActionResult<MiningPool>> PostMiningPool(MiningPool miningPool)
        {
            // Use existing Currency to avoid changes. Changes should be made using CurrencyController.
            if (miningPool.Tokens != null)
            {
                miningPool.Tokens = await context.Tokens
                    .Where(e => miningPool.Tokens.Contains(e))
                    .ToListAsync();
            }

            context.MiningPools.Add(miningPool);
            await context.SaveChangesAsync();

            return CreatedAtAction("GetMiningPool", new { id = miningPool.Id }, miningPool);
        }

        // DELETE: /MiningPools/5
        [HttpDelete("{id}")]
        [Authorize("manage_access")]
        public async Task<IActionResult> DeleteMiningPool(int id)
        {
            var miningPool = await context.MiningPools.FindAsync(id);
            if (miningPool == null)
            {
                return NotFound();
            }

            context.MiningPools.Remove(miningPool);
            await context.SaveChangesAsync();

            return NoContent();
        }

        // PUT: /MiningPools/5/Tokens
        [HttpPut("{id}/Tokens")]
        [Authorize("manage_access")]
        public async Task<IActionResult> PutMiningPoolCurrency(int id, ICollection<Token> tokens)
        {
            var miningPool = await context.MiningPools
                .Include(e => e.Tokens)
                .FirstAsync(e => e.Id == id);

            miningPool.Tokens = await context.Tokens
                .Where(e => tokens.Contains(e))
                .ToListAsync();

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

        private bool MiningPoolExists(int id)
        {
            return context.MiningPools.Any(e => e.Id == id);
        }
    }
}
