#!/bin/bash
set -e

echo "======================================"
echo "DDAP Pipeline Emulation Script"
echo "======================================"
echo ""

# Colors
GREEN='\033[0;32m'
RED='\033[0;31m'
YELLOW='\033[1;33m'
NC='\033[0m' # No Color

# Function to print status
print_status() {
    echo -e "${GREEN}✓${NC} $1"
}

print_error() {
    echo -e "${RED}✗${NC} $1"
}

print_warning() {
    echo -e "${YELLOW}⚠${NC} $1"
}

echo "Step 1: Restore dependencies"
dotnet restore Ddap.slnx > /dev/null 2>&1
print_status "Dependencies restored"

echo ""
echo "Step 2: Restore .NET tools"
dotnet tool restore > /dev/null 2>&1
print_status ".NET tools restored"

echo ""
echo "Step 3: Check code formatting"
if dotnet csharpier check . > /dev/null 2>&1; then
    print_status "Code formatting is correct"
else
    print_warning "Code formatting issues detected, auto-formatting..."
    dotnet csharpier format . > /dev/null 2>&1
    print_status "Code formatted successfully"
fi

echo ""
echo "Step 4: Build solution (Debug)"
if dotnet build Ddap.slnx --no-restore --configuration Debug > /dev/null 2>&1; then
    print_status "Build succeeded (Debug)"
else
    print_error "Build failed"
    exit 1
fi

echo ""
echo "Step 5: Run tests"
if dotnet test Ddap.slnx --no-build --configuration Debug --verbosity quiet > /dev/null 2>&1; then
    TEST_COUNT=$(dotnet test Ddap.slnx --no-build --configuration Debug --verbosity quiet 2>&1 | grep "Total tests:" | awk '{print $3}')
    PASSED_COUNT=$(dotnet test Ddap.slnx --no-build --configuration Debug --verbosity quiet 2>&1 | grep "Passed:" | awk '{print $2}')
    print_status "All tests passed ($PASSED_COUNT/$TEST_COUNT)"
else
    print_error "Some tests failed"
    exit 1
fi

echo ""
echo "Step 6: DocFX - Generate API metadata"
if docfx metadata docs/docfx.json > /dev/null 2>&1; then
    print_status "API metadata generated"
else
    print_error "API metadata generation failed"
    exit 1
fi

echo ""
echo "Step 7: DocFX - Build documentation"
if docfx build docs/docfx.json > /dev/null 2>&1; then
    print_status "Documentation built successfully"
else
    print_error "Documentation build failed"
    exit 1
fi

echo ""
echo "Step 8: Pack all NuGet packages"
if dotnet pack Ddap.slnx --no-build --configuration Debug --output ./artifacts > /dev/null 2>&1; then
    PACKAGE_COUNT=$(ls -1 ./artifacts/*.nupkg 2>/dev/null | wc -l)
    print_status "NuGet packages created ($PACKAGE_COUNT packages)"
else
    print_error "Pack failed"
    exit 1
fi

echo ""
echo "======================================"
echo -e "${GREEN}✓ All pipeline steps completed successfully!${NC}"
echo "======================================"
echo ""
echo "Summary:"
echo "  - Code formatted and verified"
echo "  - Solution built (Debug)"
echo "  - $PASSED_COUNT/$TEST_COUNT tests passed"
echo "  - Documentation generated"
echo "  - $PACKAGE_COUNT NuGet packages created"
echo ""
