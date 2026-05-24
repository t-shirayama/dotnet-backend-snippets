using DotnetBackendSnippets.Api;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DotnetBackendSnippets.Tests.Api;

// テスト対象: API Samples のスニペット動作を確認する。
public sealed class ApiSamplesTests
{
    // テスト意図: Describe Create Todo Endpoint / Returns Minimal API Metadata を確認する。
    [Fact]
    public void DescribeCreateTodoEndpoint_ReturnsMinimalApiMetadata()
    {
        ApiEndpointDefinition endpoint = ApiSamples.DescribeCreateTodoEndpoint();

        Assert.Equal("POST", endpoint.HttpMethod);
        Assert.Equal("/todos", endpoint.Pattern);
        Assert.Equal("CreateTodo", endpoint.Name);
        Assert.Equal(CorsPolicyNames.InternalApi, endpoint.CorsPolicyName);
        Assert.Equal("createTodo", endpoint.OperationId);
        Assert.Contains("Todos", endpoint.Tags);
    }

    // テスト意図: Create Todo / Returns Created Response / When Request Is Valid を確認する。
    [Fact]
    public void CreateTodo_ReturnsCreatedResponse_WhenRequestIsValid()
    {
        var request = new CreateTodoRequest("  Write tests  ", new DateOnly(2026, 5, 24));

        ApiEndpointResult<TodoResponse> result = ApiSamples.CreateTodo(request, 10);

        Assert.True(result.Succeeded);
        Assert.Equal(StatusCodes.Status201Created, result.StatusCode);
        Assert.NotNull(result.Value);
        Assert.Equal(10, result.Value.Id);
        Assert.Equal("Write tests", result.Value.Title);
        Assert.Equal("open", result.Value.Status);
        Assert.Equal("/todos/10", result.Location);
        Assert.Null(result.Problem);
    }

    // テスト意図: Create Todo / Returns Validation Problem / When Request Is Invalid を確認する。
    [Fact]
    public void CreateTodo_ReturnsValidationProblem_WhenRequestIsInvalid()
    {
        var request = new CreateTodoRequest(" ", null);

        ApiEndpointResult<TodoResponse> result = ApiSamples.CreateTodo(request, 0);

        ValidationProblemDetails problem = Assert.IsType<ValidationProblemDetails>(result.Problem);

        Assert.False(result.Succeeded);
        Assert.Equal(StatusCodes.Status400BadRequest, result.StatusCode);
        Assert.Contains("title", problem.Errors.Keys);
        Assert.Contains("nextId", problem.Errors.Keys);
    }

    // テスト意図: ToActionResult / Preserves Status Code / When Endpoint Result Succeeded を確認する。
    [Fact]
    public void ToActionResult_PreservesStatusCode_WhenEndpointResultSucceeded()
    {
        var response = new TodoResponse(1, "Ship API", null, "open");
        ApiEndpointResult<TodoResponse> endpointResult = ApiEndpointResult<TodoResponse>.Success(
            StatusCodes.Status202Accepted,
            response);

        ActionResult<TodoResponse> actionResult = ApiSamples.ToActionResult(endpointResult);

        ObjectResult objectResult = Assert.IsType<ObjectResult>(actionResult.Result);

        Assert.Equal(StatusCodes.Status202Accepted, objectResult.StatusCode);
        Assert.Same(response, objectResult.Value);
    }

    // テスト意図: ToActionResult / Returns Created Result / When Location Is Available を確認する。
    [Fact]
    public void ToActionResult_ReturnsCreatedResult_WhenLocationIsAvailable()
    {
        var response = new TodoResponse(1, "Ship API", null, "open");
        ApiEndpointResult<TodoResponse> endpointResult = ApiEndpointResult<TodoResponse>.Created("/todos/1", response);

        ActionResult<TodoResponse> actionResult = ApiSamples.ToActionResult(endpointResult);

        CreatedResult createdResult = Assert.IsType<CreatedResult>(actionResult.Result);

        Assert.Equal(StatusCodes.Status201Created, createdResult.StatusCode);
        Assert.Equal("/todos/1", createdResult.Location);
        Assert.Same(response, createdResult.Value);
    }

    // テスト意図: ToActionResult / Returns Object Result / When Endpoint Result Failed を確認する。
    [Fact]
    public void ToActionResult_ReturnsObjectResult_WhenEndpointResultFailed()
    {
        ProblemDetails problem = ApiSamples.CreateProblem(
            StatusCodes.Status404NotFound,
            "Not found.",
            "Todo was not found.");
        ApiEndpointResult<TodoResponse> endpointResult = ApiEndpointResult<TodoResponse>.Failure(problem);

        ActionResult<TodoResponse> actionResult = ApiSamples.ToActionResult(endpointResult);

        ObjectResult objectResult = Assert.IsType<ObjectResult>(actionResult.Result);

        Assert.Equal(StatusCodes.Status404NotFound, objectResult.StatusCode);
        Assert.Same(problem, objectResult.Value);
    }

    // テスト意図: Create Problem / Returns RFC 7807 Shape を確認する。
    [Fact]
    public void CreateProblem_ReturnsRfc7807Shape()
    {
        ProblemDetails problem = ApiSamples.CreateProblem(
            StatusCodes.Status409Conflict,
            "Conflict.",
            "The todo was already completed.",
            instance: "/todos/1");

        Assert.Equal(StatusCodes.Status409Conflict, problem.Status);
        Assert.Equal("Conflict.", problem.Title);
        Assert.Equal("The todo was already completed.", problem.Detail);
        Assert.Equal("https://httpstatuses.com/409", problem.Type);
        Assert.Equal("/todos/1", problem.Instance);
    }

    // テスト意図: Create Validation Problem / Converts Validation Errors を確認する。
    [Fact]
    public void CreateValidationProblem_ConvertsValidationErrors()
    {
        Dictionary<string, IReadOnlyList<string>> errors = new(StringComparer.Ordinal)
        {
            ["title"] = ["Title is required."],
        };

        ValidationProblemDetails problem = ApiSamples.CreateValidationProblem(errors);

        Assert.Equal(StatusCodes.Status400BadRequest, problem.Status);
        Assert.Equal("One or more validation errors occurred.", problem.Title);
        Assert.Equal(["Title is required."], problem.Errors["title"]);
    }

    // テスト意図: Read Todo Route And Query / Returns Normalized Route And Query Values を確認する。
    [Fact]
    public void ReadTodoRouteAndQuery_ReturnsNormalizedRouteAndQueryValues()
    {
        ApiEndpointResult<TodoRouteQuery> result = ApiSamples.ReadTodoRouteAndQuery(
            5,
            " DONE ",
            pageNumber: 2,
            pageSize: 25);

        Assert.True(result.Succeeded);
        Assert.NotNull(result.Value);
        Assert.Equal(5, result.Value.Id);
        Assert.Equal("done", result.Value.Status);
        Assert.Equal(new PagingOptions(2, 25, 25, 25), result.Value.Paging);
    }

    // テスト意図: Read Todo Route And Query / Returns Validation Problem / When Values Are Invalid を確認する。
    [Fact]
    public void ReadTodoRouteAndQuery_ReturnsValidationProblem_WhenValuesAreInvalid()
    {
        ApiEndpointResult<TodoRouteQuery> result = ApiSamples.ReadTodoRouteAndQuery(
            0,
            "closed",
            pageNumber: 0,
            pageSize: 101);

        ValidationProblemDetails problem = Assert.IsType<ValidationProblemDetails>(result.Problem);

        Assert.Contains("id", problem.Errors.Keys);
        Assert.Contains("status", problem.Errors.Keys);
        Assert.Contains("pageNumber", problem.Errors.Keys);
        Assert.Contains("pageSize", problem.Errors.Keys);
    }

    // テスト意図: Create Paging Options / Returns Validation Problem / When Skip Would Overflow Int を確認する。
    [Fact]
    public void CreatePagingOptions_ReturnsValidationProblem_WhenSkipWouldOverflowInt()
    {
        ApiEndpointResult<PagingOptions> result = ApiSamples.CreatePagingOptions(int.MaxValue, 100);

        ValidationProblemDetails problem = Assert.IsType<ValidationProblemDetails>(result.Problem);

        Assert.Contains("pageNumber", problem.Errors.Keys);
    }

    // テスト意図: Map Domain Exception / Returns Problem With Domain Code を確認する。
    [Fact]
    public void MapDomainException_ReturnsProblemWithDomainCode()
    {
        var exception = new DomainException("todo.already_done", "Todo is already done.", StatusCodes.Status409Conflict);

        ProblemDetails problem = ApiSamples.MapDomainException(exception, "/todos/1");

        Assert.Equal(StatusCodes.Status409Conflict, problem.Status);
        Assert.Equal("Domain rule violation.", problem.Title);
        Assert.Equal("Todo is already done.", problem.Detail);
        Assert.Equal("/todos/1", problem.Instance);
        Assert.Equal("todo.already_done", problem.Extensions["code"]);
    }

    // テスト意図: Require Header / Returns Problem / When Header Is Missing を確認する。
    [Fact]
    public void RequireHeader_ReturnsProblem_WhenHeaderIsMissing()
    {
        Dictionary<string, string?> headers = new(StringComparer.OrdinalIgnoreCase)
        {
            ["X-Request-Id"] = " ",
        };

        ProblemDetails? problem = ApiSamples.RequireHeader(headers, "X-Request-Id");

        Assert.NotNull(problem);
        Assert.Equal(StatusCodes.Status400BadRequest, problem.Status);
    }

    // テスト意図: Require Header / Returns No Problem / When Dictionary Header Name Differs By Case を確認する。
    [Fact]
    public void RequireHeader_ReturnsNull_WhenDictionaryHeaderNameDiffersByCase()
    {
        Dictionary<string, string?> headers = new(StringComparer.Ordinal)
        {
            ["x-request-id"] = "request-123",
        };

        ProblemDetails? problem = ApiSamples.RequireHeader(headers, "X-Request-Id");

        Assert.Null(problem);
    }

    // テスト意図: Require Header / Returns No Problem / For ASP.NET Core Header Dictionary を確認する。
    [Fact]
    public void RequireHeader_ReturnsNull_ForAspNetCoreHeaderDictionary()
    {
        var headers = new HeaderDictionary
        {
            ["x-request-id"] = "request-123",
        };

        ProblemDetails? problem = ApiSamples.RequireHeader(headers, "X-Request-Id");

        Assert.Null(problem);
    }

    // テスト意図: Should Short Circuit For Maintenance / Excludes Health Endpoint を確認する。
    [Fact]
    public void ShouldShortCircuitForMaintenance_ExcludesHealthEndpoint()
    {
        Assert.True(ApiSamples.ShouldShortCircuitForMaintenance(maintenanceEnabled: true, "/todos"));
        Assert.False(ApiSamples.ShouldShortCircuitForMaintenance(maintenanceEnabled: true, "/health"));
        Assert.False(ApiSamples.ShouldShortCircuitForMaintenance(maintenanceEnabled: false, "/todos"));
    }

    // テスト意図: Is Known CORS Policy / Returns Whether Policy Name Is Supported を確認する。
    [Theory]
    [InlineData(CorsPolicyNames.PublicReadOnly, true)]
    [InlineData(CorsPolicyNames.InternalApi, true)]
    [InlineData("unknown", false)]
    public void IsKnownCorsPolicy_ReturnsWhetherPolicyNameIsSupported(string policyName, bool expected)
    {
        bool result = ApiSamples.IsKnownCorsPolicy(policyName);

        Assert.Equal(expected, result);
    }
}
