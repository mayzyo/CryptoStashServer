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
    public class WalletsController : ControllerBase
    {
        private readonly FinanceContext context;

        public WalletsController(FinanceContext context)
        {
            this.context = context;
        }

        // GET: /Wallets
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Wallet>>> GetWallets()
        {
            return await context.Wallet.ToListAsync();
        }

        // GET /Wallets/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Wallet>> GetWallet(int id)
        {
            var wallet = await context.Wallet
                .Include(el => el.Coin)
                .FirstOrDefaultAsync(el => el.Id == id);

            if (wallet == default(Wallet))
            {
                return NotFound();
            }

            return wallet;
        }

        // PUT: /Wallets/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut]
        public async Task<IActionResult> PutWallet(string address, Wallet wallet)
        {
            if (address != wallet.Address)
            {
                return BadRequest();
            }

            Wallet existing;

            try
            {
                existing = await context.Wallet
                    .Include(e => e.Coin)
                    .FirstAsync(e => e.Address == address);
            }
            catch (InvalidOperationException)
            {
                return NotFound();
            }

            // TODO: Improve implementation.
            existing.Balance = wallet.Balance;

            context.Entry(existing).State = EntityState.Modified;

            try
            {
                await context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }

            return NoContent();
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutWallet(int id, Wallet wallet)
        {
            if (id != wallet.Id)
            {
                return BadRequest();
            }

            context.Entry(wallet).State = EntityState.Modified;

            try
            {
                await context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!WalletExists(id))
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

        // POST: /Wallets
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Wallet>> PostWallet(Wallet wallet)
        {
            if(wallet.Coin != null)
            {
                var coin = await context.Coin
                    .FirstOrDefaultAsync(e => e.Ticker == wallet.Coin.Ticker);

                if (coin != null)
                {
                    wallet.Coin = coin;
                }
            }

            context.Wallet.Add(wallet);
            await context.SaveChangesAsync();

            return CreatedAtAction("GetWallet", new { id = wallet.Id }, wallet);
        }

        // DELETE: /Wallets/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteWallet(int id)
        {
            var wallet = await context.Wallet.FindAsync(id);
            if (wallet == null)
            {
                return NotFound();
            }

            context.Wallet.Remove(wallet);
            await context.SaveChangesAsync();

            return NoContent();
        }

        private bool WalletExists(int id)
        {
            return context.Wallet.Any(e => e.Id == id);
        }
    }
}
