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
    public class HashratesController : ControllerBase
    {
        private readonly MinerContext context;

        public HashratesController(MinerContext context)
        {
            this.context = context;
        }

        // GET: /Hashrates
        [HttpGet]
        [Authorize("enumerate_access")]
        public async Task<ActionResult<IEnumerable<Hashrate>>> GetHashrates(int cursor = -1, int size = 10)
        {
            return await context.Hashrate
                .OrderByDescending(e => e.Created)
                .Pagination(cursor, size)
                .ToListAsync();
        }

        // GET /Hashrates/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Hashrate>> GetHashrate(int id)
        {
            var hashrate = await context.Hashrate
                .Include(e => e.Worker)
                .FirstOrDefaultAsync(e => e.Id == id);

            if (hashrate == default(Hashrate))
            {
                return NotFound();
            }

            return hashrate;
        }

        // PUT: /Hashrates/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        [Authorize("manage_access")]
        public async Task<IActionResult> PutHashrate(int id, Hashrate hashrate)
        {
            if (id != hashrate.Id)
            {
                return BadRequest();
            }

            context.Entry(hashrate).State = EntityState.Modified;

            try
            {
                await context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!HashrateExists(id))
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

        // POST: /Hashrates
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        [Authorize("manage_access")]
        public async Task<ActionResult<Hashrate>> PostHashrate(Hashrate hashrate)
        {
            // Get the worker that is producing the hashrate.
            var worker = await context.Worker
                .FirstOrDefaultAsync(e => 
                    e.Name == hashrate.Worker.Name && e.Address == hashrate.Worker.Address
                );

            if (worker != null)
            {
                // Attach existing worker to hashrate.
                hashrate.Worker = worker;
            } else
            {
                if(hashrate.Worker.MiningPool != null)
                {
                    // Get the mining pool that the new worker is mining to.
                    var miningPool = await context.MiningPool
                        .FirstOrDefaultAsync(e => e.Name == hashrate.Worker.MiningPool.Name);

                    // Attach existing mining pool to new worker.
                    if (miningPool != null)
                    {
                        hashrate.Worker.MiningPool = miningPool;
                    }
                }
            }

            context.Hashrate.Add(hashrate);
            await context.SaveChangesAsync();

            return CreatedAtAction("GetHashrate", new { id = hashrate.Id }, hashrate);
        }

        // DELETE: /Hashrates/5
        [HttpDelete("{id}")]
        [Authorize("manage_access")]
        public async Task<IActionResult> DeleteHashrate(int id)
        {
            var hashrate = await context.Hashrate.FindAsync(id);
            if (hashrate == null)
            {
                return NotFound();
            }

            context.Hashrate.Remove(hashrate);
            await context.SaveChangesAsync();

            return NoContent();
        }

        private bool HashrateExists(int id)
        {
            return context.Hashrate.Any(e => e.Id == id);
        }
    }
}
