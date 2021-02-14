using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using Xunit.Abstractions;

namespace Cosmos.Identity.Tests
{
    public class UserPhoneNumberStoreTests : TestServiceBase
    {
        public UserPhoneNumberStoreTests(ITestOutputHelper outputHelper) : base(outputHelper)
        {
        }

        [Fact]
        public async Task SetPhoneNumberAsyncTest()
        {
            var store = Services.GetRequiredService<IUserPhoneNumberStore<IdentityUser>>();

            var user = CreateUser();

            var phone = "999-888-1234";

            await store.SetPhoneNumberAsync(user, phone, CancellationToken.None);
            user.PhoneNumber.Should().Be(phone);

            await store.SetPhoneNumberAsync(user, null, CancellationToken.None);
            user.PhoneNumber.Should().BeNull();
        }

        [Fact]
        public async Task GetPhoneNumberAsyncTest()
        {
            var store = Services.GetRequiredService<IUserPhoneNumberStore<IdentityUser>>();

            var user = CreateUser();
            user.PhoneNumber = "800-555-1212";

            var phone = await store.GetPhoneNumberAsync(user, CancellationToken.None);
            phone.Should().NotBeNullOrEmpty();
            phone.Should().Be(user.PhoneNumber);

            user.PhoneNumber = null;

            var phoneNull = await store.GetPhoneNumberAsync(user, CancellationToken.None);
            phoneNull.Should().BeNull();
        }

        [Fact]
        public async Task GetPhoneNumberConfirmedAsyncTest()
        {
            var store = Services.GetRequiredService<IUserPhoneNumberStore<IdentityUser>>();

            var user = CreateUser();
            user.PhoneNumberConfirmed = true;

            var confirmed = await store.GetPhoneNumberConfirmedAsync(user, CancellationToken.None);
            confirmed.Should().BeTrue();

            user.PhoneNumberConfirmed = false;

            var notConfirmed = await store.GetPhoneNumberConfirmedAsync(user, CancellationToken.None);
            notConfirmed.Should().BeFalse();
        }

        [Fact]
        public async Task SetPhoneNumberConfirmedAsyncTest()
        {
            var store = Services.GetRequiredService<IUserPhoneNumberStore<IdentityUser>>();

            var user = CreateUser();
            user.PhoneNumberConfirmed = false;

            await store.SetPhoneNumberConfirmedAsync(user, true, CancellationToken.None);
            user.PhoneNumberConfirmed.Should().BeTrue();

            await store.SetPhoneNumberConfirmedAsync(user, false, CancellationToken.None);
            user.PhoneNumberConfirmed.Should().BeFalse();
        }
    }

}