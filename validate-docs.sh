#!/bin/bash

# Script to validate documentation in all languages
# Validates HTML structure, checks for broken links, and ensures all translations exist
# This script will FAIL the build if requirements are not met

set -e

DOCS_DIR="docs"
EXIT_CODE=0

# Define supported languages
LANGUAGES=("en" "pt-br" "es" "fr" "de" "ja" "zh")

# Documentation pages that should exist in all languages
# Note: DocFX-generated pages (index, get-started, architecture, advanced) are not included
# as they are generated from .md files and validated during DocFX build
REQUIRED_PAGES=(
    "database-providers.html"
    "api-providers.html"
    "philosophy.html"
    "client-getting-started.html"
    "client-rest.html"
    "client-graphql.html"
    "client-grpc.html"
    "extended-types.html"
    "raw-queries.html"
    "auto-reload.html"
    "troubleshooting.html"
)

echo "📚 Documentation Validation for Multi-Language Support"
echo "======================================================================"
echo ""

# Function to check if HTML is well-formed
check_html_validity() {
    local file=$1
    local errors=0
    
    # Basic HTML structure checks
    if ! grep -q "<html" "$file"; then
        echo "  ❌ Missing <html> tag"
        errors=$((errors + 1))
    fi
    
    if ! grep -q "<head" "$file"; then
        echo "  ❌ Missing <head> tag"
        errors=$((errors + 1))
    fi
    
    if ! grep -q "<body" "$file"; then
        echo "  ❌ Missing <body> tag"
        errors=$((errors + 1))
    fi
    
    if ! grep -q "</html>" "$file"; then
        echo "  ❌ Missing closing </html> tag"
        errors=$((errors + 1))
    fi
    
    # Check for UTF-8 encoding declaration
    if ! grep -q "charset=" "$file" && ! grep -q "UTF-8" "$file"; then
        echo "  ⚠️  Warning: No UTF-8 charset declaration found"
    fi
    
    return $errors
}

# Function to check for broken internal links
check_internal_links() {
    local file=$1
    local dir=$(dirname "$file")
    local errors=0
    
    # Extract href attributes (simplified regex, good enough for validation)
    grep -o 'href="[^"]*"' "$file" 2>/dev/null | sed 's/href="//;s/"$//' | while read -r link; do
        # Skip external links, anchors, and special protocols
        if [[ "$link" =~ ^https?:// ]] || [[ "$link" =~ ^mailto: ]] || [[ "$link" =~ ^# ]] || [[ "$link" =~ ^javascript: ]]; then
            continue
        fi
        
        # Resolve relative path
        if [[ "$link" == /* ]]; then
            # Absolute path from docs root
            target="$DOCS_DIR$link"
        else
            # Relative path
            target="$dir/$link"
        fi
        
        # Remove anchor fragments
        target="${target%%#*}"
        
        # Check if file exists
        if [ ! -f "$target" ] && [ ! -d "$target" ]; then
            echo "  ⚠️  Broken link: $link (target not found: $target)"
            errors=$((errors + 1))
        fi
    done
    
    return $errors
}

echo "🌍 Checking Language Coverage"
echo "---"

# Check English (base) documentation exists
echo "Checking English (base) documentation..."
for page in "${REQUIRED_PAGES[@]}"; do
    if [ ! -f "$DOCS_DIR/$page" ]; then
        echo "  ❌ Missing: $page"
        EXIT_CODE=1
    else
        echo "  ✅ $page"
    fi
done
echo ""

# Check translations for each language
for lang in "${LANGUAGES[@]}"; do
    if [ "$lang" = "en" ]; then
        continue  # Already checked above
    fi
    
    echo "Checking $lang translations..."
    LANG_DIR="$DOCS_DIR/$lang"
    
    if [ ! -d "$LANG_DIR" ]; then
        echo "  ❌ Language directory not found: $LANG_DIR"
        EXIT_CODE=1
        continue
    fi
    
    MISSING_PAGES=0
    for page in "${REQUIRED_PAGES[@]}"; do
        if [ ! -f "$LANG_DIR/$page" ]; then
            echo "  ❌ Missing translation: $page"
            MISSING_PAGES=$((MISSING_PAGES + 1))
            EXIT_CODE=1
        fi
    done
    
    if [ "$MISSING_PAGES" -eq 0 ]; then
        echo "  ✅ All pages translated (${#REQUIRED_PAGES[@]} pages)"
    else
        echo "  ❌ $MISSING_PAGES missing translations"
    fi
    echo ""
done

echo ""
echo "🔍 Validating HTML Structure"
echo "---"

# Validate HTML structure for all pages
for lang in "${LANGUAGES[@]}"; do
    if [ "$lang" = "en" ]; then
        LANG_DIR="$DOCS_DIR"
    else
        LANG_DIR="$DOCS_DIR/$lang"
    fi
    
    if [ ! -d "$LANG_DIR" ]; then
        continue
    fi
    
    echo "Validating $lang HTML files..."
    HTML_ERRORS=0
    
    for page in "${REQUIRED_PAGES[@]}"; do
        if [ ! -f "$LANG_DIR/$page" ]; then
            continue
        fi
        
        if ! check_html_validity "$LANG_DIR/$page"; then
            HTML_ERRORS=$((HTML_ERRORS + 1))
            EXIT_CODE=1
        fi
    done
    
    if [ "$HTML_ERRORS" -eq 0 ]; then
        echo "  ✅ All HTML files are well-formed"
    else
        echo "  ❌ $HTML_ERRORS HTML files have structural issues"
    fi
    echo ""
done

echo ""
echo "🔗 Checking Internal Links"
echo "---"

# Check internal links (just English to avoid duplication)
echo "Checking English documentation links..."
LINK_ERRORS=0

for page in "${REQUIRED_PAGES[@]}"; do
    if [ ! -f "$DOCS_DIR/$page" ]; then
        continue
    fi
    
    if ! check_internal_links "$DOCS_DIR/$page"; then
        LINK_ERRORS=$((LINK_ERRORS + 1))
    fi
done

if [ "$LINK_ERRORS" -gt 0 ]; then
    echo "  ⚠️  Found $LINK_ERRORS files with broken links (non-blocking)"
    # Don't fail build for broken links, just warn
fi

echo ""
echo "======================================================================"
echo "Documentation Validation Complete"
echo ""

if [ "$EXIT_CODE" -eq 0 ]; then
    echo "✅ SUCCESS! All documentation requirements met:"
    echo "   - All ${#REQUIRED_PAGES[@]} pages exist in English"
    echo "   - All ${#LANGUAGES[@]} language translations complete"
    echo "   - All HTML files are well-formed"
    echo ""
    exit 0
else
    echo "❌ VALIDATION FAILED!"
    echo ""
    echo "Documentation issues found. Please fix:"
    echo "  - Ensure all required pages exist in English"
    echo "  - Complete missing translations for all languages"
    echo "  - Fix any HTML structural issues"
    echo ""
    exit 1
fi
