#!/bin/bash

# Script to check per-class coverage thresholds
# Requirements: 80% line coverage and 80% branch coverage per class
# This script will FAIL the build if requirements are not met
# Prerequisite: jq must be installed (handled by CI workflow)

set -e

REPORT_FILE="coverage/report/Summary.json"
MIN_LINE_COVERAGE=80
MIN_BRANCH_COVERAGE=80

if [ ! -f "$REPORT_FILE" ]; then
    echo "❌ Coverage report not found at $REPORT_FILE"
    exit 1
fi

# Check if jq is available
if ! command -v jq &> /dev/null; then
    echo "❌ jq is not installed. Please install it to run this script."
    echo "   On Ubuntu/Debian: sudo apt-get install jq"
    echo "   On macOS: brew install jq"
    exit 1
fi

echo "📊 Checking Coverage Per File (Class Level)"
echo "Requirements: ${MIN_LINE_COVERAGE}% line coverage, ${MIN_BRANCH_COVERAGE}% branch coverage per file"
echo "=============================================================================="

# Extract overall summary
OVERALL_LINE=$(jq -r '.summary.linecoverage' "$REPORT_FILE")
OVERALL_BRANCH=$(jq -r '.summary.branchcoverage' "$REPORT_FILE")

echo ""
echo "Overall Coverage (filtered modules):"
echo "  Line Coverage: ${OVERALL_LINE}%"
echo "  Branch Coverage: ${OVERALL_BRANCH}%"
echo ""
echo "Per-File Analysis:"
echo "---"

# Create temporary file for tracking results
TEMP_RESULTS="$(mktemp)"
trap 'rm -f "$TEMP_RESULTS"' EXIT

# Iterate through each assembly and class
while IFS= read -r assembly; do
    ASSEMBLY_NAME=$(echo "$assembly" | jq -r '.name')
    
    # Get all classes in the assembly
    echo "$assembly" | jq -c '.classesinassembly[]' | while IFS= read -r class; do
        CLASS_NAME=$(echo "$class" | jq -r '.name')
        LINE_COV=$(echo "$class" | jq -r '.coverage')
        BRANCH_COV=$(echo "$class" | jq -r '.branchcoverage')
        
        # Handle null branch coverage (no branches in file)
        if [ "$BRANCH_COV" = "null" ] || [ -z "$BRANCH_COV" ]; then
            BRANCH_COV="N/A"
            BRANCH_CHECK=1
        else
            # Check if branch coverage meets threshold
            BRANCH_CHECK=$(jq -n --argjson cov "$BRANCH_COV" --argjson min "$MIN_BRANCH_COVERAGE" 'if ($cov >= $min) then 1 else 0 end')
        fi
        
        # Check if line coverage meets threshold
        LINE_CHECK=$(jq -n --argjson cov "$LINE_COV" --argjson min "$MIN_LINE_COVERAGE" 'if ($cov >= $min) then 1 else 0 end')
        
        if [ "$LINE_CHECK" = "1" ] && [ "$BRANCH_CHECK" = "1" ]; then
            echo "✅ $CLASS_NAME - Line: ${LINE_COV}%, Branch: ${BRANCH_COV}%"
            echo "PASS" >> "$TEMP_RESULTS"
        else
            echo "❌ $CLASS_NAME - Line: ${LINE_COV}%, Branch: ${BRANCH_COV}%"
            echo "FAIL|$CLASS_NAME|${LINE_COV}|${BRANCH_COV}" >> "$TEMP_RESULTS"
        fi
    done
done < <(jq -c '.coverage.assemblies[]' "$REPORT_FILE")

echo ""
echo "=============================================================================="
echo "Coverage Check Complete"
echo ""

# Count results
TOTAL_FILES=$(wc -l < "$TEMP_RESULTS")
PASSED_FILES=$(grep -c "^PASS$" "$TEMP_RESULTS" || echo "0")
FAILED_COUNT=$(grep -c "^FAIL|" "$TEMP_RESULTS" || echo "0")

echo "📊 Results:"
echo "  Total Files: $TOTAL_FILES"
echo "  Passed: $PASSED_FILES"
echo "  Failed: $FAILED_COUNT"
echo ""

if [ "$FAILED_COUNT" -gt 0 ]; then
    echo "❌ COVERAGE CHECK FAILED!"
    echo ""
    echo "$FAILED_COUNT file(s) below coverage thresholds:"
    echo ""
    grep "^FAIL|" "$TEMP_RESULTS" | while IFS='|' read -r status file line branch; do
        echo "  - $file (Line: ${line}%, Branch: ${branch}%)"
    done
    echo ""
    echo "⚠️  These files need more test coverage to meet the requirements:"
    echo "     - Minimum Line Coverage: ${MIN_LINE_COVERAGE}%"
    echo "     - Minimum Branch Coverage: ${MIN_BRANCH_COVERAGE}%"
    echo ""
    echo "Excluded modules (not validated):"
    echo "  - Data providers (*.Data.*, require database)"
    echo "  - Auth, CodeGen, Subscriptions (require infrastructure)"
    echo "  - Test assemblies, Example projects"
    echo ""
    echo "💡 To fix: Add more unit tests covering the failed files"
    echo ""
    exit 1
fi

echo "✅ All ${TOTAL_FILES} files meet the ${MIN_LINE_COVERAGE}% line and ${MIN_BRANCH_COVERAGE}% branch coverage thresholds!"
exit 0
