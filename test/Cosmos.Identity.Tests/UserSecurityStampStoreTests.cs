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
    public class UserSecurityStampStoreTests : TestServiceBase
    {
        public UserSecurityStampStoreTests(ITestOutputHelper outputHelper) : base(outputHelper)
        {
        }

        [Fact]
        public async Task SetSecurityStampAsyncTest()
        {
            var store = Services.GetRequiredService<IUserSecurityStampStore<IdentityUser>>();
            var user = CreateUser();

            var stamp = $"stamp-{DateTime.Now.Ticks}";

            await store.SetSecurityStampAsync(user, stamp, CancellationToken.None);

            user.SecurityStamp.Should().Be(stamp);
        }

        [Fact]
        public async Task GetSecurityStampAsyncTest()
        {
            var store = Services.GetRequiredService<IUserSecurityStampStore<IdentityUser>>();
            var user = CreateUser();
            user.SecurityStamp = $"stamp-{DateTime.Now.Ticks}";

            var stamp = await store.GetSecurityStampAsync(user, CancellationToken.None);
            stamp.Should().NotBeNullOrEmpty();
            stamp.Should().Be(user.SecurityStamp);
        }
    }

}