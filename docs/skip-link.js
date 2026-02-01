/**
 * Skip to Content Link Enhancement
 * Ensures the skip link properly focuses the main content for keyboard navigation
 */
(function() {
    'use strict';
    
    // Wait for DOM to be ready
    if (document.readyState === 'loading') {
        document.addEventListener('DOMContentLoaded', init);
    } else {
        init();
    }
    
    function init() {
        const skipLink = document.querySelector('.skip-to-content');
        const mainContent = document.getElementById('main-content');
        
        if (!skipLink || !mainContent) return;
        
        skipLink.addEventListener('click', function(e) {
            e.preventDefault();
            
            // Focus the main content
            mainContent.focus();
            
            // Scroll to main content
            mainContent.scrollIntoView({ behavior: 'smooth', block: 'start' });
        });
    }
})();
