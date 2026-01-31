# DDAP Documentation Localization

This document explains how to generate and maintain localized versions of the DDAP documentation homepage.

## Overview

The localization system generates language-specific versions of `index.html` in dedicated language subdirectories. Each localized page maintains the same HTML structure and functionality while replacing text content with translated versions.

## Supported Languages

- **pt-br**: Portuguese (Brazil)
- **es**: Spanish
- **fr**: French
- **de**: German
- **ja**: Japanese
- **zh**: Chinese

## Files

- `index.html` - Base English version of the homepage
- `index-translations.json` - Contains all translations for supported languages
- `generate-locales.js` - Node.js script that generates localized HTML files

## Generating Localized Pages

To generate or regenerate all localized versions:

```bash
cd docs
node generate-locales.js
```

This will:
1. Read the base `index.html`
2. Read translations from `index-translations.json`
3. Create language subdirectories if they don't exist
4. Generate localized `index.html` files in each language directory

## Directory Structure

After running the script, the structure will be:

```
docs/
├── index.html                    # English (base)
├── index-translations.json       # Translations
├── generate-locales.js          # Generator script
├── styles.css                   # Shared stylesheet
├── theme-toggle.js             # Shared script
├── language-switcher.js        # Shared script
├── pt-br/
│   └── index.html              # Portuguese version
├── es/
│   └── index.html              # Spanish version
├── fr/
│   └── index.html              # French version
├── de/
│   └── index.html              # German version
├── ja/
│   └── index.html              # Japanese version
└── zh/
    └── index.html              # Chinese version
```

## Adding New Translations

To add a new translation:

1. Edit `index-translations.json` and add a new language section following the existing pattern
2. Update the `languages` array in `generate-locales.js` to include the new language code
3. Run `node generate-locales.js` to generate the new localized file

## Translation Keys

The following elements are translated:

- `title` - Page title (in `<title>` tag)
- `description` - Meta description
- `hero_title` - Main hero title
- `hero_tagline` - Hero section tagline
- `hero_subtitle` - Hero section subtitle
- `philosophy_title` - Philosophy section title
- `philosophy_description` - Philosophy section description
- `get_started_btn` - "Get Started" button text
- `view_example_btn` - "View Example" button text
- `why_ddap` - "Why DDAP?" section title
- `quick_start` - "Quick Start" section title
- `ddap_vs_others` - Comparison section title
- `documentation` - Documentation section title
- `packages` - Packages section title
- `resources` - Resources footer section title
- `community` - Community footer section title
- `legal` - Legal footer section title
- `footer_text` - Footer description text
- `copyright` - Copyright notice
- `nav_get_started` - Navigation "Get Started" link
- `nav_philosophy` - Navigation "Philosophy" link
- `nav_github` - Navigation "GitHub" link

## How It Works

The script:

1. **Preserves Structure**: All HTML structure, classes, IDs, and attributes remain unchanged
2. **Translates Text**: Only replaces visible text content with translations
3. **Updates Metadata**: Changes `lang` attribute, `<title>`, and `<meta>` tags
4. **Fixes Paths**: Updates relative links to work from subdirectories (e.g., `styles.css` → `../styles.css`)
5. **Maintains Functionality**: All JavaScript and CSS references continue to work

## Updating Translations

When the base `index.html` changes:

1. Update `index.html` with new content
2. Add corresponding translation keys to `index-translations.json` for all languages
3. Update the replacement logic in `generate-locales.js` if needed
4. Run `node generate-locales.js` to regenerate all localized files

## Testing

After generating localized files, test by:

1. Opening each `<lang>/index.html` in a browser
2. Verifying that:
   - Text is properly translated
   - CSS styling is applied correctly
   - JavaScript functionality works
   - Navigation links work
   - All relative paths resolve correctly

## Notes

- The script preserves code examples, technical terms, and URLs
- Only user-facing text is translated
- All localized pages share the same CSS and JavaScript files from the parent directory
- The script is idempotent - running it multiple times produces the same result
