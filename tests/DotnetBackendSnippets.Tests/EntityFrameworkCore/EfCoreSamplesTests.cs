using DotnetBackendSnippets.EntityFrameworkCore;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;

namespace DotnetBackendSnippets.Tests.EntityFrameworkCore;

public sealed class EfCoreSamplesTests
{
    [Fact]
    public void AddSampleBlogDbContext_RegistersDbContext()
    {
        var services = new ServiceCollection();
        var databaseName = CreateDatabaseName();

        services.AddSampleBlogDbContext(options => options.UseInMemoryDatabase(databaseName));

        using var provider = services.BuildServiceProvider();
        using var scope = provider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<SampleBlogDbContext>();

        Assert.NotNull(dbContext);
    }

    [Fact]
    public async Task GetRecentPostsReadOnlyAsync_UsesNoTrackingProjectionAndExcludesSoftDeletedPosts()
    {
        await using var dbContext = CreateDbContext();
        await SeedAsync(dbContext);

        var result = await EfCoreSamples.GetRecentPostsReadOnlyAsync(dbContext, count: 3);

        Assert.Equal(["EF Core Projection", "EF Core Paging", "EF Core Basics"], result.Select(post => post.Title));
        Assert.Empty(dbContext.ChangeTracker.Entries());
    }

    [Fact]
    public async Task PagePostsAsync_ReturnsItemsAndTotalCount()
    {
        await using var dbContext = CreateDbContext();
        await SeedAsync(dbContext);

        var result = await EfCoreSamples.PagePostsAsync(dbContext, pageNumber: 2, pageSize: 2);

        Assert.Equal(3, result.TotalCount);
        Assert.Equal(2, result.PageNumber);
        Assert.Equal(2, result.PageSize);
        Assert.Equal(["EF Core Basics"], result.Items.Select(post => post.Title));
    }

    [Fact]
    public async Task PagePostsAsync_ReturnsEmptyItems_WhenSkipCountIsExtremelyLarge()
    {
        await using var dbContext = CreateDbContext();
        await SeedAsync(dbContext);

        var result = await EfCoreSamples.PagePostsAsync(dbContext, pageNumber: int.MaxValue, pageSize: int.MaxValue);

        Assert.Empty(result.Items);
        Assert.Equal(3, result.TotalCount);
    }

    [Fact]
    public async Task GetBlogSummariesAsync_ProjectsOnlyRequiredShape()
    {
        await using var dbContext = CreateDbContext();
        await SeedAsync(dbContext);

        var result = await EfCoreSamples.GetBlogSummariesAsync(dbContext);

        Assert.Collection(
            result,
            summary =>
            {
                Assert.Equal("Backend Notes", summary.Name);
                Assert.Equal(3, summary.PublishedPostCount);
            });
    }

    [Fact]
    public async Task GetBlogWithPostsAsync_LoadsAggregateWithInclude()
    {
        await using var dbContext = CreateDbContext();
        await SeedAsync(dbContext);

        var blog = await EfCoreSamples.GetBlogWithPostsAsync(dbContext, blogId: 1);

        Assert.NotNull(blog);
        Assert.Equal("Backend Notes", blog.Name);
        Assert.Equal(3, blog.Posts.Count);
    }

    [Fact]
    public async Task SoftDeletePostInTransactionAsync_MarksPostDeletedAndQueryFilterHidesIt()
    {
        await using var dbContext = CreateDbContext();
        await SeedAsync(dbContext);

        var deleted = await EfCoreSamples.SoftDeletePostInTransactionAsync(dbContext, postId: 2);

        Assert.True(deleted);
        Assert.DoesNotContain(await dbContext.Posts.ToListAsync(), post => post.Id == 2);

        var deletedPost = await dbContext.Posts
            .IgnoreQueryFilters()
            .SingleAsync(post => post.Id == 2);
        Assert.True(deletedPost.IsDeleted);
    }

    [Fact]
    public async Task SoftDeletePostInTransactionAsync_ReturnsFalse_WhenPostDoesNotExist()
    {
        await using var dbContext = CreateDbContext();
        await SeedAsync(dbContext);

        var deleted = await EfCoreSamples.SoftDeletePostInTransactionAsync(dbContext, postId: 999);

        Assert.False(deleted);
    }

    [Fact]
    public async Task SoftDeletePostInTransactionAsync_CommitsTransaction_WithSqliteInMemoryDatabase()
    {
        await using var connection = new SqliteConnection("Data Source=:memory:");
        await connection.OpenAsync();

        var options = new DbContextOptionsBuilder<SampleBlogDbContext>()
            .UseSqlite(connection)
            .Options;

        await using var dbContext = new SampleBlogDbContext(options);
        await dbContext.Database.EnsureCreatedAsync();
        await SeedAsync(dbContext);

        var deleted = await EfCoreSamples.SoftDeletePostInTransactionAsync(dbContext, postId: 2);

        Assert.True(deleted);
        Assert.DoesNotContain(await dbContext.Posts.ToListAsync(), post => post.Id == 2);

        var deletedPost = await dbContext.Posts
            .IgnoreQueryFilters()
            .SingleAsync(post => post.Id == 2);
        Assert.True(deletedPost.IsDeleted);
    }

    [Fact]
    public async Task UpdatePostTitleWithConcurrencyAsync_ReturnsUpdated_WhenStampMatches()
    {
        await using var connection = new SqliteConnection("Data Source=:memory:");
        await connection.OpenAsync();
        DbContextOptions<SampleBlogDbContext> options = CreateSqliteOptions(connection);
        await using var dbContext = await CreateSqliteDbContextAsync(options);
        await SeedAsync(dbContext);
        string stamp = await dbContext.Posts
            .Where(post => post.Id == 1)
            .Select(post => post.ConcurrencyStamp)
            .SingleAsync();

        PostTitleUpdateResult result = await EfCoreSamples.UpdatePostTitleWithConcurrencyAsync(
            dbContext,
            postId: 1,
            title: " Updated title ",
            expectedConcurrencyStamp: stamp,
            newConcurrencyStamp: "new-stamp");

        BlogPost post = await dbContext.Posts.SingleAsync(candidate => candidate.Id == 1);

        Assert.Equal(PostTitleUpdateResult.Updated, result);
        Assert.Equal("Updated title", post.Title);
        Assert.Equal("new-stamp", post.ConcurrencyStamp);
    }

    [Fact]
    public async Task UpdatePostTitleWithConcurrencyAsync_ReturnsConflict_WhenStampIsStale()
    {
        await using var connection = new SqliteConnection("Data Source=:memory:");
        await connection.OpenAsync();
        DbContextOptions<SampleBlogDbContext> options = CreateSqliteOptions(connection);
        await using var seedContext = await CreateSqliteDbContextAsync(options);
        await SeedAsync(seedContext);
        string originalStamp = await seedContext.Posts
            .Where(post => post.Id == 1)
            .Select(post => post.ConcurrencyStamp)
            .SingleAsync();

        await using (var concurrentContext = new SampleBlogDbContext(options))
        {
            BlogPost post = await concurrentContext.Posts.SingleAsync(candidate => candidate.Id == 1);
            post.Title = "Changed elsewhere";
            post.ConcurrencyStamp = "other-stamp";
            await concurrentContext.SaveChangesAsync();
        }

        await using var updateContext = new SampleBlogDbContext(options);
        PostTitleUpdateResult result = await EfCoreSamples.UpdatePostTitleWithConcurrencyAsync(
            updateContext,
            postId: 1,
            title: "My change",
            expectedConcurrencyStamp: originalStamp,
            newConcurrencyStamp: "my-stamp");

        Assert.Equal(PostTitleUpdateResult.ConcurrencyConflict, result);
    }

    [Fact]
    public async Task IsUniqueConstraintViolation_DetectsSqliteUniqueIndexFailure()
    {
        await using var connection = new SqliteConnection("Data Source=:memory:");
        await connection.OpenAsync();
        DbContextOptions<SampleBlogDbContext> options = CreateSqliteOptions(connection);
        await using var dbContext = await CreateSqliteDbContextAsync(options);

        dbContext.Blogs.Add(new Blog { Name = "Backend Notes" });
        dbContext.Blogs.Add(new Blog { Name = "Backend Notes" });

        DbUpdateException exception = await Assert.ThrowsAsync<DbUpdateException>(() => dbContext.SaveChangesAsync());

        Assert.True(EfCoreSamples.IsUniqueConstraintViolation(exception));
    }

    [Fact]
    public async Task ApplyAuditValues_SetsCreatedAndUpdatedTimestamps()
    {
        await using var dbContext = CreateDbContext();
        var now = new DateTimeOffset(2026, 5, 24, 10, 0, 0, TimeSpan.Zero);
        var blog = new Blog { Id = 10, Name = "Audit" };
        blog.Posts.Add(new BlogPost
        {
            Id = 10,
            Title = "Audit fields",
            PublishedAt = now,
        });
        dbContext.Blogs.Add(blog);

        EfCoreSamples.ApplyAuditValues(dbContext.ChangeTracker, now);
        await dbContext.SaveChangesAsync();

        BlogPost post = await dbContext.Posts.SingleAsync(candidate => candidate.Id == 10);
        Assert.Equal(now, post.CreatedAt);
        Assert.Equal(now, post.UpdatedAt);
    }

    [Fact]
    public void CreateAddMigrationCommand_ReturnsDotnetEfArguments()
    {
        EfCliCommand command = EfCoreSamples.CreateAddMigrationCommand(
            "AddAuditColumns",
            "src/Data/Data.csproj",
            "src/Web/Web.csproj",
            "AppDbContext");

        Assert.Equal("dotnet", command.FileName);
        Assert.Equal(
            [
                "ef",
                "migrations",
                "add",
                "AddAuditColumns",
                "--project",
                "src/Data/Data.csproj",
                "--startup-project",
                "src/Web/Web.csproj",
                "--context",
                "AppDbContext",
            ],
            command.Arguments);
    }

    [Fact]
    public void CreateMigrationBundleCommand_ReturnsSelfContainedBundleArguments()
    {
        EfCliCommand command = EfCoreSamples.CreateMigrationBundleCommand(
            "src/Data/Data.csproj",
            "src/Web/Web.csproj",
            "artifacts/efbundle",
            selfContained: true);

        Assert.Equal("dotnet", command.FileName);
        Assert.Contains("--self-contained", command.Arguments);
        Assert.Contains("artifacts/efbundle", command.Arguments);
    }

    [Fact]
    public void CreateApplyMigrationCommand_ReturnsDatabaseUpdateArguments()
    {
        EfCliCommand command = EfCoreSamples.CreateApplyMigrationCommand(
            "src/Data/Data.csproj",
            "src/Web/Web.csproj",
            "AppDbContext",
            "Default");

        Assert.Equal("dotnet", command.FileName);
        Assert.Equal(
            [
                "ef",
                "database",
                "update",
                "--project",
                "src/Data/Data.csproj",
                "--startup-project",
                "src/Web/Web.csproj",
                "--context",
                "AppDbContext",
                "--connection",
                "Name=ConnectionStrings:Default",
            ],
            command.Arguments);
    }

    [Fact]
    public async Task ExecuteInTransactionWithRetryAsync_RetriesTransientFailure()
    {
        await using var dbContext = CreateDbContext();
        var attempts = 0;

        int result = await EfCoreSamples.ExecuteInTransactionWithRetryAsync(
            dbContext,
            (_, _) =>
            {
                attempts++;

                if (attempts == 1)
                {
                    throw new TimeoutException("temporary");
                }

                return Task.FromResult(42);
            },
            exception => exception is TimeoutException,
            new TransactionRetryOptions(2, TimeSpan.Zero),
            static (_, _) => Task.CompletedTask);

        Assert.Equal(42, result);
        Assert.Equal(2, attempts);
    }

    [Fact]
    public async Task AuditSaveChangesInterceptor_SetsAuditValuesDuringSaveChanges()
    {
        var now = new DateTimeOffset(2026, 5, 24, 12, 0, 0, TimeSpan.Zero);
        var options = new DbContextOptionsBuilder<SampleBlogDbContext>()
            .UseInMemoryDatabase(CreateDatabaseName())
            .ConfigureWarnings(warnings => warnings.Ignore(InMemoryEventId.TransactionIgnoredWarning))
            .AddInterceptors(new AuditSaveChangesInterceptor(new FixedTimeProvider(now)))
            .Options;
        await using var dbContext = new SampleBlogDbContext(options);
        var blog = new Blog { Id = 20, Name = "Interceptor" };
        blog.Posts.Add(new BlogPost
        {
            Id = 20,
            Title = "Audit interceptor",
            PublishedAt = now,
        });
        dbContext.Blogs.Add(blog);

        await dbContext.SaveChangesAsync();

        BlogPost post = await dbContext.Posts.SingleAsync(candidate => candidate.Id == 20);
        Assert.Equal(now, post.CreatedAt);
        Assert.Equal(now, post.UpdatedAt);
    }

    private static SampleBlogDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<SampleBlogDbContext>()
            .UseInMemoryDatabase(CreateDatabaseName())
            .ConfigureWarnings(warnings => warnings.Ignore(InMemoryEventId.TransactionIgnoredWarning))
            .Options;

        return new SampleBlogDbContext(options);
    }

    private static DbContextOptions<SampleBlogDbContext> CreateSqliteOptions(SqliteConnection connection)
    {
        return new DbContextOptionsBuilder<SampleBlogDbContext>()
            .UseSqlite(connection)
            .Options;
    }

    private static async Task<SampleBlogDbContext> CreateSqliteDbContextAsync(DbContextOptions<SampleBlogDbContext> options)
    {
        var dbContext = new SampleBlogDbContext(options);
        await dbContext.Database.EnsureCreatedAsync();
        return dbContext;
    }

    private static async Task SeedAsync(SampleBlogDbContext dbContext)
    {
        var blog = new Blog
        {
            Id = 1,
            Name = "Backend Notes",
        };

        blog.Posts.Add(new BlogPost
        {
            Id = 1,
            Title = "EF Core Basics",
            PublishedAt = new DateTimeOffset(2026, 1, 1, 9, 0, 0, TimeSpan.Zero),
            ViewCount = 10,
        });
        blog.Posts.Add(new BlogPost
        {
            Id = 2,
            Title = "EF Core Paging",
            PublishedAt = new DateTimeOffset(2026, 1, 2, 9, 0, 0, TimeSpan.Zero),
            ViewCount = 20,
        });
        blog.Posts.Add(new BlogPost
        {
            Id = 3,
            Title = "Deleted Draft",
            PublishedAt = new DateTimeOffset(2026, 1, 3, 9, 0, 0, TimeSpan.Zero),
            ViewCount = 30,
            IsDeleted = true,
        });
        blog.Posts.Add(new BlogPost
        {
            Id = 4,
            Title = "EF Core Projection",
            PublishedAt = new DateTimeOffset(2026, 1, 4, 9, 0, 0, TimeSpan.Zero),
            ViewCount = 40,
        });

        dbContext.Blogs.Add(blog);
        await dbContext.SaveChangesAsync();
        dbContext.ChangeTracker.Clear();
    }

    private static string CreateDatabaseName()
    {
        return $"ef-core-samples-{Guid.NewGuid():N}";
    }

    private sealed class FixedTimeProvider(DateTimeOffset utcNow) : TimeProvider
    {
        public override DateTimeOffset GetUtcNow()
        {
            return utcNow;
        }
    }
}
