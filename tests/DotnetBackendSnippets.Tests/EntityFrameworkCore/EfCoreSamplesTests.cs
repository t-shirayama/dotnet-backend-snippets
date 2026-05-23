using DotnetBackendSnippets.EntityFrameworkCore;
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

    private static SampleBlogDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<SampleBlogDbContext>()
            .UseInMemoryDatabase(CreateDatabaseName())
            .ConfigureWarnings(warnings => warnings.Ignore(InMemoryEventId.TransactionIgnoredWarning))
            .Options;

        return new SampleBlogDbContext(options);
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
}
