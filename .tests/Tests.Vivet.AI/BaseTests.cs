using System;
using System.IO;
using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Vivet.AI.Extensions.Orchestration.AzureInferenceAi;

namespace Tests.Vivet.AI;

public class BaseTests
{
    protected IServiceProvider serviceProvider;

    protected readonly string tenantId = "5232f275-40e2-4d18-9dce-d619b6180b40";
    protected readonly string subTenantId = "bed6e482-6bcb-447b-b509-65d7c833e698a";
    protected readonly string userId = "a823dd01-4734-44bb-9402-29c7813652a4";
    protected readonly string language = "en";
    protected readonly string createdBy = "createdBy";
    protected readonly string source = "source";

    [TestInitialize]
    public void TestSetup()
    {
        var services = new ServiceCollection();

        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .AddEnvironmentVariables()
            .AddUserSecrets(Assembly.GetExecutingAssembly())
            .Build();

        services
            .AddSingleton<IConfiguration>(configuration)
            .AddLogging(x => x.AddConsole());

        services
            .AddAzureAiInference();

        this.serviceProvider = services
            .BuildServiceProvider();
    }
}