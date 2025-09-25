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
using Vivet.AI.Services.Requests.Agent.Models;

namespace IntegrationTests.Vivet.AI.Services;

[TestClass]
public class AgentServiceTests : BaseTests
{
    [TestMethod]
    public async Task InvokeTest()
    {
        // TODO: CONFIG / REGISTRATION: 
        // We need Agent Configuration
        // Consider if we should add the ChatCompletion to Kernel for Chat, Metadata and Summarization

        var kernelBuilder = this.ServiceProvider.GetRequiredKeyedService<IKernelBuilder>(ServiceIds.CHAT_SERVICE_ID);
        var options = this.ServiceProvider.GetRequiredService<AiOptions>();

        var agenOptions = new AgentOptions
        {
            Timeout = TimeSpan.FromSeconds(20)
        };

        kernelBuilder
            .AddAzureOpenAIChatCompletion(options.Chat.Model.Name, options.Endpoint, options.ApiKey);

        var promptExecutionSettings = new PromptExecutionSettings();

        var agentService = new AgentService(agenOptions, this.ServiceProvider, kernelBuilder, promptExecutionSettings);

        var agents = new AgentDescriptor[]
        {
            new()
            {
                Name = "PhysicsExpert"
            },
            new()
            {
                Name = "ChemistryExpert"
            }
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