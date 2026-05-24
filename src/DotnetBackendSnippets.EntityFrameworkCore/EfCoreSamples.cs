using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace DotnetBackendSnippets.EntityFrameworkCore;

/// <summary>
/// ブログサンプル用の EF Core DbContext です。
/// </summary>
/// <param name="options">DbContext の設定オプション。</param>
public sealed class SampleBlogDbContext(DbContextOptions<SampleBlogDbContext> options) : DbContext(options)
{
    /// <summary>
    /// ブログの DbSet を取得します。
    /// </summary>
    /// <value>ブログエンティティのクエリおよび保存入口。</value>
    public DbSet<Blog> Blogs => Set<Blog>();

    /// <summary>
    /// ブログ記事の DbSet を取得します。
    /// </summary>
    /// <value>ブログ記事エンティティのクエリおよび保存入口。</value>
    public DbSet<BlogPost> Posts => Set<BlogPost>();

    /// <inheritdoc />
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        ArgumentNullException.ThrowIfNull(modelBuilder);

        modelBuilder.Entity<Blog>(entity =>
        {
            entity.Property(blog => blog.Name).HasMaxLength(200).IsRequired();

            entity
                .HasMany(blog => blog.Posts)
                .WithOne(post => post.Blog)
                .HasForeignKey(post => post.BlogId)
                .IsRequired();
        });

        modelBuilder.Entity<BlogPost>(entity =>
        {
            entity.Property(post => post.Title).HasMaxLength(200).IsRequired();
            entity.HasQueryFilter(post => !post.IsDeleted);
        });
    }
}

/// <summary>
/// ブログを表すエンティティです。
/// </summary>
public sealed class Blog
{
    /// <summary>
    /// ブログ ID を取得または設定します。
    /// </summary>
    /// <value>主キーとして使う ID。</value>
    public int Id { get; set; }

    /// <summary>
    /// ブログ名を取得または設定します。
    /// </summary>
    /// <value>最大 200 文字のブログ名。</value>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// ブログに属する記事を取得します。
    /// </summary>
    /// <value>関連するブログ記事のコレクション。</value>
    public ICollection<BlogPost> Posts { get; } = new List<BlogPost>();
}

/// <summary>
/// ブログ記事を表すエンティティです。
/// </summary>
public sealed class BlogPost
{
    /// <summary>
    /// 記事 ID を取得または設定します。
    /// </summary>
    /// <value>主キーとして使う ID。</value>
    public int Id { get; set; }

    /// <summary>
    /// 所属するブログ ID を取得または設定します。
    /// </summary>
    /// <value>ブログへの外部キー。</value>
    public int BlogId { get; set; }

    /// <summary>
    /// 所属するブログを取得または設定します。
    /// </summary>
    /// <value>関連するブログエンティティ。</value>
    public Blog? Blog { get; set; }

    /// <summary>
    /// 記事タイトルを取得または設定します。
    /// </summary>
    /// <value>最大 200 文字の記事タイトル。</value>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// 公開日時を取得または設定します。
    /// </summary>
    /// <value>記事が公開された日時。</value>
    public DateTimeOffset PublishedAt { get; set; }

    /// <summary>
    /// 閲覧数を取得または設定します。
    /// </summary>
    /// <value>記事の閲覧回数。</value>
    public int ViewCount { get; set; }

    /// <summary>
    /// 論理削除済みかどうかを取得または設定します。
    /// </summary>
    /// <value>削除済みとして扱う場合は <see langword="true"/>。</value>
    public bool IsDeleted { get; set; }
}

/// <summary>
/// 記事一覧に表示する項目を表します。
/// </summary>
/// <param name="Id">記事 ID。</param>
/// <param name="Title">記事タイトル。</param>
/// <param name="PublishedAt">公開日時。</param>
/// <param name="ViewCount">閲覧数。</param>
public sealed record BlogPostListItem(int Id, string Title, DateTimeOffset PublishedAt, int ViewCount);

/// <summary>
/// ブログの集計情報を表します。
/// </summary>
/// <param name="Id">ブログ ID。</param>
/// <param name="Name">ブログ名。</param>
/// <param name="PublishedPostCount">公開記事数。</param>
public sealed record BlogSummary(int Id, string Name, int PublishedPostCount);

/// <summary>
/// ページング済みの結果を表します。
/// </summary>
/// <typeparam name="T">項目の型。</typeparam>
/// <param name="Items">取得した項目。</param>
/// <param name="PageNumber">ページ番号。</param>
/// <param name="PageSize">ページサイズ。</param>
/// <param name="TotalCount">全件数。</param>
public sealed record PagedResult<T>(IReadOnlyList<T> Items, int PageNumber, int PageSize, int TotalCount);

/// <summary>
/// EF Core の代表的な操作サンプルです。
/// </summary>
public static class EfCoreSamples
{
    /// <summary>
    /// サンプル用 DbContext を DI コンテナーに登録します。
    /// </summary>
    /// <param name="services">サービスコレクション。</param>
    /// <param name="configureOptions">DbContext オプションを設定する処理。</param>
    /// <returns>登録後のサービスコレクション。</returns>
    /// <exception cref="ArgumentNullException"><paramref name="services"/> または <paramref name="configureOptions"/> が <see langword="null"/> の場合。</exception>
    public static IServiceCollection AddSampleBlogDbContext(
        this IServiceCollection services,
        Action<DbContextOptionsBuilder> configureOptions)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configureOptions);

        services.AddDbContext<SampleBlogDbContext>(configureOptions);
        return services;
    }

    /// <summary>
    /// 最近の記事を読み取り専用で取得します。
    /// </summary>
    /// <param name="dbContext">ブログ DbContext。</param>
    /// <param name="count">取得件数。</param>
    /// <param name="cancellationToken">キャンセル通知。</param>
    /// <returns>最近の記事一覧。</returns>
    /// <exception cref="ArgumentNullException"><paramref name="dbContext"/> が <see langword="null"/> の場合。</exception>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="count"/> が負の値の場合。</exception>
    public static async Task<IReadOnlyList<BlogPostListItem>> GetRecentPostsReadOnlyAsync(
        SampleBlogDbContext dbContext,
        int count,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(dbContext);

        if (count < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(count), "Count must be zero or greater.");
        }

        return await dbContext.Posts
            .AsNoTracking()
            .OrderByDescending(post => post.PublishedAt)
            .ThenBy(post => post.Id)
            .Take(count)
            .Select(post => new BlogPostListItem(post.Id, post.Title, post.PublishedAt, post.ViewCount))
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// 記事一覧をページングして取得します。
    /// </summary>
    /// <param name="dbContext">ブログ DbContext。</param>
    /// <param name="pageNumber">ページ番号。</param>
    /// <param name="pageSize">ページサイズ。</param>
    /// <param name="cancellationToken">キャンセル通知。</param>
    /// <returns>ページング済みの記事一覧。</returns>
    /// <exception cref="ArgumentNullException"><paramref name="dbContext"/> が <see langword="null"/> の場合。</exception>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="pageNumber"/> または <paramref name="pageSize"/> が範囲外の場合。</exception>
    public static async Task<PagedResult<BlogPostListItem>> PagePostsAsync(
        SampleBlogDbContext dbContext,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(dbContext);

        if (pageNumber < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(pageNumber), "Page number must be one or greater.");
        }

        if (pageSize < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(pageSize), "Page size must be one or greater.");
        }

        var totalCount = await dbContext.Posts.AsNoTracking().CountAsync(cancellationToken);
        var skipCount = ((long)pageNumber - 1) * pageSize;

        if (skipCount >= totalCount)
        {
            return new PagedResult<BlogPostListItem>([], pageNumber, pageSize, totalCount);
        }

        var items = await dbContext.Posts
            .AsNoTracking()
            .OrderByDescending(post => post.PublishedAt)
            .ThenBy(post => post.Id)
            .Skip((int)skipCount)
            .Take(pageSize)
            .Select(post => new BlogPostListItem(post.Id, post.Title, post.PublishedAt, post.ViewCount))
            .ToListAsync(cancellationToken);

        return new PagedResult<BlogPostListItem>(items, pageNumber, pageSize, totalCount);
    }

    /// <summary>
    /// ブログごとの集計情報を取得します。
    /// </summary>
    /// <param name="dbContext">ブログ DbContext。</param>
    /// <param name="cancellationToken">キャンセル通知。</param>
    /// <returns>ブログ集計情報の一覧。</returns>
    /// <exception cref="ArgumentNullException"><paramref name="dbContext"/> が <see langword="null"/> の場合。</exception>
    public static async Task<IReadOnlyList<BlogSummary>> GetBlogSummariesAsync(
        SampleBlogDbContext dbContext,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(dbContext);

        return await dbContext.Blogs
            .AsNoTracking()
            .OrderBy(blog => blog.Name)
            .Select(blog => new BlogSummary(blog.Id, blog.Name, blog.Posts.Count))
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// ブログと記事をまとめて取得します。
    /// </summary>
    /// <param name="dbContext">ブログ DbContext。</param>
    /// <param name="blogId">ブログ ID。</param>
    /// <param name="cancellationToken">キャンセル通知。</param>
    /// <returns>見つかったブログ。存在しない場合は <see langword="null"/>。</returns>
    /// <exception cref="ArgumentNullException"><paramref name="dbContext"/> が <see langword="null"/> の場合。</exception>
    public static async Task<Blog?> GetBlogWithPostsAsync(
        SampleBlogDbContext dbContext,
        int blogId,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(dbContext);

        return await dbContext.Blogs
            .AsNoTracking()
            .Include(blog => blog.Posts)
            .SingleOrDefaultAsync(blog => blog.Id == blogId, cancellationToken);
    }

    /// <summary>
    /// トランザクション内で記事を論理削除します。
    /// </summary>
    /// <param name="dbContext">ブログ DbContext。</param>
    /// <param name="postId">記事 ID。</param>
    /// <param name="cancellationToken">キャンセル通知。</param>
    /// <returns>削除できた場合は <see langword="true"/>。</returns>
    /// <exception cref="ArgumentNullException"><paramref name="dbContext"/> が <see langword="null"/> の場合。</exception>
    public static async Task<bool> SoftDeletePostInTransactionAsync(
        SampleBlogDbContext dbContext,
        int postId,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(dbContext);

        await using var transaction = await dbContext.Database.BeginTransactionAsync(cancellationToken);
        var post = await dbContext.Posts
            .IgnoreQueryFilters()
            .SingleOrDefaultAsync(candidate => candidate.Id == postId, cancellationToken);

        if (post is null || post.IsDeleted)
        {
            return false;
        }

        post.IsDeleted = true;
        await dbContext.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        return true;
    }
}
