using System.Security.Claims;
using System.Text;
using DotnetBackendSnippets.Authentication;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace DotnetBackendSnippets.Tests.Authentication;

// テスト対象: Authentication Authorization Samples のスニペット動作を確認する。
public sealed class AuthenticationAuthorizationSamplesTests
{
    // テスト意図: Add JWT Bearer Authentication / Configures Default Scheme And Validation Parameters を確認する。
    [Fact]
    public void AddJwtBearerAuthentication_ConfiguresDefaultSchemeAndValidationParameters()
    {
        var services = new ServiceCollection();
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("0123456789abcdef0123456789abcdef"));

        services.AddJwtBearerAuthentication("https://issuer.example", "backend-api", key);

        using ServiceProvider provider = services.BuildServiceProvider();
        AuthenticationOptions authenticationOptions = provider.GetRequiredService<IOptions<AuthenticationOptions>>().Value;
        JwtBearerOptions jwtOptions = provider
            .GetRequiredService<IOptionsMonitor<JwtBearerOptions>>()
            .Get(JwtBearerDefaults.AuthenticationScheme);

        Assert.Equal(JwtBearerDefaults.AuthenticationScheme, authenticationOptions.DefaultAuthenticateScheme);
        Assert.Equal("https://issuer.example", jwtOptions.TokenValidationParameters.ValidIssuer);
        Assert.Equal("backend-api", jwtOptions.TokenValidationParameters.ValidAudience);
        Assert.Same(key, jwtOptions.TokenValidationParameters.IssuerSigningKey);
        Assert.Equal(TimeSpan.FromMinutes(2), jwtOptions.TokenValidationParameters.ClockSkew);
    }

    // テスト意図: Add Cookie Authentication / Configures Cookie Paths を確認する。
    [Fact]
    public void AddCookieAuthentication_ConfiguresCookiePaths()
    {
        var services = new ServiceCollection();

        services.AddCookieAuthentication("/login", "/denied");

        using ServiceProvider provider = services.BuildServiceProvider();
        AuthenticationOptions authenticationOptions = provider.GetRequiredService<IOptions<AuthenticationOptions>>().Value;
        CookieAuthenticationOptions cookieOptions = provider
            .GetRequiredService<IOptionsMonitor<CookieAuthenticationOptions>>()
            .Get(CookieAuthenticationDefaults.AuthenticationScheme);

        Assert.Equal(CookieAuthenticationDefaults.AuthenticationScheme, authenticationOptions.DefaultAuthenticateScheme);
        Assert.Equal("/login", cookieOptions.LoginPath);
        Assert.Equal("/denied", cookieOptions.AccessDeniedPath);
        Assert.True(cookieOptions.Cookie.HttpOnly);
    }

    // テスト意図: Add Sample Authorization Policies / Registers Role And Claim Policies を確認する。
    [Fact]
    public async Task AddSampleAuthorizationPolicies_RegistersRoleAndClaimPolicies()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddSampleAuthorizationPolicies();

        using ServiceProvider provider = services.BuildServiceProvider();
        var authorizationService = provider.GetRequiredService<IAuthorizationService>();
        var admin = CreatePrincipal(
            new Claim(ClaimTypes.Role, "Admin"),
            new Claim("tenant_id", "tenant-1"));
        var tenantMember = CreatePrincipal(new Claim("tenant_id", "tenant-1"));

        AuthorizationResult adminResult = await authorizationService.AuthorizeAsync(
            admin,
            resource: null,
            AuthorizationPolicyNames.AdminOnly);
        AuthorizationResult tenantResult = await authorizationService.AuthorizeAsync(
            tenantMember,
            resource: null,
            AuthorizationPolicyNames.TenantMember);

        Assert.True(adminResult.Succeeded);
        Assert.True(tenantResult.Succeeded);
    }

    // テスト意図: Get Required User ID / Reads Name Identifier Or Subject Claim を確認する。
    [Fact]
    public void GetRequiredUserId_ReadsNameIdentifierOrSubjectClaim()
    {
        ClaimsPrincipal nameIdentifierPrincipal = CreatePrincipal(new Claim(ClaimTypes.NameIdentifier, "user-1"));
        ClaimsPrincipal subjectPrincipal = CreatePrincipal(new Claim("sub", "user-2"));

        Assert.Equal("user-1", AuthenticationAuthorizationSamples.GetRequiredUserId(nameIdentifierPrincipal));
        Assert.Equal("user-2", AuthenticationAuthorizationSamples.GetRequiredUserId(subjectPrincipal));
    }

    // テスト意図: Can Access Tenant / Returns Whether Tenant Claim Matches を確認する。
    [Fact]
    public void CanAccessTenant_ReturnsWhetherTenantClaimMatches()
    {
        ClaimsPrincipal principal = CreatePrincipal(new Claim("tenant_id", "tenant-1"));

        Assert.True(AuthenticationAuthorizationSamples.CanAccessTenant(principal, "tenant-1"));
        Assert.False(AuthenticationAuthorizationSamples.CanAccessTenant(principal, "tenant-2"));
    }

    // テスト意図: Create Forbidden Problem / Returns Problem Details With Code を確認する。
    [Fact]
    public void CreateForbiddenProblem_ReturnsProblemDetailsWithCode()
    {
        ProblemDetails problem = AuthenticationAuthorizationSamples.CreateForbiddenProblem(
            "tenant.forbidden",
            "The current user cannot access this tenant.",
            "/tenants/tenant-2");

        Assert.Equal(403, problem.Status);
        Assert.Equal("Forbidden.", problem.Title);
        Assert.Equal("tenant.forbidden", problem.Extensions["code"]);
        Assert.Equal("/tenants/tenant-2", problem.Instance);
    }

    private static ClaimsPrincipal CreatePrincipal(params Claim[] claims)
    {
        var identity = new ClaimsIdentity(claims, authenticationType: "Test");
        return new ClaimsPrincipal(identity);
    }
}
