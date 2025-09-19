using Microsoft.Extensions.DependencyInjection;
using Microsoft.SemanticKernel;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading.Tasks;
using Vivet.AI.Config;
using Vivet.AI.Extensions.Consts;
using Vivet.AI.Services;
using Vivet.AI.Services.Requests.Agent;
using Vivet.AI.Services.Requests.Agent.Enums;

namespace IntegrationTests.Vivet.AI.Services;

[TestClass]
public class AgentServiceTests : BaseTests
{
    [TestMethod]
    public async Task InvokeTest()
    {
        var kernelBuilder = this.ServiceProvider.GetRequiredKeyedService<IKernelBuilder>(ServiceIds.CHAT_SERVICE_ID);
        var options = this.ServiceProvider.GetRequiredService<AiOptions>();

        // TODO: CONFIG / REGISTRATION: 
        // We need Agent Configuration
        // Consider if we should add the ChatCompletion to Kernel for Chat, Metadata and Summarization
        kernelBuilder
            .AddAzureOpenAIChatCompletion(options.Chat.Model.Name, options.Endpoint, options.ApiKey);

        var agenOptions = new AgentOptions
        {
            Timeout = TimeSpan.FromSeconds(20)
        };

        var agentService = new AgentService(agenOptions, kernelBuilder);

        var agents = new Agent2[]
        {
            new()
            {
                Name = "PhysicsExpert"
            },
            new()
            {
                Name = "ChemistryExpert"
            },
        };

        var response = await agentService
            .InvokeAsync(new AgentRequest
            {
                Input = "What is temperature?", 
                OrchestrationType = AgentOrchestrationType.Concurrent, 
                Agents = agents
            });

        //Console.WriteLine($"# RESULT:\n{string.Join("\n\n", output.Select(text => $"{text}"))}");
        //Console.WriteLine("\n\nORCHESTRATION HISTORY");
        //foreach (ChatMessageContent message in this.agentThread.ChatHistory)
        //{
        //    Console.WriteLine(message.Content);
        //}
    }
}