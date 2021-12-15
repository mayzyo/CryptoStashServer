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
    public class TokensController : ControllerBase
    {
        private readonly FinanceContext financeContext;
        private readonly MiningContext miningContext;

        public TokensController(FinanceContext financeContext, MiningContext miningContext)
        {
            this.financeContext = financeContext;
            this.miningContext = miningContext;
        }

        // GET: /Tokens
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Token>>> GetTokens()
        {
            return await financeContext.Tokens.ToListAsync();
        }

        // GET /Tokens/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Token>> GetToken(int id)
        {
            var token = await financeContext.Tokens.FindAsync(id);

            if (token == default(Token))
            {
                return NotFound();
            }

            return token;
        }

        // PUT: /Tokens/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        [Authorize("manage_access")]
        public async Task<IActionResult> PutToken(int id, Token token)
        {
            if (id != token.Id)
            {
                return BadRequest();
            }

            var status = await PutToken(id, token, financeContext);
            if(status != await PutToken(id, token, miningContext))
            {
                throw new Exception("Out of Sync!");
            }

            if(status == 1)
            {
                return BadRequest();
            }
            else if(status == 2)
            {
                return NotFound();
            }

            return NoContent();
        }
        public async Task<int> PutToken<T>(int id, Token token, T context) where T : DbContext, ITokenContext
        {
            if (id != token.Id)
            {
                return 1; // BadRequest();
            }

            context.Entry(token).State = EntityState.Modified;

            try
            {
                await context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TokenExists(id, context))
                {
                    return 2; // NotFound();
                }
                else
                {
                    throw;
                }
            }

            return 0; // NoContent();
        }

        // POST: /Tokens
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        [Authorize("manage_access")]
        public async Task<ActionResult<Token>> PostToken(Token token)
        {
            await PostToken(token, financeContext);
            await PostToken(token, miningContext);

            return CreatedAtAction("GetToken", new { id = token.Id }, token);
        }

        public async Task PostToken<T>(Token token, T context) where T : DbContext, ITokenContext
        {
            context.Tokens.Add(token);
            await context.SaveChangesAsync();
        }

        // DELETE: /Tokens/5
        [HttpDelete("{id}")]
        [Authorize("manage_access")]
        public async Task<IActionResult> DeleteToken(int id)
        {
            var status = await DeleteToken(id, financeContext);
            if (status != await DeleteToken(id, miningContext))
            {
                throw new Exception("Out of Sync!");
            }

            if (status == 1)
            {
                return NotFound();
            }

            return NoContent();
        }

        public async Task<int> DeleteToken<T>(int id, T context) where T : DbContext, ITokenContext
        {
            var Token = await context.Tokens.FindAsync(id);
            if (Token == null)
            {
                return 1;
            }

            context.Tokens.Remove(Token);
            await context.SaveChangesAsync();

            return 0;
        }

        // GET: /Tokens/Contexts/MiningContext
        // For debuggin only
        [HttpGet("contexts/{contextId}")]
        [Authorize("manage_access")]
        public async Task<ActionResult<IEnumerable<Token>>> GetContextTokens(string contextId)
        {
            if (contextId == "MiningContext")
            {
                return await miningContext.Tokens.ToListAsync();
            }
            else if (contextId == "FinanceContext")
            {
                return await financeContext.Tokens.ToListAsync();
            }

            return NotFound();
        }

        //// GET /Tokens/5/Wallets?cursor=&size=
        //[HttpGet("{id}/Wallets")]
        //public async Task<ActionResult<IEnumerable<Wallet>>> GetTokenWallets(int id, int cursor = -1, int size = 10)
        //{
        //    var token = await financeContext.Tokens.FindAsync(id);

        //    if (token == default(Token))
        //    {
        //        return NotFound();
        //    }

        //    var wallets = await financeContext.Wallets
        //        .Where(e => e.Tokens.Contains(token))
        //        .Pagination(cursor, size)
        //        .ToListAsync();

        //    return wallets;
        //}

        private static bool TokenExists(int id, ITokenContext context)
        {
            return context.Tokens.Any(e => e.Id == id);
        }
    }
}
