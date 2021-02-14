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
    public class UserClaimStoreTests : TestServiceBase
    {
        public UserClaimStoreTests(ITestOutputHelper outputHelper) : base(outputHelper)
        {
        }

        [Fact]
        public async Task GetClaimsAsyncTest()
        {
            var store = Services.GetRequiredService<IUserClaimStore<IdentityUser>>();

            var user = CreateUser();
            user.Claims.Add(new IdentityClaim { Type = "claim-type", Value = "claim-value" });

            var claims = await store.GetClaimsAsync(user, CancellationToken.None);
            claims.Should().NotBeNull();
            claims.Should().NotBeEmpty();
            claims[0].Type.Should().Be(user.Claims[0].Type);
            claims[0].Value.Should().Be(user.Claims[0].Value);
        }

        [Fact]
        public async Task AddClaimsAsyncTest()
        {
            var store = Services.GetRequiredService<IUserClaimStore<IdentityUser>>();
            var user = CreateUser();

            var claims = new List<Claim>
            {
                new Claim("claim-type", "claim-value")
            };

            await store.AddClaimsAsync(user, claims, CancellationToken.None);
            user.Claims.Should().NotBeEmpty();
            user.Claims[0].Type.Should().Be(claims[0].Type);
            user.Claims[0].Value.Should().Be(claims[0].Value);
        }

        [Fact]
        public async Task ReplaceClaimAsyncTest()
        {
            var store = Services.GetRequiredService<IUserClaimStore<IdentityUser>>();
            var user = CreateUser();
            user.Claims.Add(new IdentityClaim { Type = "claim-type", Value = "claim-value" });
            user.Claims.Add(new IdentityClaim { Type = "other-type", Value = "other-value" });


            var current = new Claim("claim-type", "claim-value");
            var newClaim = new Claim("claim-type", "new-value");

            await store.ReplaceClaimAsync(user, current, newClaim, CancellationToken.None);
            user.Claims.Should().NotBeEmpty();

            user.Claims[0].Type.Should().Be(newClaim.Type);
            user.Claims[0].Value.Should().Be(newClaim.Value);

        }

        [Fact]
        public async Task RemoveClaimsAsyncTest()
        {
            var store = Services.GetRequiredService<IUserClaimStore<IdentityUser>>();
            var user = CreateUser();
            user.Claims.Add(new IdentityClaim { Type = "claim-type", Value = "claim-value" });
            user.Claims.Add(new IdentityClaim { Type = "other-type", Value = "other-value" });

            var claims = new List<Claim>
            {
                new Claim("claim-type", "claim-value")
            };

            await store.RemoveClaimsAsync(user, claims, CancellationToken.None);

            user.Claims.Count.Should().Be(1);
            user.Claims[0].Type.Should().Be("other-type");
            user.Claims[0].Value.Should().Be("other-value");

        }

        [Fact]
        public async Task GetUsersForClaimAsyncTest()
        {
            var store = Services.GetRequiredService<IUserClaimStore<IdentityUser>>();
            var user = CreateUser();

            var claim = new Claim($"claim-type-{DateTime.Now.Ticks}", $"claim-value-{DateTime.Now.Ticks}");

            user.Claims.Add(new IdentityClaim
            {
                Type = claim.Type,
                Value = claim.Value
            });

            var createdUser = await store.CreateAsync(user, CancellationToken.None);
            createdUser.Should().NotBeNull();

            var foundUsers = await store.GetUsersForClaimAsync(claim, CancellationToken.None);
            foundUsers.Should().NotBeNull();
            foundUsers.Should().NotBeEmpty();
        }


        [Fact]
        public async Task GetUsersForClaimAsyncFailTest()
        {
            var store = Services.GetRequiredService<IUserClaimStore<IdentityUser>>();
            var user = CreateUser();

            var claim = new Claim($"claim-type-{DateTime.Now.Ticks}", $"claim-value-{DateTime.Now.Ticks}");

            user.Claims.Add(new IdentityClaim
            {
                Type = claim.Type + "-fail",
                Value = claim.Value
            });

            var createdUser = await store.CreateAsync(user, CancellationToken.None);
            createdUser.Should().NotBeNull();

            var foundUsers = await store.GetUsersForClaimAsync(claim, CancellationToken.None);
            foundUsers.Should().NotBeNull();
            foundUsers.Should().BeEmpty();
        }
    }

}