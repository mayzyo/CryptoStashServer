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
    public class CoinsController : ControllerBase
    {
        private readonly FinanceContext context;

        public CoinsController(FinanceContext context)
        {
            this.context = context;
        }

        // GET: /Coins
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Coin>>> GetCoins()
        {
            return await context.Coin.ToListAsync();
        }

        // GET /Coins/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Coin>> GetCoin(int id)
        {
            var coin = await context.Coin
                .FirstOrDefaultAsync(e => e.Id == id);

            if (coin == default(Coin))
            {
                return NotFound();
            }

            return coin;
        }

        // PUT: /Coins/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCoin(int id, Coin coin)
        {
            if (id != coin.Id)
            {
                return BadRequest();
            }

            context.Entry(coin).State = EntityState.Modified;

            try
            {
                await context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CoinExists(id))
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

        // POST: /Coins
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Coin>> PostCoin(Coin coin)
        {
            context.Coin.Add(coin);
            await context.SaveChangesAsync();

            return CreatedAtAction("GetCoin", new { id = coin.Id }, coin);
        }

        // DELETE: /Coins/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCoin(int id)
        {
            var coin = await context.Coin.FindAsync(id);
            if (coin == null)
            {
                return NotFound();
            }

            context.Coin.Remove(coin);
            await context.SaveChangesAsync();

            return NoContent();
        }

        private bool CoinExists(int id)
        {
            return context.Coin.Any(e => e.Id == id);
        }
    }
}
