using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using JwtRegisteredClaimNames = Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames;

namespace DotnetBackendSnippets.Authentication;

/// <summary>
/// 認証・認可サンプルで使うポリシー名を集約します。
/// </summary>
public static class AuthorizationPolicyNames
{
    /// <summary>
    /// 管理者だけが使えるポリシー名です。
    /// </summary>
    public const string AdminOnly = "admin-only";

    /// <summary>
    /// tenant_id claim を持つユーザー向けのポリシー名です。
    /// </summary>
    public const string TenantMember = "tenant-member";
}

/// <summary>
/// permission claim の値を要求する認可 requirement です。
/// </summary>
/// <param name="Permission">要求する permission。</param>
public sealed record PermissionRequirement(string Permission) : IAuthorizationRequirement;

/// <summary>
/// permission claim を確認する認可 handler です。
/// </summary>
public sealed class PermissionAuthorizationHandler : AuthorizationHandler<PermissionRequirement>
{
    /// <inheritdoc />
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        PermissionRequirement requirement)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(requirement);

        if (AuthenticationAuthorizationSamples.HasPermission(context.User, requirement.Permission))
        {
            context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }
}

/// <summary>
/// 認証・認可でよく使う ASP.NET Core 設定とヘルパーです。
/// </summary>
public static class AuthenticationAuthorizationSamples
{
    /// <summary>
    /// JWT Bearer 認証を登録します。
    /// </summary>
    /// <param name="services">サービスコレクション。</param>
    /// <param name="issuer">期待する issuer。</param>
    /// <param name="audience">期待する audience。</param>
    /// <param name="signingKey">署名検証キー。</param>
    /// <returns>登録後のサービスコレクション。</returns>
    /// <exception cref="ArgumentNullException"><paramref name="services"/> または <paramref name="signingKey"/> が <see langword="null"/> の場合。</exception>
    /// <exception cref="ArgumentException"><paramref name="issuer"/> または <paramref name="audience"/> が空白の場合。</exception>
    public static IServiceCollection AddJwtBearerAuthentication(
        this IServiceCollection services,
        string issuer,
        string audience,
        SecurityKey signingKey)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentException.ThrowIfNullOrWhiteSpace(issuer);
        ArgumentException.ThrowIfNullOrWhiteSpace(audience);
        ArgumentNullException.ThrowIfNull(signingKey);

        services
            .AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = issuer,
                    ValidateAudience = true,
                    ValidAudience = audience,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = signingKey,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.FromMinutes(2),
                };
            });

        return services;
    }

    /// <summary>
    /// サンプル用の JWT を発行します。
    /// </summary>
    /// <param name="userId">subject として入れる user id。</param>
    /// <param name="issuer">issuer。</param>
    /// <param name="audience">audience。</param>
    /// <param name="signingCredentials">署名に使う credential。</param>
    /// <param name="expiresAtUtc">有効期限。</param>
    /// <param name="additionalClaims">追加する claim。</param>
    /// <returns>署名済み JWT。</returns>
    /// <exception cref="ArgumentException"><paramref name="userId"/>、<paramref name="issuer"/>、<paramref name="audience"/> が空白の場合。</exception>
    /// <exception cref="ArgumentNullException"><paramref name="signingCredentials"/> または <paramref name="additionalClaims"/> が <see langword="null"/> の場合。</exception>
    public static string CreateJwtToken(
        string userId,
        string issuer,
        string audience,
        SigningCredentials signingCredentials,
        DateTimeOffset expiresAtUtc,
        IEnumerable<Claim> additionalClaims)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(userId);
        ArgumentException.ThrowIfNullOrWhiteSpace(issuer);
        ArgumentException.ThrowIfNullOrWhiteSpace(audience);
        ArgumentNullException.ThrowIfNull(signingCredentials);
        ArgumentNullException.ThrowIfNull(additionalClaims);

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, userId),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString("N")),
        };
        claims.AddRange(additionalClaims);

        var token = new JwtSecurityToken(
            issuer,
            audience,
            claims,
            notBefore: DateTime.UtcNow,
            expires: expiresAtUtc.UtcDateTime,
            signingCredentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    /// <summary>
    /// Cookie 認証を登録します。
    /// </summary>
    /// <param name="services">サービスコレクション。</param>
    /// <param name="loginPath">未認証時のログインパス。</param>
    /// <param name="accessDeniedPath">認可失敗時のパス。</param>
    /// <returns>登録後のサービスコレクション。</returns>
    /// <exception cref="ArgumentNullException"><paramref name="services"/> が <see langword="null"/> の場合。</exception>
    /// <exception cref="ArgumentException"><paramref name="loginPath"/> または <paramref name="accessDeniedPath"/> が空白の場合。</exception>
    public static IServiceCollection AddCookieAuthentication(
        this IServiceCollection services,
        string loginPath,
        string accessDeniedPath)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentException.ThrowIfNullOrWhiteSpace(loginPath);
        ArgumentException.ThrowIfNullOrWhiteSpace(accessDeniedPath);

        services
            .AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            })
            .AddCookie(options =>
            {
                options.LoginPath = loginPath;
                options.AccessDeniedPath = accessDeniedPath;
                options.Cookie.HttpOnly = true;
                options.Cookie.SameSite = SameSiteMode.Lax;
                options.SlidingExpiration = true;
            });

        return services;
    }

    /// <summary>
    /// role と claim を使った代表的な認可ポリシーを登録します。
    /// </summary>
    /// <param name="services">サービスコレクション。</param>
    /// <returns>登録後のサービスコレクション。</returns>
    /// <exception cref="ArgumentNullException"><paramref name="services"/> が <see langword="null"/> の場合。</exception>
    public static IServiceCollection AddSampleAuthorizationPolicies(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        services.AddAuthorization(options =>
        {
            options.AddPolicy(
                AuthorizationPolicyNames.AdminOnly,
                policy => policy.RequireRole("Admin"));

            options.AddPolicy(
                AuthorizationPolicyNames.TenantMember,
                policy => policy.RequireAuthenticatedUser().RequireClaim("tenant_id"));
        });

        services.AddSingleton<IAuthorizationHandler, PermissionAuthorizationHandler>();

        return services;
    }

    /// <summary>
    /// 認証済みユーザーから user id を取得します。
    /// </summary>
    /// <param name="principal">ユーザーを表す claims principal。</param>
    /// <returns><see cref="ClaimTypes.NameIdentifier"/> または <c>sub</c> claim の値。</returns>
    /// <exception cref="ArgumentNullException"><paramref name="principal"/> が <see langword="null"/> の場合。</exception>
    /// <exception cref="InvalidOperationException">ユーザーが未認証、または user id claim がない場合。</exception>
    public static string GetRequiredUserId(ClaimsPrincipal principal)
    {
        ArgumentNullException.ThrowIfNull(principal);

        if (principal.Identity?.IsAuthenticated != true)
        {
            throw new InvalidOperationException("The current user is not authenticated.");
        }

        string? userId = principal.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? principal.FindFirstValue("sub");

        return string.IsNullOrWhiteSpace(userId)
            ? throw new InvalidOperationException("The current user id claim is missing.")
            : userId;
    }

    /// <summary>
    /// tenant_id claim に基づいて tenant へのアクセス可否を判定します。
    /// </summary>
    /// <param name="principal">ユーザーを表す claims principal。</param>
    /// <param name="tenantId">アクセス対象 tenant id。</param>
    /// <returns>tenant_id claim が一致する場合は <see langword="true"/>。</returns>
    /// <exception cref="ArgumentNullException"><paramref name="principal"/> が <see langword="null"/> の場合。</exception>
    /// <exception cref="ArgumentException"><paramref name="tenantId"/> が空白の場合。</exception>
    public static bool CanAccessTenant(ClaimsPrincipal principal, string tenantId)
    {
        ArgumentNullException.ThrowIfNull(principal);
        ArgumentException.ThrowIfNullOrWhiteSpace(tenantId);

        return principal.Claims
            .Where(claim => string.Equals(claim.Type, "tenant_id", StringComparison.Ordinal))
            .Any(claim => string.Equals(claim.Value, tenantId, StringComparison.Ordinal));
    }

    /// <summary>
    /// scope claim に指定 scope が含まれるか判定します。
    /// </summary>
    /// <param name="principal">ユーザーを表す claims principal。</param>
    /// <param name="requiredScope">要求する scope。</param>
    /// <returns>scope claim に要求 scope が含まれる場合は <see langword="true"/>。</returns>
    /// <exception cref="ArgumentNullException"><paramref name="principal"/> が <see langword="null"/> の場合。</exception>
    /// <exception cref="ArgumentException"><paramref name="requiredScope"/> が空白の場合。</exception>
    public static bool HasScope(ClaimsPrincipal principal, string requiredScope)
    {
        ArgumentNullException.ThrowIfNull(principal);
        ArgumentException.ThrowIfNullOrWhiteSpace(requiredScope);

        return principal.Claims
            .Where(claim => claim.Type is "scope" or "scp")
            .SelectMany(claim => claim.Value.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
            .Any(scope => string.Equals(scope, requiredScope, StringComparison.Ordinal));
    }

    /// <summary>
    /// permission claim に指定 permission が含まれるか判定します。
    /// </summary>
    /// <param name="principal">ユーザーを表す claims principal。</param>
    /// <param name="requiredPermission">要求する permission。</param>
    /// <returns>permission claim が一致する場合は <see langword="true"/>。</returns>
    /// <exception cref="ArgumentNullException"><paramref name="principal"/> が <see langword="null"/> の場合。</exception>
    /// <exception cref="ArgumentException"><paramref name="requiredPermission"/> が空白の場合。</exception>
    public static bool HasPermission(ClaimsPrincipal principal, string requiredPermission)
    {
        ArgumentNullException.ThrowIfNull(principal);
        ArgumentException.ThrowIfNullOrWhiteSpace(requiredPermission);

        return principal.Claims
            .Where(claim => string.Equals(claim.Type, "permission", StringComparison.Ordinal))
            .Any(claim => string.Equals(claim.Value, requiredPermission, StringComparison.Ordinal));
    }

    /// <summary>
    /// 認可失敗を Problem Details として表します。
    /// </summary>
    /// <param name="code">クライアント向けエラーコード。</param>
    /// <param name="detail">失敗理由。</param>
    /// <param name="instance">発生箇所を示す URI。</param>
    /// <returns>認可失敗を表す Problem Details。</returns>
    /// <exception cref="ArgumentException"><paramref name="code"/> または <paramref name="detail"/> が空白の場合。</exception>
    public static ProblemDetails CreateForbiddenProblem(string code, string detail, string? instance = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(code);
        ArgumentException.ThrowIfNullOrWhiteSpace(detail);

        var problem = new ProblemDetails
        {
            Status = StatusCodes.Status403Forbidden,
            Title = "Forbidden.",
            Detail = detail,
            Type = "https://httpstatuses.com/403",
            Instance = instance,
        };

        problem.Extensions["code"] = code;
        return problem;
    }
}
