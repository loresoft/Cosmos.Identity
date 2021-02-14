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
    public class UserEmailStoreTests : TestServiceBase
    {
        public UserEmailStoreTests(ITestOutputHelper outputHelper) : base(outputHelper)
        {
        }

        [Fact]
        public async Task SetEmailAsyncTest()
        {
            var store = Services.GetRequiredService<IUserEmailStore<IdentityUser>>();
            var user = CreateUser();
            var email = $"user{DateTime.Now.Ticks}@email.com";

            await store.SetEmailAsync(user, email, CancellationToken.None);

            user.Email.Should().Be(email);
        }

        [Fact]
        public async Task GetEmailAsyncTest()
        {
            var store = Services.GetRequiredService<IUserEmailStore<IdentityUser>>();
            var user = CreateUser();

            var email = await store.GetEmailAsync(user, CancellationToken.None);
            email.Should().NotBeNullOrEmpty();
            email.Should().Be(user.Email);
        }

        [Fact]
        public async Task GetEmailConfirmedAsyncTest()
        {
            var store = Services.GetRequiredService<IUserEmailStore<IdentityUser>>();
            var user = CreateUser();

            var emailConfirmed = await store.GetEmailConfirmedAsync(user, CancellationToken.None);
            emailConfirmed.Should().BeTrue();
        }

        [Fact]
        public async Task SetEmailConfirmedAsyncTest()
        {
            var store = Services.GetRequiredService<IUserEmailStore<IdentityUser>>();
            var user = CreateUser();
            user.EmailConfirmed.Should().BeTrue();

            await store.SetEmailConfirmedAsync(user, false, CancellationToken.None);
            user.EmailConfirmed.Should().BeFalse();
        }

        [Fact]
        public async Task FindByEmailAsyncTest()
        {
            var store = Services.GetRequiredService<IUserEmailStore<IdentityUser>>();
            var user = CreateUser();

            var createResult = await store.CreateAsync(user, CancellationToken.None);
            createResult.Should().Be(IdentityResult.Success);

            var foundUser = await store.FindByEmailAsync(user.NormalizedEmail, CancellationToken.None);
            foundUser.Should().NotBeNull();
            foundUser.UserName.Should().Be(user.UserName);

        }

        [Fact]
        public async Task GetNormalizedEmailAsyncTest()
        {
            var store = Services.GetRequiredService<IUserEmailStore<IdentityUser>>();
            var user = CreateUser();

            var email = await store.GetNormalizedEmailAsync(user, CancellationToken.None);
            email.Should().NotBeNullOrEmpty();
            email.Should().Be(user.NormalizedEmail);

        }

        [Fact]
        public async Task SetNormalizedEmailAsyncTest()
        {
            var store = Services.GetRequiredService<IUserEmailStore<IdentityUser>>();
            var user = CreateUser();
            var email = $"user{DateTime.Now.Ticks}@email.com".ToUpperInvariant();

            await store.SetNormalizedEmailAsync(user, email, CancellationToken.None);

            user.NormalizedEmail.Should().Be(email);

        }
    }

}