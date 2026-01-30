/**
 * DDAP Language Switcher
 * Multi-language support with automatic detection and user preference persistence
 * WCAG AA compliant with localStorage persistence
 */

(function() {
    'use strict';
    
    const SUPPORTED_LANGUAGES = {
        'en': { name: 'English', flag: '🇺🇸', dir: 'ltr' },
        'pt-br': { name: 'Português (Brasil)', flag: '🇧🇷', dir: 'ltr' },
        'es': { name: 'Español', flag: '🇪🇸', dir: 'ltr' },
        'fr': { name: 'Français', flag: '🇫🇷', dir: 'ltr' },
        'de': { name: 'Deutsch', flag: '🇩🇪', dir: 'ltr' },
        'ja': { name: '日本語', flag: '🇯🇵', dir: 'ltr' },
        'zh': { name: '中文', flag: '🇨🇳', dir: 'ltr' }
    };
    
    const DEFAULT_LANGUAGE = 'en';
    const STORAGE_KEY = 'ddap-language';
    
    /**
     * Get the user's preferred language
     * Priority: URL path > localStorage > navigator.language > default (en)
     */
    function getPreferredLanguage() {
        // Check URL path first (e.g., /pt-br/get-started.html)
        const pathParts = window.location.pathname.split('/').filter(p => p);
        if (pathParts.length > 0) {
            const firstPart = pathParts[0];
            if (SUPPORTED_LANGUAGES[firstPart]) {
                return firstPart;
            }
        }
        
        // Check localStorage
        const savedLanguage = localStorage.getItem(STORAGE_KEY);
        if (savedLanguage && SUPPORTED_LANGUAGES[savedLanguage]) {
            return savedLanguage;
        }
        
        // Check browser language
        if (navigator.language || navigator.userLanguage) {
            const browserLang = (navigator.language || navigator.userLanguage).toLowerCase();
            
            // Direct match (e.g., 'pt-br')
            if (SUPPORTED_LANGUAGES[browserLang]) {
                return browserLang;
            }
            
            // Language code match (e.g., 'pt' -> 'pt-br')
            const langCode = browserLang.split('-')[0];
            for (const key in SUPPORTED_LANGUAGES) {
                if (key.startsWith(langCode)) {
                    return key;
                }
            }
        }
        
        // Default to English
        return DEFAULT_LANGUAGE;
    }
    
    /**
     * Get current page path relative to docs root
     */
    function getCurrentPagePath() {
        const path = window.location.pathname;
        const pathParts = path.split('/').filter(p => p);
        
        // Check if we're in a locale directory
        const locales = Object.keys(SUPPORTED_LANGUAGES);
        if (pathParts.length > 0 && locales.includes(pathParts[0])) {
            // Remove locale from path
            return pathParts.slice(1).join('/') || 'index.html';
        }
        
        return pathParts.join('/') || 'index.html';
    }
    
    /**
     * Build URL for a specific language
     */
    function buildLanguageUrl(language, pagePath) {
        // Get current path without leading slash
        const currentPath = window.location.pathname;
        const pathParts = currentPath.split('/').filter(p => p);
        
        // Check if we're already in a locale directory
        const locales = Object.keys(SUPPORTED_LANGUAGES);
        let currentLocale = null;
        let actualPath = pagePath;
        
        // Remove any existing locale from path
        if (pathParts.length > 0 && locales.includes(pathParts[0])) {
            currentLocale = pathParts[0];
            actualPath = pathParts.slice(1).join('/') || 'index.html';
        } else {
            actualPath = pathParts.join('/') || 'index.html';
        }
        
        // For English, use root path; for others, use /locale/ prefix
        if (language === 'en' || language === DEFAULT_LANGUAGE) {
            return `/${actualPath}`;
        } else {
            return `/${language}/${actualPath}`;
        }
    }
    
    /**
     * Apply language to document
     */
    function applyLanguage(language, persist = true) {
        if (!SUPPORTED_LANGUAGES[language]) {
            console.warn(`Invalid language: ${language}. Using default.`);
            language = DEFAULT_LANGUAGE;
        }
        
        const langInfo = SUPPORTED_LANGUAGES[language];
        
        // Set document language and direction
        document.documentElement.setAttribute('lang', language);
        document.documentElement.setAttribute('dir', langInfo.dir);
        
        // Save to localStorage if persist is true
        if (persist) {
            localStorage.setItem(STORAGE_KEY, language);
        }
        
        // Update language switcher UI
        updateLanguageSwitcher(language);
        
        // Announce to screen readers
        announceLanguageChange(language);
    }
    
    /**
     * Switch to a different language
     * Navigates to the static HTML page in the correct locale folder
     */
    function switchLanguage(language) {
        if (!SUPPORTED_LANGUAGES[language]) {
            console.warn(`Invalid language: ${language}`);
            return;
        }
        
        applyLanguage(language);
        
        const currentPage = getCurrentPagePath();
        
        // Navigate to the static HTML page in the correct locale folder
        window.location.href = buildLanguageUrl(language, currentPage);
    }
    
    /**
     * Update language switcher UI
     */
    function updateLanguageSwitcher(currentLanguage) {
        const switcher = document.getElementById('language-switcher');
        const button = document.getElementById('language-toggle');
        const dropdown = document.getElementById('language-dropdown');
        
        if (!switcher || !button || !dropdown) return;
        
        const langInfo = SUPPORTED_LANGUAGES[currentLanguage];
        
        // Update button
        const flagSpan = button.querySelector('.language-flag');
        const nameSpan = button.querySelector('.language-name');
        
        if (flagSpan) flagSpan.textContent = langInfo.flag;
        if (nameSpan) nameSpan.textContent = langInfo.name;
        
        button.setAttribute('aria-label', `Current language: ${langInfo.name}. Click to change language.`);
        
        // Update dropdown items
        const items = dropdown.querySelectorAll('.language-option');
        items.forEach(item => {
            const lang = item.getAttribute('data-language');
            if (lang === currentLanguage) {
                item.classList.add('active');
                item.setAttribute('aria-current', 'true');
            } else {
                item.classList.remove('active');
                item.removeAttribute('aria-current');
            }
        });
    }
    
    /**
     * Announce language change to screen readers
     */
    function announceLanguageChange(language) {
        let announcer = document.getElementById('language-announcer');
        if (!announcer) {
            announcer = document.createElement('div');
            announcer.id = 'language-announcer';
            announcer.className = 'sr-only';
            announcer.setAttribute('role', 'status');
            announcer.setAttribute('aria-live', 'polite');
            announcer.setAttribute('aria-atomic', 'true');
            document.body.appendChild(announcer);
        }
        
        const langInfo = SUPPORTED_LANGUAGES[language];
        announcer.textContent = `Language changed to ${langInfo.name}`;
        
        setTimeout(() => {
            announcer.textContent = '';
        }, 1000);
    }
    
    /**
     * Create language switcher UI
     */
    function createLanguageSwitcher() {
        // Check if already exists
        if (document.getElementById('language-switcher')) return;
        
        const currentLang = getPreferredLanguage();
        const langInfo = SUPPORTED_LANGUAGES[currentLang];
        
        // Create switcher HTML
        const switcherHTML = `
            <div id="language-switcher" class="language-switcher">
                <button id="language-toggle" class="language-toggle" aria-haspopup="true" aria-expanded="false">
                    <span class="language-flag" aria-hidden="true">${langInfo.flag}</span>
                    <span class="language-name">${langInfo.name}</span>
                    <span class="language-arrow" aria-hidden="true">▼</span>
                </button>
                <div id="language-dropdown" class="language-dropdown" role="menu" aria-label="Language selection">
                    ${Object.entries(SUPPORTED_LANGUAGES).map(([code, info]) => `
                        <button class="language-option ${code === currentLang ? 'active' : ''}" 
                                data-language="${code}" 
                                role="menuitem"
                                ${code === currentLang ? 'aria-current="true"' : ''}>
                            <span class="language-flag" aria-hidden="true">${info.flag}</span>
                            <span class="language-name">${info.name}</span>
                        </button>
                    `).join('')}
                </div>
            </div>
        `;
        
        // Insert into nav
        const nav = document.querySelector('.nav-links');
        if (nav) {
            const themeToggle = document.getElementById('theme-toggle');
            if (themeToggle) {
                themeToggle.insertAdjacentHTML('beforebegin', switcherHTML);
            } else {
                nav.insertAdjacentHTML('beforeend', switcherHTML);
            }
        }
    }
    
    /**
     * Set up language switcher event listeners
     */
    function setupLanguageSwitcher() {
        const button = document.getElementById('language-toggle');
        const dropdown = document.getElementById('language-dropdown');
        
        if (!button || !dropdown) return;
        
        // Toggle dropdown
        button.addEventListener('click', () => {
            const isExpanded = button.getAttribute('aria-expanded') === 'true';
            button.setAttribute('aria-expanded', !isExpanded);
            dropdown.classList.toggle('show');
        });
        
        // Language option clicks
        dropdown.querySelectorAll('.language-option').forEach(option => {
            option.addEventListener('click', () => {
                const language = option.getAttribute('data-language');
                if (language) {
                    switchLanguage(language);
                }
            });
        });
        
        // Close dropdown when clicking outside
        document.addEventListener('click', (e) => {
            if (!button.contains(e.target) && !dropdown.contains(e.target)) {
                button.setAttribute('aria-expanded', 'false');
                dropdown.classList.remove('show');
            }
        });
        
        // Keyboard navigation
        button.addEventListener('keydown', (e) => {
            if (e.key === 'Escape') {
                button.setAttribute('aria-expanded', 'false');
                dropdown.classList.remove('show');
            }
        });
        
        dropdown.addEventListener('keydown', (e) => {
            const options = Array.from(dropdown.querySelectorAll('.language-option'));
            const currentIndex = options.indexOf(document.activeElement);
            
            if (e.key === 'ArrowDown') {
                e.preventDefault();
                const nextIndex = (currentIndex + 1) % options.length;
                options[nextIndex].focus();
            } else if (e.key === 'ArrowUp') {
                e.preventDefault();
                const prevIndex = (currentIndex - 1 + options.length) % options.length;
                options[prevIndex].focus();
            } else if (e.key === 'Escape') {
                button.setAttribute('aria-expanded', 'false');
                dropdown.classList.remove('show');
                button.focus();
            }
        });
    }
    
    /**
     * Initialize language system
     */
    function init() {
        // Apply initial language immediately
        const initialLanguage = getPreferredLanguage();
        
        // Set language attribute early
        document.documentElement.setAttribute('lang', initialLanguage);
        document.documentElement.setAttribute('dir', SUPPORTED_LANGUAGES[initialLanguage].dir);
        
        // Wait for DOM to be ready
        if (document.readyState === 'loading') {
            document.addEventListener('DOMContentLoaded', () => {
                createLanguageSwitcher();
                setupLanguageSwitcher();
                applyLanguage(initialLanguage);
            });
        } else {
            createLanguageSwitcher();
            setupLanguageSwitcher();
            applyLanguage(initialLanguage);
        }
    }
    
    // Initialize immediately
    init();
    
    // Expose API globally for testing/debugging
    window.ddapLanguage = {
        switch: switchLanguage,
        current: function() {
            return localStorage.getItem(STORAGE_KEY) || DEFAULT_LANGUAGE;
        },
        supported: function() {
            return Object.keys(SUPPORTED_LANGUAGES);
        },
        reset: function() {
            // Clear stored preference and revert to browser-detected language
            // Note: This updates the UI without reloading the page
            localStorage.removeItem(STORAGE_KEY);
            applyLanguage(getPreferredLanguage(), false);
        }
    };
})();
