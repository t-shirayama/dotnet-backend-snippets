using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DotnetBackendSnippets.Api;

public sealed record ApiEndpointDefinition(
    string HttpMethod,
    string Pattern,
    string Name,
    string CorsPolicyName,
    string OperationId,
    string Summary,
    IReadOnlyList<string> Tags);

public sealed record CreateTodoRequest(string? Title, DateOnly? DueDate);

public sealed record TodoResponse(int Id, string Title, DateOnly? DueDate, string Status);

public sealed record PagingOptions(int PageNumber, int PageSize, int Skip, int Take);

public sealed record TodoRouteQuery(int Id, string? Status, PagingOptions Paging);

public sealed record ApiEndpointResult<T>(int StatusCode, T? Value, ProblemDetails? Problem)
{
    public bool Succeeded => Problem is null;

    public static ApiEndpointResult<T> Success(int statusCode, T value)
    {
        return new ApiEndpointResult<T>(statusCode, value, null);
    }

    public static ApiEndpointResult<T> Failure(ProblemDetails problem)
    {
        return new ApiEndpointResult<T>(
            problem.Status ?? StatusCodes.Status500InternalServerError,
            default,
            problem);
    }
}

public sealed class DomainException : Exception
{
    public DomainException(string code, string message, int statusCode)
        : base(message)
    {
        Code = code;
        StatusCode = statusCode;
    }

    public string Code { get; }

    public int StatusCode { get; }
}

public static class CorsPolicyNames
{
    public const string PublicReadOnly = "public-read-only";
    public const string InternalApi = "internal-api";
}

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

    public static bool ShouldShortCircuitForMaintenance(bool maintenanceEnabled, string requestPath)
    {
        return maintenanceEnabled
            && !string.Equals(requestPath, "/health", StringComparison.OrdinalIgnoreCase);
    }

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
