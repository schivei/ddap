# Documentation Templates

This directory contains modular templates for generating the DDAP documentation site.

## Structure

### Documentation Pages (`doc-page-*`)
- `doc-page-base.tpl.html` - Base HTML structure for documentation pages
- `doc-page-styles.css` - CSS styles for documentation pages  
- `doc-page-script.js` - JavaScript for theme management and markdown parsing

### Index Page (`index-*`)
- `index-base.tpl.html` - Base HTML structure for the index page
- `index-content.html` - HTML content (body) for the index page
- `index-styles.css` - CSS styles for the index page
- `index-script.js` - JavaScript for theme management on the index page

### Build Script
- `build-docs.sh` - Shell script to assemble final HTML files from templates

## Usage

The build script is called by the GitHub Actions workflow:

```bash
# Build a documentation page
./build-docs.sh <output_dir> <mdfile_name>

# Build the index page  
./build-docs.sh <output_dir> index
```

## Placeholders

Templates use the following placeholders:
- `{{CSS_CONTENT}}` - Replaced with CSS content
- `{{JS_CONTENT}}` - Replaced with JavaScript content  
- `{{HTML_CONTENT}}` - Replaced with HTML body content (index only)
- `{{MDFILE}}` - Replaced with markdown filename (doc pages only)

## Advantages

- **Separation of Concerns**: CSS, JS, and HTML are in separate files
- **Maintainability**: Easier to edit and update specific parts
- **Reusability**: Styles and scripts can be shared across templates
- **Version Control**: Changes are easier to track in diffs
- **Reduced YAML Size**: Keeps the workflow file clean and minimal
