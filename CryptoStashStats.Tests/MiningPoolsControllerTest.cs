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
            var currency1 = new Currency { Ticker = "eth", Name = "Ethereum" };
            var currency2 = new Currency { Ticker = "zil", Name = "Zilliqa" };
            var miningPool = new MiningPool
            {
                Name = "My Pool",
                Currencies = new List<Currency> { currency1 }
            };

            using var arrangeContext = StubContext<MiningContext>();
            arrangeContext.Currencies.Add(currency1);
            arrangeContext.Currencies.Add(currency2);
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
                    Currencies = new List<Currency>
                    {
                        new Currency { Id = 1, Ticker = "eth", Name = "Zilliqa" },
                        new Currency { Id = 2, Ticker = "zil", Name = "Zilliqa" }
                    }
                }
                );
            actContext.Dispose();

            // Assert
            using var assertContext = StubContext<MiningContext>();
            var miningPools = await assertContext.MiningPools
                .Include(e => e.Currencies)
                .ToListAsync();
            // Check if element currencies has 1 elements.
            Assert.Equal(1, miningPools.Single().Currencies.Count);
            // Check if the element is unchanged.
            Assert.Equal(
                currency1.Name,
                miningPools.Single().Currencies.First(e => e.Id == currency1.Id).Name
                );
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
