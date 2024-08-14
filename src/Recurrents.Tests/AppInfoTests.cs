using Microsoft.Extensions.Localization;

namespace Recurrents.Tests;

public class AppInfoTests
{
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public void AppInfoCreation()
    {
        var appInfo = new AppConfig { Environment = "Test" };

        appInfo.Should().NotBeNull();
        appInfo.Environment.Should().Be("Test");
    }

    [Test]
    public void When_TagsIdsAreUnique()
    {
        var tagService = new TagService(null);

        tagService.Should().NotBeNull();
        tagService.Tags.Should().NotBeNull();

        tagService.Tags
            .GroupBy(tag => new { tag.Id })
            .Where(group => group.Count() > 1)
            .Should().BeEmpty("Duplicate tags IDs found");
    }
}
