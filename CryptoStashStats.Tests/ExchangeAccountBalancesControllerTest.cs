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
    public class ExchangeAccountBalancesControllerTest : EntityControllerTest
    {
        [Fact]
        public async Task PostExchangeAccountBalance_DoesAddCurrencyToAccount()
        {
            // Arrange
            var currency = new Currency { Name = "ETHEREUM", Ticker = "ETH" };
            var currencyExchange = new CurrencyExchange { Name = "BINANCE" };
            var exchangeAccount = new ExchangeAccount
            {
                Owner = "user1",
                CurrencyExchange = currencyExchange,
                ExchangeAccountApiKey = new ExchangeAccountApiKey { PublicKey = "abcd", PrivateKey = "1234" }
            };

            using var arrangeContext = StubContext<FinanceContext>();
            arrangeContext.Currencies.Add(currency);
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
                    Currency = currency
                }
                );

            // Assert
            using var assertContext = StubContext<FinanceContext>();
            var exchangeAccountModel = Assert.Single(
                await assertContext.ExchangeAccounts
                    .Include(e => e.Currencies)
                    .ToListAsync()
                );
            Assert.NotNull(exchangeAccountModel.Currencies);
            var model = Assert.Single(exchangeAccountModel.Currencies);
            Assert.Equal(model.Id, currency.Id);
        }
    }
}
