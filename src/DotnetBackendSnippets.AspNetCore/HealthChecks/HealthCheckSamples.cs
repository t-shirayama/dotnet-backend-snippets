using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace DotnetBackendSnippets.HealthChecks;

/// <summary>
/// Kubernetes などから呼ぶ health endpoint の path を表します。
/// </summary>
/// <param name="Liveness">liveness endpoint。</param>
/// <param name="Readiness">readiness endpoint。</param>
public sealed record HealthEndpointPaths(string Liveness, string Readiness);

/// <summary>
/// 依存先の probe を表します。
/// </summary>
public interface IDependencyProbe
{
    /// <summary>
    /// 依存先名を取得します。
    /// </summary>
    /// <value>DB や外部 API などの名前。</value>
    string Name { get; }

    /// <summary>
    /// 依存先が利用可能か確認します。
    /// </summary>
    /// <param name="cancellationToken">キャンセル通知。</param>
    /// <returns>利用可能なら <see langword="true"/>。</returns>
    Task<bool> IsAvailableAsync(CancellationToken cancellationToken);
}

/// <summary>
/// 関数で依存先を確認する health check probe です。
/// </summary>
public sealed record DelegateDependencyProbe : IDependencyProbe
{
    /// <summary>
    /// <see cref="DelegateDependencyProbe"/> レコードの新しいインスタンスを初期化します。
    /// </summary>
    /// <param name="Name">依存先名。</param>
    /// <param name="CheckAsync">確認処理。</param>
    /// <exception cref="ArgumentException"><paramref name="Name"/> が空白の場合。</exception>
    /// <exception cref="ArgumentNullException"><paramref name="CheckAsync"/> が <see langword="null"/> の場合。</exception>
    public DelegateDependencyProbe(string Name, Func<CancellationToken, Task<bool>> CheckAsync)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(Name);
        ArgumentNullException.ThrowIfNull(CheckAsync);

        this.Name = Name;
        this.CheckAsync = CheckAsync;
    }

    /// <inheritdoc />
    public string Name { get; }

    /// <summary>
    /// 依存先を確認する処理を取得します。
    /// </summary>
    /// <value>キャンセル通知を受け取り利用可否を返す関数。</value>
    public Func<CancellationToken, Task<bool>> CheckAsync { get; }

    /// <inheritdoc />
    public Task<bool> IsAvailableAsync(CancellationToken cancellationToken)
    {
        return CheckAsync(cancellationToken);
    }
}

/// <summary>
/// health check / readiness のサンプルです。
/// </summary>
public static class HealthCheckSamples
{
    /// <summary>
    /// Kubernetes 向けの liveness / readiness path を作成します。
    /// </summary>
    /// <param name="basePath">health endpoint の base path。</param>
    /// <returns>liveness / readiness path。</returns>
    /// <exception cref="ArgumentException"><paramref name="basePath"/> が空白の場合。</exception>
    public static HealthEndpointPaths CreateKubernetesHealthEndpointPaths(string basePath = "/health")
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(basePath);

        string trimmedPath = basePath.Trim().Trim('/');
        string normalized = string.IsNullOrEmpty(trimmedPath)
            ? string.Empty
            : $"/{trimmedPath}";

        return new HealthEndpointPaths($"{normalized}/live", $"{normalized}/ready");
    }

    /// <summary>
    /// プロセスが起動していることだけを見る liveness 結果を作成します。
    /// </summary>
    /// <returns>healthy の health check result。</returns>
    public static HealthCheckResult CreateLivenessResult()
    {
        return HealthCheckResult.Healthy("Application process is alive.");
    }

    /// <summary>
    /// 依存先の状態から readiness 結果を作成します。
    /// </summary>
    /// <param name="dependencyResults">依存先ごとの health check 結果。</param>
    /// <returns>readiness 結果。</returns>
    /// <exception cref="ArgumentNullException"><paramref name="dependencyResults"/> が <see langword="null"/> の場合。</exception>
    public static HealthCheckResult CreateReadinessResult(
        IReadOnlyDictionary<string, HealthCheckResult> dependencyResults)
    {
        ArgumentNullException.ThrowIfNull(dependencyResults);

        Dictionary<string, object> data = dependencyResults.ToDictionary(
            pair => pair.Key,
            pair => (object)pair.Value.Status.ToString(),
            StringComparer.Ordinal);

        if (dependencyResults.Values.Any(result => result.Status == HealthStatus.Unhealthy))
        {
            return HealthCheckResult.Unhealthy("One or more required dependencies are unhealthy.", data: data);
        }

        if (dependencyResults.Values.Any(result => result.Status == HealthStatus.Degraded))
        {
            return HealthCheckResult.Degraded("One or more dependencies are degraded.", data: data);
        }

        return HealthCheckResult.Healthy("Application is ready.", data);
    }

    /// <summary>
    /// DB や外部 API のような依存先 probe を health check result に変換します。
    /// </summary>
    /// <param name="probe">依存先 probe。</param>
    /// <param name="degradedWhenUnavailable">利用不可を degraded として扱う場合は <see langword="true"/>。</param>
    /// <param name="cancellationToken">キャンセル通知。</param>
    /// <returns>依存先の health check result。</returns>
    /// <exception cref="ArgumentNullException"><paramref name="probe"/> が <see langword="null"/> の場合。</exception>
    public static async Task<HealthCheckResult> CheckDependencyAsync(
        IDependencyProbe probe,
        bool degradedWhenUnavailable = false,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(probe);

        try
        {
            bool isAvailable = await probe.IsAvailableAsync(cancellationToken);

            if (isAvailable)
            {
                return HealthCheckResult.Healthy($"{probe.Name} is available.");
            }

            return degradedWhenUnavailable
                ? HealthCheckResult.Degraded($"{probe.Name} is unavailable.")
                : HealthCheckResult.Unhealthy($"{probe.Name} is unavailable.");
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            throw;
        }
        catch (Exception exception)
        {
            return degradedWhenUnavailable
                ? HealthCheckResult.Degraded($"{probe.Name} check failed.", exception)
                : HealthCheckResult.Unhealthy($"{probe.Name} check failed.", exception);
        }
    }
}
