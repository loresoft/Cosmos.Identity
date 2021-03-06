﻿using System;
using System.Threading;
using System.Threading.Tasks;
using Cosmos.Abstracts;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using Xunit.Abstractions;

namespace Cosmos.Identity.Tests
{
    public class RoleStoreTests : TestServiceBase
    {
        public RoleStoreTests(ITestOutputHelper outputHelper) : base(outputHelper)
        {
        }

        [Fact]
        public async Task FindByIdAsyncTest()
        {
            var store = Services.GetRequiredService<IRoleStore<IdentityRole>>();

            var role = CreateRole();

            var createResult = await store.CreateAsync(role, CancellationToken.None);
            createResult.Should().Be(IdentityResult.Success);

            var foundRole = await store.FindByIdAsync(role.Id, CancellationToken.None);
            foundRole.Should().NotBeNull();
            foundRole.Name.Should().Be(role.Name);
        }

        [Fact]
        public async Task FindByIdAsyncFailTest()
        {
            var store = Services.GetRequiredService<IRoleStore<IdentityRole>>();

            var roleId = ObjectId.GenerateNewId().ToString();

            var foundRole = await store.FindByIdAsync(roleId, CancellationToken.None);
            foundRole.Should().BeNull();
        }


        [Fact]
        public async Task FindByNameAsyncTest()
        {
            var store = Services.GetRequiredService<IRoleStore<IdentityRole>>();

            var role = CreateRole();

            var createResult = await store.CreateAsync(role, CancellationToken.None);
            createResult.Should().Be(IdentityResult.Success);

            var foundRole = await store.FindByNameAsync(role.NormalizedName, CancellationToken.None);
            foundRole.Should().NotBeNull();
            foundRole.Name.Should().Be(role.Name);
        }

        [Fact]
        public async Task FindByNameAsyncFailTest()
        {
            var store = Services.GetRequiredService<IRoleStore<IdentityRole>>();

            var roleName = "RoleFail" + DateTime.Now.Ticks;

            var foundRole = await store.FindByNameAsync(roleName, CancellationToken.None);
            foundRole.Should().BeNull();
        }

        [Fact]
        public async Task GetNormalizedRoleNameAsyncTest()
        {
            var store = Services.GetRequiredService<IRoleStore<IdentityRole>>();

            var role = CreateRole();

            var roleName = await store.GetNormalizedRoleNameAsync(role, CancellationToken.None);
            roleName.Should().Be(role.NormalizedName);
        }

        [Fact]
        public async Task GetRoleIdAsyncTest()
        {
            var store = Services.GetRequiredService<IRoleStore<IdentityRole>>();

            var role = CreateRole();

            var roleName = await store.GetRoleIdAsync(role, CancellationToken.None);
            roleName.Should().Be(role.Id);
        }

        [Fact]
        public async Task GetRoleNameAsyncTest()
        {
            var store = Services.GetRequiredService<IRoleStore<IdentityRole>>();

            var role = CreateRole();

            var roleName = await store.GetRoleNameAsync(role, CancellationToken.None);
            roleName.Should().Be(role.Name);
        }

        [Fact]
        public async Task SetNormalizedRoleNameAsyncTest()
        {
            var store = Services.GetRequiredService<IRoleStore<IdentityRole>>();

            var role = CreateRole();

            string name = $"RoleNew{DateTime.Now.Ticks}".ToUpperInvariant();

            await store.SetNormalizedRoleNameAsync(role, name, CancellationToken.None);

            role.NormalizedName.Should().Be(name);
        }

        [Fact]
        public async Task SetRoleNameAsyncTest()
        {
            var store = Services.GetRequiredService<IRoleStore<IdentityRole>>();

            var role = CreateRole();

            string name = $"RoleNew{DateTime.Now.Ticks}";

            await store.SetRoleNameAsync(role, name, CancellationToken.None);

            role.Name.Should().Be(name);
        }

        [Fact]
        public async Task CreateAsyncTest()
        {
            var store = Services.GetRequiredService<IRoleStore<IdentityRole>>();

            var role = CreateRole();

            var createResult = await store.CreateAsync(role, CancellationToken.None);
            createResult.Should().Be(IdentityResult.Success);
        }

        [Fact]
        public async Task CreateAsyncNullTest()
        {
            var store = Services.GetRequiredService<IRoleStore<IdentityRole>>();

            IdentityRole role = null;

            Func<Task> testFunction = async () => await store.CreateAsync(role, CancellationToken.None);
            await testFunction.Should().ThrowAsync<ArgumentNullException>();
        }

        [Fact]
        public async Task UpdateAsyncTest()
        {
            var store = Services.GetRequiredService<IRoleStore<IdentityRole>>();

            var role = CreateRole();

            var createResult = await store.CreateAsync(role, CancellationToken.None);
            createResult.Should().Be(IdentityResult.Success);

            var foundRole = await store.FindByIdAsync(role.Id, CancellationToken.None);
            foundRole.Should().NotBeNull();
            foundRole.Name.Should().Be(role.Name);

            foundRole.Name = "role-update";
            var updateResult = await store.UpdateAsync(role, CancellationToken.None);
            updateResult.Should().Be(IdentityResult.Success);
        }

        [Fact]
        public async Task UpdateAsyncNullTest()
        {
            var store = Services.GetRequiredService<IRoleStore<IdentityRole>>();

            IdentityRole role = null;

            Func<Task> testFunction = async () => await store.UpdateAsync(role, CancellationToken.None);
            await testFunction.Should().ThrowAsync<ArgumentNullException>();
        }

        [Fact]
        public async Task DeleteAsyncTest()
        {
            var store = Services.GetRequiredService<IRoleStore<IdentityRole>>();

            var role = CreateRole();

            var createResult = await store.CreateAsync(role, CancellationToken.None);
            createResult.Should().Be(IdentityResult.Success);

            var foundRole = await store.FindByIdAsync(role.Id, CancellationToken.None);
            foundRole.Should().NotBeNull();
            foundRole.Name.Should().Be(role.Name);

            var deleteResult = await store.DeleteAsync(foundRole, CancellationToken.None);
            deleteResult.Should().Be(IdentityResult.Success);

        }

        [Fact]
        public async Task DeleteAsyncNullTest()
        {
            var store = Services.GetRequiredService<IRoleStore<IdentityRole>>();

            IdentityRole role = null;

            Func<Task> testFunction = async () => await store.DeleteAsync(role, CancellationToken.None);
            await testFunction.Should().ThrowAsync<ArgumentNullException>();
        }


    }
}