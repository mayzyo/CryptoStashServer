using CryptoStashServer.Controllers;
using CryptoStashServer.Data;
using CryptoStashServer.Models;
using CryptoStashServer.Tests.Utilities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace CryptoStashServer.Tests
{
    public class MiningWorkersControllerTest : EntityControllerTest
    {
        [Fact]
        public async Task PutMiningWorker_DoesNotChangeMiningAccount()
        {
            // Arrange
            var original = new MiningAccount { Id = 1, Identifier = "account1", Owner = "user1" };
            using var context = StubContext<MiningContext>();
            context.MiningWorkers.Add(
                new MiningWorker
                {
                    Name = "Worker 1",
                    MiningAccount = original
                }
                );
            context.SaveChanges();
            using var context2 = StubContext<MiningContext>();
            var controller = CreateControllerWithUserClaim<MiningWorkersController>(context2);

            // Act
            await controller.PutMiningWorker(
                1,
                new MiningWorker
                {
                    Id = 1,
                    Name = "Worker 1",
                    MiningAccount = new MiningAccount { Id = 1, Identifier = "account2" }
                }
                );

            // Assert
            var miningAccounts = await context2.MiningAccounts.ToListAsync();
            // Check if no new element were added.
            var miningAccount = Assert.Single(miningAccounts);
            // Check if the first element is unchanged.
            Assert.Equal(miningAccount.Identifier, original.Identifier);
        }

        [Fact]
        public async Task PostMiningWorker_DoesNotChangeMiningAccount()
        {
            // Arrange
            var original = new MiningAccount { Id = 1, Identifier = "account1", Owner = "user1" };
            using var context = StubContext<MiningContext>();
            context.MiningAccounts.Add(original);
            context.SaveChanges();
            var controller = CreateControllerWithUserClaim<MiningWorkersController>(context);

            // Act
            await controller.PostMiningWorker(
                new MiningWorker
                {
                    Name = "Worker 1",
                    MiningAccount = new MiningAccount { Id = 1, Identifier = "account2" }
                }
                );

            // Assert
            var miningAccounts = await context.MiningAccounts.ToListAsync();
            // Check if no new element were added.
            var miningAccount = Assert.Single(miningAccounts);
            // Check if the first element is unchanged.
            Assert.Equal(miningAccount.Identifier, original.Identifier);
        }
    }
}
