using CryptoStashStats.Controllers;
using CryptoStashStats.Data;
using CryptoStashStats.Models;
using CryptoStashStats.Tests.Utilities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace CryptoStashStats.Tests
{
    public class MiningPoolsControllerTest : EntityControllerTest
    {
        [Fact]
        public async Task PutMiningPool_DoesNotAlterCurrenciesProperty()
        {
            // Arrange
            var original = new Currency { Ticker = "eth", Name = "Ethereum" };
            var original2 = new Currency { Ticker = "zil", Name = "Zilliqa" };

            using var financeContext = StubContext<FinanceContext>();
            financeContext.Currencies.Add(original);
            financeContext.Currencies.Add(original2);
            financeContext.SaveChanges();

            using var miningContext = StubContext<MiningContext>();
            miningContext.MiningPools.Add(
                new MiningPool
                {
                    Name = "My Pool",
                    Currencies = new Currency[] {
                        original
                        //new Currency { Id = 1, Ticker = "eth", Name = "Ethereum" },
                    }
                }
                );
            miningContext.Currencies.Add(original2);
            miningContext.SaveChanges();

            // Act
            using var financeContext2 = StubContext<FinanceContext>();
            using var miningContext2 = StubContext<MiningContext>();
            var controller = CreateControllerWithUserClaim<MiningPoolsController>(miningContext2);
            await controller.PutMiningPool(
                1,
                new MiningPool
                {
                    Id = 1,
                    Name = "My Pool",
                    Currencies = new Currency[]
                    {
                        new Currency { Id = 1, Ticker = "eth", Name = "Ethereum" },
                        new Currency { Id = 2, Ticker = "zil", Name = "Ethereum" }
                    }
                    //Currencies = new List<Currency>()
                }
                );
            var miningPools = await miningContext2.MiningPools.Include(e => e.Currencies).ToListAsync();

            // Assert
            // Check if element currencies has 2 elements.
            Assert.Equal(2, miningPools.Single().Currencies.Count);
            // Check if the second element is unchanged.
            Assert.Equal(original2.Name, miningPools.Single().Currencies.First(e => e.Id == original2.Id).Name);
        }

        [Fact]
        public async Task PostMiningPool_DoesNotChangeCurrency()
        {
            // Arrange
            var original = new Currency { Ticker = "eth", Name = "Ethereum" };

            using var financeContext = StubContext<FinanceContext>();
            financeContext.Currencies.Add(original);
            financeContext.SaveChanges();

            using var miningContext = StubContext<MiningContext>();
            miningContext.Currencies.Add(original);
            miningContext.SaveChanges();

            // Act
            using var financeContext2 = StubContext<FinanceContext>();
            using var miningContext2 = StubContext<MiningContext>();
            var controller = CreateControllerWithUserClaim<MiningPoolsController>(miningContext2);
            await controller.PostMiningPool(
                new MiningPool
                {
                    Name = "My Pool",
                    Currencies = new Currency[] {
                        new Currency { Id = 1, Ticker = "eth", Name = "Ethereum Classic" },
                        new Currency { Ticker = "btc", Name = "Bitcoin" }
                    }
                }
                );

            var financeCurrencies = await financeContext2.Currencies.ToListAsync();
            var miningCurrencies = await miningContext2.Currencies.ToListAsync();

            // Assert
            // Check if no new element were added.
            Assert.Single(financeCurrencies);
            Assert.Single(miningCurrencies);
            // Check if the first element is unchanged.
            Assert.Equal(financeCurrencies[0].Name, original.Name);
            Assert.Equal(miningCurrencies[0].Name, original.Name);
        }

        [Fact]
        public async Task PutMiningPoolCurrency_DoesRemoveRelationship()
        {
            // Arrange
            using var context = StubContext<MiningContext>();
            context.MiningPools.Add(
                new MiningPool
                {
                    Name = "My Pool",
                    Currencies = new Currency[] { 
                        new Currency { Ticker = "eth", Name = "Ethereum" }
                    }
                }
                );
            context.SaveChanges();

            // Act
            using var context2 = StubContext<MiningContext>();
            var controller = CreateControllerWithUserClaim<MiningPoolsController>(context2);
            await controller.PutMiningPoolCurrency(1, Array.Empty<Currency>());
            var miningPools = await context2.MiningPools.Include(e => e.Currencies).ToListAsync();

            // Assert
            Assert.Empty(miningPools.Single().Currencies);
        }

        [Fact]
        public async Task PutMiningPoolCurrency_DoesNotChangeCurrency()
        {
            // Arrange
            using var context = StubContext<MiningContext>();
            context.MiningPools.Add(
                new MiningPool
                {
                    Name = "My Pool",
                    Currencies = new Currency[] {
                        new Currency { Ticker = "eth", Name = "Ethereum" }
                    }
                }
                );
            context.SaveChanges();

            // Act
            using var context2 = StubContext<MiningContext>();
            var controller = CreateControllerWithUserClaim<MiningPoolsController>(context2);
            await controller.PutMiningPoolCurrency(
                1,
                new Currency[] {
                    new Currency { Id = 1, Ticker = "eth", Name = "Bitcoin" }
                }
                );

            var miningPools = context2.MiningPools.Include(e => e.Currencies).ToList();
            var currencies = context2.Currencies.ToList();

            // Assert
            Assert.Single(miningPools);
            Assert.Single(miningPools.Single().Currencies);

            Assert.Single(currencies);
            Assert.Equal("Ethereum", currencies.Single().Name);
        }
    }
}
