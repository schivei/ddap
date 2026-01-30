#!/bin/bash
# Script to generate HTML documentation pages from markdown files
# This creates HTML wrappers that use the same layout as index.html

set -e

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
DOCS_DIR="$SCRIPT_DIR"
TEMPLATE="$DOCS_DIR/doc-template.html"

# List of markdown documentation files (excluding API reference and locales)
MD_FILES=(
    "get-started.md"
    "philosophy.md"
    "database-providers.md"
    "api-providers.md"
    "auto-reload.md"
    "templates.md"
    "architecture.md"
    "advanced.md"
    "troubleshooting.md"
    "client-getting-started.md"
    "client-rest.md"
    "client-graphql.md"
    "client-grpc.md"
    "extended-types.md"
    "raw-queries.md"
)

# Function to generate HTML from template
generate_html() {
    local md_file="$1"
    local html_file="${md_file%.md}.html"
    local title="${md_file%.md}"
    
    # Convert filename to title (e.g., "get-started" -> "Get Started")
    title=$(echo "$title" | sed 's/-/ /g' | sed 's/\b\(.\)/\u\1/g')
    
    echo "Generating $html_file from $md_file..."
    
    # Read template and replace placeholders
    local content=$(cat "$TEMPLATE")
    content="${content//\{\{TITLE\}\}/$title}"
    content="${content//\{\{MD_FILE\}\}/$md_file}"
    
    # Write HTML file
    echo "$content" > "$DOCS_DIR/$html_file"
}

# Main execution
echo "Generating documentation HTML pages..."
echo "======================================="

for md_file in "${MD_FILES[@]}"; do
    if [ -f "$DOCS_DIR/$md_file" ]; then
        generate_html "$md_file"
    else
        echo "Warning: $md_file not found, skipping..."
    fi
done

echo ""
echo "Done! Generated HTML pages for documentation."
echo "The pages use the same layout as index.html with navigation, theme toggle, and language switcher."
