#!/bin/bash
# Template validation script for Linux/Mac

set -e

TEMPLATE_PATH="templates/ddap-api"
TEMP_DIR=$(mktemp -d)
SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
REPO_ROOT="$(cd "$SCRIPT_DIR/.." && pwd)"

function cleanup {
  echo "Cleaning up temporary directory..."
  rm -rf "$TEMP_DIR"
}
trap cleanup EXIT

# Color codes for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
CYAN='\033[0;36m'
NC='\033[0m' # No Color

# Test function
test_template() {
  local name=$1
  local database_provider=$2
  local database_type=$3
  local rest=$4
  local graphql=$5
  local grpc=$6
  local include_auth=${7:-false}
  local include_subscriptions=${8:-false}
  local use_aspire=${9:-false}
  
  echo -e "${CYAN}Testing: $name${NC}"
  echo "  Provider: $database_provider, Database: $database_type"
  echo "  APIs: REST=$rest, GraphQL=$graphql, gRPC=$grpc"
  echo "  Features: Auth=$include_auth, Subscriptions=$include_subscriptions, Aspire=$use_aspire"
  
  local test_dir="$TEMP_DIR/$name"
  
  # Generate project
  dotnet new ddap-api \
    --name "$name" \
    --database-provider "$database_provider" \
    --database-type "$database_type" \
    --rest "$rest" \
    --graphql "$graphql" \
    --grpc "$grpc" \
    --include-auth "$include_auth" \
    --include-subscriptions "$include_subscriptions" \
    --use-aspire "$use_aspire" \
    --output "$test_dir" > /dev/null 2>&1
  
  # Verify files exist
  if [ ! -f "$test_dir/$name.csproj" ]; then
    echo -e "${RED}❌ FAILED: Project file not created${NC}"
    return 1
  fi
  
  # Restore packages
  cd "$test_dir"
  if ! dotnet restore > /dev/null 2>&1; then
    echo -e "${RED}❌ FAILED: Could not restore packages${NC}"
    return 1
  fi
  
  # Build project
  if ! dotnet build --no-restore > /dev/null 2>&1; then
    echo -e "${RED}❌ FAILED: Could not build project${NC}"
    return 1
  fi
  
  echo -e "${GREEN}✅ PASSED: $name${NC}"
  echo ""
}

# Main execution
echo -e "${CYAN}🧪 Template Validation Tests${NC}"
echo "=============================="
echo ""

cd "$REPO_ROOT"

# Uninstall any existing template
echo "Uninstalling existing template..."
dotnet new uninstall "$REPO_ROOT/$TEMPLATE_PATH" 2>/dev/null || true

# Install template
echo "Installing template from: $TEMPLATE_PATH"
if ! dotnet new install "$TEMPLATE_PATH" --force; then
  echo -e "${RED}❌ FAILED: Could not install template${NC}"
  exit 1
fi

echo ""
echo -e "${CYAN}Running test scenarios...${NC}"
echo ""

# Phase 1: Database Provider Tests
echo -e "${YELLOW}=== Database Provider Tests ===${NC}"
test_template "SqlServerDapper" "dapper" "sqlserver" "true" "false" "false"
test_template "SqlServerEF" "entityframework" "sqlserver" "false" "true" "false"
test_template "MySqlDapper" "dapper" "mysql" "false" "false" "true"
test_template "MySqlEF" "entityframework" "mysql" "true" "true" "false"
test_template "PostgresDapper" "dapper" "postgresql" "true" "false" "false"
test_template "PostgresEF" "entityframework" "postgresql" "false" "true" "false"
test_template "SqliteDapper" "dapper" "sqlite" "false" "false" "true"
test_template "SqliteEF" "entityframework" "sqlite" "true" "false" "false"

# Phase 2: API Provider Tests
echo -e "${YELLOW}=== API Provider Tests ===${NC}"
test_template "RestOnly" "dapper" "sqlserver" "true" "false" "false"
test_template "GraphQLOnly" "dapper" "sqlserver" "false" "true" "false"
test_template "GrpcOnly" "dapper" "sqlserver" "false" "false" "true"
test_template "RestGraphQL" "dapper" "mysql" "true" "true" "false"
test_template "RestGrpc" "entityframework" "postgresql" "true" "false" "true"
test_template "GraphQLGrpc" "dapper" "sqlite" "false" "true" "true"
test_template "AllAPIs" "entityframework" "sqlserver" "true" "true" "true"

# Phase 3: Feature Tests
echo -e "${YELLOW}=== Feature Tests ===${NC}"
test_template "WithAuth" "dapper" "sqlserver" "true" "false" "false" "true"
test_template "WithSubscriptions" "dapper" "sqlserver" "false" "true" "false" "false" "true"
test_template "WithAspire" "dapper" "sqlserver" "true" "false" "false" "false" "false" "true"
test_template "AllFeatures" "entityframework" "sqlserver" "true" "true" "false" "true" "true" "false"

# Phase 4: Complex Combinations
echo -e "${YELLOW}=== Complex Combination Tests ===${NC}"
test_template "Minimal" "dapper" "sqlite" "false" "false" "false"
test_template "Maximum" "entityframework" "sqlserver" "true" "true" "true" "true" "true" "true"

# Uninstall template
echo ""
echo "Uninstalling template..."
dotnet new uninstall "$REPO_ROOT/$TEMPLATE_PATH"

echo ""
echo -e "${GREEN}✅ All template tests passed!${NC}"
echo ""
echo "Summary:"
echo "  Total scenarios tested: 21"
echo "  Database types: SQL Server, MySQL, PostgreSQL, SQLite"
echo "  Data providers: Dapper, Entity Framework"
echo "  API types: REST, GraphQL, gRPC"
echo "  Features: Auth, Subscriptions, Aspire"
