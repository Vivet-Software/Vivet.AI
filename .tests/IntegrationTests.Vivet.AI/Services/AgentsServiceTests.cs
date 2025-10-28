using Microsoft.Extensions.DependencyInjection;
using Microsoft.SemanticKernel;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using Vivet.AI.Services.Interfaces;
using Vivet.AI.Services.Requests.Agents;
using Vivet.AI.Services.Requests.Agents.Models;

namespace IntegrationTests.Vivet.AI.Services;

[TestClass]
public class AgentsServiceTests : BaseTests
{
    private IAgentsService AgentsService => this.ServiceProvider.GetRequiredService<IAgentsService>();

    //private sealed class OrderStatusPlugin
    //{
    //    [KernelFunction]
    //    // ReSharper disable UnusedMember.Local
    //    public string CheckOrderStatus(string orderId) => $"Order {orderId} is shipped and will arrive in 2-3 days.";
    //    // ReSharper restore UnusedMember.Local
    //}

    //private sealed class OrderReturnPlugin
    //{
    //    [KernelFunction]
    //    // ReSharper disable UnusedMember.Local
    //    public string ProcessReturn(string orderId, string reason) => $"Return for order {orderId} has been processed successfully. {reason}";
    //    // ReSharper restore UnusedMember.Local
    //}

    //private sealed class OrderRefundPlugin
    //{
    //    [KernelFunction]
    //    // ReSharper disable UnusedMember.Local
    //    public string ProcessReturn(string orderId, string reason) => $"Refund for order {orderId} has been processed successfully. {reason}";
    //    // ReSharper restore UnusedMember.Local
    //}

    [TestMethod]
    public async Task InvokeWhenOrchestrationSequentialTest()
    {
        var agents = new AgentDescriptor[]
        {
            new()
            {
                Name = "Analyst",
                Description = "A agent that extracts key concepts from a product description.",
                Instructions = @"You are a marketing analyst. Given a product description, identify:
- Key features
- Target audience
- Unique selling points"
            },
            new()
            {
                Name = "copywriter",
                Description = "An agent that writes a marketing copy based on the extracted concepts.",
                Instructions = @"You are a marketing copywriter. Given a block of text describing features, audience, and USPs,
compose a compelling marketing copy (like a newsletter section) that highlights these points.
Output should be short (around 150 words), output just the copy as a single text block."
            },
            new()
            {
                Name = "editor",
                Description = "An agent that formats and proofreads the marketing copy.",
                Instructions = @"You are an editor. Given the draft copy, correct grammar, improve clarity, ensure consistent tone,
give format and make it polished. Output the final improved copy as a single text block."
            }
        };

        var response = await this.AgentsService
            .InvokeAsync(new SequentialAgentsRequest
            {
                Name = "Sequential",
                Input = "An eco-friendly stainless steel water bottle that keeps drinks cold for 24 hours",
                Agents = agents,
                ConfigOverrides =
                {
                    Plugins =
                    {
                        Memory =
                        {
                            EnableMemoryPlugin = false
                        }
                    }
                }
            });

        Assert.IsNotNull(response);
        Assert.IsNull(response.Exception);
    }

    [TestMethod]
    public async Task InvokeWhenOrchestrationConcurrentTest()
    {
        var agents = new AgentDescriptor[]
        {
            new()
            {
                Name = "Physicist",
                Description = "An expert in physics",
                Instructions = "You are an expert in physics, and anwer all questions from a physics perspective."
            },
            new()
            {
                Name = "Chemist",
                Description = "An expert in chemistry",
                Instructions = "You are an expert in chemist, and anwer all questions from a chemistry perspective."
            }
        };

        var response = await this.AgentsService
            .InvokeAsync(new ConcurrentAgentsRequest
            {
                Name = "Concurrent",
                Input = "What is temperature?",
                Agents = agents,
                ConfigOverrides =
                {
                    Plugins =
                    {
                        Memory =
                        {
                            EnableMemoryPlugin = false
                        }
                    }
                }
            });

        Assert.IsNotNull(response);
        Assert.IsNull(response.Exception);
    }

    [TestMethod]
    public async Task InvokeWhenOrchestrationGroupChatTest()
    {
        await Task.CompletedTask;
        Assert.Inconclusive();

//        var agents = new AgentDescriptor[]
//        {
//            new()
//            {
//                Name = "copywriter",
//                Description = "An agent that writes a marketing copy based on the extracted concepts.",
//                Instructions = @" You are a copywriter with ten years of experience and are known for brevity and a dry humor.
//The goal is to refine and decide on the single best copy as an expert in the field.
//Only provide a single proposal per response.
//You're laser focused on the goal at hand.
//Don't waste time with chit chat.
//Consider suggestions when refining an idea"
//            },
//            new()
//            {
//                Name = "reviewer",
//                Description = "An editor.",
//                Instructions = @"You are an art director who has opinions about copywriting born of a love for David Ogilvy.
//The goal is to determine if the given copy is acceptable to print.
//If so, state: ""I Approve"".
//If not, provide insight on how to refine suggested copy without example."
//            }
//        };

//        var response = await this.AgentsService
//            .InvokeAsync(new SequentialAgentsRequest
//            {
//                Name = "Sequential",
//                Input = "Create a slogan for a new electric SUV that is affordable and fun to drive.",
//                Agents = agents,
//                ConfigOverrides =
//                {
//                    Plugins = 
//                    { 
//                        Memory =
//                        {
//                            EnableMemoryPlugin = false
//                        }
//                    }
//                }
//            });
    }

    [TestMethod]
    public async Task InvokeWhenOrchestrationHandOffTest()
    {
        await Task.CompletedTask;
        Assert.Inconclusive();

        //var agents = new AgentDescriptor[]
        //{
        //    new()
        //    {
        //        Name = "TriageAgent",
        //        Description = "Handle customer requests.",
        //        Instructions = "A customer support agent that triages issues."
        //    },
        //    new()
        //    {
        //        Name = "OrderStatusAgent",
        //        Description = "A customer support agent that checks order status.",
        //        Instructions = "Handle order status requests.",
        //        Plugins =
        //        {
        //            CustomPlugins = 
        //            [
        //                new CustomPlugin
        //                {
        //                    Name = "OrderStatusPlugin",
        //                    Type = typeof(OrderStatusPlugin)
        //                }
        //            ]
        //        }
        //    },
        //    new()
        //    {
        //        Name = "OrderReturnAgent",
        //        Description = "A customer support agent that handles order returns.",
        //        Instructions = "Handle order return requests.",
        //        Plugins =
        //        {
        //            CustomPlugins = 
        //            [
        //                new CustomPlugin
        //                {
        //                    Name = "OrderReturnPlugin",
        //                    Type = typeof(OrderReturnPlugin)
        //                }
        //            ]
        //        }
        //    },
        //    new()
        //    {
        //        Name = "OrderRefundAgent",
        //        Description = "A customer support agent that handles order refund.",
        //        Instructions = "Handle order refund requests.",
        //        Plugins =
        //        {
        //            CustomPlugins =
        //            [
        //                new CustomPlugin
        //                {
        //                    Name = "OrderRefundPlugin",
        //                    Type = typeof(OrderRefundPlugin)
        //                }
        //            ]
        //        }
        //    }
        //};

        //var response = await this.AgentsService
        //    .InvokeAsync(new HandOffAgentsRequest
        //    {
        //        Name = "Sequential",
        //        Input = "I am a customer that needs help with my orders",
        //        Agents = agents,
        //        ConfigOverrides =
        //        {
        //            Plugins =
        //            {
        //                Memory =
        //                {
        //                    EnableMemoryPlugin = false
        //                }
        //            }
        //        }
        //    });

        //Assert.IsNotNull(response);
        //Assert.IsNull(response.Exception);
    }

    [TestMethod]
    public async Task InvokeWhenOrchestrationMagneticTest()
    {
        await Task.CompletedTask;
        Assert.Inconclusive();
    }

    [TestMethod]
    public async Task InvokeWhenBlobsTest()
    {
        await Task.CompletedTask;
        Assert.Inconclusive();
    }

    [TestMethod]
    public async Task InvokeWhenBuiltInPluginsTest()
    {
        await Task.CompletedTask;
        Assert.Inconclusive();
    }

    [TestMethod]
    public async Task InvokeWhenCustomPluginsTest()
    {
        await Task.CompletedTask;
        Assert.Inconclusive();
    }

    [TestMethod]
    public async Task InvokeWhenErrorMessageTest()
    {
        await Task.CompletedTask;
        Assert.Inconclusive();
    }
}