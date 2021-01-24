using Xunit;
using Cosmos.Identity;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Xunit.Abstractions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using System.Threading;
using FluentAssertions;

namespace Cosmos.Identity.Tests
{

    public class UserStoreTests : TestServiceBase
    {
        public UserStoreTests(ITestOutputHelper outputHelper) : base(outputHelper)
        {
        }

        [Fact]
        public async Task GetUserIdAsyncTest()
        {
            var store = Services.GetRequiredService<IUserStore<IdentityUser>>();

            var user = new IdentityUser
            {
                Id = Guid.NewGuid().ToString(),
            };

            var userId = await store.GetUserIdAsync(user, CancellationToken.None);
            userId.Should().Be(user.Id);
        }

        [Fact]
        public async Task GetUserNameAsyncTest()
        {
            var store = Services.GetRequiredService<IUserStore<IdentityUser>>();

            var user = new IdentityUser
            {
                Id = Guid.NewGuid().ToString(),
                UserName = "UserName" + DateTime.Now.Ticks
            };

            var userName = await store.GetUserNameAsync(user, CancellationToken.None);
            userName.Should().Be(user.UserName);
        }

        [Fact]
        public async Task SetUserNameAsyncTest()
        {
            var store = Services.GetRequiredService<IUserStore<IdentityUser>>();

            string userName = "UserName" + DateTime.Now.Ticks;
            var user = new IdentityUser
            {
                Id = Guid.NewGuid().ToString()
            };

            await store.SetUserNameAsync(user, userName, CancellationToken.None);
            user.UserName.Should().Be(userName);
        }

        [Fact]
        public async Task GetNormalizedUserNameAsyncTest()
        {

        }

        [Fact]
        public async Task SetNormalizedUserNameAsyncTest()
        {

        }

        [Fact]
        public async Task FindByIdAsyncTest()
        {

        }

        [Fact]
        public async Task FindByNameAsyncTest()
        {

        }
    }

}