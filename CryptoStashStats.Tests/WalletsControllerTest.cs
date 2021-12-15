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
            var currency1 = new Token { Ticker = "ETH", Name = "ETHER" };
            var currency2 = new Token { Ticker = "MATIC", Name = "POLYGON" };
            var blockchain = new Blockchain
            {
                Name = "ETHEREUM",
                NativeToken = currency1,
                Tokens = new List<Token> { currency1 }
            };
            var wallet = new Wallet
            {
                Address = "abcd",
                Tokens = new List<Token> { currency1 },
                Blockchain = blockchain
            };
            using var arrangeContext = StubContext<FinanceContext>();
            arrangeContext.Tokens.Add(currency1);
            arrangeContext.Tokens.Add(currency2);
            arrangeContext.Blockchains.Add(blockchain);
            arrangeContext.Wallets.Add(wallet);
            arrangeContext.SaveChanges();
            arrangeContext.Dispose();

            // Act
            using var actContext = StubContext<FinanceContext>();
            var controller = CreateControllerWithUserClaim<WalletsController>(actContext);
            await controller.PutWalletCurrencies(
                wallet.Id,
                new List<Token> { 
                    new Token { Id = currency1.Id, Ticker = "ETHEREUM", Name = "POLYGON" },
                    new Token { Ticker = "MATIC", Name = "POLYGON" }
                }
                );
            actContext.Dispose();

            // Assert
            using var assertContext = StubContext<FinanceContext>();
            var walletModel = await assertContext.Wallets
                .Include(e => e.Tokens)
                .FirstOrDefaultAsync();
            var model = Assert.Single(walletModel.Tokens);
            Assert.Equal(model.Name, currency1.Name);
        }
    }
}
