using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using Xunit.Abstractions;

namespace Cosmos.Identity.Tests
{
    public class UserLoginStoreTests : TestServiceBase
    {
        public UserLoginStoreTests(ITestOutputHelper outputHelper) : base(outputHelper)
        {
        }

        [Fact]
        public async Task AddLoginAsyncTest()
        {
            var store = Services.GetRequiredService<IUserLoginStore<IdentityUser>>();

            var user = CreateUser();

            var login = new UserLoginInfo("login-provider", "provider-key", "provider-name");

            await store.AddLoginAsync(user, login, CancellationToken.None);

            user.Logins.Should().NotBeEmpty();
            user.Logins[0].LoginProvider.Should().Be(login.LoginProvider);
            user.Logins[0].ProviderKey.Should().Be(login.ProviderKey);
            user.Logins[0].ProviderDisplayName.Should().Be(login.ProviderDisplayName);

        }

        [Fact]
        public async Task RemoveLoginAsyncTest()
        {
            var store = Services.GetRequiredService<IUserLoginStore<IdentityUser>>();

            var user = CreateUser();
            user.Logins.Add(new IdentityLogin
            {
                LoginProvider = "login-provider",
                ProviderKey = "provider-key",
                ProviderDisplayName = "provider-name"
            });
            user.Logins.Add(new IdentityLogin
            {
                LoginProvider = "other-provider",
                ProviderKey = "other-key",
                ProviderDisplayName = "other-name"
            });

            await store.RemoveLoginAsync(user, "login-provider", "provider-key", CancellationToken.None);

            user.Logins.Count.Should().Be(1);
            user.Logins[0].LoginProvider.Should().Be("other-provider");
            user.Logins[0].ProviderKey.Should().Be("other-key");
        }

        [Fact]
        public async Task GetLoginsAsyncTest()
        {
            var store = Services.GetRequiredService<IUserLoginStore<IdentityUser>>();

            var user = CreateUser();
            user.Logins.Add(new IdentityLogin
            {
                LoginProvider = "login-provider",
                ProviderKey = "provider-key",
                ProviderDisplayName = "provider-name"
            });
            user.Logins.Add(new IdentityLogin
            {
                LoginProvider = "other-provider",
                ProviderKey = "other-key",
                ProviderDisplayName = "other-name"
            });

            var logins = await store.GetLoginsAsync(user, CancellationToken.None);
            logins.Should().NotBeNull();
            logins.Should().NotBeEmpty();
            logins.Count.Should().Be(2);
            logins[0].LoginProvider.Should().Be("login-provider");
        }

        [Fact]
        public async Task FindByLoginAsyncTest()
        {
            var store = Services.GetRequiredService<IUserLoginStore<IdentityUser>>();

            var loginProvider = $"login-provider-{DateTime.Now.Ticks}";
            var providerKey = $"provider-key-{DateTime.Now.Ticks}";

            var user = CreateUser();
            user.Logins.Add(new IdentityLogin
            {
                LoginProvider = loginProvider,
                ProviderKey = providerKey,
                ProviderDisplayName = "provider-name"
            });

            await store.CreateAsync(user, CancellationToken.None);

            var foundUser = await store.FindByLoginAsync(loginProvider, providerKey, CancellationToken.None);

            foundUser.Should().NotBeNull();
            foundUser.UserName.Should().Be(user.UserName);
        }
    }

}