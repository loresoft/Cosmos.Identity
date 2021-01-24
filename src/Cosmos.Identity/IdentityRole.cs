using System.Collections.Generic;
using Cosmos.Abstracts;

namespace Cosmos.Identity
{
    public class IdentityRole : CosmosEntity
    {
        public IdentityRole()
        {
            Claims = new List<IdentityClaim>();
        }

        public string Name { get; set; }

        public string NormalizedName { get; set; }

        public IList<IdentityClaim> Claims { get; set; }
    }
}
