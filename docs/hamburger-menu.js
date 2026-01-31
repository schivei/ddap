/**
 * DDAP Hamburger Menu for Mobile
 * Provides collapsible navigation for mobile devices
 */

(function() {
    'use strict';
    
    /**
     * Create hamburger button HTML
     */
    function createHamburgerButton() {
        return `
            <button class="hamburger-menu" aria-label="Toggle navigation menu" aria-expanded="false">
                <span></span>
                <span></span>
                <span></span>
            </button>
        `;
    }
    
    /**
     * Initialize hamburger menu
     */
    function initHamburgerMenu() {
        // Check if already initialized
        if (document.querySelector('.hamburger-menu')) return;
        
        const nav = document.querySelector('.nav-container');
        const navLinks = document.querySelector('.nav-links');
        
        if (!nav || !navLinks) return;
        
        // Insert hamburger button after nav-controls or before nav-links
        const navControls = document.querySelector('.nav-controls');
        if (navControls && navControls.parentElement) {
            // Insert after the nav-controls container
            navControls.parentElement.insertAdjacentHTML('beforeend', createHamburgerButton());
        } else {
            // Fallback: insert before nav-links
            navLinks.insertAdjacentHTML('beforebegin', createHamburgerButton());
        }
        
        const hamburger = document.querySelector('.hamburger-menu');
        
        // Toggle menu on hamburger click
        hamburger.addEventListener('click', function() {
            const isOpen = navLinks.classList.toggle('active');
            hamburger.classList.toggle('active');
            hamburger.setAttribute('aria-expanded', isOpen);
            document.body.classList.toggle('menu-open', isOpen);
        });
        
        // Close menu when clicking on a link (but not on theme/language controls)
        const links = navLinks.querySelectorAll('a');
        links.forEach(link => {
            link.addEventListener('click', function() {
                navLinks.classList.remove('active');
                hamburger.classList.remove('active');
                hamburger.setAttribute('aria-expanded', 'false');
                document.body.classList.remove('menu-open');
            });
        });
        
        // Close menu when clicking overlay (outside menu, theme, and language controls)
        document.addEventListener('click', function(e) {
            const themeToggle = document.getElementById('theme-toggle');
            const langSwitcher = document.getElementById('language-switcher');
            const langDropdown = document.getElementById('language-dropdown');
            const navControls = document.querySelector('.nav-controls');
            
            // Don't close if clicking theme or language controls
            if (themeToggle && themeToggle.contains(e.target)) return;
            if (langSwitcher && langSwitcher.contains(e.target)) return;
            if (langDropdown && langDropdown.contains(e.target)) return;
            if (navControls && navControls.contains(e.target)) return;
            
            if (document.body.classList.contains('menu-open') && 
                !navLinks.contains(e.target) && 
                !hamburger.contains(e.target)) {
                navLinks.classList.remove('active');
                hamburger.classList.remove('active');
                hamburger.setAttribute('aria-expanded', 'false');
                document.body.classList.remove('menu-open');
            }
        });
        
        // Close menu on Escape key
        document.addEventListener('keydown', function(e) {
            if (e.key === 'Escape' && document.body.classList.contains('menu-open')) {
                navLinks.classList.remove('active');
                hamburger.classList.remove('active');
                hamburger.setAttribute('aria-expanded', 'false');
                document.body.classList.remove('menu-open');
            }
        });
    }
    
    // Initialize when DOM is ready
    if (document.readyState === 'loading') {
        document.addEventListener('DOMContentLoaded', initHamburgerMenu);
    } else {
        initHamburgerMenu();
    }
})();
