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
    public class HashratesController : ControllerBase
    {
        private readonly MinerContext context;

        public HashratesController(MinerContext context)
        {
            this.context = context;
        }

        // GET: /Hashrates
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Hashrate>>> GetHashrates()
        {
            return await context.Hashrate.ToListAsync();
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
        public async Task<ActionResult<Hashrate>> PostHashrate(Hashrate hashrate)
        {
            var worker = await context.Worker
                .FirstOrDefaultAsync(e => e.Name == hashrate.Worker.Name);

            if (worker != null)
            {
                hashrate.Worker = worker;
            } else
            {
                // Attach existing mining pool to new worker.
                if(hashrate.Worker.MiningPool != null)
                {
                    var miningPool = await context.MiningPool
                        .FirstOrDefaultAsync(e => e.Name == hashrate.Worker.MiningPool.Name);

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
