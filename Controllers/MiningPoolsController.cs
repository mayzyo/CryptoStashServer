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
    public class MiningPoolsController : ControllerBase
    {
        private readonly MinerContext context;

        public MiningPoolsController(MinerContext context)
        {
            this.context = context;
        }

        // GET: /MiningPools
        [HttpGet]
        [Authorize("enumerate_access")]
        public async Task<ActionResult<IEnumerable<MiningPool>>> GetMiningPools()
        {
            return await context.MiningPool.ToListAsync();
        }

        // GET /MiningPools/5
        [HttpGet("{id}")]
        public async Task<ActionResult<MiningPool>> GetMiningPool(int id)
        {
            var miningPool = await context.MiningPool
                .Include(e => e.PoolBalances)
                .Include(e => e.Workers)
                .FirstOrDefaultAsync(e => e.Id == id);

            if (miningPool == default(MiningPool))
            {
                return NotFound();
            }

            return miningPool;
        }

        // GET: /MiningPools/5/Workers
        [HttpGet("{id}/Workers")]
        public async Task<ActionResult<IEnumerable<Worker>>> GetMiningPoolWorkers(int id)
        {
            return await context.Worker
                .Where(e => e.MiningPool != null && e.MiningPool.Id == id)
                .ToListAsync();
        }

        // GET: /MiningPools/5/Hashrates
        //[HttpGet("{id}/Hashrates")]
        //public async Task<IEnumerable<Hashrate>> GetMiningPoolHashrates(int id, int page = 1, int size = 3)
        //{
        //    var test = new Hashrate
        //    {
        //        Average = 0,
        //        Current = 0,
        //        Reported = 0
        //    };

        //    //Task.Run(context.Worker
        //    //    .Where(e => e.MiningPool != null && e.MiningPool.Id == id)
        //    //    .Include(e => e.Hashrates)
        //    //    .Select(e => e.Hashrates)
        //    //    .ToListAsync());
        //    var t = await context.Worker
        //        .Where(e => e.MiningPool != null && e.MiningPool.Id == id)
        //        .Include(e => e.Hashrates)
        //        .Select(e => e.Hashrates)
        //        .ToListAsync();
        //        //.Aggregate((a, c) => c
        //        ////a.Zip(c, (i, t) => new Hashrate
        //        ////    {
        //        ////        Average = i.Average + t.Average,
        //        ////        Current = i.Current + t.Current,
        //        ////        Reported = i.Reported + t.Reported,
        //        ////        Created = t.Created,
        //        ////    }
        //        ////)
        //        //);
        //    //var workers = await context.Worker
        //    //    .Where(e => e.MiningPool != null && e.MiningPool.Id == id)
        //    //    .ToListAsync();

        //    //var workerHashrates = new Dictionary<Worker, IList<Hashrate>>();
        //    //IEnumerable<Hashrate> prev = new List<Hashrate>();

        //    //int count = 0;
        //    //foreach (var worker in workers)
        //    //{
        //    //    if(count++ > 1)
        //    //    {
        //    //        prev = context.Hashrate
        //    //            .Where(e => e.Worker.Id == worker.Id)
        //    //            .OrderByDescending(e => e.Created)
        //    //            .Skip((page - 1) * size)
        //    //            .Take(size)
        //    //            .ToList()
        //    //            .Zip(prev, (i, t) => new Hashrate
        //    //            {
        //    //                Average = i.Average + t.Average,
        //    //                Current = i.Current + t.Current,
        //    //                Reported = i.Reported + t.Reported,
        //    //                Created = i.Created
        //    //            });
        //    //    } else
        //    //    {
        //    //        prev = context.Hashrate
        //    //            .Where(e => e.Worker.Id == worker.Id)
        //    //            .OrderByDescending(e => e.Created)
        //    //            .Skip((page - 1) * size)
        //    //            .Take(size)
        //    //            .ToList();
        //    //    }
        //    //}

        //    //return prev;
        //}

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
            context.MiningPool.Add(miningPool);
            await context.SaveChangesAsync();

            return CreatedAtAction("GetMiningPool", new { id = miningPool.Id }, miningPool);
        }

        // DELETE: /MiningPools/5
        [HttpDelete("{id}")]
        [Authorize("manage_access")]
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
