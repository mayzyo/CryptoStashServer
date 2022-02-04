using CryptoStashServer.Controllers;
using CryptoStashServer.Data;
using CryptoStashServer.Models;
using CryptoStashServer.Tests.Utilities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace CryptoStashServer.Tests
{
    public class MiningPoolsControllerTest : EntityControllerTest
    {
        [Fact]
        public async Task PutMiningPool_DoesNotAlterTokensProperty()
        {
            // Arrange
            var currency1 = new Token { Ticker = "eth", Name = "Ethereum" };
            var currency2 = new Token { Ticker = "zil", Name = "Zilliqa" };
            var miningPool = new MiningPool
            {
                Name = "My Pool",
                Tokens = new List<Token> { currency1 }
            };

            using var arrangeContext = StubContext<MiningContext>();
            arrangeContext.Tokens.Add(currency1);
            arrangeContext.Tokens.Add(currency2);
            arrangeContext.MiningPools.Add(miningPool);
            arrangeContext.SaveChanges();
            arrangeContext.Dispose();

            // Act
            using var actContext = StubContext<MiningContext>();
            var controller = CreateControllerWithUserClaim<MiningPoolsController>(actContext);
            await controller.PutMiningPool(
                1,
                new MiningPool
                {
                    Id = 1,
                    Name = "My Pool",
                    Tokens = new List<Token>
                    {
                        new Token { Id = 1, Ticker = "eth", Name = "Zilliqa" },
                        new Token { Id = 2, Ticker = "zil", Name = "Zilliqa" }
                    }
                }
                );
            actContext.Dispose();

            // Assert
            using var assertContext = StubContext<MiningContext>();
            var miningPools = await assertContext.MiningPools
                .Include(e => e.Tokens)
                .ToListAsync();
            // Check if element currencies has 1 elements.
            Assert.Equal(1, miningPools.Single().Tokens.Count);
            // Check if the element is unchanged.
            Assert.Equal(
                currency1.Name,
                miningPools.Single().Tokens.First(e => e.Id == currency1.Id).Name
                );
        }

        [Fact]
        public async Task PostMiningPool_DoesNotChangeToken()
        {
            // Arrange
            var original = new Token { Ticker = "eth", Name = "Ethereum" };

            using var financeContext = StubContext<FinanceContext>();
            financeContext.Tokens.Add(original);
            financeContext.SaveChanges();

            using var miningContext = StubContext<MiningContext>();
            miningContext.Tokens.Add(original);
            miningContext.SaveChanges();

            // Act
            using var financeContext2 = StubContext<FinanceContext>();
            using var miningContext2 = StubContext<MiningContext>();
            var controller = CreateControllerWithUserClaim<MiningPoolsController>(miningContext2);
            await controller.PostMiningPool(
                new MiningPool
                {
                    Name = "My Pool",
                    Tokens = new Token[] {
                        new Token { Id = 1, Ticker = "eth", Name = "Ethereum Classic" },
                        new Token { Ticker = "btc", Name = "Bitcoin" }
                    }
                }
                );

            var financeCurrencies = await financeContext2.Tokens.ToListAsync();
            var miningCurrencies = await miningContext2.Tokens.ToListAsync();

            // Assert
            // Check if no new element were added.
            Assert.Single(financeCurrencies);
            Assert.Single(miningCurrencies);
            // Check if the first element is unchanged.
            Assert.Equal(financeCurrencies[0].Name, original.Name);
            Assert.Equal(miningCurrencies[0].Name, original.Name);
        }

        [Fact]
        public async Task PutMiningPoolToken_DoesRemoveRelationship()
        {
            // Arrange
            using var context = StubContext<MiningContext>();
            context.MiningPools.Add(
                new MiningPool
                {
                    Name = "My Pool",
                    Tokens = new Token[] { 
                        new Token { Ticker = "eth", Name = "Ethereum" }
                    }
                }
                );
            context.SaveChanges();

            // Act
            using var context2 = StubContext<MiningContext>();
            var controller = CreateControllerWithUserClaim<MiningPoolsController>(context2);
            await controller.PutMiningPoolTokens(1, Array.Empty<Token>());
            var miningPools = await context2.MiningPools.Include(e => e.Tokens).ToListAsync();

            // Assert
            Assert.Empty(miningPools.Single().Tokens);
        }

        [Fact]
        public async Task PutMiningPoolToken_DoesNotChangeCurrency()
        {
            // Arrange
            using var context = StubContext<MiningContext>();
            context.MiningPools.Add(
                new MiningPool
                {
                    Name = "My Pool",
                    Tokens = new Token[] {
                        new Token { Ticker = "eth", Name = "Ethereum" }
                    }
                }
                );
            context.SaveChanges();

            // Act
            using var context2 = StubContext<MiningContext>();
            var controller = CreateControllerWithUserClaim<MiningPoolsController>(context2);
            await controller.PutMiningPoolTokens(
                1,
                new Token[] {
                    new Token { Id = 1, Ticker = "eth", Name = "Bitcoin" }
                }
                );

            var miningPools = context2.MiningPools.Include(e => e.Tokens).ToList();
            var currencies = context2.Tokens.ToList();

            // Assert
            Assert.Single(miningPools);
            Assert.Single(miningPools.Single().Tokens);

            Assert.Single(currencies);
            Assert.Equal("Ethereum", currencies.Single().Name);
        }
    }
}
