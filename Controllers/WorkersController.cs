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
    public class WorkersController : ControllerBase
    {
        private readonly MinerContext context;

        public WorkersController(MinerContext context)
        {
            this.context = context;
        }

        // GET: /Workers
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Worker>>> GetWorkers()
        {
            return await context.Worker.ToListAsync();
        }

        // GET /Workers/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Worker>> GetWorker(int id)
        {
            var worker = await context.Worker
                .Include(e => e.MiningPool)
                .Include(e => e.Hashrates.OrderByDescending(x => x.Created).Take(1))
                .FirstOrDefaultAsync(el => el.Id == id);

            if (worker == default(Worker))
            {
                return NotFound();
            }

            return worker;
        }

        // GET /Workers/5/Hashrates
        [HttpGet("{id}/Hashrates")]
        public async Task<ActionResult<IEnumerable<Hashrate>>> GetWorkerHashrates(int id, int page = 1, int size = 10)
        {
            return await context.Hashrate
                .Where(e => e.Worker.Id == id)
                .OrderByDescending(e => e.Created)
                .Skip((page - 1) * size)
                .Take(size)
                .ToListAsync();
        }

        // PUT: /Workers/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
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
        public async Task<ActionResult<Worker>> PostWorker(Worker worker)
        {
            context.Worker.Add(worker);
            await context.SaveChangesAsync();

            return CreatedAtAction("GetWorker", new { id = worker.Id }, worker);
        }

        // DELETE: /Workers/5
        [HttpDelete("{id}")]
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
