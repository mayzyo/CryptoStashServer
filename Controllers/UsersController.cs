using CryptoStashStats.Data;
using CryptoStashStats.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using CryptoStashStats.Utilities;
using Microsoft.AspNetCore.Authorization;

// DEPRECATING in favour of ASP Identity.
// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace CryptoStashStats.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [Authorize]
    public class UsersController : ControllerBase
    {
        private readonly UserContext context;
        private readonly IPasswordHelper passwordHelper;

        public UsersController(UserContext context, IPasswordHelper passwordHelper)
        {
            this.context = context;
            this.passwordHelper = passwordHelper;
        }

        [HttpGet]
        public IActionResult Get()
        {
            return new JsonResult(from c in User.Claims select new { c.Type, c.Value });
        }

        // GET: /Users
        //[HttpGet]
        //public async Task<ActionResult<IEnumerable<User>>> GetUsers()
        //{
        //    return await context.User.ToListAsync();
        //}

        // GET /Users/5
        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetUser(int id)
        {
            var user = await context.User
                .FirstOrDefaultAsync(el => el.Id == id);

            if (user == default(User))
            {
                return NotFound();
            }

            return user;
        }

        // PUT: /Users/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUser(int id, User user)
        {
            if (id != user.Id)
            {
                return BadRequest();
            }

            // TODO: Password strength check.

            // Hash the password.
            user.Password = passwordHelper.HashPassword(user.Password);

            context.Entry(user).State = EntityState.Modified;

            try
            {
                await context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserExists(id))
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

        // POST: /Users
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<User>> PostUser(User user)
        {
            // TODO: Password strength check.

            // Hash the password.
            user.Password = passwordHelper.HashPassword(user.Password);

            context.User.Add(user);
            await context.SaveChangesAsync();

            return CreatedAtAction("GetUser", new { id = user.Id }, user);
        }

        // DELETE: /Users/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await context.User.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            context.User.Remove(user);
            await context.SaveChangesAsync();

            return NoContent();
        }

        private bool UserExists(int id)
        {
            return context.User.Any(e => e.Id == id);
        }
    }
}
