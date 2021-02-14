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

            var user = CreateUser();

            string key = "authenticator-key";

            await store.SetAuthenticatorKeyAsync(user, key, CancellationToken.None);

            user.Tokens.Should().NotBeNull();
            user.Tokens.Should().NotBeEmpty();

            user.Tokens[0].LoginProvider.Should().Be(UserStore<IdentityUser>.InternalLoginProvider);
            user.Tokens[0].Name.Should().Be(UserStore<IdentityUser>.AuthenticatorKeyTokenName);
            user.Tokens[0].Value.Should().Be(key);
        }

        [Fact]
        public async Task GetAuthenticatorKeyAsyncTest()
        {
            var store = Services.GetRequiredService<IUserAuthenticatorKeyStore<IdentityUser>>();


            var user = CreateUser();

            string value = "provider-key-" + DateTime.Now.Ticks;

            user.Tokens.Add(new IdentityToken
            {
                LoginProvider = UserStore<IdentityUser>.InternalLoginProvider,
                Name = UserStore<IdentityUser>.AuthenticatorKeyTokenName,
                Value = value
            });

            var token = await store.GetAuthenticatorKeyAsync(user, CancellationToken.None);

            token.Should().NotBeNull();
            token.Should().Be(value);

        }
    }

}