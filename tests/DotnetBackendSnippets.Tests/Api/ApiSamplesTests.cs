using DotnetBackendSnippets.Api;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DotnetBackendSnippets.Tests.Api;

public sealed class ApiSamplesTests
{
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
        Assert.Null(result.Problem);
    }

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

    [Fact]
    public void ToActionResult_ReturnsValue_WhenEndpointResultSucceeded()
    {
        var response = new TodoResponse(1, "Ship API", null, "open");
        ApiEndpointResult<TodoResponse> endpointResult = ApiEndpointResult<TodoResponse>.Success(
            StatusCodes.Status200OK,
            response);

        ActionResult<TodoResponse> actionResult = ApiSamples.ToActionResult(endpointResult);

        Assert.Null(actionResult.Result);
        Assert.Equal(response, actionResult.Value);
    }

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

    [Fact]
    public void CreatePagingOptions_ReturnsValidationProblem_WhenSkipWouldOverflowInt()
    {
        ApiEndpointResult<PagingOptions> result = ApiSamples.CreatePagingOptions(int.MaxValue, 100);

        ValidationProblemDetails problem = Assert.IsType<ValidationProblemDetails>(result.Problem);

        Assert.Contains("pageNumber", problem.Errors.Keys);
    }

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

    [Fact]
    public void ShouldShortCircuitForMaintenance_ExcludesHealthEndpoint()
    {
        Assert.True(ApiSamples.ShouldShortCircuitForMaintenance(maintenanceEnabled: true, "/todos"));
        Assert.False(ApiSamples.ShouldShortCircuitForMaintenance(maintenanceEnabled: true, "/health"));
        Assert.False(ApiSamples.ShouldShortCircuitForMaintenance(maintenanceEnabled: false, "/todos"));
    }

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
