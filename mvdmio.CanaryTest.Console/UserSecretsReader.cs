using Microsoft.Extensions.Configuration;

namespace mvdmio.CanaryTest.Console;

public static class UserSecretsReader
{
    public static T? Read<T>(string sectionName)
    {
        var builder = new ConfigurationBuilder()
            .AddUserSecrets<Program>();
        
        var configuration = builder.Build();
        return configuration.GetSection(sectionName).Get<T>();
    }
}