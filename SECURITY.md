# Security Policy

## Supported Versions

We release security updates for the following versions of DDAP:

| Version | Supported          |
| ------- | ------------------ |
| 1.x     | :white_check_mark: |
| < 1.0   | :x:                |

## Reporting a Vulnerability

**Please do not report security vulnerabilities through public GitHub issues.**

If you discover a security vulnerability in DDAP, please report it responsibly:

### How to Report

1. **Email**: Send details to [schivei@users.noreply.github.com](mailto:schivei@users.noreply.github.com)
   - Use subject line: "SECURITY: [Brief Description]"
   
2. **Include**:
   - Description of the vulnerability
   - Steps to reproduce the issue
   - Potential impact
   - Suggested fix (if you have one)
   - Your contact information

3. **Do NOT**:
   - Disclose the vulnerability publicly before we've had a chance to address it
   - Test the vulnerability on production systems you don't own

### What to Expect

- **Initial Response**: Within 48 hours of submission
- **Assessment**: We'll assess the vulnerability and determine severity
- **Updates**: Regular updates on the progress of fixing the issue
- **Resolution**: We aim to release a patch within 30 days for critical vulnerabilities
- **Credit**: You'll be credited in the security advisory (unless you prefer to remain anonymous)

### Disclosure Policy

- We request a 90-day disclosure embargo to allow time for patching
- Once fixed, we'll publish a security advisory
- You're welcome to publish your findings after the advisory is released

## Security Best Practices for Users

### Connection Strings

Never hard-code connection strings or credentials:

```csharp
// ❌ BAD: Hard-coded credentials
var connectionString = "Server=myserver;Database=mydb;User Id=sa;Password=MyPassword123;";

// ✅ GOOD: Use configuration
var connectionString = configuration.GetConnectionString("DefaultConnection");
```

### SQL Injection Prevention

DDAP uses parameterized queries by default, but always verify:

```csharp
// ✅ GOOD: Parameterized query (safe)
var entity = await connection.QueryFirstOrDefaultAsync<Entity>(
    "SELECT * FROM Entities WHERE Id = @Id",
    new { Id = id }
);

// ❌ BAD: String concatenation (vulnerable)
var entity = await connection.QueryFirstOrDefaultAsync<Entity>(
    $"SELECT * FROM Entities WHERE Id = {id}"
);
```

### Authentication & Authorization

When using DDAP with authentication:

```csharp
// Always validate user permissions before database operations
services.AddAuthorization(options =>
{
    options.AddPolicy("CanReadEntities", policy =>
        policy.RequireAuthenticatedUser()
              .RequireClaim("permission", "entities.read"));
});
```

### Input Validation

Always validate and sanitize user input:

```csharp
public class CreateEntityRequest
{
    [Required]
    [StringLength(100, MinimumLength = 1)]
    public string Name { get; set; }
    
    [Range(0, int.MaxValue)]
    public int Value { get; set; }
}
```

### Dependency Management

- Keep DDAP and all dependencies up to date
- Review Dependabot pull requests promptly
- Monitor security advisories

### Logging

Never log sensitive information:

```csharp
// ❌ BAD: Logs sensitive data
_logger.LogInformation($"User login: {email}, Password: {password}");

// ✅ GOOD: Logs safely
_logger.LogInformation($"User login attempt for: {email}");
```

## Known Security Considerations

### Database Access

DDAP provides direct database access. Applications using DDAP must:

1. **Implement proper authorization** - DDAP doesn't enforce permissions
2. **Validate all input** - Prevent SQL injection and other attacks
3. **Use secure connections** - Enable SSL/TLS for database connections
4. **Follow least privilege** - Database users should have minimal permissions

### API Exposure

When exposing DDAP-generated APIs:

1. **Enable authentication** - Don't expose APIs publicly without auth
2. **Use HTTPS** - Always encrypt data in transit
3. **Implement rate limiting** - Prevent abuse
4. **Validate input** - Don't trust client data

## Security Updates

Security updates are released as:

- **Critical**: Immediate patch release (1.x.y → 1.x.y+1)
- **High**: Patch in next minor release
- **Medium/Low**: Included in regular releases

Subscribe to:
- GitHub Security Advisories for this repository
- GitHub Watch notifications
- Release notes

## Security Features in DDAP

### Built-in Protections

1. **Parameterized Queries**: All database operations use parameters by default
2. **Connection Pooling**: Secure connection management
3. **No Eval/Dynamic SQL**: No dynamic SQL generation from user input
4. **Type Safety**: Strong typing prevents many injection attacks

### Recommended Additions

We recommend using DDAP with:

- **Authentication**: ASP.NET Core Identity, JWT, OAuth
- **Authorization**: Policy-based authorization
- **Rate Limiting**: AspNetCoreRateLimit
- **WAF**: Web Application Firewall in production
- **Monitoring**: Application Insights, Serilog

## Acknowledgments

We thank the security research community for responsible disclosure and helping keep DDAP secure.

---

**Last Updated**: January 2026  
**Contact**: schivei@users.noreply.github.com
