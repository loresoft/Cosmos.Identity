using System;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Cosmos.Identity
{
    public static class IdentityBuilderExtensions
    {
        public static IdentityBuilder AddCosmosStores(this IdentityBuilder builder)
        {
            Type userType = builder.UserType;
            Type roleType = builder.RoleType;

            if (userType is null)
            {
                throw new InvalidOperationException($"Must provide an identity user of type {typeof(IdentityUser).FullName} or one that extends this type.");
            }
            else if (!typeof(IdentityUser).IsAssignableFrom(userType))
            {
                throw new InvalidOperationException($"{userType.Name} must extend {typeof(IdentityUser).FullName}.");
            }
            if (roleType is null)
            {
                roleType = typeof(IdentityRole);
            }
            else if (!typeof(IdentityRole).IsAssignableFrom(roleType))
            {
                throw new InvalidOperationException($"{roleType.Name} must extend {typeof(IdentityRole).FullName}.");
            }

            Type userStoreType = typeof(UserStore<>).MakeGenericType(userType);
            //Type roleStoreType = typeof(RoleStore<>).MakeGenericType(roleType);

            builder.Services.TryAddScoped(typeof(IUserStore<>).MakeGenericType(userType), userStoreType);
            //builder.Services.TryAddScoped(typeof(IRoleStore<>).MakeGenericType(roleType), roleStoreType);

            return builder;
        }
    }
}
