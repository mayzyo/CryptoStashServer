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
            using var arrangeFContext = StubContext<FinanceContext>();
            arrangeFContext.Currencies.Add(
                new Currency { Ticker = "eth", Name = "Ethereum" }
                );
            arrangeFContext.SaveChanges();

            using var arrangeMContext = StubContext<MiningContext>();
            arrangeMContext.Currencies.Add(
                new Currency { Ticker = "eth", Name = "Ethereum" }
                );
            arrangeMContext.SaveChanges();

            arrangeFContext.Dispose();
            arrangeMContext.Dispose();

            // Act
            using var actFContext = StubContext<FinanceContext>();
            using var actMContext = StubContext<MiningContext>();
            var controller = CreateControllerWithUserClaim<CurrenciesController>(actFContext, actMContext);
            await controller.DeleteCurrency(1);

            actFContext.Dispose();
            actMContext.Dispose();

            // Assert
            using var assertFContext = StubContext<FinanceContext>();
            using var assertMContext = StubContext<MiningContext>();
            var financeResult = await assertFContext.Currencies.AnyAsync();
            var miningResult = await assertMContext.Currencies.AnyAsync();
            Assert.False(financeResult);
            Assert.False(miningResult);
        }

        //[Fact]
        //public async Task GetCurrencyWallets_ReturnsWallets()
        //{
        //    // Arrange
        //    var currency = new Currency { Name = "ETHEREUM", Ticker = "ETH" };
        //    var wallet = new Wallet
        //    {
        //        Owner = "user1",
        //        Address = "abcd",
        //        Currencies = new List<Currency>() { currency }
        //    };

        //    using var arrangeContext = StubContext<FinanceContext>();
        //    arrangeContext.Currencies.Add(currency);
        //    arrangeContext.Wallets.Add(wallet);
        //    arrangeContext.SaveChanges();
        //    arrangeContext.Dispose();

        //    // Act
        //    using var actFContext = StubContext<FinanceContext>();
        //    using var actMContext = StubContext<MiningContext>();
        //    var controller = CreateControllerWithUserClaim<CurrenciesController>(actFContext, actMContext);
        //    var result = await controller.GetCurrencyWallets(currency.Id);
        //    actFContext.Dispose();
        //    actMContext.Dispose();

        //    // Assert
        //    var viewResult = Assert.IsType<ActionResult<IEnumerable<Wallet>>>(result);
        //    var model = Assert.IsAssignableFrom<List<Wallet>>(viewResult.Value);
        //    var walletModel = Assert.Single(model);
        //    Assert.Equal(walletModel.Id, wallet.Id);
        //}
    }
}
