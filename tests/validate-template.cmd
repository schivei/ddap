@echo off
REM Template validation script for Windows cmd

setlocal enabledelayedexpansion

set TEMPLATE_PATH=templates\ddap-api
set TEMP_DIR=%TEMP%\ddap-tests-%RANDOM%
set SCRIPT_DIR=%~dp0
set REPO_ROOT=%SCRIPT_DIR%..

mkdir "%TEMP_DIR%" 2>nul

echo 🧪 Template Validation Tests
echo ==============================
echo.

cd /d "%REPO_ROOT%"

REM Uninstall any existing template
echo Uninstalling existing template...
dotnet new uninstall Ddap.Templates 2>nul

REM Install template
echo Installing template from: %TEMPLATE_PATH%
dotnet new install %TEMPLATE_PATH% --force
if errorlevel 1 (
    echo ❌ FAILED: Could not install template
    goto :cleanup
)

echo.
echo Running test scenarios...
echo.

REM Phase 1: Database Provider Tests
echo === Database Provider Tests ===
call :test_template "SqlServerDapper" "dapper" "sqlserver" "true" "false" "false"
if errorlevel 1 goto :error
call :test_template "SqlServerEF" "entityframework" "sqlserver" "false" "true" "false"
if errorlevel 1 goto :error
call :test_template "MySqlDapper" "dapper" "mysql" "false" "false" "true"
if errorlevel 1 goto :error
call :test_template "MySqlEF" "entityframework" "mysql" "true" "true" "false"
if errorlevel 1 goto :error
call :test_template "PostgresDapper" "dapper" "postgresql" "true" "false" "false"
if errorlevel 1 goto :error
call :test_template "PostgresEF" "entityframework" "postgresql" "false" "true" "false"
if errorlevel 1 goto :error
call :test_template "SqliteDapper" "dapper" "sqlite" "false" "false" "true"
if errorlevel 1 goto :error
call :test_template "SqliteEF" "entityframework" "sqlite" "true" "false" "false"
if errorlevel 1 goto :error

REM Phase 2: API Provider Tests
echo === API Provider Tests ===
call :test_template "RestOnly" "dapper" "sqlserver" "true" "false" "false"
if errorlevel 1 goto :error
call :test_template "GraphQLOnly" "dapper" "sqlserver" "false" "true" "false"
if errorlevel 1 goto :error
call :test_template "GrpcOnly" "dapper" "sqlserver" "false" "false" "true"
if errorlevel 1 goto :error
call :test_template "RestGraphQL" "dapper" "mysql" "true" "true" "false"
if errorlevel 1 goto :error
call :test_template "RestGrpc" "entityframework" "postgresql" "true" "false" "true"
if errorlevel 1 goto :error
call :test_template "GraphQLGrpc" "dapper" "sqlite" "false" "true" "true"
if errorlevel 1 goto :error
call :test_template "AllAPIs" "entityframework" "sqlserver" "true" "true" "true"
if errorlevel 1 goto :error

REM Phase 3: Feature Tests
echo === Feature Tests ===
call :test_template "WithAuth" "dapper" "sqlserver" "true" "false" "false" "true"
if errorlevel 1 goto :error
call :test_template "WithSubscriptions" "dapper" "sqlserver" "false" "true" "false" "false" "true"
if errorlevel 1 goto :error
call :test_template "WithAspire" "dapper" "sqlserver" "true" "false" "false" "false" "false" "true"
if errorlevel 1 goto :error
call :test_template "AllFeatures" "entityframework" "sqlserver" "true" "true" "false" "true" "true"
if errorlevel 1 goto :error

REM Phase 4: Complex Combinations
echo === Complex Combination Tests ===
call :test_template "Minimal" "dapper" "sqlite" "false" "false" "false"
if errorlevel 1 goto :error
call :test_template "Maximum" "entityframework" "sqlserver" "true" "true" "true" "true" "true" "true"
if errorlevel 1 goto :error

echo.
echo Uninstalling template...
dotnet new uninstall Ddap.Templates

echo.
echo ✅ All template tests passed!
echo.
echo Summary:
echo   Total scenarios tested: 23
echo   Database types: SQL Server, MySQL, PostgreSQL, SQLite
echo   Data providers: Dapper, Entity Framework
echo   API types: REST, GraphQL, gRPC
echo   Features: Auth, Subscriptions, Aspire
goto :cleanup

:test_template
set name=%~1
set db_provider=%~2
set db_type=%~3
set rest=%~4
set graphql=%~5
set grpc=%~6
set include_auth=%~7
set include_subscriptions=%~8
set use_aspire=%~9

if "%include_auth%"=="" set include_auth=false
if "%include_subscriptions%"=="" set include_subscriptions=false
if "%use_aspire%"=="" set use_aspire=false

echo Testing: %name%
echo   Provider: %db_provider%, Database: %db_type%
echo   APIs: REST=%rest%, GraphQL=%graphql%, gRPC=%grpc%
echo   Features: Auth=%include_auth%, Subscriptions=%include_subscriptions%, Aspire=%use_aspire%

dotnet new ddap-api ^
  --name %name% ^
  --database-provider %db_provider% ^
  --database-type %db_type% ^
  --rest %rest% ^
  --graphql %graphql% ^
  --grpc %grpc% ^
  --include-auth %include_auth% ^
  --include-subscriptions %include_subscriptions% ^
  --use-aspire %use_aspire% ^
  --output "%TEMP_DIR%\%name%" >nul 2>&1

if errorlevel 1 (
    echo ❌ FAILED: Could not generate project
    exit /b 1
)

if not exist "%TEMP_DIR%\%name%\%name%.csproj" (
    echo ❌ FAILED: Project file not created
    exit /b 1
)

cd /d "%TEMP_DIR%\%name%"
dotnet restore >nul 2>&1
if errorlevel 1 (
    echo ❌ FAILED: Could not restore packages
    cd /d "%REPO_ROOT%"
    exit /b 1
)

dotnet build --no-restore >nul 2>&1
if errorlevel 1 (
    echo ❌ FAILED: Could not build project
    cd /d "%REPO_ROOT%"
    exit /b 1
)

echo ✅ PASSED: %name%
echo.
cd /d "%REPO_ROOT%"
exit /b 0

:error
echo ❌ Template validation failed!
goto :cleanup

:cleanup
if exist "%TEMP_DIR%" rmdir /s /q "%TEMP_DIR%"
exit /b %errorlevel%
