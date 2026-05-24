using DotnetBackendSnippets.Security;
using Microsoft.AspNetCore.DataProtection;

namespace DotnetBackendSnippets.Tests.Security;

// テスト対象: Data Protection Samples のスニペット動作を確認する。
public sealed class DataProtectionSamplesTests
{
    // テスト意図: Protect String / Round Trips Protected Value を確認する。
    [Fact]
    public void ProtectString_RoundTripsProtectedValue()
    {
        string directory = Path.Combine(Path.GetTempPath(), $"dp-{Guid.NewGuid():N}");
        Directory.CreateDirectory(directory);

        try
        {
            IDataProtectionProvider provider = DataProtectionProvider.Create(new DirectoryInfo(directory));
            IDataProtector protector = DataProtectionSamples.CreateProtector(provider, "tokens.refresh");

            string protectedValue = DataProtectionSamples.ProtectString(protector, "refresh-token");
            string actual = DataProtectionSamples.UnprotectString(protector, protectedValue);

            Assert.NotEqual("refresh-token", protectedValue);
            Assert.Equal("refresh-token", actual);
        }
        finally
        {
            Directory.Delete(directory, recursive: true);
        }
    }

    // テスト意図: Get Required Secret Async / Returns Secret From Provider を確認する。
    [Fact]
    public async Task GetRequiredSecretAsync_ReturnsSecretFromProvider()
    {
        var provider = new DictionarySecretProvider(new Dictionary<string, string?>(StringComparer.Ordinal)
        {
            ["ConnectionStrings:Default"] = "Data Source=orders.db",
        });

        string secret = await DataProtectionSamples.GetRequiredSecretAsync(provider, "ConnectionStrings:Default");

        Assert.Equal("Data Source=orders.db", secret);
    }

    private sealed class DictionarySecretProvider(IReadOnlyDictionary<string, string?> secrets) : ISecretProvider
    {
        public Task<string?> GetSecretAsync(string name, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(secrets.GetValueOrDefault(name));
        }
    }
}
