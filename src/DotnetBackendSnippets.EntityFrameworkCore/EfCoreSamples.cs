using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Storage;
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
            entity.HasIndex(blog => blog.Name).IsUnique();

            entity
                .HasMany(blog => blog.Posts)
                .WithOne(post => post.Blog)
                .HasForeignKey(post => post.BlogId)
                .IsRequired();
        });

        modelBuilder.Entity<BlogPost>(entity =>
        {
            entity.Property(post => post.Title).HasMaxLength(200).IsRequired();
            entity.Property(post => post.ConcurrencyStamp).HasMaxLength(40).IsConcurrencyToken();
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
    /// 楽観的同時実行制御に使う値を取得または設定します。
    /// </summary>
    /// <value>更新時に一致確認する concurrency token。</value>
    public string ConcurrencyStamp { get; set; } = Guid.NewGuid().ToString("N");

    /// <summary>
    /// 作成日時を取得または設定します。
    /// </summary>
    /// <value>UTC の作成日時。</value>
    public DateTimeOffset CreatedAt { get; set; }

    /// <summary>
    /// 更新日時を取得または設定します。
    /// </summary>
    /// <value>UTC の更新日時。</value>
    public DateTimeOffset UpdatedAt { get; set; }

    /// <summary>
    /// 削除日時を取得または設定します。
    /// </summary>
    /// <value>論理削除された日時。未削除の場合は <see langword="null"/>。</value>
    public DateTimeOffset? DeletedAt { get; set; }

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
/// EF CLI コマンドを安全に組み立てるための値を表します。
/// </summary>
/// <param name="FileName">実行ファイル名。</param>
/// <param name="Arguments">個別に渡す引数。</param>
public sealed record EfCliCommand(string FileName, IReadOnlyList<string> Arguments);

/// <summary>
/// transaction retry の設定を表します。
/// </summary>
/// <param name="MaxAttempts">最大試行回数。</param>
/// <param name="Delay">リトライ間の待機時間。</param>
public sealed record TransactionRetryOptions(int MaxAttempts, TimeSpan Delay);

/// <summary>
/// 記事タイトル更新の結果を表します。
/// </summary>
public enum PostTitleUpdateResult
{
    /// <summary>
    /// 更新に成功しました。
    /// </summary>
    Updated,

    /// <summary>
    /// 対象記事が見つかりません。
    /// </summary>
    NotFound,

    /// <summary>
    /// 楽観的同時実行制御の競合が発生しました。
    /// </summary>
    ConcurrencyConflict,
}

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

        var post = await dbContext.Posts
            .IgnoreQueryFilters()
            .SingleOrDefaultAsync(candidate => candidate.Id == postId, cancellationToken);

        if (post is null || post.IsDeleted)
        {
            return false;
        }

        await using var transaction = await dbContext.Database.BeginTransactionAsync(cancellationToken);

        post.IsDeleted = true;
        post.DeletedAt = DateTimeOffset.UtcNow;
        await dbContext.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        return true;
    }

    /// <summary>
    /// 記事タイトルを楽観的同時実行制御つきで更新します。
    /// </summary>
    /// <param name="dbContext">ブログ DbContext。</param>
    /// <param name="postId">記事 ID。</param>
    /// <param name="title">新しいタイトル。</param>
    /// <param name="expectedConcurrencyStamp">クライアントが読み取った concurrency token。</param>
    /// <param name="newConcurrencyStamp">保存時に設定する新しい concurrency token。</param>
    /// <param name="cancellationToken">キャンセル通知。</param>
    /// <returns>更新結果。</returns>
    /// <exception cref="ArgumentNullException"><paramref name="dbContext"/> が <see langword="null"/> の場合。</exception>
    /// <exception cref="ArgumentException"><paramref name="title"/>、<paramref name="expectedConcurrencyStamp"/>、<paramref name="newConcurrencyStamp"/> が空白の場合。</exception>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="newConcurrencyStamp"/> が 40 文字を超える場合。</exception>
    public static async Task<PostTitleUpdateResult> UpdatePostTitleWithConcurrencyAsync(
        SampleBlogDbContext dbContext,
        int postId,
        string title,
        string expectedConcurrencyStamp,
        string newConcurrencyStamp,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(dbContext);
        ArgumentException.ThrowIfNullOrWhiteSpace(title);
        ArgumentException.ThrowIfNullOrWhiteSpace(expectedConcurrencyStamp);
        ArgumentException.ThrowIfNullOrWhiteSpace(newConcurrencyStamp);

        if (newConcurrencyStamp.Length > 40)
        {
            throw new ArgumentOutOfRangeException(nameof(newConcurrencyStamp), "Concurrency stamp must be 40 characters or fewer.");
        }

        BlogPost? post = await dbContext.Posts.SingleOrDefaultAsync(
            candidate => candidate.Id == postId,
            cancellationToken);

        if (post is null)
        {
            return PostTitleUpdateResult.NotFound;
        }

        dbContext.Entry(post).Property(candidate => candidate.ConcurrencyStamp).OriginalValue = expectedConcurrencyStamp;
        post.Title = title.Trim();
        post.ConcurrencyStamp = newConcurrencyStamp;

        try
        {
            await dbContext.SaveChangesAsync(cancellationToken);
            return PostTitleUpdateResult.Updated;
        }
        catch (DbUpdateConcurrencyException)
        {
            return PostTitleUpdateResult.ConcurrencyConflict;
        }
    }

    /// <summary>
    /// 変更追跡中のエンティティへ監査日時を設定します。
    /// </summary>
    /// <param name="changeTracker">DbContext の change tracker。</param>
    /// <param name="utcNow">設定する UTC 日時。</param>
    /// <exception cref="ArgumentNullException"><paramref name="changeTracker"/> が <see langword="null"/> の場合。</exception>
    public static void ApplyAuditValues(ChangeTracker changeTracker, DateTimeOffset utcNow)
    {
        ArgumentNullException.ThrowIfNull(changeTracker);

        foreach (EntityEntry<BlogPost> entry in changeTracker.Entries<BlogPost>())
        {
            if (entry.State == EntityState.Added)
            {
                entry.Entity.CreatedAt = utcNow;
                entry.Entity.UpdatedAt = utcNow;
            }
            else if (entry.State == EntityState.Modified)
            {
                entry.Entity.UpdatedAt = utcNow;
            }
        }
    }

    /// <summary>
    /// EF Core の transaction 内処理を一時的な失敗だけ retry します。
    /// </summary>
    /// <typeparam name="T">戻り値の型。</typeparam>
    /// <param name="dbContext">ブログ DbContext。</param>
    /// <param name="operation">transaction 内で実行する処理。</param>
    /// <param name="isTransient">例外を一時的な失敗として扱うか判定する関数。</param>
    /// <param name="options">retry 設定。</param>
    /// <param name="delayAsync">待機処理の差し替え関数。</param>
    /// <param name="cancellationToken">キャンセル通知。</param>
    /// <returns>operation の戻り値。</returns>
    /// <exception cref="ArgumentNullException">必須引数が <see langword="null"/> の場合。</exception>
    /// <exception cref="ArgumentOutOfRangeException">最大試行回数が 1 未満、または待機時間が負の値の場合。</exception>
    public static async Task<T> ExecuteInTransactionWithRetryAsync<T>(
        SampleBlogDbContext dbContext,
        Func<SampleBlogDbContext, CancellationToken, Task<T>> operation,
        Func<Exception, bool> isTransient,
        TransactionRetryOptions options,
        Func<TimeSpan, CancellationToken, Task>? delayAsync = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(dbContext);
        ArgumentNullException.ThrowIfNull(operation);
        ArgumentNullException.ThrowIfNull(isTransient);
        ArgumentNullException.ThrowIfNull(options);

        if (options.MaxAttempts < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(options), "Max attempts must be positive.");
        }

        if (options.Delay < TimeSpan.Zero)
        {
            throw new ArgumentOutOfRangeException(nameof(options), "Delay must be zero or greater.");
        }

        delayAsync ??= Task.Delay;

        for (var attempt = 1; ; attempt++)
        {
            await using IDbContextTransaction transaction = await dbContext.Database.BeginTransactionAsync(cancellationToken);

            try
            {
                T result = await operation(dbContext, cancellationToken);
                await transaction.CommitAsync(cancellationToken);
                return result;
            }
            catch (Exception exception) when (attempt < options.MaxAttempts && isTransient(exception))
            {
                await transaction.RollbackAsync(cancellationToken);
                dbContext.ChangeTracker.Clear();
                await delayAsync(options.Delay, cancellationToken);
            }
        }
    }

    /// <summary>
    /// 保存例外が代表的な unique constraint violation かどうかを判定します。
    /// </summary>
    /// <param name="exception">EF Core の保存例外。</param>
    /// <returns>一意制約違反と判断できる場合は <see langword="true"/>。</returns>
    /// <exception cref="ArgumentNullException"><paramref name="exception"/> が <see langword="null"/> の場合。</exception>
    public static bool IsUniqueConstraintViolation(DbUpdateException exception)
    {
        ArgumentNullException.ThrowIfNull(exception);

        string message = exception.InnerException?.Message ?? exception.Message;

        return message.Contains("UNIQUE constraint failed", StringComparison.OrdinalIgnoreCase)
            || message.Contains("duplicate key", StringComparison.OrdinalIgnoreCase)
            || message.Contains("23505", StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// migration を追加する dotnet-ef コマンドを組み立てます。
    /// </summary>
    /// <param name="migrationName">migration 名。</param>
    /// <param name="projectPath">DbContext を含むプロジェクトパス。</param>
    /// <param name="startupProjectPath">起動プロジェクトパス。</param>
    /// <param name="contextName">DbContext 名。</param>
    /// <returns>実行ファイル名と引数を分けたコマンド。</returns>
    /// <exception cref="ArgumentException">いずれかの引数が空白の場合。</exception>
    public static EfCliCommand CreateAddMigrationCommand(
        string migrationName,
        string projectPath,
        string startupProjectPath,
        string contextName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(migrationName);
        ArgumentException.ThrowIfNullOrWhiteSpace(projectPath);
        ArgumentException.ThrowIfNullOrWhiteSpace(startupProjectPath);
        ArgumentException.ThrowIfNullOrWhiteSpace(contextName);

        return new EfCliCommand(
            "dotnet",
            [
                "ef",
                "migrations",
                "add",
                migrationName,
                "--project",
                projectPath,
                "--startup-project",
                startupProjectPath,
                "--context",
                contextName,
            ]);
    }

    /// <summary>
    /// migration bundle を作成する dotnet-ef コマンドを組み立てます。
    /// </summary>
    /// <param name="projectPath">DbContext を含むプロジェクトパス。</param>
    /// <param name="startupProjectPath">起動プロジェクトパス。</param>
    /// <param name="outputPath">bundle の出力パス。</param>
    /// <param name="selfContained">自己完結形式で出力する場合は <see langword="true"/>。</param>
    /// <returns>実行ファイル名と引数を分けたコマンド。</returns>
    /// <exception cref="ArgumentException">いずれかの path が空白の場合。</exception>
    public static EfCliCommand CreateMigrationBundleCommand(
        string projectPath,
        string startupProjectPath,
        string outputPath,
        bool selfContained = false)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(projectPath);
        ArgumentException.ThrowIfNullOrWhiteSpace(startupProjectPath);
        ArgumentException.ThrowIfNullOrWhiteSpace(outputPath);

        List<string> arguments =
        [
            "ef",
            "migrations",
            "bundle",
            "--project",
            projectPath,
            "--startup-project",
            startupProjectPath,
            "--output",
            outputPath,
        ];

        if (selfContained)
        {
            arguments.Add("--self-contained");
        }

        return new EfCliCommand("dotnet", arguments);
    }

    /// <summary>
    /// migration を DB に適用する dotnet-ef コマンドを組み立てます。
    /// </summary>
    /// <param name="projectPath">DbContext を含むプロジェクトパス。</param>
    /// <param name="startupProjectPath">起動プロジェクトパス。</param>
    /// <param name="contextName">DbContext 名。</param>
    /// <param name="connectionStringName">利用する connection string 名。</param>
    /// <returns>実行ファイル名と引数を分けたコマンド。</returns>
    /// <exception cref="ArgumentException">いずれかの引数が空白の場合。</exception>
    public static EfCliCommand CreateApplyMigrationCommand(
        string projectPath,
        string startupProjectPath,
        string contextName,
        string connectionStringName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(projectPath);
        ArgumentException.ThrowIfNullOrWhiteSpace(startupProjectPath);
        ArgumentException.ThrowIfNullOrWhiteSpace(contextName);
        ArgumentException.ThrowIfNullOrWhiteSpace(connectionStringName);

        return new EfCliCommand(
            "dotnet",
            [
                "ef",
                "database",
                "update",
                "--project",
                projectPath,
                "--startup-project",
                startupProjectPath,
                "--context",
                contextName,
                "--connection",
                $"Name=ConnectionStrings:{connectionStringName}",
            ]);
    }
}

/// <summary>
/// SaveChanges 時に監査カラムを設定する interceptor です。
/// </summary>
public sealed class AuditSaveChangesInterceptor : SaveChangesInterceptor
{
    private readonly TimeProvider timeProvider;

    /// <summary>
    /// <see cref="AuditSaveChangesInterceptor"/> クラスの新しいインスタンスを作成します。
    /// </summary>
    /// <param name="timeProvider">現在時刻の取得元。</param>
    /// <exception cref="ArgumentNullException"><paramref name="timeProvider"/> が <see langword="null"/> の場合。</exception>
    public AuditSaveChangesInterceptor(TimeProvider timeProvider)
    {
        ArgumentNullException.ThrowIfNull(timeProvider);

        this.timeProvider = timeProvider;
    }

    /// <inheritdoc />
    public override InterceptionResult<int> SavingChanges(
        DbContextEventData eventData,
        InterceptionResult<int> result)
    {
        ApplyAuditValues(eventData.Context);
        return base.SavingChanges(eventData, result);
    }

    /// <inheritdoc />
    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        ApplyAuditValues(eventData.Context);
        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    private void ApplyAuditValues(DbContext? dbContext)
    {
        if (dbContext is null)
        {
            return;
        }

        EfCoreSamples.ApplyAuditValues(dbContext.ChangeTracker, timeProvider.GetUtcNow());
    }
}
