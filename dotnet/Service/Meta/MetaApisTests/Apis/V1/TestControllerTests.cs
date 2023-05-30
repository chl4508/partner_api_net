using Colorverse.ApisTests.Server;
using System.Reflection;
using Xunit;

namespace Colorverse.Meta.Apis.V1.Tests;

/// <summary>
/// 
/// </summary>
public class TestControllerTests
{
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task Uuid_Create_OK()
    {
        var testServer = new TestApiServer();
        testServer.AddControllers(Assembly.GetAssembly(typeof(TestController)));

        using var client = testServer.GetClient();

        var response = await client.GetAsync("/v1/meta/test/uuid?domainType=UserAccount");
        response.EnsureSuccessStatusCode();
        var result = await response.ReadContentAsJsonElementAsync();

        var uuid = result.GetProperty("uuid").GetString();
        Assert.True(!string.IsNullOrWhiteSpace(uuid));
    }

}
