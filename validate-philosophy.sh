#!/bin/bash

# Script to validate DDAP philosophy compliance in code changes
# Checks for common violations of "Developer in Control" principles
# This script will FAIL the build if critical violations are found

set -e

REPORT_FILE="philosophy-report.md"
EXIT_CODE=0
VIOLATIONS_FOUND=0

echo "🎯 DDAP Philosophy Compliance Validation"
echo "======================================================================"
echo ""

# Get list of changed C# files
if [ -n "$GITHUB_BASE_REF" ]; then
    # Running in GitHub Actions
    git fetch origin "$GITHUB_BASE_REF" --depth=50 2>/dev/null || true
    CHANGED_FILES=$(git diff --name-only "origin/$GITHUB_BASE_REF"...HEAD | grep '\.cs$' || true)
else
    # Running locally
    CHANGED_FILES=$(git diff --name-only HEAD~1 | grep '\.cs$' || true)
fi

if [ -z "$CHANGED_FILES" ]; then
    echo "ℹ️  No C# files changed - skipping philosophy check"
    echo "✅ **No C# files changed** - Philosophy check skipped" > "$REPORT_FILE"
    exit 0
fi

echo "📝 Analyzing changed files:"
echo "$CHANGED_FILES"
echo ""

# Initialize report
cat > "$REPORT_FILE" << EOF
### Philosophy Compliance Report

**Files Analyzed:** $(echo "$CHANGED_FILES" | wc -l)

EOF

# Principle 1: No Forced Dependencies
echo "🔍 Checking Principle 1: No Forced Dependencies"
echo "---"

FORCED_DEPS=()
while IFS= read -r file; do
    if [ ! -f "$file" ]; then
        continue
    fi
    
    # Check for hardcoded package references (community packages)
    if grep -q "Pomelo\.EntityFrameworkCore" "$file" 2>/dev/null; then
        echo "⚠️  $file: References Pomelo (community package)"
        FORCED_DEPS+=("$file: Pomelo.EntityFrameworkCore reference")
    fi
    
    # Check for hardcoded connection strings
    if grep -q "ConfigurationManager\.ConnectionStrings" "$file" 2>/dev/null; then
        echo "⚠️  $file: Uses ConfigurationManager.ConnectionStrings (hidden config)"
        FORCED_DEPS+=("$file: Hidden configuration via ConfigurationManager")
    fi
    
    # Check for new SqlConnection/MySqlConnection without DI
    if grep -E "new (SqlConnection|MySqlConnection|NpgsqlConnection)\(" "$file" 2>/dev/null; then
        echo "⚠️  $file: Creates database connection directly (not DI)"
        FORCED_DEPS+=("$file: Direct database connection instantiation")
    fi
    
done <<< "$CHANGED_FILES"

if [ ${#FORCED_DEPS[@]} -eq 0 ]; then
    echo "✅ No forced dependencies found"
    echo "#### ✅ Principle 1: No Forced Dependencies" >> "$REPORT_FILE"
    echo "All dependencies are properly injected or configurable." >> "$REPORT_FILE"
    echo "" >> "$REPORT_FILE"
else
    echo "❌ Found ${#FORCED_DEPS[@]} forced dependency issue(s)"
    echo "#### ❌ Principle 1: No Forced Dependencies - VIOLATIONS FOUND" >> "$REPORT_FILE"
    echo "" >> "$REPORT_FILE"
    for violation in "${FORCED_DEPS[@]}"; do
        echo "- ⚠️  $violation" >> "$REPORT_FILE"
    done
    echo "" >> "$REPORT_FILE"
    VIOLATIONS_FOUND=$((VIOLATIONS_FOUND + ${#FORCED_DEPS[@]}))
    EXIT_CODE=1
fi
echo ""

# Principle 2: Explicit Over Implicit
echo "🔍 Checking Principle 2: Explicit Over Implicit"
echo "---"

IMPLICIT_BEHAVIORS=()
while IFS= read -r file; do
    if [ ! -f "$file" ]; then
        continue
    fi
    
    # Check for magic strings/numbers without constants
    # Ignore: comments (//), string literals in error messages (InvalidOperationException), and const declarations
    if grep -E "\"\w+://|localhost|127\.0\.0\.1" "$file" 2>/dev/null | \
       grep -v "const " | \
       grep -v "//" | \
       grep -v "throw new" | \
       grep -v "InvalidOperationException" | \
       grep -v "Exception(" | \
       head -1 | grep -q .; then
        echo "⚠️  $file: Contains hardcoded URLs/IPs in executable code"
        IMPLICIT_BEHAVIORS+=("$file: Hardcoded connection details")
    fi
    
    # Check for empty catch blocks (hiding errors)
    if grep -A1 "catch" "$file" 2>/dev/null | grep -q "^[[:space:]]*}[[:space:]]*$"; then
        echo "⚠️  $file: Empty catch block found (hiding errors)"
        IMPLICIT_BEHAVIORS+=("$file: Empty catch block")
    fi
    
done <<< "$CHANGED_FILES"

if [ ${#IMPLICIT_BEHAVIORS[@]} -eq 0 ]; then
    echo "✅ All behaviors are explicit"
    echo "#### ✅ Principle 2: Explicit Over Implicit" >> "$REPORT_FILE"
    echo "Configuration and behavior are transparent." >> "$REPORT_FILE"
    echo "" >> "$REPORT_FILE"
else
    echo "❌ Found ${#IMPLICIT_BEHAVIORS[@]} implicit behavior(s)"
    echo "#### ⚠️ Principle 2: Explicit Over Implicit - WARNINGS" >> "$REPORT_FILE"
    echo "" >> "$REPORT_FILE"
    for violation in "${IMPLICIT_BEHAVIORS[@]}"; do
        echo "- ⚠️  $violation" >> "$REPORT_FILE"
    done
    echo "" >> "$REPORT_FILE"
    VIOLATIONS_FOUND=$((VIOLATIONS_FOUND + ${#IMPLICIT_BEHAVIORS[@]}))
    # Don't fail for these, just warn
fi
echo ""

# Principle 3: Async/Await for I/O
echo "🔍 Checking Code Quality: Async/Await for I/O Operations"
echo "---"

SYNC_IO=()
while IFS= read -r file; do
    if [ ! -f "$file" ]; then
        continue
    fi
    
    # Check for synchronous database calls (common anti-pattern)
    # Ignore: comments, documentation examples, and exception messages
    if grep -E "\.Query\(|\.Execute\(|\.QueryFirst\(|\.QuerySingle\(" "$file" 2>/dev/null | \
       grep -v "Async" | \
       grep -v "//" | \
       grep -v "throw new" | \
       grep -v '"' | \
       head -1 | grep -q .; then
        echo "⚠️  $file: Synchronous database call detected"
        SYNC_IO+=("$file: Synchronous Dapper call")
    fi
    
    # Check for .Result or .Wait() (deadlock risk)
    # Ignore: comments, documentation, and string literals
    if grep -E "\.Result|\.Wait\(\)" "$file" 2>/dev/null | \
       grep -v "//" | \
       grep -v '"' | \
       grep -v "throw new" | \
       head -1 | grep -q .; then
        echo "❌ $file: Uses .Result or .Wait() - DEADLOCK RISK"
        SYNC_IO+=("$file: Blocking async call (.Result/.Wait)")
        EXIT_CODE=1
    fi
    
done <<< "$CHANGED_FILES"

if [ ${#SYNC_IO[@]} -eq 0 ]; then
    echo "✅ All I/O operations are async"
    echo "#### ✅ Code Quality: Async/Await" >> "$REPORT_FILE"
    echo "All I/O operations use async/await properly." >> "$REPORT_FILE"
    echo "" >> "$REPORT_FILE"
else
    echo "❌ Found ${#SYNC_IO[@]} synchronous I/O issue(s)"
    echo "#### ❌ Code Quality: Async/Await - VIOLATIONS FOUND" >> "$REPORT_FILE"
    echo "" >> "$REPORT_FILE"
    for violation in "${SYNC_IO[@]}"; do
        echo "- ❌ $violation" >> "$REPORT_FILE"
    done
    echo "" >> "$REPORT_FILE"
    VIOLATIONS_FOUND=$((VIOLATIONS_FOUND + ${#SYNC_IO[@]}))
fi
echo ""

# Principle 4: Official Packages
echo "🔍 Checking Principle 4: Official Packages Preferred"
echo "---"

# This is better checked in .csproj files, but we can check imports
COMMUNITY_PACKAGES=()
while IFS= read -r file; do
    if [ ! -f "$file" ]; then
        continue
    fi
    
    # Check for known community package imports
    if grep -E "using Pomelo\.|using MySqlConnector\.|using Npgsql\." "$file" 2>/dev/null; then
        # MySqlConnector is actually the preferred one for Dapper, so this is informational
        echo "ℹ️  $file: Uses community package imports (verify if official alternative exists)"
    fi
    
done <<< "$CHANGED_FILES"

echo "#### ℹ️ Principle 4: Official Packages" >> "$REPORT_FILE"
echo "Package selection should prefer official vendor packages. Review .csproj files for dependencies." >> "$REPORT_FILE"
echo "" >> "$REPORT_FILE"
echo ""

# Summary
echo "======================================================================"
echo "Philosophy Compliance Check Complete"
echo ""

if [ "$VIOLATIONS_FOUND" -eq 0 ]; then
    echo "✅ SUCCESS! All $(echo "$CHANGED_FILES" | wc -l) changed files comply with DDAP philosophy"
    echo "" >> "$REPORT_FILE"
    echo "---" >> "$REPORT_FILE"
    echo "" >> "$REPORT_FILE"
    echo "✅ **All checks passed!** Code complies with DDAP \"Developer in Control\" philosophy." >> "$REPORT_FILE"
    exit 0
else
    echo "❌ PHILOSOPHY VIOLATIONS FOUND: $VIOLATIONS_FOUND issue(s)"
    echo ""
    echo "Please review and fix the violations above to maintain DDAP philosophy compliance."
    echo ""
    echo "" >> "$REPORT_FILE"
    echo "---" >> "$REPORT_FILE"
    echo "" >> "$REPORT_FILE"
    echo "❌ **$VIOLATIONS_FOUND violation(s) found.** Please review and address the issues above." >> "$REPORT_FILE"
    echo "" >> "$REPORT_FILE"
    echo "**Philosophy Reference:** See [.github/copilot-instructions.md](.github/copilot-instructions.md)" >> "$REPORT_FILE"
    exit "$EXIT_CODE"
fi
