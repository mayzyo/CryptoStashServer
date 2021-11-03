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
    public class WalletsController : ControllerBase
    {
        private readonly FinanceContext context;

        public WalletsController(FinanceContext context)
        {
            this.context = context;
        }

        // GET: /Wallets
        [HttpGet]
        [Authorize("enumerate_access")]
        public async Task<ActionResult<IEnumerable<Wallet>>> GetWallets(int cursor = -1, int size = 10)
        {
            var owner = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Claim has sub assigned, therefore it has to be code flow (user) access.
            if (owner != null)
            {
                return await context.Wallet
                    .Include(el => el.Coin)
                    .Where(e => e.Owner == owner)
                    .Pagination(cursor, size)
                    .ToListAsync();
            }

            // No sub found in the claim, it has to be client credential access.
            return await context.Wallet
                .Include(el => el.Coin)
                .Pagination(cursor, size)
                .ToListAsync();
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

        // PUT: /Wallets
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut]
        [Authorize("manage_access")]
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
        [Authorize("manage_access")]
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
        [Authorize("manage_access")]
        public async Task<ActionResult<Wallet>> PostWallet(Wallet wallet)
        {
            // Get the Coin that is stored by the wallet.
            if (wallet.Coin != null)
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
        [Authorize("manage_access")]
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
