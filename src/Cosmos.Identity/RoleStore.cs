using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Cosmos.Abstracts;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;


namespace Cosmos.Identity
{
    public class RoleStore<TRole> : CosmosRepository<TRole>,
        IQueryableRoleStore<TRole>,
        IRoleClaimStore<TRole>
        where TRole : IdentityRole
    {
        public RoleStore(
            ILoggerFactory logFactory,
            IOptions<CosmosRepositoryOptions> repositoryOptions,
            ICosmosFactory databaseFactory
        ) : base(logFactory, repositoryOptions, databaseFactory)
        {

        }

        #region IRoleStore
        public async Task<TRole> FindByIdAsync(string roleId, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            return await FindAsync(roleId, cancellationToken: cancellationToken);
        }

        public async Task<TRole> FindByNameAsync(string normalizedRoleName, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            return await FindOneAsync(u => u.NormalizedName == normalizedRoleName, cancellationToken: cancellationToken);
        }

        public Task<string> GetNormalizedRoleNameAsync(TRole role, CancellationToken cancellationToken)
        {
            ValidateParameters(role, cancellationToken);

            return Task.FromResult(role.NormalizedName);
        }

        public Task<string> GetRoleIdAsync(TRole role, CancellationToken cancellationToken)
        {
            ValidateParameters(role, cancellationToken);

            return Task.FromResult(role.Id);
        }

        public Task<string> GetRoleNameAsync(TRole role, CancellationToken cancellationToken)
        {
            ValidateParameters(role, cancellationToken);

            return Task.FromResult(role.Name);
        }

        public Task SetNormalizedRoleNameAsync(TRole role, string normalizedName, CancellationToken cancellationToken)
        {
            if (normalizedName == null)
                throw new ArgumentNullException(nameof(normalizedName));

            ValidateParameters(role, cancellationToken);

            role.NormalizedName = normalizedName;
            return Task.CompletedTask;
        }

        public Task SetRoleNameAsync(TRole role, string roleName, CancellationToken cancellationToken)
        {
            if (roleName == null)
                throw new ArgumentNullException(nameof(roleName));

            ValidateParameters(role, cancellationToken);

            role.Name = roleName;
            return Task.CompletedTask;
        }

        async Task<IdentityResult> IRoleStore<TRole>.CreateAsync(TRole role, CancellationToken cancellationToken)
        {
            ValidateParameters(role, cancellationToken);

            await base.CreateAsync(role, cancellationToken);

            return IdentityResult.Success;
        }

        async Task<IdentityResult> IRoleStore<TRole>.DeleteAsync(TRole role, CancellationToken cancellationToken)
        {
            ValidateParameters(role, cancellationToken);

            await base.DeleteAsync(role, cancellationToken);

            return IdentityResult.Success;
        }

        async Task<IdentityResult> IRoleStore<TRole>.UpdateAsync(TRole role, CancellationToken cancellationToken)
        {
            ValidateParameters(role, cancellationToken);

            await base.UpdateAsync(role, cancellationToken);

            return IdentityResult.Success;
        }
        #endregion

        #region IQueryableRoleStore
        public IQueryable<TRole> Roles => GetContainerAsync().Result.GetItemLinqQueryable<TRole>();
        #endregion

        #region IRoleClaimStore
        public Task<IList<Claim>> GetClaimsAsync(TRole role, CancellationToken cancellationToken = default)
        {
            ValidateParameters(role, cancellationToken);

            var claims = role.Claims
                .Select(c => new Claim(c.Type, c.Value))
                .ToList();

            return Task.FromResult<IList<Claim>>(claims);
        }

        public Task AddClaimAsync(TRole role, Claim claim, CancellationToken cancellationToken = default)
        {
            ValidateParameters(role, cancellationToken);

            if (claim == null)
                throw new ArgumentNullException(nameof(claim));

            var roleClaim = new IdentityClaim
            {
                Type = claim.Type,
                Value = claim.Value
            };
            role.Claims.Add(roleClaim);

            return Task.CompletedTask;
        }

        public Task RemoveClaimAsync(TRole role, Claim claim, CancellationToken cancellationToken = default)
        {
            ValidateParameters(role, cancellationToken);

            if (claim == null)
                throw new ArgumentNullException(nameof(claim));

            var matched = role.Claims
                .Where(u => u.Value == claim.Value && u.Type == claim.Type)
                .ToList();

            foreach (var m in matched)
                role.Claims.Remove(m);

            return Task.CompletedTask;
        }
        #endregion

        #region IDisposable
        public void Dispose() { }
        #endregion

        protected void ValidateParameters(IdentityRole role, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (role == null)
                throw new ArgumentNullException(nameof(role));
        }

    }
}
