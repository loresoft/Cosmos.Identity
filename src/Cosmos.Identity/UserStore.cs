﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Cosmos.Abstracts;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Cosmos.Identity
{
    public class UserStore<TUser> : CosmosRepository<TUser>,
        IUserLoginStore<TUser>,
        IUserClaimStore<TUser>,
        IUserPasswordStore<TUser>,
        IUserSecurityStampStore<TUser>,
        IUserEmailStore<TUser>,
        IUserLockoutStore<TUser>,
        IUserPhoneNumberStore<TUser>,
        IQueryableUserStore<TUser>,
        IUserTwoFactorStore<TUser>,
        IUserAuthenticationTokenStore<TUser>,
        IUserAuthenticatorKeyStore<TUser>,
        IUserTwoFactorRecoveryCodeStore<TUser>,
        IUserRoleStore<TUser>
        where TUser : IdentityUser
    {
        public const string InternalLoginProvider = "InternalProvider";
        public const string AuthenticatorKeyTokenName = "AuthenticatorKey";
        public const string RecoveryCodeTokenName = "RecoveryCodes";

        public UserStore(
            ILoggerFactory logFactory,
            IOptions<CosmosRepositoryOptions> repositoryOptions,
            ICosmosFactory databaseFactory
        ) : base(logFactory, repositoryOptions, databaseFactory)
        {

        }

        #region IUserLoginStore
        public Task AddLoginAsync(TUser user, UserLoginInfo login, CancellationToken cancellationToken)
        {
            ValidateParameters(user, cancellationToken);

            if (login == null)
                throw new ArgumentNullException(nameof(login));

            var userLogin = new IdentityLogin
            {
                LoginProvider = login.LoginProvider,
                ProviderKey = login.ProviderKey,
                ProviderDisplayName = login.ProviderDisplayName
            };

            user.Logins.Add(userLogin);

            return Task.CompletedTask;
        }

        public Task RemoveLoginAsync(TUser user, string loginProvider, string providerKey, CancellationToken cancellationToken)
        {
            ValidateParameters(user, cancellationToken);

            var matchedLogins = user.Logins
                .Where(u => u.LoginProvider == loginProvider && u.ProviderKey == providerKey)
                .ToList();

            foreach (var matchedLogin in matchedLogins)
                user.Logins.Remove(matchedLogin);

            return Task.CompletedTask;
        }

        public Task<IList<UserLoginInfo>> GetLoginsAsync(TUser user, CancellationToken cancellationToken)
        {
            ValidateParameters(user, cancellationToken);

            IList<UserLoginInfo> result = user.Logins
                .Select(u => new UserLoginInfo(u.LoginProvider, u.ProviderKey, u.ProviderDisplayName))
                .ToList();

            return Task.FromResult(result);
        }

        public async Task<TUser> FindByLoginAsync(string loginProvider, string providerKey, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            return await FindOneAsync(u => u.Logins.Any(l => l.LoginProvider == loginProvider && l.ProviderKey == providerKey), cancellationToken);
        }
        #endregion

        #region IUserStore
        public Task<string> GetUserIdAsync(TUser user, CancellationToken cancellationToken)
        {
            ValidateParameters(user, cancellationToken);

            return Task.FromResult(user.Id);
        }

        public Task<string> GetUserNameAsync(TUser user, CancellationToken cancellationToken)
        {
            ValidateParameters(user, cancellationToken);

            return Task.FromResult(user.UserName);
        }

        public Task SetUserNameAsync(TUser user, string userName, CancellationToken cancellationToken)
        {
            ValidateParameters(user, cancellationToken);

            user.UserName = userName;
            return Task.CompletedTask;
        }

        public Task<string> GetNormalizedUserNameAsync(TUser user, CancellationToken cancellationToken)
        {
            ValidateParameters(user, cancellationToken);

            return Task.FromResult(user.NormalizedUserName);
        }

        public Task SetNormalizedUserNameAsync(TUser user, string normalizedName, CancellationToken cancellationToken)
        {
            if (normalizedName == null)
                throw new ArgumentNullException(nameof(normalizedName));

            ValidateParameters(user, cancellationToken);

            user.NormalizedUserName = normalizedName;
            return Task.CompletedTask;
        }

        async Task<IdentityResult> IUserStore<TUser>.CreateAsync(TUser user, CancellationToken cancellationToken)
        {
            ValidateParameters(user, cancellationToken);

            await base.CreateAsync(user, cancellationToken);

            return IdentityResult.Success;
        }

        async Task<IdentityResult> IUserStore<TUser>.UpdateAsync(TUser user, CancellationToken cancellationToken)
        {
            ValidateParameters(user, cancellationToken);

            await base.UpdateAsync(user, cancellationToken);

            return IdentityResult.Success;
        }

        async Task<IdentityResult> IUserStore<TUser>.DeleteAsync(TUser user, CancellationToken cancellationToken)
        {
            ValidateParameters(user, cancellationToken);

            await base.DeleteAsync(user, cancellationToken);

            return IdentityResult.Success;
        }

        public async Task<TUser> FindByIdAsync(string userId, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            return await FindAsync(userId, cancellationToken: cancellationToken);
        }

        public async Task<TUser> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            return await FindOneAsync(u => u.NormalizedUserName == normalizedUserName, cancellationToken: cancellationToken);
        }
        #endregion

        #region IUserClaimStore
        public Task<IList<Claim>> GetClaimsAsync(TUser user, CancellationToken cancellationToken)
        {
            ValidateParameters(user, cancellationToken);

            var claims = user.Claims
                .Select(c => new Claim(c.Type, c.Value))
                .ToList();

            return Task.FromResult<IList<Claim>>(claims);
        }

        public Task AddClaimsAsync(TUser user, IEnumerable<Claim> claims, CancellationToken cancellationToken)
        {
            ValidateParameters(user, cancellationToken);

            if (claims == null)
                throw new ArgumentNullException(nameof(claims));

            foreach (var claim in claims)
            {
                var userClaim = new IdentityClaim
                {
                    Type = claim.Type,
                    Value = claim.Value
                };
                user.Claims.Add(userClaim);
            }

            return Task.CompletedTask;
        }

        public Task ReplaceClaimAsync(TUser user, Claim claim, Claim newClaim, CancellationToken cancellationToken)
        {
            ValidateParameters(user, cancellationToken);

            if (claim == null)
                throw new ArgumentNullException(nameof(claim));

            if (newClaim == null)
                throw new ArgumentNullException(nameof(newClaim));

            var matched = user.Claims
                .Where(uc => uc.Value == claim.Value && uc.Type == claim.Type);

            foreach (var m in matched)
            {
                m.Value = newClaim.Value;
                m.Type = newClaim.Type;
            }

            return Task.CompletedTask;
        }

        public Task RemoveClaimsAsync(TUser user, IEnumerable<Claim> claims, CancellationToken cancellationToken)
        {
            ValidateParameters(user, cancellationToken);

            if (claims == null)
                throw new ArgumentNullException(nameof(claims));

            foreach (var claim in claims)
            {
                var matched = user.Claims
                    .Where(u => u.Value == claim.Value && u.Type == claim.Type)
                    .ToList();

                foreach (var m in matched)
                    user.Claims.Remove(m);
            }

            return Task.CompletedTask;
        }

        public async Task<IList<TUser>> GetUsersForClaimAsync(Claim claim, CancellationToken cancellationToken)
        {
            if (claim == null)
                throw new ArgumentNullException(nameof(claim));

            cancellationToken.ThrowIfCancellationRequested();

            var users = await FindAllAsync(u => u.Claims.Any(c => c.Type == claim.Type && c.Value == claim.Value), cancellationToken);

            return users.ToList();
        }
        #endregion

        #region IUserPasswordStore
        public Task SetPasswordHashAsync(TUser user, string passwordHash, CancellationToken cancellationToken)
        {
            ValidateParameters(user, cancellationToken);

            user.PasswordHash = passwordHash;
            return Task.CompletedTask;
        }

        public Task<string> GetPasswordHashAsync(TUser user, CancellationToken cancellationToken)
        {
            ValidateParameters(user, cancellationToken);

            return Task.FromResult(user.PasswordHash);
        }

        public Task<bool> HasPasswordAsync(TUser user, CancellationToken cancellationToken)
        {
            ValidateParameters(user, cancellationToken);

            return Task.FromResult(!string.IsNullOrEmpty(user.PasswordHash));
        }
        #endregion

        #region IUserSecurityStampStore
        public Task SetSecurityStampAsync(TUser user, string stamp, CancellationToken cancellationToken)
        {
            ValidateParameters(user, cancellationToken);

            user.SecurityStamp = stamp;
            return Task.CompletedTask;
        }

        public Task<string> GetSecurityStampAsync(TUser user, CancellationToken cancellationToken)
        {
            ValidateParameters(user, cancellationToken);

            return Task.FromResult(user.SecurityStamp);
        }
        #endregion

        #region IUserEmailStore
        public Task SetEmailAsync(TUser user, string email, CancellationToken cancellationToken)
        {
            ValidateParameters(user, cancellationToken);

            user.Email = email;
            return Task.CompletedTask;
        }

        public Task<string> GetEmailAsync(TUser user, CancellationToken cancellationToken)
        {
            ValidateParameters(user, cancellationToken);

            return Task.FromResult(user.Email);
        }

        public Task<bool> GetEmailConfirmedAsync(TUser user, CancellationToken cancellationToken)
        {
            ValidateParameters(user, cancellationToken);

            return Task.FromResult(user.EmailConfirmed);
        }

        public Task SetEmailConfirmedAsync(TUser user, bool confirmed, CancellationToken cancellationToken)
        {
            ValidateParameters(user, cancellationToken);

            user.EmailConfirmed = confirmed;
            return Task.CompletedTask;
        }

        public async Task<TUser> FindByEmailAsync(string normalizedEmail, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            return await FindOneAsync(u => u.NormalizedEmail == normalizedEmail, cancellationToken);
        }

        public Task<string> GetNormalizedEmailAsync(TUser user, CancellationToken cancellationToken)
        {
            ValidateParameters(user, cancellationToken);

            return Task.FromResult(user.NormalizedEmail);
        }

        public Task SetNormalizedEmailAsync(TUser user, string normalizedEmail, CancellationToken cancellationToken)
        {
            ValidateParameters(user, cancellationToken);

            user.NormalizedEmail = normalizedEmail;
            return Task.CompletedTask;
        }
        #endregion

        #region IUserLockoutStore
        public Task<DateTimeOffset?> GetLockoutEndDateAsync(TUser user, CancellationToken cancellationToken)
        {
            ValidateParameters(user, cancellationToken);

            return Task.FromResult(user.LockoutEnd);
        }

        public Task SetLockoutEndDateAsync(TUser user, DateTimeOffset? lockoutEnd, CancellationToken cancellationToken)
        {
            ValidateParameters(user, cancellationToken);

            user.LockoutEnd = lockoutEnd;
            return Task.CompletedTask;
        }

        public Task<int> IncrementAccessFailedCountAsync(TUser user, CancellationToken cancellationToken)
        {
            ValidateParameters(user, cancellationToken);

            user.AccessFailedCount++;
            return Task.FromResult(user.AccessFailedCount);
        }

        public Task ResetAccessFailedCountAsync(TUser user, CancellationToken cancellationToken)
        {
            ValidateParameters(user, cancellationToken);

            user.AccessFailedCount = 0;
            return Task.CompletedTask;
        }

        public Task<int> GetAccessFailedCountAsync(TUser user, CancellationToken cancellationToken)
        {
            ValidateParameters(user, cancellationToken);

            return Task.FromResult(user.AccessFailedCount);
        }

        public Task<bool> GetLockoutEnabledAsync(TUser user, CancellationToken cancellationToken)
        {
            ValidateParameters(user, cancellationToken);

            return Task.FromResult(user.LockoutEnabled);
        }

        public Task SetLockoutEnabledAsync(TUser user, bool enabled, CancellationToken cancellationToken)
        {
            ValidateParameters(user, cancellationToken);

            user.LockoutEnabled = enabled;
            return Task.CompletedTask;
        }
        #endregion

        #region IUserPhoneNumberStore
        public Task SetPhoneNumberAsync(TUser user, string phoneNumber, CancellationToken cancellationToken)
        {
            ValidateParameters(user, cancellationToken);

            user.PhoneNumber = phoneNumber;
            return Task.CompletedTask;
        }

        public Task<string> GetPhoneNumberAsync(TUser user, CancellationToken cancellationToken)
        {
            ValidateParameters(user, cancellationToken);

            return Task.FromResult(user.PhoneNumber);
        }

        public Task<bool> GetPhoneNumberConfirmedAsync(TUser user, CancellationToken cancellationToken)
        {
            ValidateParameters(user, cancellationToken);

            return Task.FromResult(user.PhoneNumberConfirmed);
        }

        public Task SetPhoneNumberConfirmedAsync(TUser user, bool confirmed, CancellationToken cancellationToken)
        {
            ValidateParameters(user, cancellationToken);

            user.PhoneNumberConfirmed = confirmed;
            return Task.CompletedTask;
        }
        #endregion

        #region IQueryableUserStore
        public IQueryable<TUser> Users => GetContainerAsync().Result.GetItemLinqQueryable<TUser>();
        #endregion

        #region IUserTwoFactorStore
        public Task SetTwoFactorEnabledAsync(TUser user, bool enabled, CancellationToken cancellationToken)
        {
            ValidateParameters(user, cancellationToken);

            user.TwoFactorEnabled = enabled;
            return Task.CompletedTask;
        }

        public Task<bool> GetTwoFactorEnabledAsync(TUser user, CancellationToken cancellationToken)
        {
            ValidateParameters(user, cancellationToken);

            return Task.FromResult(user.TwoFactorEnabled);
        }
        #endregion

        #region IUserAuthenticationTokenStore
        public Task SetTokenAsync(TUser user, string loginProvider, string name, string value, CancellationToken cancellationToken)
        {
            ValidateParameters(user, cancellationToken);

            var token = user.Tokens
                .FirstOrDefault(t => t.LoginProvider == loginProvider && t.Name == name);

            if (token == null)
            {
                token = new IdentityToken
                {
                    LoginProvider = loginProvider,
                    Name = name
                };
                user.Tokens.Add(token);
            }

            token.Value = value;

            return Task.CompletedTask;
        }

        public Task RemoveTokenAsync(TUser user, string loginProvider, string name, CancellationToken cancellationToken)
        {
            ValidateParameters(user, cancellationToken);

            var matched = user.Tokens
                .Where(t => t.LoginProvider == loginProvider && t.Name == name)
                .ToList();

            foreach (var m in matched)
                user.Tokens.Remove(m);

            return Task.CompletedTask;
        }

        public Task<string> GetTokenAsync(TUser user, string loginProvider, string name, CancellationToken cancellationToken)
        {
            ValidateParameters(user, cancellationToken);

            var token = user.Tokens
                .Where(t => t.LoginProvider == loginProvider && t.Name == name)
                .Select(t => t.Value)
                .FirstOrDefault();

            return Task.FromResult(token);
        }
        #endregion

        #region IUserAuthenticatorKeyStore
        public Task SetAuthenticatorKeyAsync(TUser user, string key, CancellationToken cancellationToken)
        {
            return SetTokenAsync(user, InternalLoginProvider, AuthenticatorKeyTokenName, key, cancellationToken);
        }

        public Task<string> GetAuthenticatorKeyAsync(TUser user, CancellationToken cancellationToken)
        {
            return GetTokenAsync(user, InternalLoginProvider, AuthenticatorKeyTokenName, cancellationToken);
        }
        #endregion

        #region IUserTwoFactorRecoveryCodeStore
        public Task ReplaceCodesAsync(TUser user, IEnumerable<string> recoveryCodes, CancellationToken cancellationToken)
        {
            ValidateParameters(user, cancellationToken);

            if (recoveryCodes == null)
                throw new ArgumentNullException(nameof(recoveryCodes));

            var mergedCodes = string.Join(";", recoveryCodes);
            return SetTokenAsync(user, InternalLoginProvider, RecoveryCodeTokenName, mergedCodes, cancellationToken);
        }

        public async Task<bool> RedeemCodeAsync(TUser user, string code, CancellationToken cancellationToken)
        {
            ValidateParameters(user, cancellationToken);

            var mergedCodes = await GetTokenAsync(user, InternalLoginProvider, RecoveryCodeTokenName, cancellationToken) ?? "";

            var splitCodes = mergedCodes.Split(';');
            if (splitCodes.Contains(code))
            {
                var updatedCodes = splitCodes
                    .Where(s => s != code)
                    .ToList();

                await ReplaceCodesAsync(user, updatedCodes, cancellationToken);

                return true;
            }

            return false;
        }

        public async Task<int> CountCodesAsync(TUser user, CancellationToken cancellationToken)
        {
            ValidateParameters(user, cancellationToken);

            var mergedCodes = await GetTokenAsync(user, InternalLoginProvider, RecoveryCodeTokenName, cancellationToken) ?? "";
            if (mergedCodes.Length > 0)
                return mergedCodes.Split(';').Length;

            return 0;
        }
        #endregion

        #region IUserRoleStore
        public Task AddToRoleAsync(TUser user, string roleName, CancellationToken cancellationToken)
        {
            ValidateParameters(user, cancellationToken);

            user.Roles.Add(roleName);
            return Task.CompletedTask;
        }

        public Task RemoveFromRoleAsync(TUser user, string roleName, CancellationToken cancellationToken)
        {
            ValidateParameters(user, cancellationToken);

            user.Roles.Remove(roleName);
            return Task.CompletedTask;
        }

        public Task<IList<string>> GetRolesAsync(TUser user, CancellationToken cancellationToken)
        {
            ValidateParameters(user, cancellationToken);

            IList<string> roles = user.Roles.ToList();
            return Task.FromResult(roles);
        }

        public Task<bool> IsInRoleAsync(TUser user, string roleName, CancellationToken cancellationToken)
        {
            ValidateParameters(user, cancellationToken);

            var isInRole = user.Roles.Contains(roleName);
            return Task.FromResult(isInRole);
        }

        public async Task<IList<TUser>> GetUsersInRoleAsync(string roleName, CancellationToken cancellationToken)
        {
            var users = await FindAllAsync(u => u.Roles.Any(r => r == roleName), cancellationToken);

            return users.ToList();
        }
        #endregion

        #region IDisposable
        public void Dispose() { }
        #endregion

        protected void ValidateParameters(IdentityUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
                throw new ArgumentNullException(nameof(user));
        }
    }
}
