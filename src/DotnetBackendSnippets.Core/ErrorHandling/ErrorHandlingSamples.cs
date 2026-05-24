using System.Globalization;

namespace DotnetBackendSnippets.ErrorHandling;

/// <summary>
/// 例外を使わずに操作結果を返すための値を表します。
/// </summary>
/// <typeparam name="T">成功時の値の型。</typeparam>
/// <param name="Succeeded">操作が成功したかどうか。</param>
/// <param name="Value">成功時の値。</param>
/// <param name="Error">失敗時のエラーメッセージ。</param>
public sealed record OperationResult<T>(bool Succeeded, T? Value, string? Error)
{
    /// <summary>
    /// 成功結果を作成します。
    /// </summary>
    /// <param name="value">成功時の値。</param>
    /// <returns>成功を表す結果。</returns>
    public static OperationResult<T> Success(T value) => new(true, value, null);

    /// <summary>
    /// 失敗結果を作成します。
    /// </summary>
    /// <param name="error">エラーメッセージ。</param>
    /// <returns>失敗を表す結果。</returns>
    public static OperationResult<T> Failure(string error) => new(false, default, error);
}

/// <summary>
/// エラー処理の基本的な実装例を提供します。
/// </summary>
public static class ErrorHandlingSamples
{
    /// <summary>
    /// 文字列を正の整数として解析します。
    /// </summary>
    /// <param name="value">解析する文字列。</param>
    /// <returns>解析結果。成功時は正の整数を含みます。</returns>
    public static OperationResult<int> TryParsePositiveInt(string value)
    {
        if (!int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var number))
        {
            return OperationResult<int>.Failure("Value must be an integer.");
        }

        if (number <= 0)
        {
            return OperationResult<int>.Failure("Value must be positive.");
        }

        return OperationResult<int>.Success(number);
    }

    /// <summary>
    /// 状態が不正な場合に例外を投げます。
    /// </summary>
    /// <param name="isValid">状態が有効かどうか。</param>
    /// <exception cref="InvalidOperationException"><paramref name="isValid"/> が <see langword="false"/> の場合。</exception>
    public static void ThrowIfInvalidState(bool isValid)
    {
        if (!isValid)
        {
            throw new InvalidOperationException("The current state is invalid.");
        }
    }
}
