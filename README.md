# Cosmos.Identity
Cosmos DB provider for ASP.NET Core Identity framework

[![Build status](https://ci.appveyor.com/api/projects/status/aj2b6swwjdl0v8wr?svg=true)](https://ci.appveyor.com/project/LoreSoft/cosmos-identity)

[![NuGet Version](https://img.shields.io/nuget/v/Cosmos.Identity.svg?style=flat-square)](https://www.nuget.org/packages/Cosmos.Identity/)

[![Coverage Status](https://coveralls.io/repos/github/loresoft/Cosmos.Identity/badge.svg?branch=main)](https://coveralls.io/github/loresoft/Cosmos.Identity?branch=main)

## Download

The Cosmos.Identity library is available on nuget.org via package name `Cosmos.Identity`.

To install Cosmos.Identity, run the following command in the Package Manager Console

    PM> Install-Package Cosmos.Identity
    
More information about NuGet package available at
<https://nuget.org/packages/Cosmos.Identity>

## Development Builds

Development builds are available on the myget.org feed.  A development build is promoted to the main NuGet feed when it's determined to be stable. 

In your Package Manager settings add the following package source for development builds:
<http://www.myget.org/F/loresoft/>

## Usage

appsettings.json configuration

```json
{
  "CosmosRepository": {
    "ConnectionString": "AccountEndpoint=https://localhost:8081/;AccountKey=C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==",
    "DatabaseId": "SampleWebsite",
    "OptimizeBandwidth": false,
    "AllowBulkExecution": true
  }
}
```

Startup.cs

```c#
using Cosmos.Identity;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using IdentityRole = Cosmos.Identity.IdentityRole;
using IdentityUser = Cosmos.Identity.IdentityUser;

namespace Sample.Website
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCosmosRepository();

            services
                .AddIdentity<IdentityUser, IdentityRole>()
                .AddCosmosStores()
                .AddDefaultUI()
                .AddDefaultTokenProviders();

            services.AddRazorPages();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
            });
        }
    }
}
```
