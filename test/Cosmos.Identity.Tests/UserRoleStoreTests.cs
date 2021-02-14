using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using Xunit.Abstractions;

namespace Cosmos.Identity.Tests
{
    public class UserRoleStoreTests : TestServiceBase
    {
        public UserRoleStoreTests(ITestOutputHelper outputHelper) : base(outputHelper)
        {
        }

        [Fact]
        public async Task AddToRoleAsyncTest()
        {
            var store = Services.GetRequiredService<IUserRoleStore<IdentityUser>>();

            var user = CreateUser();
            var roleName = "role";

            await store.AddToRoleAsync(user, roleName, CancellationToken.None);
            user.Roles.Should().NotBeEmpty();
            user.Roles.First().Should().Be(roleName);

        }

        [Fact]
        public async Task RemoveFromRoleAsyncTest()
        {
            var store = Services.GetRequiredService<IUserRoleStore<IdentityUser>>();

            var user = CreateUser();
            user.Roles.Add("role");
            user.Roles.Add("other");

            await store.RemoveFromRoleAsync(user, "role", CancellationToken.None);
            user.Roles.Count.Should().Be(1);
            user.Roles.First().Should().Be("other");
        }

        [Fact]
        public async Task GetRolesAsyncTest()
        {
            var store = Services.GetRequiredService<IUserRoleStore<IdentityUser>>();

            var user = CreateUser();
            user.Roles.Add("role");
            user.Roles.Add("other");

            var roles = await store.GetRolesAsync(user, CancellationToken.None);
            roles.Count.Should().Be(2);
            roles[0].Should().Be("role");
        }

        [Fact]
        public async Task IsInRoleAsyncTest()
        {
            var store = Services.GetRequiredService<IUserRoleStore<IdentityUser>>();

            var user = CreateUser();
            user.Roles.Add("role");
            user.Roles.Add("other");

            var result = await store.IsInRoleAsync(user, "role", CancellationToken.None);
            result.Should().BeTrue();

            var resultFail = await store.IsInRoleAsync(user, "role2", CancellationToken.None);
            resultFail.Should().BeFalse();
        }

        [Fact]
        public async Task GetUsersInRoleAsyncTest()
        {
            var store = Services.GetRequiredService<IUserRoleStore<IdentityUser>>();

            var user = CreateUser();

            var roleName = $"role-{DateTime.Now.Ticks}";
            user.Roles.Add(roleName);

            await store.CreateAsync(user, CancellationToken.None);

            var users = await store.GetUsersInRoleAsync(roleName, CancellationToken.None);

            users.Should().NotBeNull();
            users.Should().NotBeEmpty();
            users.Count.Should().Be(1);
            users[0].UserName.Should().Be(user.UserName);
        }

        [Fact]
        public async Task GetUsersInRoleAsyncFailTest()
        {
            var store = Services.GetRequiredService<IUserRoleStore<IdentityUser>>();

            var user = CreateUser();

            var roleName = $"role-{DateTime.Now.Ticks}";
            user.Roles.Add(roleName);

            await store.CreateAsync(user, CancellationToken.None);

            var users = await store.GetUsersInRoleAsync(roleName + "fail", CancellationToken.None);

            users.Should().NotBeNull();
            users.Should().BeEmpty();
        }
    }

}