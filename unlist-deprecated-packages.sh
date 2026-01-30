#!/bin/bash

# Script to unlist deprecated NuGet packages from nuget.org
# This script identifies Ddap.* packages by schivei that are no longer in the current solution
# and unlists all their versions from NuGet.org

set -e  # Exit on error

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

echo -e "${BLUE}========================================${NC}"
echo -e "${BLUE}Ddap Deprecated Package Unlist Script${NC}"
echo -e "${BLUE}========================================${NC}"
echo

# Check if NUGET_API_KEY is set
if [ -z "$NUGET_API_KEY" ]; then
    echo -e "${RED}Error: NUGET_API_KEY environment variable is not set${NC}"
    echo "Please set it with: export NUGET_API_KEY=your-api-key"
    exit 1
fi

# Check if dotnet CLI is available
if ! command -v dotnet &> /dev/null; then
    echo -e "${RED}Error: dotnet CLI is not installed${NC}"
    exit 1
fi

# Check if jq is available (for JSON parsing)
if ! command -v jq &> /dev/null; then
    echo -e "${YELLOW}Warning: jq is not installed. Installing...${NC}"
    # Try to install jq
    if command -v apt-get &> /dev/null; then
        sudo apt-get update && sudo apt-get install -y jq
    elif command -v yum &> /dev/null; then
        sudo yum install -y jq
    elif command -v brew &> /dev/null; then
        brew install jq
    else
        echo -e "${RED}Error: Could not install jq. Please install it manually.${NC}"
        exit 1
    fi
fi

# Current packages in the solution (extracted from Ddap.slnx)
CURRENT_PACKAGES=(
    "Ddap.Aspire"
    "Ddap.Auth"
    "Ddap.Client.Core"
    "Ddap.Client.GraphQL"
    "Ddap.Client.Grpc"
    "Ddap.Client.Rest"
    "Ddap.CodeGen"
    "Ddap.Core"
    "Ddap.Data.Dapper"
    "Ddap.Data.EntityFramework"
    "Ddap.GraphQL"
    "Ddap.Grpc"
    "Ddap.Rest"
    "Ddap.Subscriptions"
    "Ddap.Templates"
)

echo -e "${GREEN}Current packages in solution:${NC}"
printf '%s\n' "${CURRENT_PACKAGES[@]}" | sort
echo

# Known deprecated packages (packages that were removed from the solution)
DEPRECATED_PACKAGES=(
    "Ddap.Memory"
)

echo -e "${YELLOW}Known deprecated packages:${NC}"
printf '%s\n' "${DEPRECATED_PACKAGES[@]}"
echo

# Function to get all versions of a package from NuGet.org
get_package_versions() {
    local package_id=$1
    local url="https://api.nuget.org/v3-flatcontainer/${package_id,,}/index.json"
    
    echo -e "${BLUE}Fetching versions for ${package_id}...${NC}"
    
    # Fetch package versions
    local response=$(curl -s "$url")
    
    if [ $? -ne 0 ] || [ -z "$response" ]; then
        echo -e "${YELLOW}  Package ${package_id} not found on NuGet.org or error fetching${NC}"
        return 1
    fi
    
    # Parse versions using jq
    local versions=$(echo "$response" | jq -r '.versions[]?' 2>/dev/null)
    
    if [ -z "$versions" ]; then
        echo -e "${YELLOW}  No versions found for ${package_id}${NC}"
        return 1
    fi
    
    echo "$versions"
    return 0
}

# Function to unlist a specific package version
unlist_package_version() {
    local package_id=$1
    local version=$2
    local dry_run=$3
    
    if [ "$dry_run" = "true" ]; then
        echo -e "${YELLOW}  [DRY RUN] Would unlist ${package_id} version ${version}${NC}"
        return 0
    fi
    
    echo -e "${BLUE}  Unlisting ${package_id} version ${version}...${NC}"
    
    # Use dotnet nuget delete to unlist the package
    local result=$(dotnet nuget delete "$package_id" "$version" \
        --api-key "$NUGET_API_KEY" \
        --source https://api.nuget.org/v3/index.json \
        --non-interactive 2>&1)
    
    if [ $? -eq 0 ]; then
        echo -e "${GREEN}    ✓ Successfully unlisted ${package_id} ${version}${NC}"
        return 0
    else
        # Check if already unlisted
        if echo "$result" | grep -q "already unlisted\|does not exist"; then
            echo -e "${YELLOW}    ⚠ ${package_id} ${version} already unlisted or doesn't exist${NC}"
            return 0
        else
            echo -e "${RED}    ✗ Failed to unlist ${package_id} ${version}${NC}"
            echo -e "${RED}    Error: $result${NC}"
            return 1
        fi
    fi
}

# Function to unlist all versions of a package
unlist_all_versions() {
    local package_id=$1
    local dry_run=$2
    
    echo -e "${BLUE}========================================${NC}"
    echo -e "${BLUE}Processing package: ${package_id}${NC}"
    echo -e "${BLUE}========================================${NC}"
    
    # Get all versions
    local versions=$(get_package_versions "$package_id")
    
    if [ $? -ne 0 ] || [ -z "$versions" ]; then
        echo -e "${YELLOW}No versions to unlist for ${package_id}${NC}"
        echo
        return 0
    fi
    
    # Count versions
    local version_count=$(echo "$versions" | wc -l)
    echo -e "${GREEN}Found ${version_count} version(s) to unlist${NC}"
    echo
    
    # Unlist each version
    local success_count=0
    local fail_count=0
    
    while IFS= read -r version; do
        if unlist_package_version "$package_id" "$version" "$dry_run"; then
            ((success_count++))
        else
            ((fail_count++))
        fi
    done <<< "$versions"
    
    echo
    echo -e "${GREEN}Summary for ${package_id}:${NC}"
    echo -e "  ${GREEN}✓ Successfully unlisted: ${success_count}${NC}"
    if [ $fail_count -gt 0 ]; then
        echo -e "  ${RED}✗ Failed to unlist: ${fail_count}${NC}"
    fi
    echo
}

# Main execution
echo -e "${BLUE}========================================${NC}"
echo -e "${BLUE}Starting unlist process...${NC}"
echo -e "${BLUE}========================================${NC}"
echo

# Check for dry-run mode
DRY_RUN=false
if [ "$1" = "--dry-run" ] || [ "$1" = "-d" ]; then
    DRY_RUN=true
    echo -e "${YELLOW}========================================${NC}"
    echo -e "${YELLOW}RUNNING IN DRY-RUN MODE${NC}"
    echo -e "${YELLOW}No actual changes will be made${NC}"
    echo -e "${YELLOW}========================================${NC}"
    echo
fi

# Process each deprecated package
TOTAL_PROCESSED=0
for package in "${DEPRECATED_PACKAGES[@]}"; do
    unlist_all_versions "$package" "$DRY_RUN"
    ((TOTAL_PROCESSED++))
done

echo -e "${BLUE}========================================${NC}"
echo -e "${BLUE}Unlist process completed!${NC}"
echo -e "${BLUE}========================================${NC}"
echo -e "${GREEN}Total deprecated packages processed: ${TOTAL_PROCESSED}${NC}"
echo

if [ "$DRY_RUN" = "true" ]; then
    echo -e "${YELLOW}This was a DRY RUN. No actual changes were made.${NC}"
    echo -e "${YELLOW}Run without --dry-run to perform actual unlisting.${NC}"
else
    echo -e "${GREEN}All deprecated package versions have been unlisted from NuGet.org${NC}"
fi

echo
echo -e "${BLUE}Note: Unlisted packages are still downloadable if you know the exact version,${NC}"
echo -e "${BLUE}but they won't appear in search results or package listings.${NC}"
