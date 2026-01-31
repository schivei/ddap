#!/usr/bin/env node

/**
 * Generate static HTML pages for all documentation pages in all supported languages.
 * This replaces the dynamic markdown loading approach with pre-generated static HTML.
 */

const fs = require('fs');
const path = require('path');
const { marked } = require('marked');
const DOMPurify = require('isomorphic-dompurify');

// Configuration
const DOCS_ROOT = __dirname;
const LOCALES = ['en', 'pt-br', 'es', 'fr', 'de', 'ja', 'zh'];
const LOCALE_NAMES = {
    'en': 'English',
    'pt-br': 'Português (Brasil)',
    'es': 'Español',
    'fr': 'Français',
    'de': 'Deutsch',
    'ja': '日本語',
    'zh': '中文'
};

// Documentation pages to generate (without .md extension)
const DOC_PAGES = [
    'get-started',
    'philosophy',
    'why-ddap',
    'known-issues',
    'database-providers',
    'api-providers',
    'architecture',
    'auto-reload',
    'templates',
    'advanced',
    'troubleshooting',
    'extended-types',
    'raw-queries',
    'client-getting-started',
    'client-rest',
    'client-graphql',
    'client-grpc'
];

/**
 * Read the HTML template
 */
function readTemplate() {
    const templatePath = path.join(DOCS_ROOT, 'doc-template.html');
    return fs.readFileSync(templatePath, 'utf-8');
}

/**
 * Read markdown content for a page in a specific locale
 */
function readMarkdown(pageName, locale) {
    let mdPath;
    
    if (locale === 'en') {
        // English is in root
        mdPath = path.join(DOCS_ROOT, `${pageName}.md`);
    } else {
        // Other locales are in locales/{lang}/
        mdPath = path.join(DOCS_ROOT, 'locales', locale, `${pageName}.md`);
    }
    
    if (!fs.existsSync(mdPath)) {
        console.log(`Warning: ${mdPath} not found, using English fallback`);
        mdPath = path.join(DOCS_ROOT, `${pageName}.md`);
    }
    
    return fs.readFileSync(mdPath, 'utf-8');
}

/**
 * Generate HTML for a specific page and locale
 */
function generatePage(pageName, locale) {
    const template = readTemplate();
    const markdown = readMarkdown(pageName, locale);
    
    // Convert markdown to HTML
    const htmlContent = marked.parse(markdown);
    const sanitizedContent = DOMPurify.sanitize(htmlContent);
    
    // Replace placeholders in template
    let html = template.replace('<!-- CONTENT_PLACEHOLDER -->', `
        <div id="markdown-content" class="markdown-body">
            ${sanitizedContent}
        </div>
    `);
    
    // Update title
    const titleMatch = markdown.match(/^#\s+(.+)$/m);
    const pageTitle = titleMatch ? titleMatch[1] : pageName;
    html = html.replace(/<title>.*?<\/title>/, `<title>${pageTitle} - DDAP</title>`);
    
    // Update language in HTML tag and set current language
    html = html.replace('<html lang="en"', `<html lang="${locale}"`);
    
    // Fix relative paths for pages in subfolders
    if (locale !== 'en') {
        // Add ../ prefix to all relative resource paths
        html = html.replace(/href="(styles\.css|lib\/[^"]+)"/g, 'href="../$1"');
        html = html.replace(/src="(lib\/[^"]+|language-switcher\.js|theme-toggle\.js)"/g, 'src="../$1"');
        
        // Fix navigation links to point back to root for English or to locale folders
        html = html.replace(/href="index\.html"/g, `href="../${locale}/index.html"`);
        html = html.replace(/href="([a-z-]+)\.html"/g, `href="$1.html"`); // Keep same folder links as-is
    }
    
    // Inject script to set current language in localStorage
    const langScript = `
    <script>
        // Set current language
        localStorage.setItem('ddap-language', '${locale}');
        
        // Remove loading indicator since content is static
        document.addEventListener('DOMContentLoaded', function() {
            const loadingDiv = document.getElementById('loading');
            if (loadingDiv) loadingDiv.style.display = 'none';
        });
    </script>
    `;
    html = html.replace('</body>', `${langScript}</body>`);
    
    // Remove dynamic loading script section completely
    // Find and remove the entire script block that does markdown loading
    html = html.replace(/<script>\s*\/\/ Markdown loading[\s\S]*?<\/script>/g, '');
    html = html.replace(/<script>[\s\S]*?fetchMarkdownContent[\s\S]*?<\/script>/g, '');
    html = html.replace(/<script>[\s\S]*?Error Loading Document[\s\S]*?<\/script>/g, '');
    
    return html;
}

/**
 * Main function to generate all pages
 */
function main() {
    console.log('Generating static HTML pages for all locales...\n');
    
    let totalGenerated = 0;
    
    for (const locale of LOCALES) {
        console.log(`\n=== Generating ${LOCALE_NAMES[locale]} (${locale}) ===`);
        
        // Determine output directory
        let outputDir;
        if (locale === 'en') {
            outputDir = DOCS_ROOT; // English in root
        } else {
            outputDir = path.join(DOCS_ROOT, locale);
            // Create directory if it doesn't exist
            if (!fs.existsSync(outputDir)) {
                fs.mkdirSync(outputDir, { recursive: true });
            }
        }
        
        // Generate each page
        for (const pageName of DOC_PAGES) {
            try {
                const html = generatePage(pageName, locale);
                const outputPath = path.join(outputDir, `${pageName}.html`);
                fs.writeFileSync(outputPath, html, 'utf-8');
                console.log(`  ✓ ${pageName}.html`);
                totalGenerated++;
            } catch (error) {
                console.error(`  ✗ ${pageName}.html - ERROR: ${error.message}`);
            }
        }
    }
    
    console.log(`\n✅ Generated ${totalGenerated} HTML pages across ${LOCALES.length} locales`);
    console.log(`   ${DOC_PAGES.length} pages × ${LOCALES.length} locales = ${DOC_PAGES.length * LOCALES.length} total`);
}

// Run the script
main();
