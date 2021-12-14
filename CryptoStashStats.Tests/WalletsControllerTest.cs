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
    public class WalletsControllerTest : EntityControllerTest
    {
        [Fact]
        public async Task PutWalletCurrencies_DoesNotChangeBlockchainCurrency()
        {
            // Arrange
            var currency1 = new Currency { Ticker = "ETH", Name = "ETHER" };
            var currency2 = new Currency { Ticker = "MATIC", Name = "POLYGON" };
            var blockchain = new Blockchain
            {
                Name = "ETHEREUM",
                NativeCurrency = currency1,
                Currencies = new List<Currency> { currency1 }
            };
            var wallet = new Wallet
            {
                Address = "abcd",
                Currencies = new List<Currency> { currency1 },
                Blockchain = blockchain
            };
            using var arrangeContext = StubContext<FinanceContext>();
            arrangeContext.Currencies.Add(currency1);
            arrangeContext.Currencies.Add(currency2);
            arrangeContext.Blockchains.Add(blockchain);
            arrangeContext.Wallets.Add(wallet);
            arrangeContext.SaveChanges();
            arrangeContext.Dispose();

            // Act
            using var actContext = StubContext<FinanceContext>();
            var controller = CreateControllerWithUserClaim<WalletsController>(actContext);
            await controller.PutWalletCurrencies(
                wallet.Id,
                new List<Currency> { 
                    new Currency { Id = currency1.Id, Ticker = "ETHEREUM", Name = "POLYGON" },
                    new Currency { Ticker = "MATIC", Name = "POLYGON" }
                }
                );
            actContext.Dispose();

            // Assert
            using var assertContext = StubContext<FinanceContext>();
            var walletModel = await assertContext.Wallets
                .Include(e => e.Currencies)
                .FirstOrDefaultAsync();
            var model = Assert.Single(walletModel.Currencies);
            Assert.Equal(model.Name, currency1.Name);
        }
    }
}
