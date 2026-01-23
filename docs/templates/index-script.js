        const VALID_THEMES = ['light', 'dark', 'high-contrast'];

        function setTheme(theme) {
            // Validate theme
            if (!VALID_THEMES.includes(theme)) {
                console.warn('Invalid theme:', theme);
                theme = 'light';
            }
            
            document.documentElement.setAttribute('data-theme', theme);
            try {
                localStorage.setItem('ddap-theme', theme);
            } catch (e) {
                console.warn('Could not save theme preference:', e);
            }
            document.querySelectorAll('.theme-btn').forEach(btn => {
                btn.classList.toggle('active', btn.getAttribute('data-theme') === theme);
            });
        }
        
        let savedTheme = null;
        try {
            savedTheme = localStorage.getItem('ddap-theme');
        } catch (e) {
            // localStorage might not be available
        }
        const systemPrefersDark = window.matchMedia('(prefers-color-scheme: dark)').matches;
        const systemPrefersContrast = window.matchMedia('(prefers-contrast: more)').matches;
        const initialTheme = savedTheme || (systemPrefersContrast ? 'high-contrast' : (systemPrefersDark ? 'dark' : 'light'));
        setTheme(initialTheme);
        
        document.querySelectorAll('.theme-btn').forEach(btn => {
            btn.addEventListener('click', () => setTheme(btn.getAttribute('data-theme')));
        });
        
        window.matchMedia('(prefers-color-scheme: dark)').addEventListener('change', (e) => {
            try {
                if (!localStorage.getItem('ddap-theme')) {
                    setTheme(e.matches ? 'dark' : 'light');
                }
            } catch (err) {
                // localStorage might not be available
            }
        });
        
        window.matchMedia('(prefers-contrast: more)').addEventListener('change', (e) => {
            try {
                if (!localStorage.getItem('ddap-theme')) {
                    const prefersDark = window.matchMedia('(prefers-color-scheme: dark)').matches;
                    setTheme(e.matches ? 'high-contrast' : (prefersDark ? 'dark' : 'light'));
                }
            } catch (err) {
                // localStorage might not be available
            }
        });
    </script>
