using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using Xunit.Abstractions;

namespace Cosmos.Identity.Tests
{
    public class UserTwoFactorStoreTests : TestServiceBase
    {
        public UserTwoFactorStoreTests(ITestOutputHelper outputHelper) : base(outputHelper)
        {
        }

        [Fact]
        public async Task SetTwoFactorEnabledAsyncTrueTest()
        {
            var store = Services.GetRequiredService<IUserTwoFactorStore<IdentityUser>>();

            var user = new IdentityUser
            {
                Id = Guid.NewGuid().ToString(),
                UserName = "UserName" + DateTime.Now.Ticks,
                TwoFactorEnabled = false
            };

            await store.SetTwoFactorEnabledAsync(user, true, CancellationToken.None);
            user.TwoFactorEnabled.Should().BeTrue();
        }

        [Fact]
        public async Task SetTwoFactorEnabledAsyncFalseTest()
        {
            var store = Services.GetRequiredService<IUserTwoFactorStore<IdentityUser>>();

            var user = new IdentityUser
            {
                Id = Guid.NewGuid().ToString(),
                UserName = "UserName" + DateTime.Now.Ticks,
                TwoFactorEnabled = true
            };

            await store.SetTwoFactorEnabledAsync(user, false, CancellationToken.None);
            user.TwoFactorEnabled.Should().BeFalse();
        }

        [Fact]
        public async Task GetTwoFactorEnabledAsyncTest()
        {
            var store = Services.GetRequiredService<IUserTwoFactorStore<IdentityUser>>();

            var user = new IdentityUser
            {
                Id = Guid.NewGuid().ToString(),
                UserName = "UserName" + DateTime.Now.Ticks,
                TwoFactorEnabled = true
            };

            var result = await store.GetTwoFactorEnabledAsync(user, CancellationToken.None);
            result.Should().BeTrue();

        }
    }

}