using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace CryptoStashServer.Tests.Utilities
{
    public abstract class EntityControllerTest
    {
        public T StubContext<T>(string dbName, [CallerMemberName] string memberName = "") where T : DbContext
        {
            var options = new DbContextOptionsBuilder<T>()
                .UseInMemoryDatabase(databaseName: dbName)
                .Options;

            return (T)Activator.CreateInstance(typeof(T), new object[] { options });
        }

        public T StubContext<T>([CallerMemberName] string memberName = "") where T : DbContext
        {
            var options = new DbContextOptionsBuilder<T>()
                .UseInMemoryDatabase(databaseName: $"{GetType().Name}_{memberName}")
                .Options;

            return (T)Activator.CreateInstance(typeof(T), new object[] { options });
        }

        public T CreateControllerWithUserClaim<T>(params object[] args) where T : ControllerBase
        {
            var controller = (T)Activator.CreateInstance(typeof(T), args);
            controller.ControllerContext = new ControllerContext
            {
                // Mock user claim to bypass ownership check.
                HttpContext = new DefaultHttpContext { User = new ClaimsPrincipal() }
            };
            return controller;
        }

        public string GetCaller([CallerMemberName] string memberName = "")
        {
            return memberName;
        }
    }
}
