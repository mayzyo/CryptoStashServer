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
    [Authorize("mining_audience")]
    public class PayoutsController : ControllerBase
    {
        private readonly MinerContext context;

        public PayoutsController(MinerContext context)
        {
            this.context = context;
        }

        // GET: /Payouts
        [HttpGet]
        [Authorize("enumerate_access")]
        public async Task<ActionResult<IEnumerable<Payout>>> GetPayouts()
        {
            return await context.Payout.ToListAsync();
        }

        // GET /Payouts/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Payout>> GetPayout(int id)
        {
            var payout = await context.Payout
                .Include(e => e.MiningPool)
                .FirstOrDefaultAsync(e => e.Id == id);

            if (payout == default(Payout))
            {
                return NotFound();
            }

            return payout;
        }

        // PUT: /Payouts/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        [Authorize("manage_access")]
        public async Task<IActionResult> PutPayout(int id, Payout payout)
        {
            if (id != payout.Id)
            {
                return BadRequest();
            }

            context.Entry(payout).State = EntityState.Modified;

            try
            {
                await context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PayoutExists(id))
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

        // POST: /Payouts
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        [Authorize("manage_access")]
        public async Task<ActionResult<Payout>> PostPayout(Payout payout)
        {
            // Check if payout already exist based on TXHash instead of Id.
            // TODO: This doesn't seem necessary. Post actions usually result in failure if element already exists.
            if(PayoutExists(payout.TXHash))
            {
                return await context.Payout
                    .FirstOrDefaultAsync(e => e.TXHash == payout.TXHash);
            }

            // Get existing child objects from database.
            var miningPool = await context.MiningPool
                .FirstOrDefaultAsync(e => e.Name == payout.MiningPool.Name);

            // Attach exisiting child objects to the new element.
            if (miningPool != null)
            {
                payout.MiningPool = miningPool;
            }

            context.Payout.Add(payout);
            await context.SaveChangesAsync();

            return CreatedAtAction("GetPayout", new { id = payout.Id }, payout);
        }

        // DELETE: /Payouts/5
        [HttpDelete("{id}")]
        [Authorize("manage_access")]
        public async Task<IActionResult> DeletePayout(int id)
        {
            var payout = await context.Payout.FindAsync(id);
            if (payout == null)
            {
                return NotFound();
            }

            context.Payout.Remove(payout);
            await context.SaveChangesAsync();

            return NoContent();
        }

        private bool PayoutExists(int id)
        {
            return context.Payout.Any(e => e.Id == id);
        }

        private bool PayoutExists(string txHash)
        {
            return context.Payout.Any(e => e.TXHash == txHash);
        }
    }
}
