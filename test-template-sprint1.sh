#!/bin/bash
# Sprint 1: Template API Provider Flags Test Script
# Tests the simplified computed symbols for REST, GraphQL, and gRPC flags

# Don't exit on error - we want to run all tests
set +e

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
NC='\033[0m' # No Color

# Test counter
TESTS_PASSED=0
TESTS_FAILED=0
TEMP_DIR="/tmp/ddap-template-tests-$$"

# Cleanup function
cleanup() {
    echo -e "\n${YELLOW}Cleaning up test directories...${NC}"
    rm -rf "$TEMP_DIR"
}

# Trap to ensure cleanup happens
trap cleanup EXIT

# Dynamically determine repository root before changing directories
SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
REPO_ROOT="${SCRIPT_DIR}"

# Create temp directory
mkdir -p "$TEMP_DIR"
cd "$TEMP_DIR"

# Function to verify package reference
verify_package() {
    local project_dir=$1
    local package_name=$2
    local should_exist=$3
    
    if grep -q "PackageReference Include=\"${package_name}\"" "${project_dir}/${project_dir}.csproj"; then
        if [ "$should_exist" = "true" ]; then
            echo -e "${GREEN}✓${NC} ${package_name} found (expected)"
            return 0
        else
            echo -e "${RED}✗${NC} ${package_name} found (unexpected)"
            return 1
        fi
    else
        if [ "$should_exist" = "false" ]; then
            echo -e "${GREEN}✓${NC} ${package_name} not found (expected)"
            return 0
        else
            echo -e "${RED}✗${NC} ${package_name} not found (unexpected)"
            return 1
        fi
    fi
}

# Function to run a test
run_test() {
    local test_name=$1
    local project_name=$2
    shift 2
    local template_args=("$@")
    
    echo -e "\n${YELLOW}━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━${NC}"
    echo -e "${YELLOW}Test: ${test_name}${NC}"
    echo -e "${YELLOW}━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━${NC}"
    
    # Generate project
    echo "Command: dotnet new ddap-api --name ${project_name} ${template_args[*]}"
    if dotnet new ddap-api --name "${project_name}" "${template_args[@]}" --force > /dev/null 2>&1; then
        echo -e "${GREEN}✓${NC} Project generated successfully"
    else
        echo -e "${RED}✗${NC} Failed to generate project"
        ((TESTS_FAILED++))
        return 1
    fi
    
    # Verify csproj exists
    if [ ! -f "${project_name}/${project_name}.csproj" ]; then
        echo -e "${RED}✗${NC} .csproj file not found"
        ((TESTS_FAILED++))
        return 1
    fi
    
    echo -e "\nVerifying package references:"
    return 0
}

# Install template
echo -e "${YELLOW}Installing template from source...${NC}"

cd "${REPO_ROOT}"
if dotnet new install ./templates/ddap-api --force > /dev/null 2>&1; then
    echo -e "${GREEN}✓${NC} Template installed successfully"
else
    echo -e "${RED}✗${NC} Failed to install template"
    exit 1
fi

cd "$TEMP_DIR"

# ============================================================================
# Test 1: REST only
# ============================================================================
if run_test "REST API Only" "TestRest" "--rest" "true" "--graphql" "false" "--grpc" "false"; then
    if verify_package "TestRest" "Ddap.Rest" "true" && \
       verify_package "TestRest" "Ddap.GraphQL" "false" && \
       verify_package "TestRest" "Ddap.Grpc" "false"; then
        echo -e "${GREEN}✓${NC} Test 1 PASSED"
        ((TESTS_PASSED++))
    else
        echo -e "${RED}✗${NC} Test 1 FAILED"
        ((TESTS_FAILED++))
    fi
fi

# ============================================================================
# Test 2: GraphQL only
# ============================================================================
if run_test "GraphQL API Only" "TestGraphQL" "--rest" "false" "--graphql" "true" "--grpc" "false"; then
    if verify_package "TestGraphQL" "Ddap.Rest" "false" && \
       verify_package "TestGraphQL" "Ddap.GraphQL" "true" && \
       verify_package "TestGraphQL" "Ddap.Grpc" "false"; then
        echo -e "${GREEN}✓${NC} Test 2 PASSED"
        ((TESTS_PASSED++))
    else
        echo -e "${RED}✗${NC} Test 2 FAILED"
        ((TESTS_FAILED++))
    fi
fi

# ============================================================================
# Test 3: gRPC only
# ============================================================================
if run_test "gRPC API Only" "TestGrpc" "--rest" "false" "--graphql" "false" "--grpc" "true"; then
    if verify_package "TestGrpc" "Ddap.Rest" "false" && \
       verify_package "TestGrpc" "Ddap.GraphQL" "false" && \
       verify_package "TestGrpc" "Ddap.Grpc" "true"; then
        echo -e "${GREEN}✓${NC} Test 3 PASSED"
        ((TESTS_PASSED++))
    else
        echo -e "${RED}✗${NC} Test 3 FAILED"
        ((TESTS_FAILED++))
    fi
fi

# ============================================================================
# Test 4: REST + GraphQL
# ============================================================================
if run_test "REST + GraphQL" "TestRestGraphQL" "--rest" "true" "--graphql" "true" "--grpc" "false"; then
    if verify_package "TestRestGraphQL" "Ddap.Rest" "true" && \
       verify_package "TestRestGraphQL" "Ddap.GraphQL" "true" && \
       verify_package "TestRestGraphQL" "Ddap.Grpc" "false"; then
        echo -e "${GREEN}✓${NC} Test 4 PASSED"
        ((TESTS_PASSED++))
    else
        echo -e "${RED}✗${NC} Test 4 FAILED"
        ((TESTS_FAILED++))
    fi
fi

# ============================================================================
# Test 5: All APIs
# ============================================================================
if run_test "All APIs" "TestAllApis" "--rest" "true" "--graphql" "true" "--grpc" "true"; then
    if verify_package "TestAllApis" "Ddap.Rest" "true" && \
       verify_package "TestAllApis" "Ddap.GraphQL" "true" && \
       verify_package "TestAllApis" "Ddap.Grpc" "true"; then
        echo -e "${GREEN}✓${NC} Test 5 PASSED"
        ((TESTS_PASSED++))
    else
        echo -e "${RED}✗${NC} Test 5 FAILED"
        ((TESTS_FAILED++))
    fi
fi

# ============================================================================
# Summary
# ============================================================================
echo -e "\n${YELLOW}━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━${NC}"
echo -e "${YELLOW}Test Summary${NC}"
echo -e "${YELLOW}━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━${NC}"
echo -e "Tests Passed: ${GREEN}${TESTS_PASSED}${NC}"
echo -e "Tests Failed: ${RED}${TESTS_FAILED}${NC}"
echo -e "Total Tests:  $((TESTS_PASSED + TESTS_FAILED))"

if [ $TESTS_FAILED -eq 0 ]; then
    echo -e "\n${GREEN}✓ All tests passed!${NC}"
    exit 0
else
    echo -e "\n${RED}✗ Some tests failed!${NC}"
    exit 1
fi
