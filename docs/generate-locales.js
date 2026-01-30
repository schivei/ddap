#!/usr/bin/env node

const fs = require('fs');
const path = require('path');

// Configuration
const baseHtmlPath = path.join(__dirname, 'index.html');
const translationsPath = path.join(__dirname, 'index-translations.json');
const languages = ['pt-br', 'es', 'fr', 'de', 'ja', 'zh'];

// Read base HTML and translations
const baseHtml = fs.readFileSync(baseHtmlPath, 'utf8');
const translations = JSON.parse(fs.readFileSync(translationsPath, 'utf8'));

// Function to replace translatable content
function localizeHtml(html, lang, t) {
    let localized = html;

    // Update lang attribute
    localized = localized.replace(
        /<html lang="en"/,
        `<html lang="${lang}"`
    );

    // Update title
    localized = localized.replace(
        /<title>.*?<\/title>/,
        `<title>${t.title}</title>`
    );

    // Update meta description
    localized = localized.replace(
        /<meta name="description" content=".*?">/,
        `<meta name="description" content="${t.description}">`
    );

    // Update hreflang URLs to use actual subdirectories
    localized = localized.replace(
        /href="https:\/\/schivei\.github\.io\/ddap\/index\.html\?lang=pt-br"/,
        'href="https://schivei.github.io/ddap/pt-br/index.html"'
    );
    localized = localized.replace(
        /href="https:\/\/schivei\.github\.io\/ddap\/index\.html\?lang=es"/,
        'href="https://schivei.github.io/ddap/es/index.html"'
    );
    localized = localized.replace(
        /href="https:\/\/schivei\.github\.io\/ddap\/index\.html\?lang=fr"/,
        'href="https://schivei.github.io/ddap/fr/index.html"'
    );
    localized = localized.replace(
        /href="https:\/\/schivei\.github\.io\/ddap\/index\.html\?lang=de"/,
        'href="https://schivei.github.io/ddap/de/index.html"'
    );
    localized = localized.replace(
        /href="https:\/\/schivei\.github\.io\/ddap\/index\.html\?lang=ja"/,
        'href="https://schivei.github.io/ddap/ja/index.html"'
    );
    localized = localized.replace(
        /href="https:\/\/schivei\.github\.io\/ddap\/index\.html\?lang=zh"/,
        'href="https://schivei.github.io/ddap/zh/index.html"'
    );

    // Update Skip to content text
    localized = localized.replace(
        /<a href="#main-content" class="skip-to-content">Skip to main content<\/a>/,
        `<a href="#main-content" class="skip-to-content">${t.skip_to_content}</a>`
    );

    // Update hero tagline
    localized = localized.replace(
        /<p class="hero-tagline">Dynamic Data API Provider<\/p>/,
        `<p class="hero-tagline">${t.hero_tagline}</p>`
    );

    // Update hero subtitle
    localized = localized.replace(
        /<p class="hero-subtitle">You control everything\. We handle the boilerplate\.<\/p>/,
        `<p class="hero-subtitle">${t.hero_subtitle}</p>`
    );

    // Update philosophy title
    localized = localized.replace(
        /<h2 class="philosophy-title">Developer in Control<\/h2>/,
        `<h2 class="philosophy-title">${t.philosophy_title}</h2>`
    );

    // Update philosophy description
    localized = localized.replace(
        /<p class="philosophy-description">\s*Unlike frameworks that force decisions on you[\s\S]*?<\/p>/,
        `<p class="philosophy-description">
                            ${t.philosophy_description}
                        </p>`
    );

    // Update Get Started button
    localized = localized.replace(
        /<a href="get-started\.html" class="btn btn-primary">\s*Get Started/,
        `<a href="../get-started.html" class="btn btn-primary">
                            ${t.get_started_btn}`
    );

    // Update View Example button
    localized = localized.replace(
        /View Example\s*<span aria-hidden="true">↓<\/span>/,
        `${t.view_example_btn}
                            <span aria-hidden="true">↓</span>`
    );

    // Update "Why DDAP?" section title
    localized = localized.replace(
        /<h2 id="features-title" class="section-title">Why DDAP\?<\/h2>/,
        `<h2 id="features-title" class="section-title">${t.why_ddap}</h2>`
    );

    // Update Quick Start section title
    localized = localized.replace(
        /<h2 id="quick-start-title" class="section-title">Quick Start<\/h2>/,
        `<h2 id="quick-start-title" class="section-title">${t.quick_start}</h2>`
    );

    // Update comparison section title
    localized = localized.replace(
        /<h2 id="comparison-title" class="section-title">DDAP vs Other Frameworks<\/h2>/,
        `<h2 id="comparison-title" class="section-title">${t.ddap_vs_others}</h2>`
    );

    // Update Documentation section title
    localized = localized.replace(
        /<h2 id="documentation-title" class="section-title">Documentation<\/h2>/,
        `<h2 id="documentation-title" class="section-title">${t.documentation}</h2>`
    );

    // Update Packages section title
    localized = localized.replace(
        /<h2 id="packages-title" class="section-title">Packages<\/h2>/,
        `<h2 id="packages-title" class="section-title">${t.packages}</h2>`
    );

    // Update navigation links
    localized = localized.replace(
        /<a href="get-started\.html">Get Started<\/a>/,
        `<a href="../get-started.html">${t.nav_get_started}</a>`
    );

    localized = localized.replace(
        /<a href="philosophy\.html">Philosophy<\/a>/,
        `<a href="../philosophy.html">${t.nav_philosophy}</a>`
    );

    // Update footer Resources section title
    localized = localized.replace(
        /<h3>Resources<\/h3>/,
        `<h3>${t.resources}</h3>`
    );

    // Update footer Community section title
    localized = localized.replace(
        /<h3>Community<\/h3>/,
        `<h3>${t.community}</h3>`
    );

    // Update footer Legal section title
    localized = localized.replace(
        /<h3>Legal<\/h3>/,
        `<h3>${t.legal}</h3>`
    );

    // Update footer text
    localized = localized.replace(
        /<p>Built with ❤️ by developers who believe in control, not constraints\.<\/p>/,
        `<p>${t.footer_text}</p>`
    );

    // Update copyright
    localized = localized.replace(
        /<p class="copyright">&copy; 2024 DDAP Contributors\. All rights reserved\.<\/p>/,
        `<p class="copyright">${t.copyright}</p>`
    );

    // Update footer Get Started link
    localized = localized.replace(
        /<li><a href="get-started\.html">Getting Started<\/a><\/li>/,
        `<li><a href="../get-started.html">${t.nav_get_started}</a></li>`
    );

    // Update footer Philosophy link
    localized = localized.replace(
        /<li><a href="philosophy\.html">Philosophy<\/a><\/li>/,
        `<li><a href="../philosophy.html">${t.nav_philosophy}</a></li>`
    );

    // Update all documentation links to be relative
    const docPages = [
        'philosophy.html',
        'get-started.html',
        'database-providers.html',
        'api-providers.html',
        'auto-reload.html',
        'templates.html',
        'architecture.html',
        'advanced.html',
        'troubleshooting.html'
    ];

    docPages.forEach(page => {
        const regex = new RegExp(`href="${page}"`, 'g');
        localized = localized.replace(regex, `href="../${page}"`);
    });

    // Update CSS and JS references to be relative
    localized = localized.replace(
        /href="styles\.css"/,
        'href="../styles.css"'
    );

    localized = localized.replace(
        /src="theme-toggle\.js"/,
        'src="../theme-toggle.js"'
    );

    localized = localized.replace(
        /src="language-switcher\.js"/,
        'src="../language-switcher.js"'
    );

    return localized;
}

// Generate localized files
console.log('Generating localized index.html files...\n');

languages.forEach(lang => {
    const t = translations[lang];
    if (!t) {
        console.error(`❌ No translations found for ${lang}`);
        return;
    }

    const langDir = path.join(__dirname, lang);
    
    // Create directory if it doesn't exist
    if (!fs.existsSync(langDir)) {
        fs.mkdirSync(langDir, { recursive: true });
        console.log(`✅ Created directory: ${lang}/`);
    } else {
        console.log(`📁 Directory exists: ${lang}/`);
    }

    // Generate localized HTML
    const localizedHtml = localizeHtml(baseHtml, lang, t);

    // Write to file
    const outputPath = path.join(langDir, 'index.html');
    fs.writeFileSync(outputPath, localizedHtml, 'utf8');
    console.log(`✅ Generated: ${lang}/index.html`);
});

console.log('\n✨ All localized files generated successfully!');
