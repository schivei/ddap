namespace Ddap.Rest.Models;

/// <summary>
/// Represents query parameters for filtering, sorting, and pagination.
/// </summary>
/// <example>
/// <code>
/// // Query string example:
/// // GET /api/entity?pageNumber=1&amp;pageSize=20&amp;filter=status eq 'active'&amp;orderBy=createdDate desc
///
/// // Usage in controller:
/// [HttpGet]
/// public async Task&lt;IActionResult&gt; GetEntities([FromQuery] QueryParameters parameters)
/// {
///     var result = await _service.GetPagedAsync(parameters);
///     return Ok(result);
/// }
/// </code>
/// </example>
public class QueryParameters
{
    private int _pageSize = 10;
    private const int MaxPageSize = 100;

    /// <summary>
    /// Gets or sets the page number (1-based).
    /// </summary>
    /// <value>The page number. Default is 1.</value>
    public int PageNumber { get; set; } = 1;

    /// <summary>
    /// Gets or sets the page size (maximum items per page).
    /// </summary>
    /// <value>The page size. Default is 10, maximum is 100.</value>
    public int PageSize
    {
        get => _pageSize;
        set => _pageSize = value > MaxPageSize ? MaxPageSize : value;
    }

    /// <summary>
    /// Gets or sets the filter expression using OData-like syntax.
    /// </summary>
    /// <value>The filter expression.</value>
    /// <example>
    /// <code>
    /// // Simple filters:
    /// "name eq 'John'"
    /// "age gt 18"
    /// "status ne 'inactive'"
    ///
    /// // Complex filters with AND/OR:
    /// "name eq 'John' and age gt 18"
    /// "status eq 'active' or status eq 'pending'"
    ///
    /// // Contains operation:
    /// "name contains 'Smith'"
    /// </code>
    /// </example>
    public string? Filter { get; set; }

    /// <summary>
    /// Gets or sets the sort expression.
    /// </summary>
    /// <value>The sort expression with field name and optional direction (asc/desc).</value>
    /// <example>
    /// <code>
    /// "name"           // ascending by default
    /// "name asc"       // ascending explicitly
    /// "createdDate desc" // descending
    /// "name asc, age desc" // multiple fields
    /// </code>
    /// </example>
    public string? OrderBy { get; set; }
}
