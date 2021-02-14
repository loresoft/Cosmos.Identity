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
    public class RoleClaimStoreTests : TestServiceBase
    {
        public RoleClaimStoreTests(ITestOutputHelper outputHelper) : base(outputHelper)
        {
        }

        [Fact]
        public async Task GetClaimsAsyncTest()
        {
            var store = Services.GetRequiredService<IRoleClaimStore<IdentityRole>>();

            var role = CreateRole();
            role.Claims.Add(new IdentityClaim { Type = "claim-type", Value = "claim-value" });

            var claims = await store.GetClaimsAsync(role, CancellationToken.None);
            claims.Should().NotBeNull();
            claims.Should().NotBeEmpty();
            claims[0].Type.Should().Be(role.Claims[0].Type);
            claims[0].Value.Should().Be(role.Claims[0].Value);
        }

        [Fact]
        public async Task AddClaimAsyncTest()
        {
            var store = Services.GetRequiredService<IRoleClaimStore<IdentityRole>>();

            var role = CreateRole();

            var claim = new Claim("claim-type", "claim-value");

            await store.AddClaimAsync(role, claim, CancellationToken.None);
            role.Claims.Should().NotBeEmpty();
            role.Claims[0].Type.Should().Be(claim.Type);
            role.Claims[0].Value.Should().Be(claim.Value);
        }

        [Fact]
        public async Task RemoveClaimAsyncTest()
        {
            var store = Services.GetRequiredService<IRoleClaimStore<IdentityRole>>();

            var role = CreateRole();
            role.Claims.Add(new IdentityClaim { Type = "claim-type", Value = "claim-value" });
            role.Claims.Add(new IdentityClaim { Type = "other-type", Value = "other-value" });

            var claim = new Claim("claim-type", "claim-value");

            await store.RemoveClaimAsync(role, claim, CancellationToken.None);

            role.Claims.Count.Should().Be(1);
            role.Claims[0].Type.Should().Be("other-type");
            role.Claims[0].Value.Should().Be("other-value");
        }
    }
}