#!/bin/bash

# Script to organize orphan markdown files into appropriate directories
# Run this periodically to keep the repository root clean

set -e

ROOT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
cd "$ROOT_DIR"

# Colors for output
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
NC='\033[0m' # No Color

echo "🧹 Organizing markdown files..."

# Create directories if they don't exist
mkdir -p docs/analysis
mkdir -p docs/sprints
mkdir -p docs/testing
mkdir -p docs/archive

# Counter for moved files
MOVED=0

# Move analysis files
for file in ANALISE_*.md ANALYSIS_*.md PHILOSOPHY_*.md PACKAGE_*.md STRATEGIC_*.md; do
    if [ -f "$file" ]; then
        echo -e "${YELLOW}Moving:${NC} $file → docs/analysis/"
        mv "$file" docs/analysis/
        ((MOVED++))
    fi
done

# Move sprint-related files
for file in SPRINT*.md ROTEIRO_*.md PROXIMOS_*.md GUIA_*.md ESTRATEGIA_*.md; do
    if [ -f "$file" ]; then
        echo -e "${YELLOW}Moving:${NC} $file → docs/sprints/"
        mv "$file" docs/sprints/
        ((MOVED++))
    fi
done

# Move testing files
for file in TEST_*.md TESTING_*.md TEMPLATE_TESTING*.md TOOLING_TESTING*.md WEBSITE_TESTING*.md; do
    if [ -f "$file" ]; then
        echo -e "${YELLOW}Moving:${NC} $file → docs/testing/"
        mv "$file" docs/testing/
        ((MOVED++))
    fi
done

# Move project management files
for file in PROJECT_*.md FINAL_*.md IMPLEMENTATION_*.md DOCUMENTATION_*.md PIPELINE_*.md MULTI_*.md; do
    if [ -f "$file" ]; then
        echo -e "${YELLOW}Moving:${NC} $file → docs/archive/"
        mv "$file" docs/archive/
        ((MOVED++))
    fi
done

# Move old/backup files
for file in README.old.md *.backup.md; do
    if [ -f "$file" ]; then
        echo -e "${YELLOW}Moving:${NC} $file → docs/archive/"
        mv "$file" docs/archive/
        ((MOVED++))
    fi
done

# Remove truly temporary files (if any)
for file in TEMP_*.md WIP_*.md DRAFT_*.md; do
    if [ -f "$file" ]; then
        echo -e "${YELLOW}Removing:${NC} $file"
        rm -f "$file"
        ((MOVED++))
    fi
done

# Create README files in each docs subdirectory
cat > docs/analysis/README.md << 'EOF'
# Analysis Documentation

This directory contains analysis documents created during the DDAP improvement epic:

- **Philosophy Compliance Analysis**: Evaluation of how well the project adheres to the "Developer in Control" philosophy
- **Package Inventory**: Analysis of NuGet packages used in the project
- **Version Analysis**: Strategy for package versioning and auto-updates
- **Strategic Roadmap**: Long-term plans for LINQ support and multi-language clients

These documents provide context and reasoning for decisions made during the project's evolution.
EOF

cat > docs/sprints/README.md << 'EOF'
# Sprint Documentation

This directory contains sprint planning and execution documents:

- **Sprint Instructions**: Detailed guides for each sprint's implementation
- **Sprint Analysis**: Technical analysis and decisions made during sprints
- **Epic Strategy**: Overall epic management and PR chaining strategy
- **Roadmaps**: Sequential guides for implementing all sprints

These documents track the evolution of the project through iterative improvements.
EOF

cat > docs/testing/README.md << 'EOF'
# Testing Documentation

This directory contains test reports and testing strategies:

- **Template Testing**: Comprehensive testing of the .NET template generator
- **Tooling Testing**: Evaluation of development tooling (CSharpier, Husky, etc.)
- **Website Testing**: Multi-language documentation site testing
- **Test Summaries**: Overall test results and findings

These documents ensure quality and document testing procedures.
EOF

cat > docs/archive/README.md << 'EOF'
# Archive

This directory contains historical project documentation:

- **Project Summaries**: Overall project improvement summaries
- **Implementation Reports**: Detailed implementation documentation
- **Pipeline Reports**: CI/CD pipeline testing and validation
- **Old Files**: Previous versions of documentation

These files are kept for historical reference.
EOF

# Summary
if [ $MOVED -gt 0 ]; then
    echo -e "${GREEN}✓${NC} Moved $MOVED files"
    echo ""
    echo "Organized structure:"
    echo "  docs/analysis/  - Analysis and philosophy documents"
    echo "  docs/sprints/   - Sprint planning and execution"
    echo "  docs/testing/   - Test reports and strategies"
    echo "  docs/archive/   - Historical documentation"
else
    echo -e "${GREEN}✓${NC} No files to move - repository is already clean!"
fi

echo ""
echo "📝 Files to keep in root:"
echo "  ✓ README.md"
echo "  ✓ CONTRIBUTING.md"
echo "  ✓ CODE_OF_CONDUCT.md"
echo "  ✓ SECURITY.md"
echo "  ✓ LICENSE"
echo "  ✓ COVERAGE.md"
echo ""
echo "🎉 Done!"
