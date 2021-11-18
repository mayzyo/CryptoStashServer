using CryptoStashStats.Controllers;
using CryptoStashStats.Data;
using CryptoStashStats.Models;
using CryptoStashStats.Tests.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace CryptoStashStats.Tests
{
    public class CurrenciesControllerTest : EntityControllerTest
    {
        [Fact]
        public async Task GetCurrencies_ReturnsSingleValueFromMultipleContext()
        {
            // Arrange
            using var financeContext = StubContext<FinanceContext>();
            financeContext.Currencies.Add(
                new Currency { Ticker = "eth", Name = "Ethereum" }
                );
            financeContext.SaveChanges();

            using var miningContext = StubContext<MiningContext>();
            miningContext.Currencies.Add(
                new Currency { Ticker = "eth", Name = "Ethereum" }
                );
            miningContext.SaveChanges();

            // Act
            using var financeContext2 = StubContext<FinanceContext>();
            using var miningContext2 = StubContext<MiningContext>();
            var controller = new CurrenciesController(financeContext2, miningContext2);
            var result = await controller.GetCurrencies();

            // Assert
            var viewResult = Assert.IsType<ActionResult<IEnumerable<Currency>>>(result);
            var model = Assert.IsAssignableFrom<List<Currency>>(viewResult.Value);
            Assert.Single(model);
        }

        [Fact]
        public async Task PostCurrency_PropagatesToMultipleContext()
        {
            // Arrange
            using var financeContext = StubContext<FinanceContext>();
            using var miningContext = StubContext<MiningContext>();

            // Act
            var controller = new CurrenciesController(financeContext, miningContext);
            await controller.PostCurrency(
                new Currency { Ticker = "eth", Name = "Ethereum" }
                );
            var financeCurrency = await financeContext.Currencies.FirstAsync();
            var miningCurrency = await miningContext.Currencies.FirstAsync();

            // Assert
            var financeModel = Assert.IsAssignableFrom<Currency>(financeCurrency);
            var miningModel = Assert.IsAssignableFrom<Currency>(miningCurrency);
            Assert.Equal(financeModel, miningModel);
        }

        [Fact]
        public async Task DeleteCurrency_DeletesFromMultipleContext()
        {
            // Arrange
            using var financeContext = StubContext<FinanceContext>();
            financeContext.Currencies.Add(
                new Currency { Ticker = "eth", Name = "Ethereum" }
                );
            financeContext.SaveChanges();

            using var miningContext = StubContext<MiningContext>();
            miningContext.Currencies.Add(
                new Currency { Ticker = "eth", Name = "Ethereum" }
                );
            miningContext.SaveChanges();

            // Act
            using var financeContext2 = StubContext<FinanceContext>();
            using var miningContext2 = StubContext<MiningContext>();
            var controller = new CurrenciesController(financeContext2, miningContext2);
            await controller.DeleteCurrency(1);
            var financeResult = await financeContext2.Currencies.AnyAsync();
            var miningResult = await miningContext2.Currencies.AnyAsync();

            // Assert
            Assert.Equal(financeResult, miningResult);
            Assert.False(financeResult);
            Assert.False(miningResult);
        }
    }
}
