using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DotnetBackendSnippets.Api;

/// <summary>
/// API エンドポイントの説明情報を表します。
/// </summary>
/// <param name="HttpMethod">HTTP メソッド。</param>
/// <param name="Pattern">ルートパターン。</param>
/// <param name="Name">エンドポイント名。</param>
/// <param name="CorsPolicyName">適用する CORS ポリシー名。</param>
/// <param name="OperationId">OpenAPI の operationId。</param>
/// <param name="Summary">エンドポイントの概要。</param>
/// <param name="Tags">OpenAPI タグ。</param>
public sealed record ApiEndpointDefinition(
    string HttpMethod,
    string Pattern,
    string Name,
    string CorsPolicyName,
    string OperationId,
    string Summary,
    IReadOnlyList<string> Tags);

/// <summary>
/// Todo 作成リクエストを表します。
/// </summary>
/// <param name="Title">Todo のタイトル。</param>
/// <param name="DueDate">期限日。</param>
public sealed record CreateTodoRequest(string? Title, DateOnly? DueDate);

/// <summary>
/// Todo レスポンスを表します。
/// </summary>
/// <param name="Id">Todo ID。</param>
/// <param name="Title">Todo のタイトル。</param>
/// <param name="DueDate">期限日。</param>
/// <param name="Status">Todo の状態。</param>
public sealed record TodoResponse(int Id, string Title, DateOnly? DueDate, string Status);

/// <summary>
/// ページングに使う値を表します。
/// </summary>
/// <param name="PageNumber">ページ番号。</param>
/// <param name="PageSize">ページサイズ。</param>
/// <param name="Skip">読み飛ばす件数。</param>
/// <param name="Take">取得する件数。</param>
public sealed record PagingOptions(int PageNumber, int PageSize, int Skip, int Take);

/// <summary>
/// Todo 取得のルート値とクエリ値を表します。
/// </summary>
/// <param name="Id">Todo ID。</param>
/// <param name="Status">状態フィルター。</param>
/// <param name="Paging">ページング設定。</param>
public sealed record TodoRouteQuery(int Id, string? Status, PagingOptions Paging);

/// <summary>
/// API 処理の成功または失敗を表します。
/// </summary>
/// <typeparam name="T">成功時の値の型。</typeparam>
/// <param name="StatusCode">HTTP ステータスコード。</param>
/// <param name="Value">成功時の値。</param>
/// <param name="Problem">失敗時の Problem Details。</param>
public sealed record ApiEndpointResult<T>(int StatusCode, T? Value, ProblemDetails? Problem)
{
    /// <summary>
    /// 処理が成功したかどうかを取得します。
    /// </summary>
    /// <value>Problem Details がない場合は <see langword="true"/>。</value>
    public bool Succeeded => Problem is null;

    /// <summary>
    /// 成功結果を作成します。
    /// </summary>
    /// <param name="statusCode">HTTP ステータスコード。</param>
    /// <param name="value">成功時の値。</param>
    /// <returns>成功を表す結果。</returns>
    public static ApiEndpointResult<T> Success(int statusCode, T value)
    {
        return new ApiEndpointResult<T>(statusCode, value, null);
    }

    /// <summary>
    /// 失敗結果を作成します。
    /// </summary>
    /// <param name="problem">失敗内容を表す Problem Details。</param>
    /// <returns>失敗を表す結果。</returns>
    public static ApiEndpointResult<T> Failure(ProblemDetails problem)
    {
        return new ApiEndpointResult<T>(
            problem.Status ?? StatusCodes.Status500InternalServerError,
            default,
            problem);
    }
}

/// <summary>
/// ドメインルール違反を表す例外です。
/// </summary>
public sealed class DomainException : Exception
{
    /// <summary>
    /// <see cref="DomainException"/> クラスの新しいインスタンスを初期化します。
    /// </summary>
    /// <param name="code">業務エラーコード。</param>
    /// <param name="message">エラーメッセージ。</param>
    /// <param name="statusCode">対応する HTTP ステータスコード。</param>
    public DomainException(string code, string message, int statusCode)
        : base(message)
    {
        Code = code;
        StatusCode = statusCode;
    }

    /// <summary>
    /// 業務エラーコードを取得します。
    /// </summary>
    /// <value>クライアントへ返す識別用コード。</value>
    public string Code { get; }

    /// <summary>
    /// 対応する HTTP ステータスコードを取得します。
    /// </summary>
    /// <value>Problem Details に設定するステータスコード。</value>
    public int StatusCode { get; }
}

/// <summary>
/// CORS ポリシー名を集約します。
/// </summary>
public static class CorsPolicyNames
{
    /// <summary>
    /// 公開読み取り専用 API 向けの CORS ポリシー名です。
    /// </summary>
    public const string PublicReadOnly = "public-read-only";

    /// <summary>
    /// 内部 API 向けの CORS ポリシー名です。
    /// </summary>
    public const string InternalApi = "internal-api";
}

/// <summary>
/// ASP.NET Core API でよく使う処理のサンプルです。
/// </summary>
public static class ApiSamples
{
    private const int DefaultPageSize = 20;
    private const int MaxPageSize = 100;

    private static readonly HashSet<string> AllowedStatuses = new(StringComparer.OrdinalIgnoreCase)
    {
        "open",
        "done",
    };

    private static readonly HashSet<string> KnownCorsPolicyNames = new(StringComparer.Ordinal)
    {
        CorsPolicyNames.PublicReadOnly,
        CorsPolicyNames.InternalApi,
    };

    /// <summary>
    /// Todo 作成エンドポイントの説明情報を返します。
    /// </summary>
    /// <returns>エンドポイント定義。</returns>
    public static ApiEndpointDefinition DescribeCreateTodoEndpoint()
    {
        return new ApiEndpointDefinition(
            "POST",
            "/todos",
            "CreateTodo",
            CorsPolicyNames.InternalApi,
            "createTodo",
            "Create a todo item.",
            ["Todos"]);
    }

    /// <summary>
    /// Todo 作成リクエストを検証し、レスポンスを作成します。
    /// </summary>
    /// <param name="request">作成リクエスト。</param>
    /// <param name="nextId">採番済みの次 ID。</param>
    /// <returns>成功または検証エラーの結果。</returns>
    /// <exception cref="ArgumentNullException"><paramref name="request"/> が <see langword="null"/> の場合。</exception>
    public static ApiEndpointResult<TodoResponse> CreateTodo(CreateTodoRequest request, int nextId)
    {
        ArgumentNullException.ThrowIfNull(request);

        Dictionary<string, IReadOnlyList<string>> errors = ValidateCreateTodo(request);

        if (nextId <= 0)
        {
            errors["nextId"] = ["The next id must be positive."];
        }

        if (errors.Count > 0)
        {
            return ApiEndpointResult<TodoResponse>.Failure(CreateValidationProblem(errors));
        }

        var response = new TodoResponse(
            nextId,
            request.Title!.Trim(),
            request.DueDate,
            "open");

        return ApiEndpointResult<TodoResponse>.Success(StatusCodes.Status201Created, response);
    }

    /// <summary>
    /// API 結果を MVC の <see cref="ActionResult{TValue}"/> に変換します。
    /// </summary>
    /// <typeparam name="T">レスポンス値の型。</typeparam>
    /// <param name="result">API 処理結果。</param>
    /// <returns>MVC で返せるアクション結果。</returns>
    /// <exception cref="ArgumentNullException"><paramref name="result"/> が <see langword="null"/> の場合。</exception>
    public static ActionResult<T> ToActionResult<T>(ApiEndpointResult<T> result)
    {
        ArgumentNullException.ThrowIfNull(result);

        if (result.Succeeded && result.Value is not null)
        {
            return new ActionResult<T>(result.Value);
        }

        return new ObjectResult(result.Problem)
        {
            StatusCode = result.StatusCode,
        };
    }

    /// <summary>
    /// 標準的な Problem Details を作成します。
    /// </summary>
    /// <param name="statusCode">HTTP ステータスコード。</param>
    /// <param name="title">エラーの短いタイトル。</param>
    /// <param name="detail">エラーの詳細。</param>
    /// <param name="type">エラー種別 URI。</param>
    /// <param name="instance">発生箇所を示す URI。</param>
    /// <returns>Problem Details。</returns>
    public static ProblemDetails CreateProblem(
        int statusCode,
        string title,
        string detail,
        string? type = null,
        string? instance = null)
    {
        return new ProblemDetails
        {
            Status = statusCode,
            Title = title,
            Detail = detail,
            Type = type ?? $"https://httpstatuses.com/{statusCode}",
            Instance = instance,
        };
    }

    /// <summary>
    /// 検証エラー用の Problem Details を作成します。
    /// </summary>
    /// <param name="errors">フィールド別のエラー一覧。</param>
    /// <returns>検証エラーを表す Problem Details。</returns>
    /// <exception cref="ArgumentNullException"><paramref name="errors"/> が <see langword="null"/> の場合。</exception>
    public static ValidationProblemDetails CreateValidationProblem(
        IReadOnlyDictionary<string, IReadOnlyList<string>> errors)
    {
        ArgumentNullException.ThrowIfNull(errors);

        var convertedErrors = errors.ToDictionary(
            pair => pair.Key,
            pair => pair.Value.ToArray(),
            StringComparer.Ordinal);

        return new ValidationProblemDetails(convertedErrors)
        {
            Status = StatusCodes.Status400BadRequest,
            Title = "One or more validation errors occurred.",
            Type = "https://httpstatuses.com/400",
        };
    }

    /// <summary>
    /// ルート値とクエリ値を検証して読み取ります。
    /// </summary>
    /// <param name="id">ルートの Todo ID。</param>
    /// <param name="status">状態クエリ。</param>
    /// <param name="pageNumber">ページ番号。</param>
    /// <param name="pageSize">ページサイズ。</param>
    /// <returns>読み取った値または検証エラーの結果。</returns>
    public static ApiEndpointResult<TodoRouteQuery> ReadTodoRouteAndQuery(
        int id,
        string? status,
        int pageNumber = 1,
        int pageSize = DefaultPageSize)
    {
        Dictionary<string, IReadOnlyList<string>> errors = [];

        if (id <= 0)
        {
            errors["id"] = ["Route parameter 'id' must be positive."];
        }

        string? normalizedStatus = string.IsNullOrWhiteSpace(status)
            ? null
            : status.Trim().ToLowerInvariant();

        if (normalizedStatus is not null && !AllowedStatuses.Contains(normalizedStatus))
        {
            errors["status"] = ["Query parameter 'status' must be 'open' or 'done'."];
        }

        ApiEndpointResult<PagingOptions> pagingResult = CreatePagingOptions(pageNumber, pageSize);

        if (!pagingResult.Succeeded && pagingResult.Problem is ValidationProblemDetails pagingProblem)
        {
            foreach (KeyValuePair<string, string[]> error in pagingProblem.Errors)
            {
                errors[error.Key] = error.Value;
            }
        }

        if (errors.Count > 0)
        {
            return ApiEndpointResult<TodoRouteQuery>.Failure(CreateValidationProblem(errors));
        }

        var query = new TodoRouteQuery(id, normalizedStatus, pagingResult.Value!);

        return ApiEndpointResult<TodoRouteQuery>.Success(StatusCodes.Status200OK, query);
    }

    /// <summary>
    /// ページング設定を検証して作成します。
    /// </summary>
    /// <param name="pageNumber">ページ番号。</param>
    /// <param name="pageSize">ページサイズ。</param>
    /// <returns>ページング設定または検証エラーの結果。</returns>
    public static ApiEndpointResult<PagingOptions> CreatePagingOptions(int pageNumber, int pageSize)
    {
        Dictionary<string, IReadOnlyList<string>> errors = [];

        if (pageNumber <= 0)
        {
            errors["pageNumber"] = ["Query parameter 'pageNumber' must be positive."];
        }

        if (pageSize is <= 0 or > MaxPageSize)
        {
            errors["pageSize"] = [$"Query parameter 'pageSize' must be between 1 and {MaxPageSize}."];
        }

        long skip = ((long)pageNumber - 1) * pageSize;

        if (skip > int.MaxValue)
        {
            errors["pageNumber"] = ["The requested page is too large."];
        }

        if (errors.Count > 0)
        {
            return ApiEndpointResult<PagingOptions>.Failure(CreateValidationProblem(errors));
        }

        var paging = new PagingOptions(pageNumber, pageSize, (int)skip, pageSize);

        return ApiEndpointResult<PagingOptions>.Success(StatusCodes.Status200OK, paging);
    }

    /// <summary>
    /// ドメイン例外を Problem Details に変換します。
    /// </summary>
    /// <param name="exception">変換するドメイン例外。</param>
    /// <param name="instance">発生箇所を示す URI。</param>
    /// <returns>Problem Details。</returns>
    /// <exception cref="ArgumentNullException"><paramref name="exception"/> が <see langword="null"/> の場合。</exception>
    public static ProblemDetails MapDomainException(DomainException exception, string? instance = null)
    {
        ArgumentNullException.ThrowIfNull(exception);

        ProblemDetails problem = CreateProblem(
            exception.StatusCode,
            "Domain rule violation.",
            exception.Message,
            instance: instance);

        problem.Extensions["code"] = exception.Code;

        return problem;
    }

    /// <summary>
    /// 必須ヘッダーが存在するか検証します。
    /// </summary>
    /// <param name="headers">ヘッダー一覧。</param>
    /// <param name="headerName">必須ヘッダー名。</param>
    /// <returns>不足時は Problem Details、存在時は <see langword="null"/>。</returns>
    /// <exception cref="ArgumentNullException"><paramref name="headers"/> が <see langword="null"/> の場合。</exception>
    public static ProblemDetails? RequireHeader(
        IReadOnlyDictionary<string, string?> headers,
        string headerName)
    {
        ArgumentNullException.ThrowIfNull(headers);

        return headers.TryGetValue(headerName, out string? value) && !string.IsNullOrWhiteSpace(value)
            ? null
            : CreateProblem(
                StatusCodes.Status400BadRequest,
                "Missing required header.",
                $"Header '{headerName}' is required.");
    }

    /// <summary>
    /// メンテナンス中にリクエストを短絡すべきか判定します。
    /// </summary>
    /// <param name="maintenanceEnabled">メンテナンス中かどうか。</param>
    /// <param name="requestPath">リクエストパス。</param>
    /// <returns>短絡する場合は <see langword="true"/>。</returns>
    public static bool ShouldShortCircuitForMaintenance(bool maintenanceEnabled, string requestPath)
    {
        return maintenanceEnabled
            && !string.Equals(requestPath, "/health", StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// CORS ポリシー名が既知かどうかを判定します。
    /// </summary>
    /// <param name="policyName">確認する CORS ポリシー名。</param>
    /// <returns>既知のポリシー名なら <see langword="true"/>。</returns>
    public static bool IsKnownCorsPolicy(string policyName)
    {
        return KnownCorsPolicyNames.Contains(policyName);
    }

    private static Dictionary<string, IReadOnlyList<string>> ValidateCreateTodo(CreateTodoRequest request)
    {
        Dictionary<string, IReadOnlyList<string>> errors = [];

        if (string.IsNullOrWhiteSpace(request.Title))
        {
            errors["title"] = ["Title is required."];
        }
        else if (request.Title.Trim().Length > 100)
        {
            errors["title"] = ["Title must be 100 characters or fewer."];
        }

        return errors;
    }
}
