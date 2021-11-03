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
    public class WorkersController : ControllerBase
    {
        private readonly MinerContext context;

        public WorkersController(MinerContext context)
        {
            this.context = context;
        }

        // GET: /Workers
        [HttpGet]
        [Authorize("enumerate_access")]
        public async Task<ActionResult<IEnumerable<Worker>>> GetWorkers()
        {
            var owner = User.FindFirstValue(ClaimTypes.NameIdentifier);
            IQueryable<Worker> workers = context.Worker.Include(e => e.MiningPool);
            // Check if sub is assigned, lacking of is indictive of a client credential access; otherwise a code flow (user) access.
            workers = owner != null ? workers.Where(e => e.Owner == owner) : workers;
            return await workers.ToListAsync();
            //// Claim has sub assigned, therefore it has to be code flow (user) access.
            //if (owner != null)
            //{
            //    return await context.Worker
            //        .Include(e => e.MiningPool)
            //        .Where(e => e.Owner == owner)
            //        .ToListAsync();
            //}

            //// No sub found in the claim, it has to be client credential access.
            //return await context.Worker
            //    .Include(e => e.MiningPool)
            //    .ToListAsync();
        }

        // GET /Workers/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Worker>> GetWorker(int id)
        {
            var worker = await context.Worker
                .Include(e => e.MiningPool)
                //.Include(e => e.Hashrates.OrderByDescending(x => x.Created).Take(1))
                .FirstOrDefaultAsync(el => el.Id == id);

            if (worker == default(Worker))
            {
                return NotFound();
            }

            return worker;
        }

        // GET /Workers/5/Hashrates
        [HttpGet("{id}/Hashrates")]
        public async Task<ActionResult<IEnumerable<Hashrate>>> GetHashrates(int id, int cursor = -1, int size = 10)
        {
            if (User.FindFirstValue(ClaimTypes.NameIdentifier) != context.Worker.Find(id).Owner)
            {
                return Forbid();
            }

            return await context.Hashrate
                .Where(e => e.Worker.Id == id)
                .OrderByDescending(e => e.Created)
                .Pagination(cursor, size)
                .ToListAsync();
            //return await context.Hashrate
            //    .Where(e => e.Worker.Id == id)
            //    .OrderByDescending(e => e.Created)
            //    .Skip((page - 1) * size)
            //    .Take(size)
            //    .ToListAsync();
        }

        // PUT: /Workers/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        [Authorize("manage_access")]
        public async Task<IActionResult> PutWorker(int id, Worker worker)
        {
            if (id != worker.Id)
            {
                return BadRequest();
            }

            context.Entry(worker).State = EntityState.Modified;

            try
            {
                await context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!WorkerExists(id))
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

        // POST: /Workers
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        [Authorize("manage_access")]
        public async Task<ActionResult<Worker>> PostWorker(Worker worker)
        {
            context.Worker.Add(worker);
            await context.SaveChangesAsync();

            return CreatedAtAction("GetWorker", new { id = worker.Id }, worker);
        }

        // DELETE: /Workers/5
        [HttpDelete("{id}")]
        [Authorize("manage_access")]
        public async Task<IActionResult> DeleteWorker(int id)
        {
            var worker = await context.Worker.FindAsync(id);
            if (worker == null)
            {
                return NotFound();
            }

            context.Worker.Remove(worker);
            await context.SaveChangesAsync();

            return NoContent();
        }

        private bool WorkerExists(int id)
        {
            return context.Worker.Any(e => e.Id == id);
        }
    }
}
