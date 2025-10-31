using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ImageToText;

namespace Vivet.AI.Hosting.HealthChecks;

/// <summary>
/// Image Extraction Model Health Check.
/// </summary>
/// <param name="imageToTextService">The <see cref="IImageToTextService"/>.</param>
/// <param name="promptExecutionSettings">The <see cref="PromptExecutionSettings"/>.</param>
public class ImageExtractionModelHealthCheck(IImageToTextService imageToTextService, PromptExecutionSettings promptExecutionSettings)
    : IHealthCheck
{
    private const string BASE64 = "iVBORw0KGgoAAAANSUhEUgAAAFsAAAA1CAYAAAAuyJezAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAJcEhZcwAADsMAAA7DAcdvqGQAAAJ/SURBVHhe7ZkPjcIwGMVnAQ1YwAMS0IAFHOAAByhAAQYwgAM8cHkLvXx7abtdt71wyfslX3Ksf/dr13a77m1kdHzBrIdlC7FsIZYtxLKFWLYQyxZi2UIsW4hlC7FsIZYtxLKFWLYQyxZi2UIsW4hlC7FsIbNlHw6Hd9d12dhut3369XrlYj24nvJuNpv34/EYpN/v9/56zHO5XAZ5IqfT6Tcv/q7xer369tE/9DP2e7fb9eXR/pLMls2CS7Hf7/sbjJzP50EevjlOT8GDkkAbsb0SGLA4iLXI9bsVmezU8QjLnCr7eDwO8iWmyEZZrm8sak/TX1hUdrxBzIacrDgrOb0mG492zPt8Pgd5wZhsbg+BZeR2uw3y4XccFJRbgtVkJ3gmxY7zzddko+5YV25212RjcGJbiNJekkAZ1PP1MzvBQufIxlORfmPN5bW0JjtuntwPFf9KNohCWVhNdjxx5AZKweqyowBEXB9bZCNPSVpN9lg/FawmG+sdr9csp0U2KM3ukuw4QFwGxOUpF9yvVhaVPRa80bTKjptdHMBW2ZzOwflbkcnOdbhVNohPTTpVlGTzSYTrGpvZub63sKps3BROAbkzMZgjOwrE5gdKsgG/9tfgmf6VsvkGx5gjG/Dsrsnmbzi8pEUsO1N3nN1Ir8lmgbkPXwnOa9kf4uyOr/S5/Dy7ESgf28UAcr8s+wNvfrX8OLXwN5YpYdkBPs8jMItzQDi/uo8F96uV2bLja3Du41CNsX8exPRa3RAY12sEf8lj8ERgk0Q5/raN2Y/BQnrpJNXCbNlmOpYtxLKFWLYQyxZi2UIsW4hlC7FsIZYtxLKFWLYQyxZi2UIsW8gPoiwC4yQtPXUAAAAASUVORK5CYII=";
    private const string DATA_URI = $"data:image/png;base64,{BASE64}";

    private readonly IImageToTextService audioToTextService = imageToTextService ?? throw new ArgumentNullException(nameof(imageToTextService));
    private readonly PromptExecutionSettings promptExecutionSettings = promptExecutionSettings ?? throw new ArgumentNullException(nameof(promptExecutionSettings));

    /// <inheritdoc />
    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        if (context == null)
            throw new ArgumentNullException(nameof(context));

        try
        {
            var imageContent = new ImageContent(DATA_URI);

            var contents = await this.audioToTextService
                .GetTextContentsAsync(imageContent, this.promptExecutionSettings, null, cancellationToken);

            if (!contents.Any())
            {
                return HealthCheckResult.Unhealthy("No content");
            }

            return HealthCheckResult.Healthy("Success");
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy(ex.Message, ex);
        }
    }
}