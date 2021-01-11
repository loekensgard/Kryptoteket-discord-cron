using Kryptoteket.Sync.Configurations;
using Kryptoteket.Sync.CosmosDB;
using Kryptoteket.Sync.CosmosDB.Repositories;
using Kryptoteket.Sync.Interfaces;
using Kryptoteket.Sync.Services;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;

[assembly: FunctionsStartup(typeof(Kryptoteket.Sync.Startup))]
namespace Kryptoteket.Sync
{
    class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(builder.GetContext().ApplicationRootPath)
                .AddJsonFile("settings.json")
                .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();

            builder.Services.Configure<DiscordConfiguration>(options => config.GetSection("Discord").Bind(options));

            builder.Services.AddDbContext<RegistryContext>(options => options.UseCosmos(
                config.GetSection("CosmosDB")["Endpoint"],
                config.GetSection("CosmosDB")["Key"],
                config.GetSection("CosmosDB")["DatabaseName"]));

            builder.Services.AddScoped<IUserBetRepository, UserBetRepository>();
            builder.Services.AddScoped<IBetRepository, BetRepository>();
            builder.Services.AddScoped<IBetWinnersRepository, BetWinnersRepository>();

            builder.Services.AddHttpClient<ICoinGeckoAPIService, CoinGeckoAPIService>()
                .ConfigureHttpClient((service, context) =>
                {
                    context.BaseAddress = new Uri(config.GetSection("CoinGecko")["Base"]);
                });

            builder.Services.AddHttpClient<IDiscordWebhookService, DiscordWebhookService>()
                .ConfigureHttpClient((service, context) =>
                {
                    context.BaseAddress = new Uri(config.GetSection("Discord")["Base"]);
                });

            builder.Services.AddTransient<HttpResponseService>();
        }
    }
}
