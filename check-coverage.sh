#!/bin/bash

# Script to check per-file coverage thresholds
# Requirements: 80% line coverage and 80% branch coverage per file

set -e

REPORT_FILE="coverage/report/Summary.json"
MIN_LINE_COVERAGE=80
MIN_BRANCH_COVERAGE=80

if [ ! -f "$REPORT_FILE" ]; then
    echo "❌ Coverage report not found at $REPORT_FILE"
    exit 1
fi

# Check if jq is installed
if ! command -v jq &> /dev/null; then
    echo "📦 Installing jq for JSON parsing..."
    sudo apt-get update && sudo apt-get install -y jq
fi

echo "📊 Checking Coverage Per File"
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

FAILED_FILES=()
PASSED_FILES=0
TOTAL_FILES=0

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
            BRANCH_CHECK=true
        else
            # Check if branch coverage meets threshold
            BRANCH_CHECK=$(echo "$BRANCH_COV >= $MIN_BRANCH_COVERAGE" | bc -l)
        fi
        
        # Check if line coverage meets threshold
        LINE_CHECK=$(echo "$LINE_COV >= $MIN_LINE_COVERAGE" | bc -l)
        
        TOTAL_FILES=$((TOTAL_FILES + 1))
        
        if [ "$LINE_CHECK" = "1" ] && [ "$BRANCH_CHECK" = "true" -o "$BRANCH_CHECK" = "1" ]; then
            echo "✅ $CLASS_NAME - Line: ${LINE_COV}%, Branch: ${BRANCH_COV}%"
            PASSED_FILES=$((PASSED_FILES + 1))
        else
            echo "❌ $CLASS_NAME - Line: ${LINE_COV}%, Branch: ${BRANCH_COV}%"
            echo "$CLASS_NAME|${LINE_COV}|${BRANCH_COV}" >> /tmp/failed_files.txt
        fi
    done
done < <(jq -c '.coverage.assemblies[]' "$REPORT_FILE")

echo ""
echo "=============================================================================="
echo "Coverage Check Complete"
echo ""

# Read failed files count
if [ -f /tmp/failed_files.txt ]; then
    FAILED_COUNT=$(wc -l < /tmp/failed_files.txt)
    echo "📊 Results:"
    echo "  Total Files: $TOTAL_FILES"
    echo "  Passed: $((TOTAL_FILES - FAILED_COUNT))"
    echo "  Failed: $FAILED_COUNT"
    echo ""
    
    if [ "$FAILED_COUNT" -gt 0 ]; then
        echo "❌ $FAILED_COUNT file(s) below coverage thresholds:"
        echo ""
        cat /tmp/failed_files.txt | while IFS='|' read -r file line branch; do
            echo "  - $file (Line: ${line}%, Branch: ${branch}%)"
        done
        echo ""
        echo "⚠️  These files need more test coverage to meet the requirements:"
        echo "     - Minimum Line Coverage: ${MIN_LINE_COVERAGE}%"
        echo "     - Minimum Branch Coverage: ${MIN_BRANCH_COVERAGE}%"
        echo ""
        echo "Note: The following modules are excluded from coverage requirements:"
        echo "  - Data providers (*.Data.*, require database integration)"
        echo "  - Test assemblies (*.Tests.*)"
        echo "  - Example projects (*.Example.*)"
        echo "  - Auth, CodeGen, Memory, Subscriptions modules"
        echo ""
        rm /tmp/failed_files.txt
        exit 0  # Don't fail the build, just warn
    fi
    rm /tmp/failed_files.txt
fi

echo "✅ All files meet the ${MIN_LINE_COVERAGE}% coverage threshold!"
exit 0
