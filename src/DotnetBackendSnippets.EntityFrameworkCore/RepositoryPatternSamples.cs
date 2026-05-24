using Microsoft.EntityFrameworkCore;

namespace DotnetBackendSnippets.EntityFrameworkCore;

/// <summary>
/// 記事作成時に repository へ渡す入力を表します。
/// </summary>
/// <param name="BlogId">記事を追加するブログ ID。</param>
/// <param name="Title">記事タイトル。</param>
/// <param name="PublishedAt">公開日時。</param>
public sealed record CreateBlogPostRequest(int BlogId, string Title, DateTimeOffset PublishedAt);

/// <summary>
/// 記事 repository の読み取り・書き込み操作を表します。
/// </summary>
public interface IBlogPostRepository
{
    /// <summary>
    /// 公開済みの記事を読み取り専用 DTO として取得します。
    /// </summary>
    /// <param name="postId">記事 ID。</param>
    /// <param name="cancellationToken">キャンセル通知。</param>
    /// <returns>記事一覧 DTO。見つからない場合は <see langword="null"/>。</returns>
    Task<BlogPostListItem?> GetPublishedPostAsync(int postId, CancellationToken cancellationToken = default);

    /// <summary>
    /// ブログに記事を追加します。
    /// </summary>
    /// <param name="request">記事作成リクエスト。</param>
    /// <param name="cancellationToken">キャンセル通知。</param>
    /// <returns>追加した記事 entity。</returns>
    Task<BlogPost> AddPostAsync(CreateBlogPostRequest request, CancellationToken cancellationToken = default);
}

/// <summary>
/// EF Core DbContext を薄く包む repository のサンプルです。
/// </summary>
public sealed class EfCoreBlogPostRepository : IBlogPostRepository
{
    private readonly SampleBlogDbContext dbContext;

    /// <summary>
    /// <see cref="EfCoreBlogPostRepository"/> クラスの新しいインスタンスを作成します。
    /// </summary>
    /// <param name="dbContext">ブログ DbContext。</param>
    /// <exception cref="ArgumentNullException"><paramref name="dbContext"/> が <see langword="null"/> の場合。</exception>
    public EfCoreBlogPostRepository(SampleBlogDbContext dbContext)
    {
        ArgumentNullException.ThrowIfNull(dbContext);

        this.dbContext = dbContext;
    }

    /// <inheritdoc />
    public Task<BlogPostListItem?> GetPublishedPostAsync(
        int postId,
        CancellationToken cancellationToken = default)
    {
        return dbContext.Posts
            .AsNoTracking()
            .Where(post => post.Id == postId && post.PublishedAt <= DateTimeOffset.UtcNow)
            .Select(post => new BlogPostListItem(post.Id, post.Title, post.PublishedAt, post.ViewCount))
            .SingleOrDefaultAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<BlogPost> AddPostAsync(
        CreateBlogPostRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentException.ThrowIfNullOrWhiteSpace(request.Title);

        var post = new BlogPost
        {
            BlogId = request.BlogId,
            Title = request.Title.Trim(),
            PublishedAt = request.PublishedAt,
        };

        dbContext.Posts.Add(post);
        await dbContext.SaveChangesAsync(cancellationToken);

        return post;
    }
}

/// <summary>
/// Repository pattern の使いどころを示すサンプルです。
/// </summary>
public static class RepositoryPatternSamples
{
    /// <summary>
    /// repository の抽象化が有効かどうかを判断します。
    /// </summary>
    /// <param name="hasDomainSpecificQueries">業務固有の query があるかどうか。</param>
    /// <param name="needsPersistenceSwapInTests">テストで永続化先を差し替える必要があるかどうか。</param>
    /// <param name="justWrappingDbSetCrud">DbSet の CRUD をそのまま包むだけかどうか。</param>
    /// <returns>repository 抽象化を検討する価値がある場合は <see langword="true"/>。</returns>
    public static bool ShouldUseRepository(
        bool hasDomainSpecificQueries,
        bool needsPersistenceSwapInTests,
        bool justWrappingDbSetCrud)
    {
        return !justWrappingDbSetCrud && (hasDomainSpecificQueries || needsPersistenceSwapInTests);
    }
}
