using Microsoft.Extensions.DependencyInjection;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.AzureOpenAI;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Text;
using System.Threading.Tasks;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using Vivet.AI.Config;
using Vivet.AI.Extensions.Consts;
using Vivet.AI.Services;
using Vivet.AI.Services.Requests.Agent;
using Vivet.AI.Services.Requests.Agent.Enums;
using Vivet.AI.Services.Requests.Agent.Models;
using Vivet.AI.Services.Requests.Agent.Models.Plugins.BuiltIn;

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

        var agentOptions = new AgentOptions
        {
            Timeout = TimeSpan.FromSeconds(200)
        };

        kernelBuilder
            .AddAzureOpenAIChatCompletion(options.Chat.Model.Name, options.Endpoint, options.ApiKey);

        var promptExecutionSettings = new PromptExecutionSettings();

        var agentService = new AgentService(agentOptions, this.ServiceProvider, kernelBuilder, promptExecutionSettings);

        var agents = new AgentDescriptor[]
        {
            new()
            {
                Name = "ChemistryExpert",
                Instructions = "You are an expert in chemist, and anwer all questions from a chemistry perspective."
            },
            new()
            {
                Name = "PhysicsExpert",
                Instructions = "You are an expert in physics, and anwer all questions from a physics perspective."
            },
        };

        var response = await agentService
            .InvokeAsync(new AgentRequest
            {
                Name = "My Agent Orchestration",
                Input = "What is temperature?", 
                OrchestrationType = AgentOrchestrationType.Concurrent,
                Agents = agents,
                Plugins =
                {
                    Context =
                    {
                        Memory = new AgentMemoryContext
                        {
                            AgentId = Guid.NewGuid().ToString()
                        }
                    }
                }
            });

        //Console.WriteLine($"# RESULT:\n{string.Join("\n\n", output.Select(text => $"{text}"))}");
        //Console.WriteLine("\n\nORCHESTRATION HISTORY");
        //foreach (ChatMessageContent message in this.agentThread.ChatHistory)
        //{
        //    Console.WriteLine(message.Content);
        //}
    }



    [TestMethod]
    public async Task InvokeTest2()
    {
        var kernelBuilder = this.ServiceProvider.GetRequiredKeyedService<IKernelBuilder>(ServiceIds.CHAT_SERVICE_ID);
        var options = this.ServiceProvider.GetRequiredService<AiOptions>();

        var agentOptions = new AgentOptions
        {
            Timeout = TimeSpan.FromSeconds(200)
        };

        kernelBuilder
            .AddAzureOpenAIChatCompletion(options.Chat.Model.Name, options.Endpoint, options.ApiKey);

        var promptExecutionSettings = new AzureOpenAIPromptExecutionSettings
        {
            ModelId = null,
            ExtensionData = null,
            FunctionChoiceBehavior = null,
            ResponseFormat = "text",
            TokenSelectionBiases = null,
            ToolCallBehavior = ToolCallBehavior.AutoInvokeKernelFunctions,
            User = null,
            ChatSystemPrompt = null,
            Logprobs = null,
            TopLogprobs = null,
            Store = false,
            Metadata = null,
            MaxTokens = 2048
        };

        var agentService = new AgentService(agentOptions, this.ServiceProvider, kernelBuilder, promptExecutionSettings);

        var agents = new AgentDescriptor[]
        {
            new()
            {
                Name = "ChemistryExpert",
                Instructions = "You are a helpful assistant."
                //Instructions = "You are an expert in chemist, and anwer all questions from a chemistry perspective."
            },
            //new()
            //{
            //    Id = "PhysicsExpert",
            //    Instructions = "You are an expert in physics, and anwer all questions from a physics perspective."
            //},
        };

        var response = await agentService
            .InvokeAsync(new AgentRequest
            {
                Name = "My Agent Orchestration",
                Input = "Search the web for the current temparature of Copenhagen",
                //Input = "What is temperature? Also find the current temparature of Copenhagen",
                OrchestrationType = AgentOrchestrationType.Concurrent,
                Agents = agents,
                Plugins =
                {
                    Context =
                    {
                        Memory = new AgentMemoryContext
                        {
                            AgentId = Guid.NewGuid().ToString()
                        }
                    }
                }
            });

        //Console.WriteLine($"# RESULT:\n{string.Join("\n\n", output.Select(text => $"{text}"))}");
        //Console.WriteLine("\n\nORCHESTRATION HISTORY");
        //foreach (ChatMessageContent message in this.agentThread.ChatHistory)
        //{
        //    Console.WriteLine(message.Content);
        //}
    }

}