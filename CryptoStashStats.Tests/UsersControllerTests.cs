using CryptoStashStats.Controllers;
using CryptoStashStats.Data;
using CryptoStashStats.Models;
using CryptoStashStats.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace CryptoStashStats.Tests
{
    public class UsersControllerTests
    {
        [Fact]
        public async Task GetUsers_ReturnsAViewResult_WithAListOfUsers()
        {
            var dbName = $"{this.GetType().Name}_{GetCaller()}";
            //Assert.Equal("GetUsers_ReturnsAViewResult_WithAListOfUsers", dbName);

            // Insert seed data into the database using one instance of the context
            using (var context = GenerateNewContext<UserContext>(dbName))
            {
                // Arrange
                context.User.Add(
                new User()
                {
                    Username = "Michael"
                }
                );
                context.SaveChanges();
            }

            using (var context = GenerateNewContext<UserContext>(dbName))
            {
                var passwordHelper = new Mock<IPasswordHelper>();
                var controller = new UsersController(context, passwordHelper.Object);
                // Act
                var result = await controller.GetUsers();
                // Assert
                var viewResult = Assert.IsType<ActionResult<IEnumerable<User>>>(result);
                var model = Assert.IsAssignableFrom<List<User>>(viewResult.Value);
                Assert.Single(model);
            }
        }

        [Fact]
        public async Task PutUser_ReturnsBadRequestResult_WhenModelStateIsInvalid()
        {
            var dbName = $"{this.GetType().Name}_{GetCaller()}";
            // Insert seed data into the database using one instance of the context
            using (var context = GenerateNewContext<UserContext>(dbName))
            {
                // Arrange
                var passwordHelper = new Mock<IPasswordHelper>();
                var controller = new UsersController(context, passwordHelper.Object);
                var newUser = new User() { Id = 2, Username = "Michael" };
                // Act
                var result = await controller.PutUser(1, newUser);
                // Assert
                Assert.IsType<BadRequestResult>(result);
            }
        }

        private T GenerateNewContext<T>(string testName) where T : DbContext
        {
            var options = new DbContextOptionsBuilder<T>()
                .UseInMemoryDatabase(databaseName: testName)
                .Options;

            return (T)Activator.CreateInstance(typeof(T), new object[] { options });
        }

        private string GetCaller([System.Runtime.CompilerServices.CallerMemberName] string memberName = "")
        {
            return memberName;
        }
    }
}
