# DdapApi

This project was generated using the DDAP templates.

## Configuration

<!--#if (UseDapper) -->
**Database Provider:** Dapper (lightweight, high-performance)
<!--#endif -->
<!--#if (UseEntityFramework) -->
**Database Provider:** Entity Framework Core (full ORM with LINQ support)
<!--#endif -->

<!--#if (UseSqlServer) -->
**Database:** Microsoft SQL Server
<!--#endif -->
<!--#if (UseMySQL) -->
**Database:** MySQL
<!--#endif -->
<!--#if (UsePostgreSQL) -->
**Database:** PostgreSQL
<!--#endif -->
<!--#if (UseSQLite) -->
**Database:** SQLite
<!--#endif -->

**API Providers:**
<!--#if (IncludeRest) -->
- REST API (available at `/api/entity`)
<!--#endif -->
<!--#if (IncludeGraphQL) -->
- GraphQL (available at `/graphql`)
<!--#endif -->
<!--#if (IncludeGrpc) -->
- gRPC services
<!--#endif -->

<!--#if (include-auth) -->
**Authentication:** JWT authentication enabled
<!--#endif -->
<!--#if (include-subscriptions) -->
**Subscriptions:** Real-time subscriptions enabled
<!--#endif -->
<!--#if (use-aspire) -->
**Aspire:** .NET Aspire orchestration enabled
<!--#endif -->

## Getting Started

### 1. Set up the database connection

For security, use User Secrets to store your connection string:

```bash
dotnet user-secrets init
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "your-connection-string"
```

<!--#if (UseSqlServer) -->
Example connection string for SQL Server:
```
Server=localhost;Database=DdapDb;Integrated Security=true;TrustServerCertificate=true;
```
<!--#endif -->
<!--#if (UseMySQL) -->
Example connection string for MySQL:
```
Server=localhost;Database=DdapDb;User=root;Password=secret;
```
<!--#endif -->
<!--#if (UsePostgreSQL) -->
Example connection string for PostgreSQL:
```
Host=localhost;Database=DdapDb;Username=postgres;Password=secret;
```
<!--#endif -->
<!--#if (UseSQLite) -->
The SQLite database will be created automatically at `ddap.db`.
<!--#endif -->

<!--#if (include-auth) -->
### 2. Configure JWT Secret

Set your JWT secret key using User Secrets:

```bash
dotnet user-secrets set "Jwt:SecretKey" "your-256-bit-secret-key-here-must-be-32-chars"
```

<!--#endif -->
### <!--#if (include-auth) -->3<!--#else -->2<!--#endif -->. Run the application

<!--#if (use-aspire) -->
Run using the Aspire AppHost:

```bash
cd ../DdapApi.AppHost
dotnet run
```

The Aspire dashboard will open at `http://localhost:15888`.
<!--#else -->
```bash
dotnet run
```
<!--#endif -->

### <!--#if (include-auth) -->4<!--#else -->3<!--#endif -->. Test the API

<!--#if (IncludeRest) -->
**REST API:**
```bash
curl http://localhost:5000/api/entity
```
<!--#endif -->

<!--#if (IncludeGraphQL) -->
**GraphQL:**

Open `http://localhost:5000/graphql` in your browser for the GraphQL playground, or use curl:

```bash
curl -X POST http://localhost:5000/graphql \
  -H "Content-Type: application/json" \
  -d '{"query": "{ entities { name propertyCount } }"}'
```
<!--#endif -->

<!--#if (include-auth) -->
**Authentication:**

Get a JWT token:
```bash
curl -X POST http://localhost:5000/auth/token \
  -H "Content-Type: application/json" \
  -d '{"username": "admin", "password": "password"}'
```
<!--#endif -->

## Next Steps

- Customize the generated code to fit your needs
- Add your own endpoints and business logic
- Configure additional DDAP options in `Program.cs`
- Read the [DDAP documentation](https://schivei.github.io/ddap) for advanced features

## Resources

- [DDAP Documentation](https://schivei.github.io/ddap)
- [GitHub Repository](https://github.com/schivei/ddap)
- [API Providers Guide](https://schivei.github.io/ddap/api-providers.html)
- [Database Providers Guide](https://schivei.github.io/ddap/database-providers.html)
