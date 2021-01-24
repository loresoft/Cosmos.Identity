﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using Cosmos.Abstracts;

namespace Cosmos.Identity
{
    public class IdentityUser : CosmosEntity
    {
        public IdentityUser()
        {
            Claims = new List<IdentityClaim>();
            Logins = new List<IdentityLogin>();
            Tokens = new List<IdentityToken>();

            Roles = new HashSet<string>();
        }

        public string UserName { get; set; }

        public virtual string NormalizedUserName { get; set; }


        public string Email { get; set; }

        public virtual string NormalizedEmail { get; set; }


        [DefaultValue(false)]
        public bool EmailConfirmed { get; set; }


        public string PasswordHash { get; set; }

        public string SecurityStamp { get; set; }


        public string PhoneNumber { get; set; }

        [DefaultValue(false)]
        public bool PhoneNumberConfirmed { get; set; }


        [DefaultValue(false)]
        public bool TwoFactorEnabled { get; set; }


        public DateTimeOffset? LockoutEnd { get; set; }

        [DefaultValue(false)]
        public bool LockoutEnabled { get; set; }

        [DefaultValue(0)]
        public int AccessFailedCount { get; set; }


        public ISet<string> Roles { get; set; }

        public IList<IdentityClaim> Claims { get; set; }

        public IList<IdentityLogin> Logins { get; set; }

        public IList<IdentityToken> Tokens { get; set; }
    }
}
