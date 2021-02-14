using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using Xunit.Abstractions;

namespace Cosmos.Identity.Tests
{
    public class UserAuthenticatorKeyStoreTests : TestServiceBase
    {
        public UserAuthenticatorKeyStoreTests(ITestOutputHelper outputHelper) : base(outputHelper)
        {
        }

        [Fact]
        public async Task SetAuthenticatorKeyAsyncTest()
        {
            var store = Services.GetRequiredService<IUserAuthenticatorKeyStore<IdentityUser>>();

            string key = "authenticator-key";

            string userName = "UserName" + DateTime.Now.Ticks;
            var user = new IdentityUser
            {
                Id = Guid.NewGuid().ToString(),
                UserName = userName,
                NormalizedUserName = userName.ToUpperInvariant()
            };

            await store.SetAuthenticatorKeyAsync(user, key, CancellationToken.None);

            user.Tokens.Should().NotBeNull();
            user.Tokens.Should().NotBeEmpty();

            user.Tokens[0].LoginProvider.Should().Be("[AspNetUserStore]");
            user.Tokens[0].Name.Should().Be("AuthenticatorKey");
            user.Tokens[0].Value.Should().Be(key);
        }

        [Fact]
        public async Task GetAuthenticatorKeyAsyncTest()
        {
            var store = Services.GetRequiredService<IUserAuthenticatorKeyStore<IdentityUser>>();

            string loginProvider = "[AspNetUserStore]";
            string name = "AuthenticatorKey";
            string value = "provider-key";

            string userName = "UserName" + DateTime.Now.Ticks;
            var user = new IdentityUser
            {
                Id = Guid.NewGuid().ToString(),
                UserName = userName,
                NormalizedUserName = userName.ToUpperInvariant(),
                Tokens = new List<IdentityToken>
                {
                    new IdentityToken
                    {
                        LoginProvider = loginProvider,
                        Name = name,
                        Value = value
                    }
                }
            };

            var token = await store.GetAuthenticatorKeyAsync(user, CancellationToken.None);

            token.Should().NotBeNull();
            token.Should().Be(value);

        }
    }

}