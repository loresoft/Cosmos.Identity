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
    public class UserPasswordStoreTests : TestServiceBase
    {
        public UserPasswordStoreTests(ITestOutputHelper outputHelper) : base(outputHelper)
        {
        }

        [Fact]
        public async Task SetPasswordHashAsyncTest()
        {
            var store = Services.GetRequiredService<IUserPasswordStore<IdentityUser>>();

            var user = CreateUser();
            user.PasswordHash = null;

            var password = "password-" + DateTime.Now.Ticks;

            await store.SetPasswordHashAsync(user, password, CancellationToken.None);

            user.PasswordHash.Should().Be(password);

        }

        [Fact]
        public async Task GetPasswordHashAsyncTest()
        {
            var store = Services.GetRequiredService<IUserPasswordStore<IdentityUser>>();

            var user = CreateUser();
            user.PasswordHash = "password-" + DateTime.Now.Ticks; ;

            var password = await store.GetPasswordHashAsync(user, CancellationToken.None);
            password.Should().NotBeNullOrEmpty();
            password.Should().Be(password);
        }

        [Fact]
        public async Task HasPasswordAsyncTest()
        {
            var store = Services.GetRequiredService<IUserPasswordStore<IdentityUser>>();

            var user = CreateUser();
            user.PasswordHash = null;

            var noPassword = await store.HasPasswordAsync(user, CancellationToken.None);
            noPassword.Should().BeFalse();

            user.PasswordHash = "password-" + DateTime.Now.Ticks; ;

            var hasPassword = await store.HasPasswordAsync(user, CancellationToken.None);
            hasPassword.Should().BeTrue();

        }


    }

}