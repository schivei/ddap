/**
 * DDAP Theme Toggle
 * 3-way theme switcher: Light → Dark → High Contrast → Light
 * WCAG AA/AAA compliant with localStorage persistence
 */

(function() {
    'use strict';
    
    const THEMES = ['light', 'dark', 'high-contrast'];
    const STORAGE_KEY = 'ddap-theme';
    const THEME_ICONS = {
        'light': '🌙',
        'dark': '⚡',
        'high-contrast': '☀️'
    };
    const THEME_LABELS = {
        'light': 'Light theme',
        'dark': 'Dark theme',
        'high-contrast': 'High contrast theme'
    };
    
    /**
     * Get the user's preferred theme
     * Priority: localStorage > prefers-color-scheme > default (light)
     */
    function getPreferredTheme() {
        // Check localStorage first
        const savedTheme = localStorage.getItem(STORAGE_KEY);
        if (savedTheme && THEMES.includes(savedTheme)) {
            return savedTheme;
        }
        
        // Check system preference
        if (window.matchMedia) {
            if (window.matchMedia('(prefers-color-scheme: dark)').matches) {
                return 'dark';
            }
            // Check for high contrast preference
            if (window.matchMedia('(prefers-contrast: more)').matches) {
                return 'high-contrast';
            }
        }
        
        // Default to light
        return 'light';
    }
    
    /**
     * Apply theme to document
     */
    function applyTheme(theme) {
        if (!THEMES.includes(theme)) {
            console.warn(`Invalid theme: ${theme}. Using default.`);
            theme = 'light';
        }
        
        document.documentElement.setAttribute('data-theme', theme);
        localStorage.setItem(STORAGE_KEY, theme);
        
        // Update toggle button
        updateThemeToggle(theme);
        
        // Announce to screen readers
        announceThemeChange(theme);
    }
    
    /**
     * Get the next theme in the cycle
     */
    function getNextTheme(currentTheme) {
        const currentIndex = THEMES.indexOf(currentTheme);
        const nextIndex = (currentIndex + 1) % THEMES.length;
        return THEMES[nextIndex];
    }
    
    /**
     * Toggle to the next theme
     */
    function toggleTheme() {
        const currentTheme = document.documentElement.getAttribute('data-theme') || 'light';
        const nextTheme = getNextTheme(currentTheme);
        applyTheme(nextTheme);
    }
    
    /**
     * Update theme toggle button appearance and label
     */
    function updateThemeToggle(currentTheme) {
        const toggle = document.getElementById('theme-toggle');
        if (!toggle) return;
        
        const nextTheme = getNextTheme(currentTheme);
        const icon = toggle.querySelector('.theme-icon');
        
        if (icon) {
            icon.textContent = THEME_ICONS[nextTheme];
        }
        
        // Update aria-label to describe what will happen on click
        toggle.setAttribute('aria-label', `Switch to ${THEME_LABELS[nextTheme]}`);
    }
    
    /**
     * Announce theme change to screen readers
     */
    function announceThemeChange(theme) {
        // Create or update announcement element
        let announcer = document.getElementById('theme-announcer');
        if (!announcer) {
            announcer = document.createElement('div');
            announcer.id = 'theme-announcer';
            announcer.className = 'sr-only';
            announcer.setAttribute('role', 'status');
            announcer.setAttribute('aria-live', 'polite');
            announcer.setAttribute('aria-atomic', 'true');
            document.body.appendChild(announcer);
        }
        
        // Announce the change
        announcer.textContent = `Theme changed to ${THEME_LABELS[theme]}`;
        
        // Clear after announcement
        setTimeout(() => {
            announcer.textContent = '';
        }, 1000);
    }
    
    /**
     * Initialize theme system
     */
    function init() {
        // Apply initial theme immediately (before DOM loads)
        const initialTheme = getPreferredTheme();
        applyTheme(initialTheme);
        
        // Wait for DOM to be ready
        if (document.readyState === 'loading') {
            document.addEventListener('DOMContentLoaded', setupThemeToggle);
        } else {
            setupThemeToggle();
        }
    }
    
    /**
     * Set up theme toggle button event listeners
     */
    function setupThemeToggle() {
        const toggle = document.getElementById('theme-toggle');
        if (!toggle) {
            console.warn('Theme toggle button not found');
            return;
        }
        
        // Click handler
        toggle.addEventListener('click', toggleTheme);
        
        // Keyboard handler (Space and Enter)
        toggle.addEventListener('keydown', function(e) {
            if (e.key === ' ' || e.key === 'Enter') {
                e.preventDefault();
                toggleTheme();
            }
        });
        
        // Update button to reflect current theme
        const currentTheme = document.documentElement.getAttribute('data-theme');
        updateThemeToggle(currentTheme);
    }
    
    /**
     * Listen for system theme changes
     */
    function watchSystemTheme() {
        if (!window.matchMedia) return;
        
        // Watch for dark mode changes
        const darkModeQuery = window.matchMedia('(prefers-color-scheme: dark)');
        darkModeQuery.addEventListener('change', function(e) {
            // Only respond if user hasn't set a preference
            if (!localStorage.getItem(STORAGE_KEY)) {
                applyTheme(e.matches ? 'dark' : 'light');
            }
        });
        
        // Watch for high contrast changes
        const highContrastQuery = window.matchMedia('(prefers-contrast: more)');
        highContrastQuery.addEventListener('change', function(e) {
            // Only respond if user hasn't set a preference
            if (!localStorage.getItem(STORAGE_KEY)) {
                if (e.matches) {
                    applyTheme('high-contrast');
                }
            }
        });
    }
    
    /**
     * Add smooth transition class after page load
     * Prevents theme transition on initial load
     */
    function enableTransitions() {
        document.documentElement.classList.add('theme-transitions-enabled');
    }
    
    // Initialize immediately
    init();
    
    // Watch for system theme changes
    watchSystemTheme();
    
    // Enable transitions after a short delay
    window.addEventListener('load', function() {
        setTimeout(enableTransitions, 100);
    });
    
    // Expose toggle function globally for testing/debugging
    window.ddapTheme = {
        toggle: toggleTheme,
        apply: applyTheme,
        current: function() {
            return document.documentElement.getAttribute('data-theme');
        },
        reset: function() {
            localStorage.removeItem(STORAGE_KEY);
            applyTheme(getPreferredTheme());
        }
    };
})();
