using System.Text.RegularExpressions;
using Microsoft.Extensions.Hosting;

namespace DotnetBackendSnippets.Deployment;

/// <summary>
/// Dockerfile の生成設定を表します。
/// </summary>
/// <param name="AssemblyName">起動する assembly 名。</param>
/// <param name="Port">公開ポート。</param>
/// <param name="UserId">non-root 実行に使う user id。</param>
public sealed record DockerfileSpec(string AssemblyName, int Port = 8080, int UserId = 64198);

/// <summary>
/// デプロイ・運用に関する小さなサンプルです。
/// </summary>
public static partial class DeploymentSamples
{
    private const string LineFeed = "\n";

    /// <summary>
    /// ASP.NET Core アプリ向けの non-root Dockerfile を生成します。
    /// </summary>
    /// <param name="spec">Dockerfile 設定。</param>
    /// <returns>Dockerfile の内容。</returns>
    /// <exception cref="ArgumentNullException"><paramref name="spec"/> が <see langword="null"/> の場合。</exception>
    /// <exception cref="ArgumentException">assembly 名が空白、または許可されない文字を含む場合。</exception>
    /// <exception cref="ArgumentOutOfRangeException">port または user id が不正な場合。</exception>
    public static string CreateAspNetCoreDockerfile(DockerfileSpec spec)
    {
        ArgumentNullException.ThrowIfNull(spec);
        ArgumentException.ThrowIfNullOrWhiteSpace(spec.AssemblyName);

        if (!DockerAssemblyNameRegex().IsMatch(spec.AssemblyName))
        {
            throw new ArgumentException("Assembly name must contain only letters, digits, dot, hyphen, or underscore.", nameof(spec));
        }

        if (spec.Port <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(spec), "Port must be positive.");
        }

        if (spec.UserId <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(spec), "User id must be positive.");
        }

        string[] lines =
        [
            "FROM mcr.microsoft.com/dotnet/aspnet:8.0",
            "WORKDIR /app",
            $"RUN adduser --disabled-password --uid {spec.UserId} appuser",
            "COPY ./publish/ ./",
            $"ENV ASPNETCORE_URLS=http://+:{spec.Port}",
            $"EXPOSE {spec.Port}",
            "USER appuser",
            $"ENTRYPOINT [\"dotnet\", \"{spec.AssemblyName}.dll\"]",
        ];

        return string.Join(LineFeed, lines);
    }

    /// <summary>
    /// docker compose の service 定義を生成します。
    /// </summary>
    /// <param name="serviceName">service 名。</param>
    /// <param name="imageName">image 名。</param>
    /// <param name="environmentVariables">環境変数。</param>
    /// <returns>compose service の YAML 断片。</returns>
    /// <exception cref="ArgumentNullException"><paramref name="environmentVariables"/> が <see langword="null"/> の場合。</exception>
    /// <exception cref="ArgumentException"><paramref name="serviceName"/> または <paramref name="imageName"/> が空白、または service 名が許可されない形式の場合。</exception>
    public static string CreateComposeService(
        string serviceName,
        string imageName,
        IReadOnlyDictionary<string, string> environmentVariables)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(serviceName);
        ArgumentException.ThrowIfNullOrWhiteSpace(imageName);
        ArgumentNullException.ThrowIfNull(environmentVariables);

        if (!ComposeServiceNameRegex().IsMatch(serviceName))
        {
            throw new ArgumentException("Service name must contain only letters, digits, dot, hyphen, or underscore.", nameof(serviceName));
        }

        var lines = new List<string>
        {
            "services:",
            $"  {serviceName}:",
            $"    image: {imageName}",
            "    environment:",
        };

        foreach (KeyValuePair<string, string> variable in environmentVariables.OrderBy(pair => pair.Key, StringComparer.Ordinal))
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(variable.Key, nameof(environmentVariables));
            ArgumentNullException.ThrowIfNull(variable.Value, nameof(environmentVariables));

            lines.Add($"      {variable.Key}: \"{EscapeYamlDoubleQuotedValue(variable.Value)}\"");
        }

        return string.Join(LineFeed, lines);
    }

    /// <summary>
    /// shutdown timeout を持つ HostOptions を作成します。
    /// </summary>
    /// <param name="shutdownTimeout">graceful shutdown の最大待機時間。</param>
    /// <returns>HostOptions。</returns>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="shutdownTimeout"/> が 0 以下の場合。</exception>
    public static HostOptions CreateGracefulShutdownOptions(TimeSpan shutdownTimeout)
    {
        if (shutdownTimeout <= TimeSpan.Zero)
        {
            throw new ArgumentOutOfRangeException(nameof(shutdownTimeout), "Shutdown timeout must be positive.");
        }

        return new HostOptions { ShutdownTimeout = shutdownTimeout };
    }

    private static string EscapeYamlDoubleQuotedValue(string value)
    {
        return value.Replace("\\", "\\\\", StringComparison.Ordinal)
            .Replace("\"", "\\\"", StringComparison.Ordinal)
            .Replace("\r", "\\r", StringComparison.Ordinal)
            .Replace("\n", "\\n", StringComparison.Ordinal);
    }

    [GeneratedRegex("^[A-Za-z0-9_.-]+$")]
    private static partial Regex ComposeServiceNameRegex();

    [GeneratedRegex("^[A-Za-z0-9_.-]+$")]
    private static partial Regex DockerAssemblyNameRegex();
}
