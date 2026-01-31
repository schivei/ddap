#!/bin/bash

# Static File Validation Script
# Validates Markdown, JSON, YAML, and XML files
# No npm dependencies required - uses Python and standard Unix tools

set -e

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

# Counters
TOTAL_FILES=0
PASSED_FILES=0
FAILED_FILES=0
WARNING_FILES=0

echo "📋 Static File Validation"
echo "============================================================"

# Function to check if file should be validated
should_validate() {
    local file="$1"
    
    # Skip if file doesn't exist or is not a regular file
    [[ ! -f "$file" ]] && return 1
    
    # Skip excluded patterns
    [[ "$file" =~ node_modules/ ]] && return 1
    [[ "$file" =~ bin/ ]] && return 1
    [[ "$file" =~ obj/ ]] && return 1
    [[ "$file" =~ coverage.*/ ]] && return 1
    [[ "$file" =~ \.git/ ]] && return 1
    [[ "$file" =~ \.vs/ ]] && return 1
    
    return 0
}

# Function to validate JSON
validate_json() {
    local file="$1"
    
    if ! should_validate "$file"; then
        return 0
    fi
    
    TOTAL_FILES=$((TOTAL_FILES + 1))
    
    # Use Python's json module for validation
    if python3 -c "import json; json.load(open('$file'))" 2>/dev/null; then
        echo -e "${GREEN}✅${NC} $file"
        PASSED_FILES=$((PASSED_FILES + 1))
        return 0
    else
        echo -e "${RED}❌${NC} $file - Invalid JSON syntax"
        FAILED_FILES=$((FAILED_FILES + 1))
        # Show detailed error
        python3 -c "import json; json.load(open('$file'))" 2>&1 | head -3
        return 1
    fi
}

# Function to validate YAML
validate_yaml() {
    local file="$1"
    
    if ! should_validate "$file"; then
        return 0
    fi
    
    TOTAL_FILES=$((TOTAL_FILES + 1))
    
    # Use Python's yaml module for validation
    if python3 -c "import yaml; yaml.safe_load(open('$file'))" 2>/dev/null; then
        echo -e "${GREEN}✅${NC} $file"
        PASSED_FILES=$((PASSED_FILES + 1))
        return 0
    else
        echo -e "${RED}❌${NC} $file - Invalid YAML syntax"
        FAILED_FILES=$((FAILED_FILES + 1))
        # Show detailed error
        python3 -c "import yaml; yaml.safe_load(open('$file'))" 2>&1 | head -3
        return 1
    fi
}

# Function to validate Markdown
validate_markdown() {
    local file="$1"
    local has_warnings=0
    local has_errors=0
    
    if ! should_validate "$file"; then
        return 0
    fi
    
    TOTAL_FILES=$((TOTAL_FILES + 1))
    
    # Check for trailing whitespace
    if grep -n ' $' "$file" >/dev/null 2>&1; then
        if [[ $has_warnings -eq 0 ]]; then
            echo -e "${YELLOW}⚠️${NC}  $file"
        fi
        echo "   - Has trailing whitespace on some lines"
        has_warnings=1
    fi
    
    # Check for very long lines (warning, not error)
    local long_lines=$(awk 'length > 120' "$file" | wc -l)
    if [[ $long_lines -gt 0 ]]; then
        if [[ $has_warnings -eq 0 ]]; then
            echo -e "${YELLOW}⚠️${NC}  $file"
        fi
        echo "   - Has $long_lines line(s) exceeding 120 characters (consider breaking)"
        has_warnings=1
    fi
    
    # Check for multiple consecutive blank lines
    if grep -Pzo '\n\n\n' "$file" >/dev/null 2>&1; then
        if [[ $has_warnings -eq 0 ]]; then
            echo -e "${YELLOW}⚠️${NC}  $file"
        fi
        echo "   - Has multiple consecutive blank lines"
        has_warnings=1
    fi
    
    if [[ $has_warnings -eq 0 ]]; then
        echo -e "${GREEN}✅${NC} $file"
        PASSED_FILES=$((PASSED_FILES + 1))
    else
        WARNING_FILES=$((WARNING_FILES + 1))
    fi
    
    return 0
}

# Function to validate XML
validate_xml() {
    local file="$1"
    
    if ! should_validate "$file"; then
        return 0
    fi
    
    TOTAL_FILES=$((TOTAL_FILES + 1))
    
    # Use Python's xml module for validation
    if python3 -c "import xml.etree.ElementTree as ET; ET.parse('$file')" 2>/dev/null; then
        echo -e "${GREEN}✅${NC} $file"
        PASSED_FILES=$((PASSED_FILES + 1))
        return 0
    else
        echo -e "${RED}❌${NC} $file - Invalid XML"
        FAILED_FILES=$((FAILED_FILES + 1))
        python3 -c "import xml.etree.ElementTree as ET; ET.parse('$file')" 2>&1 | head -3
        return 1
    fi
}

# Get list of tracked files
echo "🔍 Finding files to validate..."
echo ""

# Validate Markdown files
echo "📝 Validating Markdown files..."
while IFS= read -r file; do
    [[ -z "$file" ]] && continue
    validate_markdown "$file" || true
done < <(git ls-files '*.md' 2>/dev/null)

echo ""

# Validate JSON files
echo "📋 Validating JSON files..."
while IFS= read -r file; do
    [[ -z "$file" ]] && continue
    validate_json "$file" || true
done < <(git ls-files '*.json' 2>/dev/null)

echo ""

# Validate YAML files
echo "📄 Validating YAML files..."
while IFS= read -r file; do
    [[ -z "$file" ]] && continue
    validate_yaml "$file" || true
done < <(git ls-files '*.yml' '*.yaml' 2>/dev/null)

echo ""

# Validate XML files
echo "📑 Validating XML files..."
while IFS= read -r file; do
    [[ -z "$file" ]] && continue
    validate_xml "$file" || true
done < <(git ls-files '*.xml' '*.csproj' '*.props' '*.targets' 2>/dev/null)

echo ""
echo "============================================================"
echo "📊 Validation Summary"
echo "============================================================"
echo "Total files checked: $TOTAL_FILES"
echo -e "${GREEN}Passed: $PASSED_FILES${NC}"
if [[ $WARNING_FILES -gt 0 ]]; then
    echo -e "${YELLOW}Warnings: $WARNING_FILES${NC}"
fi
if [[ $FAILED_FILES -gt 0 ]]; then
    echo -e "${RED}Failed: $FAILED_FILES${NC}"
fi
echo "============================================================"

if [[ $FAILED_FILES -gt 0 ]]; then
    echo ""
    echo -e "${RED}❌ Validation failed!${NC}"
    echo "Please fix the errors above before committing."
    exit 1
fi

if [[ $WARNING_FILES -gt 0 ]]; then
    echo ""
    echo -e "${YELLOW}⚠️  Some files have warnings (non-blocking)${NC}"
    echo "Consider fixing these for better code quality."
fi

echo ""
echo -e "${GREEN}✅ All static files validated successfully!${NC}"
exit 0
