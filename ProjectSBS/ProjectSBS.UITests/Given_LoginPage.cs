namespace ProjectSBS.UITests;

public class Given_LoginPage : TestBase
{
    [Test]
    public async Task When_SmokeTest()
    {
        // Add delay to allow for the splash screen to disappear
        await Task.Delay(5000);

        // Query for the MainPage Text Block
        static IAppQuery textBlock(IAppQuery q) => q.All().Marked("LoginWithMicrosoft");
        App.WaitForElement(textBlock);

        // Take a screenshot and add it to the test results
        TakeScreenshot("After launch");
    }
}
