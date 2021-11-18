using CryptoStashStats.Controllers;
using CryptoStashStats.Data;
using CryptoStashStats.Models;
using CryptoStashStats.Tests.Utilities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace CryptoStashStats.Tests
{
    public class MiningAccountsControllerTest : EntityControllerTest
    {
        [Fact]
        public async Task PutMiningAccount_DoesNotChangeMiningPool()
        {
            // Arrange
            var original = new MiningPool { Name = "My Pool" };
            using var context = StubContext<MiningContext>();
            context.MiningAccounts.Add(
                new MiningAccount
                {
                    Owner = "sub1",
                    Identifier = "abcd",
                    MiningPool = original
                }
                );
            context.SaveChanges();
            using var context2 = StubContext<MiningContext>();
            var controller = CreateControllerWithUserClaim<MiningAccountsController>(context2);

            // Act
            await controller.PutMiningAccount(
                1,
                new MiningAccount
                {
                    Id = 1,
                    Owner = "sub1",
                    Identifier = "abcd",
                    MiningPool = new MiningPool { Id = 1, Name = "My Pool2" }
                }
                );

            // Assert
            var miningPools = await context2.MiningPools.ToListAsync();
            // Check if no new element were added.
            var miningPool = Assert.Single(miningPools);
            // Check if the first element is unchanged.
            Assert.Equal(miningPool.Name, original.Name);
        }

        [Fact]
        public async Task PostMiningAccount_DoesNotChangeMiningPool()
        {
            // Arrange
            var original = new MiningPool { Name = "My Pool" };
            using var context = StubContext<MiningContext>();
            context.MiningPools.Add(original);
            context.SaveChanges();
            var controller = CreateControllerWithUserClaim<MiningAccountsController>(context);

            // Act
            await controller.PostMiningAccount(
                new MiningAccount
                {
                    Owner = "sub1",
                    Identifier = "abcd",
                    MiningPool = new MiningPool { Id = 1, Name = "Your Pool" }
                }
                );

            // Assert
            var miningPools = await context.MiningPools.ToListAsync();
            // Check if no new element were added.
            var miningPool = Assert.Single(miningPools);
            // Check if the first element is unchanged.
            Assert.Equal(miningPool.Name, original.Name);
        }
    }
}
