using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Xunit.Abstractions;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using Cosmos.Abstracts;

namespace Cosmos.Identity.Tests
{
    public abstract class TestServiceBase
    {
        protected ITestOutputHelper OutputHelper { get; }

        protected IConfiguration Configuration { get; }

        protected IServiceProvider Services { get; }

        protected TestServiceBase(ITestOutputHelper outputHelper)
        {
            OutputHelper = outputHelper;

            var builder = new ConfigurationBuilder();
            Configure(builder);

            Configuration = builder.Build();

            var services = new ServiceCollection();
            ConfigureServices(services);

            Services = services.BuildServiceProvider();
        }

        protected virtual void Configure(IConfigurationBuilder configuration)
        {
            var enviromentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Test";

            configuration
                .AddJsonFile("appsettings.json")
                .AddJsonFile($"appsettings.{enviromentName}.json", true)
                .AddEnvironmentVariables();
        }

        protected virtual void ConfigureServices(IServiceCollection services)
        {
            services
                .AddSingleton(Configuration)
                .AddLogging((builder) => builder
                    .AddXUnit(OutputHelper)
                    .SetMinimumLevel(LogLevel.Debug)
                )
                .AddCosmosRepository();

            services
                .AddIdentity<IdentityUser, IdentityRole>()
                .AddCosmosStores();
        }


        protected virtual IdentityUser CreateUser()
        {
            string userName = "UserName" + DateTime.Now.Ticks;

            var user = new IdentityUser
            {
                Id = ObjectId.GenerateNewId().ToString(),
                UserName = userName,
                NormalizedUserName = userName.ToUpperInvariant(),
                Email = $"{userName}@mailinator.com",
                NormalizedEmail = $"{userName}@mailinator.com".ToUpperInvariant(),
                EmailConfirmed = true,
                PhoneNumber = "888-555-1212",
                PhoneNumberConfirmed = true
            };

            return user;
        }

        protected virtual IdentityRole CreateRole()
        {
            string name = "Role" + DateTime.Now.Ticks;

            var role = new IdentityRole
            {
                Id = ObjectId.GenerateNewId().ToString(),
                Name = name,
                NormalizedName = name.ToUpperInvariant(),
            };

            return role;
        }
    }

}