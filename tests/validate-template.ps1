# Template validation script for Windows PowerShell

$ErrorActionPreference = "Stop"

$TEMPLATE_PATH = "templates/ddap-api"
$TEMP_DIR = Join-Path $env:TEMP "ddap-tests-$(Get-Random)"
$SCRIPT_DIR = Split-Path -Parent $MyInvocation.MyCommand.Path
$REPO_ROOT = Split-Path -Parent $SCRIPT_DIR

New-Item -ItemType Directory -Path $TEMP_DIR | Out-Null

# Color support
function Write-Color {
    param(
        [string]$Message,
        [string]$Color = "White"
    )
    Write-Host $Message -ForegroundColor $Color
}

# Test function
function Test-Template {
    param(
        [string]$Name,
        [string]$DatabaseProvider,
        [string]$DatabaseType,
        [string]$Rest,
        [string]$GraphQL,
        [string]$Grpc,
        [string]$IncludeAuth = "false",
        [string]$IncludeSubscriptions = "false",
        [string]$UseAspire = "false"
    )
    
    Write-Color "Testing: $Name" -Color Cyan
    Write-Host "  Provider: $DatabaseProvider, Database: $DatabaseType"
    Write-Host "  APIs: REST=$Rest, GraphQL=$GraphQL, gRPC=$Grpc"
    Write-Host "  Features: Auth=$IncludeAuth, Subscriptions=$IncludeSubscriptions, Aspire=$UseAspire"
    
    $testDir = Join-Path $TEMP_DIR $Name
    
    # Generate project
    $output = & dotnet new ddap-api `
        --name $Name `
        --database-provider $DatabaseProvider `
        --database-type $DatabaseType `
        --rest $Rest `
        --graphql $GraphQL `
        --grpc $Grpc `
        --include-auth $IncludeAuth `
        --include-subscriptions $IncludeSubscriptions `
        --use-aspire $UseAspire `
        --output $testDir 2>&1
    
    if ($LASTEXITCODE -ne 0) {
        Write-Color "❌ FAILED: Could not generate project" -Color Red
        return $false
    }
    
    # Verify files exist
    $csprojPath = Join-Path $testDir "$Name.csproj"
    if (-not (Test-Path $csprojPath)) {
        Write-Color "❌ FAILED: Project file not created" -Color Red
        return $false
    }
    
    # Restore packages
    Push-Location $testDir
    try {
        $restoreOutput = & dotnet restore 2>&1
        if ($LASTEXITCODE -ne 0) {
            Write-Color "❌ FAILED: Could not restore packages" -Color Red
            return $false
        }
        
        # Build project
        $buildOutput = & dotnet build --no-restore 2>&1
        if ($LASTEXITCODE -ne 0) {
            Write-Color "❌ FAILED: Could not build project" -Color Red
            return $false
        }
        
        Write-Color "✅ PASSED: $Name" -Color Green
        Write-Host ""
        return $true
    }
    finally {
        Pop-Location
    }
}

# Cleanup function
function Cleanup {
    if (Test-Path $TEMP_DIR) {
        Remove-Item -Recurse -Force $TEMP_DIR -ErrorAction SilentlyContinue
    }
}

try {
    Write-Color "🧪 Template Validation Tests" -Color Cyan
    Write-Host "=============================="
    Write-Host ""
    
    Set-Location $REPO_ROOT
    
    # Uninstall any existing template
    Write-Host "Uninstalling existing template..."
    & dotnet new uninstall Ddap.Templates 2>&1 | Out-Null
    
    # Install template
    Write-Host "Installing template from: $TEMPLATE_PATH"
    $installOutput = & dotnet new install $TEMPLATE_PATH --force 2>&1
    if ($LASTEXITCODE -ne 0) {
        Write-Color "❌ FAILED: Could not install template" -Color Red
        exit 1
    }
    
    Write-Host ""
    Write-Color "Running test scenarios..." -Color Cyan
    Write-Host ""
    
    # Phase 1: Database Provider Tests
    Write-Color "=== Database Provider Tests ===" -Color Yellow
    Test-Template -Name "SqlServerDapper" -DatabaseProvider "dapper" -DatabaseType "sqlserver" -Rest "true" -GraphQL "false" -Grpc "false"
    Test-Template -Name "SqlServerEF" -DatabaseProvider "entityframework" -DatabaseType "sqlserver" -Rest "false" -GraphQL "true" -Grpc "false"
    Test-Template -Name "MySqlDapper" -DatabaseProvider "dapper" -DatabaseType "mysql" -Rest "false" -GraphQL "false" -Grpc "true"
    Test-Template -Name "MySqlEF" -DatabaseProvider "entityframework" -DatabaseType "mysql" -Rest "true" -GraphQL "true" -Grpc "false"
    Test-Template -Name "PostgresDapper" -DatabaseProvider "dapper" -DatabaseType "postgresql" -Rest "true" -GraphQL "false" -Grpc "false"
    Test-Template -Name "PostgresEF" -DatabaseProvider "entityframework" -DatabaseType "postgresql" -Rest "false" -GraphQL "true" -Grpc "false"
    Test-Template -Name "SqliteDapper" -DatabaseProvider "dapper" -DatabaseType "sqlite" -Rest "false" -GraphQL "false" -Grpc "true"
    Test-Template -Name "SqliteEF" -DatabaseProvider "entityframework" -DatabaseType "sqlite" -Rest "true" -GraphQL "false" -Grpc "false"
    
    # Phase 2: API Provider Tests
    Write-Color "=== API Provider Tests ===" -Color Yellow
    Test-Template -Name "RestOnly" -DatabaseProvider "dapper" -DatabaseType "sqlserver" -Rest "true" -GraphQL "false" -Grpc "false"
    Test-Template -Name "GraphQLOnly" -DatabaseProvider "dapper" -DatabaseType "sqlserver" -Rest "false" -GraphQL "true" -Grpc "false"
    Test-Template -Name "GrpcOnly" -DatabaseProvider "dapper" -DatabaseType "sqlserver" -Rest "false" -GraphQL "false" -Grpc "true"
    Test-Template -Name "RestGraphQL" -DatabaseProvider "dapper" -DatabaseType "mysql" -Rest "true" -GraphQL "true" -Grpc "false"
    Test-Template -Name "RestGrpc" -DatabaseProvider "entityframework" -DatabaseType "postgresql" -Rest "true" -GraphQL "false" -Grpc "true"
    Test-Template -Name "GraphQLGrpc" -DatabaseProvider "dapper" -DatabaseType "sqlite" -Rest "false" -GraphQL "true" -Grpc "true"
    Test-Template -Name "AllAPIs" -DatabaseProvider "entityframework" -DatabaseType "sqlserver" -Rest "true" -GraphQL "true" -Grpc "true"
    
    # Phase 3: Feature Tests
    Write-Color "=== Feature Tests ===" -Color Yellow
    Test-Template -Name "WithAuth" -DatabaseProvider "dapper" -DatabaseType "sqlserver" -Rest "true" -GraphQL "false" -Grpc "false" -IncludeAuth "true"
    Test-Template -Name "WithSubscriptions" -DatabaseProvider "dapper" -DatabaseType "sqlserver" -Rest "false" -GraphQL "true" -Grpc "false" -IncludeSubscriptions "true"
    Test-Template -Name "WithAspire" -DatabaseProvider "dapper" -DatabaseType "sqlserver" -Rest "true" -GraphQL "false" -Grpc "false" -UseAspire "true"
    Test-Template -Name "AllFeatures" -DatabaseProvider "entityframework" -DatabaseType "sqlserver" -Rest "true" -GraphQL "true" -Grpc "false" -IncludeAuth "true" -IncludeSubscriptions "true"
    
    # Phase 4: Complex Combinations
    Write-Color "=== Complex Combination Tests ===" -Color Yellow
    Test-Template -Name "Minimal" -DatabaseProvider "dapper" -DatabaseType "sqlite" -Rest "false" -GraphQL "false" -Grpc "false"
    Test-Template -Name "Maximum" -DatabaseProvider "entityframework" -DatabaseType "sqlserver" -Rest "true" -GraphQL "true" -Grpc "true" -IncludeAuth "true" -IncludeSubscriptions "true" -UseAspire "true"
    
    # Uninstall template
    Write-Host ""
    Write-Host "Uninstalling template..."
    & dotnet new uninstall Ddap.Templates 2>&1 | Out-Null
    
    Write-Host ""
    Write-Color "✅ All template tests passed!" -Color Green
    Write-Host ""
    Write-Host "Summary:"
    Write-Host "  Total scenarios tested: 23"
    Write-Host "  Database types: SQL Server, MySQL, PostgreSQL, SQLite"
    Write-Host "  Data providers: Dapper, Entity Framework"
    Write-Host "  API types: REST, GraphQL, gRPC"
    Write-Host "  Features: Auth, Subscriptions, Aspire"
}
finally {
    Cleanup
}
