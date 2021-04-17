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
using Cosmos.Abstracts;
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
                Id = ObjectId.GenerateNewId().ToString(),
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
                Id = ObjectId.GenerateNewId().ToString(),
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
                Id = ObjectId.GenerateNewId().ToString()
            };

            await store.SetUserNameAsync(user, userName, CancellationToken.None);
            user.UserName.Should().Be(userName);
        }

        [Fact]
        public async Task GetNormalizedUserNameAsyncTest()
        {
            var store = Services.GetRequiredService<IUserStore<IdentityUser>>();

            var user = new IdentityUser
            {
                Id = ObjectId.GenerateNewId().ToString(),
                UserName = "UserName" + DateTime.Now.Ticks,
            };

            user.NormalizedUserName = user.UserName.ToUpperInvariant();

            var userName = await store.GetNormalizedUserNameAsync(user, CancellationToken.None);
            userName.Should().Be(user.NormalizedUserName);
        }

        [Fact]
        public async Task SetNormalizedUserNameAsyncTest()
        {
            var store = Services.GetRequiredService<IUserStore<IdentityUser>>();

            var user = new IdentityUser
            {
                Id = ObjectId.GenerateNewId().ToString(),
                UserName = "UserName" + DateTime.Now.Ticks,
            };
            var normalizedUserName = user.UserName.ToUpperInvariant();

            await store.SetNormalizedUserNameAsync(user, normalizedUserName, CancellationToken.None);
            user.NormalizedUserName.Should().Be(normalizedUserName);
        }

        [Fact]
        public async Task CreateAsyncTest()
        {
            var store = Services.GetRequiredService<IUserStore<IdentityUser>>();

            var user = CreateUser();

            var createResult = await store.CreateAsync(user, CancellationToken.None);
            createResult.Should().Be(IdentityResult.Success);
        }

        [Fact]
        public async Task CreateAsyncNullTest()
        {
            var store = Services.GetRequiredService<IUserStore<IdentityUser>>();

            IdentityUser user = null;

            Func<Task> testFunction = async () => await store.CreateAsync(user, CancellationToken.None);
            await testFunction.Should().ThrowAsync<ArgumentNullException>();
        }

        [Fact]
        public async Task UpdateAsyncTest()
        {
            var store = Services.GetRequiredService<IUserStore<IdentityUser>>();

            var user = CreateUser();

            var createResult = await store.CreateAsync(user, CancellationToken.None);
            createResult.Should().Be(IdentityResult.Success);

            var foundUser = await store.FindByIdAsync(user.Id, CancellationToken.None);
            foundUser.Should().NotBeNull();
            foundUser.UserName.Should().Be(user.UserName);

            foundUser.PhoneNumber = "999-555-1212";
            var updateResult = await store.UpdateAsync(user, CancellationToken.None);
            updateResult.Should().Be(IdentityResult.Success);
        }

        [Fact]
        public async Task UpdateAsyncNullTest()
        {
            var store = Services.GetRequiredService<IUserStore<IdentityUser>>();

            IdentityUser user = null;

            Func<Task> testFunction = async () => await store.UpdateAsync(user, CancellationToken.None);
            await testFunction.Should().ThrowAsync<ArgumentNullException>();
        }

        [Fact]
        public async Task DeleteAsyncTest()
        {
            var store = Services.GetRequiredService<IUserStore<IdentityUser>>();

            var user = CreateUser();

            var createResult = await store.CreateAsync(user, CancellationToken.None);
            createResult.Should().Be(IdentityResult.Success);

            var foundUser = await store.FindByIdAsync(user.Id, CancellationToken.None);
            foundUser.Should().NotBeNull();
            foundUser.UserName.Should().Be(user.UserName);

            var deleteResult = await store.DeleteAsync(foundUser, CancellationToken.None);
            deleteResult.Should().Be(IdentityResult.Success);

        }

        [Fact]
        public async Task DeleteAsyncNullTest()
        {
            var store = Services.GetRequiredService<IUserStore<IdentityUser>>();

            IdentityUser user = null;

            Func<Task> testFunction = async () => await store.DeleteAsync(user, CancellationToken.None);
            await testFunction.Should().ThrowAsync<ArgumentNullException>();
        }

        [Fact]
        public async Task FindByIdAsyncTest()
        {
            var store = Services.GetRequiredService<IUserStore<IdentityUser>>();

            var user = CreateUser();

            var createResult = await store.CreateAsync(user, CancellationToken.None);
            createResult.Should().Be(IdentityResult.Success);

            var foundUser = await store.FindByIdAsync(user.Id, CancellationToken.None);
            foundUser.Should().NotBeNull();
            foundUser.UserName.Should().Be(user.UserName);

        }

        [Fact]
        public async Task FindByIdAsyncFailTest()
        {
            var store = Services.GetRequiredService<IUserStore<IdentityUser>>();

            var userId = ObjectId.GenerateNewId().ToString();

            var foundUser = await store.FindByIdAsync(userId, CancellationToken.None);
            foundUser.Should().BeNull();
        }

        [Fact]
        public async Task FindByNameAsyncTest()
        {
            var store = Services.GetRequiredService<IUserStore<IdentityUser>>();

            var user = CreateUser();

            var createResult = await store.CreateAsync(user, CancellationToken.None);
            createResult.Should().Be(IdentityResult.Success);

            var foundUser = await store.FindByNameAsync(user.NormalizedUserName, CancellationToken.None);
            foundUser.Should().NotBeNull();
            foundUser.UserName.Should().Be(user.UserName);

        }

        [Fact]
        public async Task FindByNameAsyncFailTest()
        {
            var store = Services.GetRequiredService<IUserStore<IdentityUser>>();

            var userName = "UserFail" + DateTime.Now.Ticks;

            var foundUser = await store.FindByNameAsync(userName, CancellationToken.None);
            foundUser.Should().BeNull();
        }
    }

}