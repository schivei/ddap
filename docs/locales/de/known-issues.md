# Known Issues

This document lists current known limitations and issues with DDAP that users should be aware of.

## Current Limitations

### 1. .NET 10 Requirement

**Issue:** DDAP requires .NET 10 SDK.

**Status:** By design - DDAP uses latest .NET features for performance and developer experience.

**Workaround:** Install .NET 10 SDK from https://dotnet.microsoft.com/download

---

### 2. Primary Keys Required

**Issue:** Tables without primary keys are not discoverable by DDAP.

**Status:** By design - DDAP requires primary keys for entity identification.

**Workaround:** Add primary keys to all tables you want exposed via the API.

```sql
-- Add primary key to existing table
ALTER TABLE MyTable ADD CONSTRAINT PK_MyTable PRIMARY KEY (Id);
```

---

### 3. Package Migration (v0.x to v1.0+)

**Issue:** Database-specific Dapper packages (`Ddap.Data.Dapper.SqlServer`, etc.) no longer exist.

**Status:** Intentional architecture change - Following "Developer in Control" philosophy.

**Migration:**

**Old** (v0.x):
```xml
<PackageReference Include="Ddap.Data.Dapper.SqlServer" Version="0.x" />
```

**New** (v1.0+):
```xml
<PackageReference Include="Ddap.Data.Dapper" Version="1.*" />
<PackageReference Include="Microsoft.Data.SqlClient" Version="5.*" />
```

See [Troubleshooting - Migration from Old API](troubleshooting.md#migration-from-old-api) for complete migration guide.

---

### 4. Auto-Reload Performance

**Issue:** Schema reload may cause brief service disruption or increased memory usage.

**Status:** Known - Trade-off between zero-downtime and resource usage.

**Workarounds:**

1. **For zero downtime:**
   ```csharp
   options.AutoReload.Behavior = ReloadBehavior.ServeOldSchema;
   ```

2. **For minimal memory:**
   ```csharp
   options.AutoReload.Strategy = ReloadStrategy.HotReload;
   ```

3. **Increase idle timeout to reduce reload frequency:**
   ```csharp
   options.AutoReload.IdleTimeout = TimeSpan.FromMinutes(15);
   ```

See [Auto-Reload Documentation](auto-reload.md) for details.

---

### 5. Template Generation - Aspire Support

**Issue:** Templates with Aspire integration may require additional manual configuration.

**Status:** Known - Aspire integration is complex and may need customization.

**Workaround:** Follow [Templates Documentation](templates.md) for Aspire-specific setup instructions.

---

## Reporting Issues

Found a bug or limitation not listed here?

1. **Check:** [GitHub Issues](https://github.com/schivei/ddap/issues)
2. **Search:** [Troubleshooting Guide](troubleshooting.md)
3. **Report:** [Open a new issue](https://github.com/schivei/ddap/issues/new)

When reporting, include:
- DDAP version (`Ddap.Core` package version)
- .NET version (`dotnet --version`)
- Database type and version
- Error message and stack trace
- Minimal reproduction code

---

## Fixed Issues

Issues that have been resolved in recent versions:

### ✅ MySQL Provider Choice (Fixed in 1.0.0)

**Was:** Template forced `Pomelo.EntityFrameworkCore.MySql`.

**Now:** Uses official `MySql.EntityFrameworkCore` by default, with Pomelo as documented alternative.

User maintains full control over MySQL provider choice.

---

### ✅ Non-Existent Packages (Fixed in 1.0.0)

**Was:** Templates referenced non-existent packages like `Ddap.Data.Dapper.SqlServer`.

**Now:** Uses base `Ddap.Data.Dapper` package with official database drivers.

All templates generate correctly with proper package references.

---

## Next Steps

- [Troubleshooting](troubleshooting.md) - Detailed solutions for common issues
- [Get Started](get-started.md) - Begin using DDAP
- [Philosophy](philosophy.md) - Understanding DDAP's "Developer in Control" approach
- [GitHub Issues](https://github.com/schivei/ddap/issues) - Report bugs and request features
