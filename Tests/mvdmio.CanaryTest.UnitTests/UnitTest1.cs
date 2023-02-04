namespace mvdmio.CanaryTest.UnitTests;

public class Tests
{
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public void Test1()
    {
        var uri = new Uri("https://inzameling.jewel.eu");
        Console.WriteLine(uri.Authority);
    }
}