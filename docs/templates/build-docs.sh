#!/bin/bash
# Script to build documentation HTML files from templates
# Usage: ./build-docs.sh <output_dir> <mdfile_name>

set -e

OUTPUT_DIR="${1:-docs/_site}"
MDFILE="${2}"
SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"

# Function to build a documentation page
build_doc_page() {
    local mdfile="$1"
    local output_file="$2"
    
    # Read templates
    local html_template=$(cat "$SCRIPT_DIR/doc-page-base.tpl.html")
    local css_content=$(cat "$SCRIPT_DIR/doc-page-styles.css")
    local js_content=$(cat "$SCRIPT_DIR/doc-page-script.js")
    
    # Replace {{MDFILE}} in JS content first
    js_content="${js_content//\{\{MDFILE\}\}/$mdfile}"
    
    # Replace placeholders in HTML template
    html_template="${html_template//\{\{CSS_CONTENT\}\}/$css_content}"
    html_template="${html_template//\{\{JS_CONTENT\}\}/$js_content}"
    
    # Write output
    echo "$html_template" > "$output_file"
}

# Function to build index page
build_index_page() {
    local output_file="$1"
    
    # Read templates
    local html_template=$(cat "$SCRIPT_DIR/index-base.tpl.html")
    local css_content=$(cat "$SCRIPT_DIR/index-styles.css")
    local html_content=$(cat "$SCRIPT_DIR/index-content.html")
    local js_content=$(cat "$SCRIPT_DIR/index-script.js")
    
    # Replace placeholders
    html_template="${html_template//\{\{CSS_CONTENT\}\}/$css_content}"
    html_template="${html_template//\{\{HTML_CONTENT\}\}/$html_content}"
    html_template="${html_template//\{\{JS_CONTENT\}\}/$js_content}"
    
    # Write output
    echo "$html_template" > "$output_file"
}

# Main execution
if [ "$MDFILE" = "index" ]; then
    build_index_page "$OUTPUT_DIR/index.html"
elif [ -n "$MDFILE" ]; then
    build_doc_page "$MDFILE" "$OUTPUT_DIR/${MDFILE}.html"
else
    echo "Error: No file specified"
    exit 1
fi
