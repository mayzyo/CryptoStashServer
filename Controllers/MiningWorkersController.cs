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
    public class MiningWorkersController : ControllerBase
    {
        private readonly MiningContext context;

        private IQueryable<MiningWorker> MiningWorkers
        {
            get
            {
                var owner = User.FindFirstValue(ClaimTypes.NameIdentifier);
                // Check if sub is assigned, lacking of is indictive of a client credential access; otherwise a code flow (user) access.
                var miningWorkers = owner != null
                    ? context.MiningWorkers.Where(e => e.MiningAccount.Owner == owner)
                    : context.MiningWorkers;
                return miningWorkers;
            }
        }

        public MiningWorkersController(MiningContext context)
        {
            this.context = context;
        }

        // GET: /MiningWorkers
        [HttpGet]
        [Authorize("read_access")]
        public async Task<ActionResult<IEnumerable<MiningWorker>>> GetMiningWorkers()
        {
            return await MiningWorkers
                .Include(e => e.MiningAccount)
                .ToListAsync();
        }

        // GET /MiningWorkers/5
        [HttpGet("{id}")]
        [Authorize("read_access")]
        public async Task<ActionResult<MiningWorker>> GetMiningWorker(int id)
        {
            var miningWorker = await MiningWorkers
                .Include(e => e.MiningAccount)
                .FirstOrDefaultAsync(e => e.Id == id);

            if (miningWorker == default(MiningWorker))
            {
                return NotFound();
            }

            return miningWorker;
        }

        // PUT: /MiningWorkers/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        [Authorize("manage_access")]
        public async Task<IActionResult> PutMiningWorker(int id, MiningWorker miningWorker)
        {
            if (id != miningWorker.Id)
            {
                return BadRequest();
            }

            if (await NotMiningAccountOwner(miningWorker.MiningAccountId))
            {
                return Forbid();
            }

            context.Entry(miningWorker).State = EntityState.Modified;

            try
            {
                await context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MiningWorkerExists(id))
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

        // POST: /MiningWorkers
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        [Authorize("manage_access")]
        public async Task<ActionResult<MiningWorker>> PostMiningWorker(MiningWorker miningWorker)
        {
            miningWorker.MiningAccount = await context.MiningAccounts
                .FirstOrDefaultAsync(e => e.Id == miningWorker.MiningAccount.Id);

            if (NotMiningAccountOwner(miningWorker.MiningAccount))
            {
                return Forbid();
            }

            context.MiningWorkers.Add(miningWorker);
            await context.SaveChangesAsync();

            return CreatedAtAction("GetMiningWorker", new { id = miningWorker.Id }, miningWorker);
        }

        // DELETE: /MiningWorkers/5
        [HttpDelete("{id}")]
        [Authorize("manage_access")]
        public async Task<IActionResult> DeleteMiningWorker(int id)
        {
            var miningWorker = await context.MiningWorkers.FindAsync(id);
            if (miningWorker == null)
            {
                return NotFound();
            }

            if (NotMiningAccountOwner(miningWorker.MiningAccount))
            {
                return Forbid();
            }

            context.MiningWorkers.Remove(miningWorker);
            await context.SaveChangesAsync();

            return NoContent();
        }

        // POST: /MiningWorkers/HashRates
        [HttpPost("HashRates")]
        [Authorize("manage_access")]
        public async Task<ActionResult<MiningWorkerHashRate>> PostMiningWorkerHashRate(MiningWorkerHashRate miningWorkerHashRate)
        {
            if(miningWorkerHashRate.MiningWorker.Id != 0)
            {
                miningWorkerHashRate.MiningWorker = await context.MiningWorkers
                    .FindAsync(miningWorkerHashRate.MiningWorker.Id);
            } else
            {
                miningWorkerHashRate.MiningWorker.MiningAccount = await context.MiningAccounts
                    .FindAsync(miningWorkerHashRate.MiningWorker.MiningAccount.Id);
            }

            context.MiningWorkerHashRates.Add(miningWorkerHashRate);
            await context.SaveChangesAsync();

            return CreatedAtAction(
                "GetMiningWorker",
                new { id = miningWorkerHashRate.MiningWorker.Id },
                miningWorkerHashRate.MiningWorker
                );
        }

        private bool MiningWorkerExists(int id)
        {
            return context.MiningWorkers.Any(e => e.Id == id);
        }

        private bool NotMiningAccountOwner(MiningAccount miningAccount)
        {
            var owner = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return owner != null
                && User.FindFirstValue(ClaimTypes.NameIdentifier) != miningAccount.Owner;
        }

        private async Task<bool> NotMiningAccountOwner(int miningAccountId)
        {
            var owner = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var miningAccount = await context.MiningAccounts.FindAsync(miningAccountId);
            return owner != null
                && User.FindFirstValue(ClaimTypes.NameIdentifier) != miningAccount.Owner;
        }
    }
}