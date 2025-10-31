using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using Vivet.AI.Services.Interfaces;
using Vivet.AI.Services.Models.Blobs;
using Vivet.AI.Services.Models.Blobs.Data;
using Vivet.AI.Services.Models.MimeTypes;
using Vivet.AI.Services.Requests.Vision;

namespace IntegrationTests.Vivet.AI.Services;

[TestClass]
public class VisionServiceTests : BaseTests
{
    private IVisionService VisionService => this.ServiceProvider.GetRequiredService<IVisionService>();

    [TestMethod]
    public async Task ExtractTextWhenImageTest()
    {
        const string BASE_64 = "iVBORw0KGgoAAAANSUhEUgAAALwAAAA4CAYAAABHaJJlAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAJcEhZcwAADsMAAA7DAcdvqGQAAAUxSURBVHhe7ZuLUSMxEESdAjGQAjkQAjGQAhmQARkQARGQAAmQATlw1Vc1rnafZqy1d42P6Velwl5p9Rm1RiP5bvdtTCN2+sCY34wFb1phwZtWWPCmFRa8aYUFb1phwZtWWPCmFRa8aYUFb1phwZtWWPCmFRa8aYUFb1phwZtWWPCmFRa8acWvF/zd3d33brf7m56eng7y8D3yUM4s5+3tbW/Dm5ub78/PTy1S8vj4uH8fn7fm1ws+jIl0f39/kIfvnG+W8/z8fGDD9/d3LVLCc6DzswWbzvLt7e3U6n19fd2Xg5f4+PjQIidzTYLHuLi9l5cXLbLn4eHhwCaVkHgceO+SWPAEG6IazLlGq6j68NOCz8T59fV1UA5Jw7FAy8KWl+TcubPgTzBaRdWHSwsewFtHe/g8guPiSNkZQ8uuabsZzp07C/4Eo1VUffgJwXOogjQK3/gwzQneXNGyl+bcubPgC6PhBgBnAT4b4DOeZbcDVR9mBY8zhpaFx63azeDzCtIojuebJU7w5gr3axQiYZHAvlon3sPz0SICYePYhdB2PMPfGPfM3MW8xe6Gv1ioaNuCT4wGoXA4MEooo1R9UBEr8L68uLI0ajcDk8/vjkSq9UcaxfGcr4tnxmbZJQGX0UWKFHN0bO5Qd9YHLEK2r87PFvw7yyvCg6sGM2M0NVaW1ONWfagED+8zI/ZI2ucK3aEY1BN5EAT3UeN4LovEwl1iMwhSPb3m6zvR1rG5W2JDnZ8tuJjglyQ1mk4652Or5QnR60+uVw1aCV4nEp44FlNs0VXdFRp38yLldlEOXpvLsjC5rC4cHRuHLxCrhjjIZzgPia9G2f5qJ87TnQFtxlhHu88SG57K1QteQ4BRHMtGVy/I76pBVRRM5YUDfV+9ZIberHBIxHWinHpqHj+X5YWuNlMnANBXFlxlN6RR2AMqwesBXXdfDXd0frbg6gWv2/ZMyvqgBlXBBioY9X5BNdkVenfOguTnIRAWBcfxXJYXjXrWrF/Z+AE/V7sxlQ14F9EFFXAfqnbW4mKCrwZTGU3zZhLDz7UP2YTrIssEr+UyYY1gMcQOwvXxrsKeMp6r589CnapfWo7h52o3RuvgtmbqsODPFLx6Es7TPvyk4HVccX0Y39nraxwPz8/PdMxad9YvLcfwc7Ubo3VY8BODqYw2uz1nVH3IBK/eMxO89juLc0foYkFszv3hEEX7gzz2+npdqQsksxm3p7/68vtqN0ZtwG3xOUgXZWDBi9F0sjPxZVR9yAQP+Hl2aOWwRAUzg8bm3KYe8PQmir/rQV4XU3Zo5TL6ewDnqd2Yau7UvjomnduqnbW4esEDvctFeTYeDAevhnL6I1DVB50QRm8Y+FfV0bXkSFTH0DYijRYYt6f2GKFXfrBPdS25xG5MNXe607S6lqwGUxkNaFhTJd0Bqj5UglfvU6XRDzczqCAijRZPZgMdU5DVPUqjOo7lB9Xc6dXnsVS1sxb/heCBbvlZWuKpKsGDkRfSBG+7JHZnskWlYwB6VRpJ/zkBM2Mz2GC0WLVMxrG5yxYqEjw+L8yqnbX4d5ZXhLfekdcK9L+JZQKCMVGPbukwFCZXjQ24rB7uOEzIDlUQGt7TEABtcphwKrro0F+NdYMlZYORzWBjhFOjhRVUdmNY0Kh31B/0Qa9WsVBgO170eo7Ygk0Fb8y1YcGbVljwphUWvGmFBW9aYcGbVljwphUWvGmFBW9aYcGbVljwphUWvGmFBW9aYcGbVljwphUWvGmFBW9aYcGbVvwB+JN0WaoiXJoAAAAASUVORK5CYII=";
        const string EXPECTED = "Hello World";

        var response = await this.VisionService
            .ExtractText(new ImageTextExtractionRequest
            {
                Blob = new ImageBlob
                {
                    Data = new BlobDataBase64
                    {
                        Base64 = BASE_64
                    },
                    MimeType = ImageMimeType.Png
                }
            });

        Assert.IsNotNull(response);

        var transcribedText = response.Texts.FirstOrDefault();
        Assert.IsNotNull(transcribedText);
        Assert.AreEqual(EXPECTED, transcribedText.Content);
        Assert.AreEqual("english", transcribedText.Language);
    }

    [TestMethod]
    public async Task ExtractTextWhenDocumentTest()
    {
        await Task.CompletedTask;

        Assert.Inconclusive();
    }

    [TestMethod]
    public async Task ExtractImagesWhenVideoTest()
    {
        await Task.CompletedTask;

        Assert.Inconclusive();
    }

    [TestMethod]
    public async Task ExtractImagesWhenDocumentTest()
    {
        await Task.CompletedTask;

        Assert.Inconclusive();
    }
}