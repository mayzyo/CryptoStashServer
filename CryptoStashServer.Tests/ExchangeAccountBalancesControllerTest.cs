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
    public class ExchangeAccountBalancesControllerTest : EntityControllerTest
    {
        [Fact]
        public async Task PostExchangeAccountBalance_DoesAddCurrencyToAccount()
        {
            // Arrange
            var currency = new Token { Name = "ETHEREUM", Ticker = "ETH" };
            var currencyExchange = new CurrencyExchange { Name = "BINANCE" };
            var exchangeAccount = new ExchangeAccount
            {
                Owner = "user1",
                CurrencyExchange = currencyExchange,
                ExchangeAccountApiKey = new ExchangeAccountApiKey { PublicKey = "abcd", PrivateKey = "1234" }
            };

            using var arrangeContext = StubContext<FinanceContext>();
            arrangeContext.Tokens.Add(currency);
            arrangeContext.CurrencyExchanges.Add(currencyExchange);
            arrangeContext.ExchangeAccounts.Add(exchangeAccount);
            arrangeContext.SaveChanges();
            arrangeContext.Dispose();

            // Act
            using var actContext = StubContext<FinanceContext>();
            var controller = CreateControllerWithUserClaim<ExchangeAccountBalancesController>(actContext);
            await controller.PostExchangeAccountBalance(
                exchangeAccount.Id,
                new ExchangeAccountBalance
                {
                    ExchangeAccount = exchangeAccount,
                    Savings = 1000,
                    Token = currency
                }
                );

            // Assert
            using var assertContext = StubContext<FinanceContext>();
            var exchangeAccountModel = Assert.Single(
                await assertContext.ExchangeAccounts
                    .Include(e => e.Tokens)
                    .ToListAsync()
                );
            Assert.NotNull(exchangeAccountModel.Tokens);
            var model = Assert.Single(exchangeAccountModel.Tokens);
            Assert.Equal(model.Id, currency.Id);
        }
    }
}
