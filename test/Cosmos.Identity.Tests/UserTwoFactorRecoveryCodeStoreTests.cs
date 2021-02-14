using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using Xunit.Abstractions;

namespace Cosmos.Identity.Tests
{
    public class UserTwoFactorRecoveryCodeStoreTests : TestServiceBase
    {
        public UserTwoFactorRecoveryCodeStoreTests(ITestOutputHelper outputHelper) : base(outputHelper)
        {
        }

        [Fact]
        public async Task ReplaceCodesAsyncTest()
        {
            var store = Services.GetRequiredService<IUserTwoFactorRecoveryCodeStore<IdentityUser>>();

            var user = CreateUser();

            var codes = new[] { "code-1", "code-2" };

            await store.ReplaceCodesAsync(user, codes, CancellationToken.None);

            user.Tokens.Should().NotBeNull();
            user.Tokens.Should().NotBeEmpty();
            user.Tokens.Count.Should().Be(1);

            user.Tokens[0].LoginProvider.Should().Be(UserStore<IdentityUser>.InternalLoginProvider);
            user.Tokens[0].Name.Should().Be(UserStore<IdentityUser>.RecoveryCodeTokenName);
            user.Tokens[0].Value.Should().Be("code-1;code-2");
        }

        [Fact]
        public async Task RedeemCodeAsyncTest()
        {
            var store = Services.GetRequiredService<IUserTwoFactorRecoveryCodeStore<IdentityUser>>();

            var user = CreateUser();

            user.Tokens.Add(new IdentityToken
            {
                LoginProvider = UserStore<IdentityUser>.InternalLoginProvider,
                Name = UserStore<IdentityUser>.RecoveryCodeTokenName,
                Value = "code-1;code-2"
            });

            var found = await store.RedeemCodeAsync(user, "code-1", CancellationToken.None);
            found.Should().BeTrue();

            user.Tokens.Should().NotBeNull();
            user.Tokens.Should().NotBeEmpty();
            user.Tokens.Count.Should().Be(1);

            user.Tokens[0].Value.Should().Be("code-2");

            var notFound = await store.RedeemCodeAsync(user, "code-8", CancellationToken.None);
            notFound.Should().BeFalse();

            user.Tokens.Should().NotBeNull();
            user.Tokens.Should().NotBeEmpty();
            user.Tokens.Count.Should().Be(1);
        }

        [Fact]
        public async Task CountCodesAsyncTest()
        {
            var store = Services.GetRequiredService<IUserTwoFactorRecoveryCodeStore<IdentityUser>>();

            var user = CreateUser();

            var count = await store.CountCodesAsync(user, CancellationToken.None);
            count.Should().Be(0);

            user.Tokens.Add(new IdentityToken
            {
                LoginProvider = UserStore<IdentityUser>.InternalLoginProvider,
                Name = UserStore<IdentityUser>.RecoveryCodeTokenName,
                Value = "code-1;code-2"
            });

            count = await store.CountCodesAsync(user, CancellationToken.None);
            count.Should().Be(2);
        }
    }

}