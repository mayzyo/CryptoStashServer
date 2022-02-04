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
    public class ExchangeAccountsControllerTest : EntityControllerTest
    {
        [Fact]
        public async Task PutExchangeAccount_DoesNotChangeForeignObjects()
        {
            // Arrange
            var currencyExchange = new CurrencyExchange { Name = "BINANCE" };
            var apiKey = new ExchangeAccountApiKey { PublicKey = "abcd", PrivateKey = "1234" };
            var exchangeAccount = new ExchangeAccount
            {
                Owner = "user1",
                CurrencyExchange = currencyExchange,
                ExchangeAccountApiKey = apiKey
            };

            using var arrangeContext = StubContext<FinanceContext>();
            arrangeContext.CurrencyExchanges.Add(currencyExchange);
            arrangeContext.ExchangeAccounts.Add(exchangeAccount);
            arrangeContext.SaveChanges();
            arrangeContext.Dispose();

            // Act
            using var actContext = StubContext<FinanceContext>();
            var controller = CreateControllerWithUserClaim<ExchangeAccountsController>(actContext);
            await controller.PutExchangeAccount(
                exchangeAccount.Id,
                new ExchangeAccount
                {
                    Owner = "user1",
                    CurrencyExchange = new CurrencyExchange { Name = "HUOBI" },
                    ExchangeAccountApiKey = new ExchangeAccountApiKey { PublicKey = "efgh", PrivateKey = "1234" }
        }
                );
            actContext.Dispose();

            // Assert
            using var assertConext = StubContext<FinanceContext>();
            var model = Assert.Single(
                await assertConext.ExchangeAccounts
                    .Include(e => e.CurrencyExchange)
                    .Include(e => e.ExchangeAccountApiKey)
                    .ToListAsync()
            );
            Assert.Equal(model.CurrencyExchange.Name, currencyExchange.Name);
            Assert.Equal(model.ExchangeAccountApiKey.PublicKey, apiKey.PublicKey);

            var currencyExchangeModel = Assert.Single(await assertConext.CurrencyExchanges.ToListAsync());
            Assert.Equal(currencyExchangeModel.Name, currencyExchange.Name);

            var apiKeyModel = Assert.Single(await assertConext.ExchangeAccountApiKeys.ToListAsync());
            Assert.Equal(apiKeyModel.PublicKey, apiKey.PublicKey);
        }

        [Fact]
        public async Task PostExchangeAccount_DoesCreateApiKey()
        {
            // Arrange
            var apiKey = new ExchangeAccountApiKey { PublicKey = "abcd", PrivateKey = "1234" };
            var currencyExchange = new CurrencyExchange { Name = "BINANCE" };

            using var context = StubContext<FinanceContext>();
            context.CurrencyExchanges.Add(currencyExchange);
            context.SaveChanges();

            var controller = CreateControllerWithUserClaim<ExchangeAccountsController>(context);

            // Act
            await controller.PostExchangeAccount(
                new ExchangeAccount {
                    Owner = "user1",
                    CurrencyExchange = currencyExchange,
                    ExchangeAccountApiKey = apiKey
                }
                );

            // Assert
            var model = Assert.Single(await context.ExchangeAccountApiKeys.ToListAsync());
            Assert.Equal(model.PublicKey, apiKey.PublicKey);
        }

        [Fact]
        public async Task DeleteExchangeAccount_DoesDeleteApiKey()
        {
            // Arrange
            var currencyExchange = new CurrencyExchange { Name = "BINANCE" };
            var exchangeAccount = new ExchangeAccount
            {
                Owner = "user1",
                CurrencyExchange = currencyExchange,
                ExchangeAccountApiKey = new ExchangeAccountApiKey { PublicKey = "abcd", PrivateKey = "1234" }
            };

            using var context = StubContext<FinanceContext>();
            context.CurrencyExchanges.Add(currencyExchange);
            context.ExchangeAccounts.Add(exchangeAccount);
            context.SaveChanges();

            var controller = CreateControllerWithUserClaim<ExchangeAccountsController>(context);

            // Act
            await controller.DeleteExchangeAccount(exchangeAccount.Id);

            // Assert
            Assert.Empty(await context.ExchangeAccounts.ToListAsync());
            Assert.Empty(await context.ExchangeAccountApiKeys.ToListAsync());
        }

        [Fact]
        public async Task PutExchangeAccountApiKey_DoesNotLeaveOrphan()
        {
            // Arrange
            var currencyExchange = new CurrencyExchange { Name = "BINANCE" };
            var exchangeAccount = new ExchangeAccount
            {
                Owner = "user1",
                CurrencyExchange = currencyExchange,
                ExchangeAccountApiKey = new ExchangeAccountApiKey { PublicKey = "abcd", PrivateKey = "1234" }
            };

            using var context = StubContext<FinanceContext>();
            context.CurrencyExchanges.Add(currencyExchange);
            context.ExchangeAccounts.Add(exchangeAccount);
            context.SaveChanges();

            var controller = CreateControllerWithUserClaim<ExchangeAccountsController>(context);

            // Act
            var newApiKey = new ExchangeAccountApiKey { PublicKey = "efgh", PrivateKey = "5678" };
            exchangeAccount.ExchangeAccountApiKey = newApiKey;
            await controller.PutExchangeAccountApiKey(exchangeAccount.Id, newApiKey);

            // Assert
            var model = Assert.Single(await context.ExchangeAccountApiKeys.ToListAsync());
            Assert.Equal(model.PublicKey, newApiKey.PublicKey);
        }
    }
}
