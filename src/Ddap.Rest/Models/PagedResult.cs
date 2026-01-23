namespace Ddap.Rest.Models;

/// <summary>
/// Represents a paginated result set with metadata.
/// </summary>
/// <typeparam name="T">The type of items in the result.</typeparam>
/// <example>
/// <code>
/// public async Task&lt;PagedResult&lt;User&gt;&gt; GetUsersAsync(QueryParameters parameters)
/// {
///     var query = _context.Users.AsQueryable();
///     
///     // Apply filtering
///     if (!string.IsNullOrEmpty(parameters.Filter))
///     {
///         query = QueryFilterBuilder.ApplyFilter(query, parameters.Filter);
///     }
///     
///     // Get total count
///     var totalCount = await query.CountAsync();
///     
///     // Apply sorting and pagination
///     var items = await query
///         .OrderBy(parameters.OrderBy ?? "id")
///         .Skip((parameters.PageNumber - 1) * parameters.PageSize)
///         .Take(parameters.PageSize)
///         .ToListAsync();
///     
///     return new PagedResult&lt;User&gt;(items, totalCount, parameters.PageNumber, parameters.PageSize);
/// }
/// </code>
/// </example>
public class PagedResult<T>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PagedResult{T}"/> class.
    /// </summary>
    /// <param name="items">The items for the current page.</param>
    /// <param name="totalCount">The total count of items across all pages.</param>
    /// <param name="pageNumber">The current page number.</param>
    /// <param name="pageSize">The page size.</param>
    public PagedResult(IEnumerable<T> items, int totalCount, int pageNumber, int pageSize)
    {
        Items = items;
        TotalCount = totalCount;
        PageNumber = pageNumber;
        PageSize = pageSize;
        TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
        HasPrevious = pageNumber > 1;
        HasNext = pageNumber < TotalPages;
    }

    /// <summary>
    /// Gets the items for the current page.
    /// </summary>
    public IEnumerable<T> Items { get; }

    /// <summary>
    /// Gets the total count of items across all pages.
    /// </summary>
    public int TotalCount { get; }

    /// <summary>
    /// Gets the current page number (1-based).
    /// </summary>
    public int PageNumber { get; }

    /// <summary>
    /// Gets the page size.
    /// </summary>
    public int PageSize { get; }

    /// <summary>
    /// Gets the total number of pages.
    /// </summary>
    public int TotalPages { get; }

    /// <summary>
    /// Gets a value indicating whether there is a previous page.
    /// </summary>
    public bool HasPrevious { get; }

    /// <summary>
    /// Gets a value indicating whether there is a next page.
    /// </summary>
    public bool HasNext { get; }
}
