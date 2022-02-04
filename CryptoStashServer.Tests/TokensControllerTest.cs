using CryptoStashServer.Controllers;
using CryptoStashServer.Data;
using CryptoStashServer.Models;
using CryptoStashServer.Tests.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace CryptoStashServer.Tests
{
    public class TokensControllerTest : EntityControllerTest
    {
        [Fact]
        public async Task GetCurrencies_ReturnsSingleValueFromMultipleContext()
        {
            // Arrange
            using var financeContext = StubContext<FinanceContext>();
            financeContext.Tokens.Add(
                new Token { Ticker = "eth", Name = "Ethereum" }
                );
            financeContext.SaveChanges();

            using var miningContext = StubContext<MiningContext>();
            miningContext.Tokens.Add(
                new Token { Ticker = "eth", Name = "Ethereum" }
                );
            miningContext.SaveChanges();

            // Act
            using var financeContext2 = StubContext<FinanceContext>();
            using var miningContext2 = StubContext<MiningContext>();
            var controller = new TokensController(financeContext2, miningContext2);
            var result = await controller.GetTokens();

            // Assert
            var viewResult = Assert.IsType<ActionResult<IEnumerable<Token>>>(result);
            var model = Assert.IsAssignableFrom<List<Token>>(viewResult.Value);
            Assert.Single(model);
        }

        [Fact]
        public async Task PostCurrency_PropagatesToMultipleContext()
        {
            // Arrange
            using var financeContext = StubContext<FinanceContext>();
            using var miningContext = StubContext<MiningContext>();

            // Act
            var controller = new TokensController(financeContext, miningContext);
            await controller.PostToken(
                new Token { Ticker = "eth", Name = "Ethereum" }
                );
            var financeToken = await financeContext.Tokens.FirstAsync();
            var miningToken = await miningContext.Tokens.FirstAsync();

            // Assert
            var financeModel = Assert.IsAssignableFrom<Token>(financeToken);
            var miningModel = Assert.IsAssignableFrom<Token>(miningToken);
            Assert.Equal(financeModel, miningModel);
        }

        [Fact]
        public async Task DeleteCurrency_DeletesFromMultipleContext()
        {
            // Arrange
            using var arrangeFContext = StubContext<FinanceContext>();
            arrangeFContext.Tokens.Add(
                new Token { Ticker = "eth", Name = "Ethereum" }
                );
            arrangeFContext.SaveChanges();

            using var arrangeMContext = StubContext<MiningContext>();
            arrangeMContext.Tokens.Add(
                new Token { Ticker = "eth", Name = "Ethereum" }
                );
            arrangeMContext.SaveChanges();

            arrangeFContext.Dispose();
            arrangeMContext.Dispose();

            // Act
            using var actFContext = StubContext<FinanceContext>();
            using var actMContext = StubContext<MiningContext>();
            var controller = CreateControllerWithUserClaim<TokensController>(actFContext, actMContext);
            await controller.DeleteToken(1);

            actFContext.Dispose();
            actMContext.Dispose();

            // Assert
            using var assertFContext = StubContext<FinanceContext>();
            using var assertMContext = StubContext<MiningContext>();
            var financeResult = await assertFContext.Tokens.AnyAsync();
            var miningResult = await assertMContext.Tokens.AnyAsync();
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
