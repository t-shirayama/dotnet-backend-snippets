using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace DotnetBackendSnippets.EntityFrameworkCore;

public sealed class SampleBlogDbContext(DbContextOptions<SampleBlogDbContext> options) : DbContext(options)
{
    public DbSet<Blog> Blogs => Set<Blog>();

    public DbSet<BlogPost> Posts => Set<BlogPost>();

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

public sealed class Blog
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public ICollection<BlogPost> Posts { get; } = new List<BlogPost>();
}

public sealed class BlogPost
{
    public int Id { get; set; }

    public int BlogId { get; set; }

    public Blog? Blog { get; set; }

    public string Title { get; set; } = string.Empty;

    public DateTimeOffset PublishedAt { get; set; }

    public int ViewCount { get; set; }

    public bool IsDeleted { get; set; }
}

public sealed record BlogPostListItem(int Id, string Title, DateTimeOffset PublishedAt, int ViewCount);

public sealed record BlogSummary(int Id, string Name, int PublishedPostCount);

public sealed record PagedResult<T>(IReadOnlyList<T> Items, int PageNumber, int PageSize, int TotalCount);

public static class EfCoreSamples
{
    public static IServiceCollection AddSampleBlogDbContext(
        this IServiceCollection services,
        Action<DbContextOptionsBuilder> configureOptions)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configureOptions);

        services.AddDbContext<SampleBlogDbContext>(configureOptions);
        return services;
    }

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
