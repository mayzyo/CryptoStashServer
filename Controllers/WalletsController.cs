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
    [Authorize("finance_audience")]
    public class WalletsController : ControllerBase
    {
        private readonly FinanceContext context;

        private IQueryable<Wallet> Wallets
        {
            get
            {
                var owner = User.FindFirstValue(ClaimTypes.NameIdentifier);
                // Check if sub is assigned, lacking of is indictive of a client credential access; otherwise a code flow (user) access.
                var wallets = owner != null
                    ? context.Wallets.Where(e => e.Owner == owner)
                    : context.Wallets;
                return wallets;
            }
        }

        public WalletsController(FinanceContext context)
        {
            this.context = context;
        }

        // GET: /Wallets
        [HttpGet]
        [Authorize("read_access")]
        public async Task<ActionResult<IEnumerable<Wallet>>> GetWallets()
        {
            return await Wallets.ToListAsync();
        }

        // GET /Wallets/5
        [HttpGet("{id}")]
        [Authorize("read_access")]
        public async Task<ActionResult<Wallet>> GetWallet(int id)
        {
            var wallet = await Wallets
                .Include(e => e.Currencies)
                .FirstOrDefaultAsync(e => e.Id == id);

            if (wallet == default(Wallet))
            {
                return NotFound();
            }

            return wallet;
        }

        // PUT: /Wallets/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        [Authorize("write_access")]
        public async Task<IActionResult> PutWallet(int id, Wallet wallet)
        {
            if (id != wallet.Id)
            {
                return BadRequest();
            }

            if (await NotWalletOwner(id))
            {
                return Forbid();
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
        [Authorize("write_access")]
        public async Task<ActionResult<Wallet>> PostWallet(Wallet wallet)
        {
            if (NotWalletOwner(wallet))
            {
                return Forbid();
            }

            // Use existing Currency to avoid changes. Changes should be made using CurrencyController.
            if (wallet.Currencies != null)
            {
                wallet.Currencies = await context.Currencies
                    .Where(e => wallet.Currencies.Contains(e))
                    .ToListAsync();
            }

            context.Wallets.Add(wallet);
            await context.SaveChangesAsync();

            return CreatedAtAction("GetWallet", new { id = wallet.Id }, wallet);
        }

        // DELETE: /Wallets/5
        [HttpDelete("{id}")]
        [Authorize("write_access")]
        public async Task<IActionResult> DeleteWallet(int id)
        {
            var wallet = await context.Wallets.FindAsync(id);
            if (wallet == null)
            {
                return NotFound();
            }

            if (NotWalletOwner(wallet))
            {
                return Forbid();
            }

            context.Wallets.Remove(wallet);
            await context.SaveChangesAsync();

            return NoContent();
        }

        private bool WalletExists(int id)
        {
            return context.Wallets.Any(e => e.Id == id);
        }

        private bool NotWalletOwner(Wallet wallet)
        {
            var owner = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return owner != null
                && User.FindFirstValue(ClaimTypes.NameIdentifier) != wallet.Owner;
        }

        private async Task<bool> NotWalletOwner(int id)
        {
            var wallet = await context.Wallets.FindAsync(id);
            context.Entry(wallet).State = EntityState.Detached;
            return NotWalletOwner(wallet);
        }
    }
}
