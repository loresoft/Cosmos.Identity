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
    public class UserLockoutStoreTests : TestServiceBase
    {
        public UserLockoutStoreTests(ITestOutputHelper outputHelper) : base(outputHelper)
        {
        }

        [Fact]
        public async Task GetLockoutEndDateAsyncTest()
        {
            var store = Services.GetRequiredService<IUserLockoutStore<IdentityUser>>();

            var user = CreateUser();
            user.LockoutEnd = DateTimeOffset.Now;

            var lockoutEnd = await store.GetLockoutEndDateAsync(user, CancellationToken.None);
            lockoutEnd.Should().NotBeNull();
            lockoutEnd.Should().Be(user.LockoutEnd);
        }

        [Fact]
        public async Task GetLockoutEndDateAsyncNullTest()
        {
            var store = Services.GetRequiredService<IUserLockoutStore<IdentityUser>>();

            var user = CreateUser();
            user.LockoutEnd = null;

            var lockoutEnd = await store.GetLockoutEndDateAsync(user, CancellationToken.None);
            lockoutEnd.Should().BeNull();
        }


        [Fact]
        public async Task SetLockoutEndDateAsyncTest()
        {
            var store = Services.GetRequiredService<IUserLockoutStore<IdentityUser>>();

            var user = CreateUser();
            user.LockoutEnd = null;

            var lockoutEnd = DateTimeOffset.Now;

            await store.SetLockoutEndDateAsync(user, lockoutEnd, CancellationToken.None);
            user.LockoutEnd.Should().Be(lockoutEnd);

            await store.SetLockoutEndDateAsync(user, null, CancellationToken.None);
            user.LockoutEnd.Should().BeNull();
        }

        [Fact]
        public async Task IncrementAccessFailedCountAsyncTest()
        {
            var store = Services.GetRequiredService<IUserLockoutStore<IdentityUser>>();

            var user = CreateUser();
            user.AccessFailedCount = 0;

            await store.IncrementAccessFailedCountAsync(user, CancellationToken.None);
            user.AccessFailedCount.Should().Be(1);
        }

        [Fact]
        public async Task ResetAccessFailedCountAsyncTest()
        {
            var store = Services.GetRequiredService<IUserLockoutStore<IdentityUser>>();

            var user = CreateUser();
            user.AccessFailedCount = 1;

            await store.ResetAccessFailedCountAsync(user, CancellationToken.None);
            user.AccessFailedCount.Should().Be(0);
        }

        [Fact]
        public async Task GetAccessFailedCountAsyncTest()
        {
            var store = Services.GetRequiredService<IUserLockoutStore<IdentityUser>>();

            var user = CreateUser();
            user.AccessFailedCount = 0;

            var failedCount = await store.GetAccessFailedCountAsync(user, CancellationToken.None);
            failedCount.Should().Be(0);

            user.AccessFailedCount = 1;

            failedCount = await store.GetAccessFailedCountAsync(user, CancellationToken.None);
            failedCount.Should().Be(1);
        }

        [Fact]
        public async Task GetLockoutEnabledAsyncTest()
        {
            var store = Services.GetRequiredService<IUserLockoutStore<IdentityUser>>();

            var user = CreateUser();
            user.LockoutEnabled = true;

            var lockoutEnabled = await store.GetLockoutEnabledAsync(user, CancellationToken.None);
            lockoutEnabled.Should().BeTrue();

            user.LockoutEnabled = false;

            var lockoutDisabled = await store.GetLockoutEnabledAsync(user, CancellationToken.None);
            lockoutDisabled.Should().BeFalse();
        }

        [Fact]
        public async Task SetLockoutEnabledAsyncTest()
        {
            var store = Services.GetRequiredService<IUserLockoutStore<IdentityUser>>();

            var user = CreateUser();
            user.LockoutEnabled = false;

            await store.SetLockoutEnabledAsync(user, true, CancellationToken.None);
            user.LockoutEnabled.Should().BeTrue();

            await store.SetLockoutEnabledAsync(user, false, CancellationToken.None);
            user.LockoutEnabled.Should().BeFalse();
        }
    }

}