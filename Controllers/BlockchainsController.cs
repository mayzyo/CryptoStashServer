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
            return await context.Blockchains
                .Include(e => e.NativeToken)
                .ToListAsync();
        }

        // GET /Blockchains/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Blockchain>> GetBlockchain(int id)
        {
            var blockchain = await context.Blockchains
                .Include(e => e.NativeToken)
                .Include(e => e.Tokens)
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
            if (blockchain.NativeToken != null && blockchain.NativeToken.Id != 0)
            {
                blockchain.NativeToken = await context.Tokens
                    .FindAsync(blockchain.NativeToken.Id);
            }
            // Use existing Currency to avoid changes. Changes should be made using CurrencyController.
            if (blockchain.Tokens != null)
            {
                blockchain.Tokens = await context.Tokens
                    .Where(e => blockchain.Tokens.Contains(e))
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

        // PUT: /Blockchains/5/Tokens
        [HttpPut("{id}/Tokens")]
        [Authorize("manage_access")]
        public async Task<IActionResult> PutBlockchainTokens(int id, ICollection<Token> tokens)
        {
            var blockchain = await context.Blockchains
                .Include(e => e.Tokens)
                .FirstAsync(e => e.Id == id);

            blockchain.Tokens = await context.Tokens
                .Where(e => tokens.Contains(e))
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

        // GET /Blockchains/5/Wallets
        [HttpGet("{id}/Wallets")]
        [Authorize("manage_access")]
        public async Task<ActionResult<IEnumerable<Wallet>>> GetBlockchainWallets(int id, int cursor = -1, int size = 10)
        {
            if (!BlockchainExists(id))
            {
                return NotFound();
            }

            return await context.Wallets
                .Where(e => e.Blockchain.Id == id)
                .OrderByDescending(e => e.Created)
                .Pagination(cursor, size)
                .ToListAsync();
        }

        private bool BlockchainExists(int id)
        {
            return context.Blockchains.Any(e => e.Id == id);
        }
    }
}
