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
    [Authorize("finance_audience")]
    public class BlockchainsController : ControllerBase
    {
        private readonly FinanceContext context;

        public BlockchainsController(FinanceContext context)
        {
            this.context = context;
        }

        // GET: /Blockchains
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Blockchain>>> GetBlockchains()
        {
            return await context.Blockchains.ToListAsync();
        }

        // GET /Blockchains/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Blockchain>> GetBlockchain(int id)
        {
            var blockchain = await context.Blockchains
                .Include(e => e.NativeCurrency)
                .Include(e => e.Currencies)
                .FirstOrDefaultAsync(e => e.Id == id);

            if (blockchain == default(Blockchain))
            {
                return NotFound();
            }

            return blockchain;
        }

        // PUT: /Blockchains/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        [Authorize("manage_access")]
        public async Task<IActionResult> PutBlockchain(int id, Blockchain blockchain)
        {
            if (id != blockchain.Id)
            {
                return BadRequest();
            }

            context.Entry(blockchain).State = EntityState.Modified;

            try
            {
                await context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BlockchainExists(id))
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

        // POST: /Blockchains
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        [Authorize("manage_access")]
        public async Task<ActionResult<Blockchain>> PostBlockchain(Blockchain blockchain)
        {
            if (blockchain.NativeCurrency != null && blockchain.NativeCurrency.Id != 0)
            {
                blockchain.NativeCurrency = await context.Currencies
                    .FindAsync(blockchain.NativeCurrency.Id);
            }
            // Use existing Currency to avoid changes. Changes should be made using CurrencyController.
            if (blockchain.Currencies != null)
            {
                blockchain.Currencies = await context.Currencies
                    .Where(e => blockchain.Currencies.Contains(e))
                    .ToListAsync();
            }

            // Use empty list to avoid changes. Changes should be made using WalletController.
            blockchain.Wallets = new List<Wallet>();

            context.Blockchains.Add(blockchain);
            await context.SaveChangesAsync();

            return CreatedAtAction("GetBlockchain", new { id = blockchain.Id }, blockchain);
        }

        // DELETE: /Blockchains/5
        [HttpDelete("{id}")]
        [Authorize("manage_access")]
        public async Task<IActionResult> DeleteBlockchain(int id)
        {
            var blockchain = await context.Blockchains.FindAsync(id);
            if (blockchain == null)
            {
                return NotFound();
            }

            context.Blockchains.Remove(blockchain);
            await context.SaveChangesAsync();

            return NoContent();
        }

        // PUT: /Blockchains/5/Currencies
        [HttpPut("{id}/Currencies")]
        [Authorize("manage_access")]
        public async Task<IActionResult> PutBlockchainCurrencies(int id, ICollection<Currency> currencies)
        {
            var blockchain = await context.Blockchains
                .Include(e => e.Currencies)
                .FirstAsync(e => e.Id == id);

            blockchain.Currencies = await context.Currencies
                .Where(e => currencies.Contains(e))
                .ToListAsync();

            context.Entry(blockchain).State = EntityState.Modified;

            try
            {
                await context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BlockchainExists(id))
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

        private bool BlockchainExists(int id)
        {
            return context.Blockchains.Any(e => e.Id == id);
        }
    }
}
