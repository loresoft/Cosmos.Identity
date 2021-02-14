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
    public class UserAuthenticationTokenStoreTests : TestServiceBase
    {
        public UserAuthenticationTokenStoreTests(ITestOutputHelper outputHelper) : base(outputHelper)
        {
        }

        [Fact]
        public async Task SetTokenAsyncTest()
        {
            var store = Services.GetRequiredService<IUserAuthenticationTokenStore<IdentityUser>>();

            string loginProvider = "provider";
            string name = "provider-name";
            string value = "provider-value";

            string userName = "UserName" + DateTime.Now.Ticks;
            var user = new IdentityUser
            {
                Id = Guid.NewGuid().ToString(),
                UserName = userName,
                NormalizedUserName = userName.ToUpperInvariant()
            };

            await store.SetTokenAsync(user, loginProvider, name, value, CancellationToken.None);

            user.Tokens.Should().NotBeNull();
            user.Tokens.Should().NotBeEmpty();

            user.Tokens[0].LoginProvider.Should().Be(loginProvider);
            user.Tokens[0].Name.Should().Be(name);
            user.Tokens[0].Value.Should().Be(value);
        }

        [Fact]
        public async Task RemoveTokenAsyncTest()
        {
            var store = Services.GetRequiredService<IUserAuthenticationTokenStore<IdentityUser>>();

            string loginProvider = "provider";
            string name = "provider-name";
            string value = "provider-value";

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

            await store.RemoveTokenAsync(user, loginProvider, name, CancellationToken.None);

            user.Tokens.Should().NotBeNull();
            user.Tokens.Should().BeEmpty();
        }

        [Fact]
        public async Task RemoveTokenAsyncFailTest()
        {
            var store = Services.GetRequiredService<IUserAuthenticationTokenStore<IdentityUser>>();

            string loginProvider = "provider";
            string name = "provider-name";
            string value = "provider-value";

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

            await store.RemoveTokenAsync(user, loginProvider, name + "fail", CancellationToken.None);

            user.Tokens.Should().NotBeNull();
            user.Tokens.Should().NotBeEmpty();
        }

        [Fact]
        public async Task GetTokenAsyncTest()
        {
            var store = Services.GetRequiredService<IUserAuthenticationTokenStore<IdentityUser>>();

            string loginProvider = "provider";
            string name = "provider-name";
            string value = "provider-value";

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

            var token = await store.GetTokenAsync(user, loginProvider, name, CancellationToken.None);

            token.Should().NotBeNull();
            token.Should().Be(value);
        }

        [Fact]
        public async Task GetTokenAsyncFailTest()
        {
            var store = Services.GetRequiredService<IUserAuthenticationTokenStore<IdentityUser>>();

            string loginProvider = "provider";
            string name = "provider-name";
            string value = "provider-value";

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

            var token = await store.GetTokenAsync(user, loginProvider, name + "fail", CancellationToken.None);

            token.Should().BeNull();
        }
    }

}