using DotnetBackendSnippets.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DotnetBackendSnippets.Tests.EntityFrameworkCore;

// テスト対象: Repository Pattern Samples のスニペット動作を確認する。
public sealed class RepositoryPatternSamplesTests
{
    // テスト意図: Blog Post Repository / Adds And Reads Published Post を確認する。
    [Fact]
    public async Task BlogPostRepository_AddsAndReadsPublishedPost()
    {
        await using var dbContext = CreateDbContext();
        dbContext.Blogs.Add(new Blog { Id = 1, Name = "Backend Notes" });
        await dbContext.SaveChangesAsync();
        var repository = new EfCoreBlogPostRepository(dbContext);

        BlogPost created = await repository.AddPostAsync(new CreateBlogPostRequest(
            BlogId: 1,
            Title: " Repository Pattern ",
            PublishedAt: DateTimeOffset.UtcNow.AddMinutes(-1)));
        BlogPostListItem? item = await repository.GetPublishedPostAsync(created.Id);

        Assert.NotNull(item);
        Assert.Equal("Repository Pattern", item.Title);
    }

    // テスト意図: Should Use Repository / Avoids Wrapping Plain DbSet CRUD を確認する。
    [Theory]
    [InlineData(true, false, false, true)]
    [InlineData(false, true, false, true)]
    [InlineData(true, true, true, false)]
    [InlineData(false, false, false, false)]
    public void ShouldUseRepository_AvoidsWrappingPlainDbSetCrud(
        bool hasDomainSpecificQueries,
        bool needsPersistenceSwapInTests,
        bool justWrappingDbSetCrud,
        bool expected)
    {
        bool result = RepositoryPatternSamples.ShouldUseRepository(
            hasDomainSpecificQueries,
            needsPersistenceSwapInTests,
            justWrappingDbSetCrud);

        Assert.Equal(expected, result);
    }

    private static SampleBlogDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<SampleBlogDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString("N"))
            .Options;

        return new SampleBlogDbContext(options);
    }
}
