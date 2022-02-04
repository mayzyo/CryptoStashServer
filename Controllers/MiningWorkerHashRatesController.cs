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
    [Route("MiningWorkers/{workerId}/HashRates")]
    [ApiController]
    [Authorize("mining_audience")]
    public class MiningWorkerHashRatesController : ControllerBase
    {
        private readonly MiningContext context;

        private IQueryable<MiningWorkerHashRate> MiningWorkerHashRates
        {
            get
            {
                var owner = User.FindFirstValue(ClaimTypes.NameIdentifier);
                // Check if sub is assigned, lacking of is indictive of a client credential access; otherwise a code flow (user) access.
                var balances = owner != null
                    ? context.MiningWorkerHashRates.Where(e => e.MiningWorker.MiningAccount.Owner == owner)
                    : context.MiningWorkerHashRates;
                return balances;
            }
        }

        public MiningWorkerHashRatesController(MiningContext context)
        {
            this.context = context;
        }

        // GET: /MiningWorkers/5/HashRates?cursor=&size=
        [HttpGet]
        [Authorize("read_access")]
        public async Task<ActionResult<IEnumerable<MiningWorkerHashRate>>> GetMiningWorkerHashRates(
            int workerId,
            int cursor = -1,
            int size = 10
            )
        {
            return await MiningWorkerHashRates
                .Where(e => e.MiningWorker.Id == workerId)
                .OrderByDescending(e => e.Created)
                .Pagination(cursor, size)
                .ToListAsync();
        }

        // GET /MiningWorkers/5/HashRates/5
        [HttpGet("{id}")]
        [Authorize("read_access")]
        public async Task<ActionResult<MiningWorkerHashRate>> GetMiningWorkerHashRate(int workerId, int id)
        {
            var miningWorkerHashRate = await context.MiningWorkerHashRates
                .Include(e => e.MiningWorker)
                .FirstOrDefaultAsync(e => e.Id == id);

            if (miningWorkerHashRate == default(MiningWorkerHashRate) || miningWorkerHashRate.MiningWorker.Id != workerId)
            {
                return NotFound();
            }

            return miningWorkerHashRate;
        }

        // PUT: /MiningWorkers/5/HashRates/5
        // Does not need owner verification because it's an internal API.
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        [Authorize("manage_access")]
        public async Task<IActionResult> PutMiningWorkerHashRate(int workerId, int id, MiningWorkerHashRate miningWorkerHashRate)
        {
            if (id != miningWorkerHashRate.Id || workerId != miningWorkerHashRate.MiningWorker.Id)
            {
                return BadRequest();
            }

            context.Entry(miningWorkerHashRate).State = EntityState.Modified;

            try
            {
                await context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MiningWorkerHashRateExists(id))
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

        // POST: /MiningWorkers/5/HashRates
        // Does not need owner verification because it's an internal API.
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        [Authorize("manage_access")]
        public async Task<ActionResult<MiningWorkerHashRate>> PostMiningWorkerHashRate(int workerId, MiningWorkerHashRate miningWorkerHashRate)
        {
            if (workerId != miningWorkerHashRate.MiningWorker.Id)
            {
                return BadRequest();
            }

            // Get existing MiningWorker from database.
            miningWorkerHashRate.MiningWorker = await context.MiningWorkers
                .FindAsync(miningWorkerHashRate.MiningWorker.Id);

            if (miningWorkerHashRate.MiningWorker == null)
            {
                return BadRequest();
            }

            context.MiningWorkerHashRates.Add(miningWorkerHashRate);
            await context.SaveChangesAsync();

            return CreatedAtAction(
                "GetMiningWorkerHashRate",
                new { workerId, id = miningWorkerHashRate.Id },
                miningWorkerHashRate
                );
        }

        // DELETE: /MiningWorkers/5/HashRates/5
        // Does not need owner verification because it's an internal API.
        [HttpDelete("{id}")]
        [Authorize("manage_access")]
        public async Task<IActionResult> DeleteMiningWorkerHashRate(int workerId, int id)
        {
            var miningWorkerHashRate = await context.MiningWorkerHashRates.FindAsync(id);

            if (miningWorkerHashRate == null || miningWorkerHashRate.MiningWorker.Id != workerId)
            {
                return NotFound();
            }

            context.MiningWorkerHashRates.Remove(miningWorkerHashRate);
            await context.SaveChangesAsync();

            return NoContent();
        }

        private bool MiningWorkerHashRateExists(int id)
        {
            return context.MiningWorkerHashRates.Any(e => e.Id == id);
        }
    }
}
