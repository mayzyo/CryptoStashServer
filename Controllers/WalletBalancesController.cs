﻿using CryptoStashServer.Data;
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
    [Route("Wallets/{walletId}/Balances")]
    [ApiController]
    [Authorize("finance_audience")]
    public class WalletBalancesController : ControllerBase
    {
        private readonly FinanceContext context;

        private IQueryable<WalletBalance> WalletBalances
        {
            get
            {
                var owner = User.FindFirstValue(ClaimTypes.NameIdentifier);
                // Check if sub is assigned, lacking of is indictive of a client credential access; otherwise a code flow (user) access.
                var balances = owner != null
                    ? context.WalletBalances.Where(e => e.Wallet.Owner == owner)
                    : context.WalletBalances;
                return balances;
            }
        }

        public WalletBalancesController(FinanceContext context)
        {
            this.context = context;
        }

        // GET: /Wallets/5/Balances?tokenId=&cursor=&size=
        [HttpGet]
        [Authorize("read_access")]
        public async Task<ActionResult<IEnumerable<WalletBalance>>> GetWalletBalances(
            int walletId,
            int? tokenId,
            int cursor = -1,
            int size = 10
            )
        {
            var walletBalances = tokenId == null
                ? WalletBalances.Include(e => e.Token)
                : WalletBalances.Where(e => e.Token.Id == tokenId);

            return await walletBalances
                .Where(e => e.Wallet.Id == walletId)
                .OrderByDescending(e => e.Created)
                .Pagination(cursor, size)
                .ToListAsync();
        }

        // GET /Wallets/5/Balances/5
        [HttpGet("{id}")]
        [Authorize("read_access")]
        public async Task<ActionResult<WalletBalance>> GetWalletBalance(int walletId, int id)
        {
            var walletBalance = await WalletBalances
                .Include(e => e.Wallet)
                .FirstOrDefaultAsync(e => e.Id == id);

            if (walletBalance == default(WalletBalance) || walletBalance.Wallet.Id != walletId)
            {
                return NotFound();
            }

            return walletBalance;
        }

        // PUT: /Wallets/5/Balances/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        [Authorize("manage_access")]
        public async Task<IActionResult> PutWalletBalance(int walletId, int id, WalletBalance walletBalance)
        {
            if (id != walletBalance.Id || walletId != walletBalance.Wallet.Id)
            {
                return BadRequest();
            }

            context.Entry(walletBalance).State = EntityState.Modified;

            try
            {
                await context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!WalletBalanceExists(id))
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

        // POST: /Wallets/5/Balances
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        // Does not need owner verification because it's an internal API.
        [HttpPost]
        [Authorize("manage_access")]
        public async Task<ActionResult<WalletBalance>> PostWalletBalance(int walletId, WalletBalance walletBalance)
        {
            if (walletId != walletBalance.Wallet.Id)
            {
                return BadRequest();
            }

            // Get existing account from database.
            walletBalance.Wallet = await context.Wallets
                .Include(e => e.Blockchain)
                .ThenInclude(e => e.Tokens)
                .Include(e => e.Tokens)
                .FirstOrDefaultAsync(e => e.Id == walletBalance.Wallet.Id);

            if (walletBalance.Wallet == null)
            {
                return BadRequest("Associated wallet not found");
            }
            else if (walletBalance.Token == null)
            {
                return BadRequest("Associated token doesn't exist");
            }

            // Get existing token from database.
            walletBalance.Token = await context.Tokens
                .FindAsync(walletBalance.Token.Id);

            if (walletBalance.Wallet.Tokens == null)
            {
                walletBalance.Wallet.Tokens = new List<Token> { walletBalance.Token };
            }
            else if (!walletBalance.Wallet.Tokens.Contains(walletBalance.Token))
            {
                walletBalance.Wallet.Tokens.Add(walletBalance.Token);
            }

            // Add new tokens to existing tokens in blockchain
            if (walletBalance.Wallet.Blockchain.Tokens != null)
            {
                walletBalance.Wallet.Blockchain.Tokens = walletBalance.Wallet.Blockchain.Tokens
                    .Union(walletBalance.Wallet.Tokens)
                    .ToList();
            }
            else // Set currencies as blockchain's currencies
            {
                walletBalance.Wallet.Blockchain.Tokens = walletBalance.Wallet.Tokens;
            }

            context.WalletBalances.Add(walletBalance);
            await context.SaveChangesAsync();

            return CreatedAtAction(
                "GetWalletBalance",
                new { walletId, id = walletBalance.Id },
                walletBalance
                );
        }

        // DELETE: /Wallets/5/Balances/5
        // Does not need owner verification because it's an internal API.
        [HttpDelete("{id}")]
        [Authorize("manage_access")]
        public async Task<IActionResult> DeleteWalletBalance(int walletId, int id)
        {
            var walletBalance = await context.WalletBalances.FindAsync(id);
            if (walletBalance == null || walletBalance.Wallet.Id != walletId)
            {
                return NotFound();
            }

            context.WalletBalances.Remove(walletBalance);
            await context.SaveChangesAsync();

            return NoContent();
        }

        private bool WalletBalanceExists(int id)
        {
            return context.WalletBalances.Any(e => e.Id == id);
        }
    }
}
