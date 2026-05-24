using DotnetBackendSnippets.EntityFrameworkCore;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;

namespace DotnetBackendSnippets.Tests.EntityFrameworkCore;

// テスト対象: EF Core Samples のスニペット動作を確認する。
public sealed class EfCoreSamplesTests
{
    // テスト意図: Add Sample Blog DB Context / Registers DB Context を確認する。
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

    // テスト意図: Get Recent Posts Read Only Async / Uses No Tracking Projection And Excludes Soft Deleted Posts を確認する。
    [Fact]
    public async Task GetRecentPostsReadOnlyAsync_UsesNoTrackingProjectionAndExcludesSoftDeletedPosts()
    {
        await using var dbContext = CreateDbContext();
        await SeedAsync(dbContext);

        var result = await EfCoreSamples.GetRecentPostsReadOnlyAsync(dbContext, count: 3);

        Assert.Equal(["EF Core Projection", "EF Core Paging", "EF Core Basics"], result.Select(post => post.Title));
        Assert.Empty(dbContext.ChangeTracker.Entries());
    }

    // テスト意図: Page Posts Async / Returns Items And Total Count を確認する。
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

    // テスト意図: Page Posts Async / Returns Empty Items / When Skip Count Is Extremely Large を確認する。
    [Fact]
    public async Task PagePostsAsync_ReturnsEmptyItems_WhenSkipCountIsExtremelyLarge()
    {
        await using var dbContext = CreateDbContext();
        await SeedAsync(dbContext);

        var result = await EfCoreSamples.PagePostsAsync(dbContext, pageNumber: int.MaxValue, pageSize: int.MaxValue);

        Assert.Empty(result.Items);
        Assert.Equal(3, result.TotalCount);
    }

    // テスト意図: Get Blog Summaries Async / Projects Only Required Shape を確認する。
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

    // テスト意図: Get Blog With Posts Async / Loads Aggregate With Include を確認する。
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

    // テスト意図: Soft Delete Post In Transaction Async / Marks Post Deleted And Query Filter Hides It を確認する。
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

    // テスト意図: Soft Delete Post In Transaction Async / Returns False / When Post Does Not Exist を確認する。
    [Fact]
    public async Task SoftDeletePostInTransactionAsync_ReturnsFalse_WhenPostDoesNotExist()
    {
        await using var dbContext = CreateDbContext();
        await SeedAsync(dbContext);

        var deleted = await EfCoreSamples.SoftDeletePostInTransactionAsync(dbContext, postId: 999);

        Assert.False(deleted);
    }

    // テスト意図: Soft Delete Post In Transaction Async / Commits Transaction / With SQLite in-memory Database を確認する。
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

    // テスト意図: Update Post Title With Concurrency Async / Returns Updated / When Stamp Matches を確認する。
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

    // テスト意図: Update Post Title With Concurrency Async / Returns Conflict / When Stamp Is Stale を確認する。
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

    // テスト意図: Update Post Title With Concurrency Async / Rejects Too Long New Stamp を確認する。
    [Fact]
    public async Task UpdatePostTitleWithConcurrencyAsync_RejectsTooLongNewStamp()
    {
        await using var dbContext = CreateDbContext();

        var exception = await Assert.ThrowsAsync<ArgumentOutOfRangeException>(
            () => EfCoreSamples.UpdatePostTitleWithConcurrencyAsync(
                dbContext,
                postId: 1,
                title: "Title",
                expectedConcurrencyStamp: "expected",
                newConcurrencyStamp: new string('x', 41)));

        Assert.Equal("newConcurrencyStamp", exception.ParamName);
    }

    // テスト意図: Is Unique Constraint Violation / Detects SQLite Unique Index Failure を確認する。
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

    // テスト意図: Apply Audit Values / Sets Created And Updated Timestamps を確認する。
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

    // テスト意図: Create Add Migration Command / Returns dotnet ef Arguments を確認する。
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

    // テスト意図: Create Migration Bundle Command / Returns Self Contained Bundle Arguments を確認する。
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

    // テスト意図: Create Apply Migration Command / Returns Database Update Arguments を確認する。
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

    // テスト意図: Execute In Transaction With Retry Async / Retries Transient Failure を確認する。
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

    // テスト意図: Audit Save Changes Interceptor / Sets Audit Values During Save Changes を確認する。
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

    // テスト意図: Audit Save Changes Interceptor / Throws Argument Null Exception / When Time Provider Is Null を確認する。
    [Fact]
    public void AuditSaveChangesInterceptor_ThrowsArgumentNullException_WhenTimeProviderIsNull()
    {
        var exception = Assert.Throws<ArgumentNullException>(() => new AuditSaveChangesInterceptor(null!));

        Assert.Equal("timeProvider", exception.ParamName);
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
