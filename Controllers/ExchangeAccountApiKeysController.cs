//using CryptoStashStats.Data;
//using CryptoStashStats.Models;
//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Security.Claims;
//using System.Threading.Tasks;

//// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

//namespace CryptoStashStats.Controllers
//{
//    [Route("ExchangeAccounts/{accountId}/ApiKeys")]
//    [ApiController]
//    [Authorize("finance_audience")]
//    public class ExchangeAccountApiKeysController : ControllerBase
//    {
//        private readonly FinanceContext context;

//        private IQueryable<ExchangeAccountApiKey> ExchangeAccountApiKeys
//        {
//            get
//            {
//                var owner = User.FindFirstValue(ClaimTypes.NameIdentifier);
//                // Check if sub is assigned, lacking of is indictive of a client credential access; otherwise a code flow (user) access.
//                var exchangeAccountApiKeys = owner != null
//                    ? context.ExchangeAccountApiKeys.Where(e => e.ExchangeAccount.Owner == owner)
//                    : context.ExchangeAccountApiKeys;
//                return exchangeAccountApiKeys;
//            }
//        }

//        public ExchangeAccountApiKeysController(FinanceContext context)
//        {
//            this.context = context;
//        }

//        // GET: /ExchangeAccounts/5/ApiKeys
//        [HttpGet]
//        [Authorize("read_access")]
//        public async Task<ActionResult<IEnumerable<ExchangeAccountApiKey>>> GetExchangeAccountApiKeys(int accountId)
//        {
//            return await ExchangeAccountApiKeys
//                //.Where(e => e.ExchangeAccount.Id == accountId)
//                .ToListAsync();
//        }

//        // GET /ExchangeAccounts/5/ApiKeys
//        [HttpGet("{id}")]
//        [Authorize("read_access")]
//        public async Task<ActionResult<ExchangeAccountApiKey>> GetExchangeAccountApiKey(int accountId, int id)
//        {
//            var exchangeAccountApiKey = await ExchangeAccountApiKeys
//                .Where(e => e.ExchangeAccount.Id == accountId)
//                .Include(e => e.ExchangeAccount)
//                .FirstOrDefaultAsync(e => e.Id == id);

//            if (exchangeAccountApiKey == default(ExchangeAccountApiKey) || accountId != exchangeAccountApiKey.ExchangeAccount.Id)
//            {
//                return NotFound();
//            }

//            return exchangeAccountApiKey;
//        }

//        // PUT: /ExchangeAccounts/5/ApiKeys/5
//        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
//        [HttpPut("{id}")]
//        [Authorize("write_access")]
//        public async Task<IActionResult> PutExchangeAccountApiKey(int accountId, int id, ExchangeAccountApiKey exchangeAccountApiKey)
//        {
//            if (id != exchangeAccountApiKey.Id)
//            {
//                return BadRequest();
//            }

//            if (await NotExchangeAccountApiKeyOwner(id))
//            {
//                return Forbid();
//            }

//            context.Entry(exchangeAccountApiKey).State = EntityState.Modified;

//            try
//            {
//                await context.SaveChangesAsync();
//            }
//            catch (DbUpdateConcurrencyException)
//            {
//                if (!ExchangeAccountApiKeyExists(id))
//                {
//                    return NotFound();
//                }
//                else
//                {
//                    throw;
//                }
//            }

//            return NoContent();
//        }

//        // POST: /ExchangeAccounts/5/ApiKeys
//        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
//        [HttpPost]
//        [Authorize("write_access")]
//        public async Task<ActionResult<ExchangeAccountApiKey>> PostExchangeAccountApiKey(int accountId, ExchangeAccountApiKey exchangeAccountApiKey)
//        {
//            if (NotExchangeAccountApiKeyOwner(exchangeAccountApiKey))
//            {
//                return Forbid();
//            }

//            // Use existing Currency to avoid changes. Changes should be made using CurrencyController.
//            exchangeAccountApiKey.ExchangeAccount = await context.ExchangeAccounts.FindAsync(exchangeAccountApiKey.ExchangeAccount.Id);

//            context.ExchangeAccountApiKeys.Add(exchangeAccountApiKey);
//            await context.SaveChangesAsync();

//            return CreatedAtAction(
//                "GetExchangeAccountApiKey",
//                new { accountId, id = exchangeAccountApiKey.Id },
//                exchangeAccountApiKey
//                );
//        }

//        // DELETE: /ExchangeAccounts/5/ApiKeys/5
//        [HttpDelete("{id}")]
//        [Authorize("write_access")]
//        public async Task<IActionResult> DeleteExchangeAccountApiKey(int accountId, int id)
//        {
//            var exchangeAccountApiKey = await context.ExchangeAccountApiKeys.FindAsync(id);
//            if (exchangeAccountApiKey == null || accountId != exchangeAccountApiKey.ExchangeAccount.Id)
//            {
//                return NotFound();
//            }

//            if (NotExchangeAccountApiKeyOwner(exchangeAccountApiKey))
//            {
//                return Forbid();
//            }

//            context.ExchangeAccountApiKeys.Remove(exchangeAccountApiKey);
//            await context.SaveChangesAsync();

//            return NoContent();
//        }

//        private bool ExchangeAccountApiKeyExists(int id)
//        {
//            return context.ExchangeAccountApiKeys.Any(e => e.Id == id);
//        }

//        private bool NotExchangeAccountApiKeyOwner(ExchangeAccountApiKey exchangeAccountApiKey)
//        {
//            var owner = User.FindFirstValue(ClaimTypes.NameIdentifier);
//            return owner != null
//                && User.FindFirstValue(ClaimTypes.NameIdentifier) != exchangeAccountApiKey.ExchangeAccount.Owner;
//        }

//        private async Task<bool> NotExchangeAccountApiKeyOwner(int id)
//        {
//            var exchangeAccountApiKey = await context.ExchangeAccountApiKeys.FindAsync(id);
//            return NotExchangeAccountApiKeyOwner(exchangeAccountApiKey);
//        }
//    }
//}
