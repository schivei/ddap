        function setTheme(theme) {
            document.documentElement.setAttribute('data-theme', theme);
            localStorage.setItem('ddap-theme', theme);
            document.querySelectorAll('.theme-btn').forEach(btn => {
                btn.classList.toggle('active', btn.getAttribute('data-theme') === theme);
            });
        }
        
        const savedTheme = localStorage.getItem('ddap-theme');
        const systemPrefersDark = window.matchMedia('(prefers-color-scheme: dark)').matches;
        const systemPrefersContrast = window.matchMedia('(prefers-contrast: more)').matches;
        const initialTheme = savedTheme || (systemPrefersContrast ? 'high-contrast' : (systemPrefersDark ? 'dark' : 'light'));
        setTheme(initialTheme);
        
        document.querySelectorAll('.theme-btn').forEach(btn => {
            btn.addEventListener('click', () => setTheme(btn.getAttribute('data-theme')));
        });
        
        window.matchMedia('(prefers-color-scheme: dark)').addEventListener('change', (e) => {
            if (!localStorage.getItem('ddap-theme')) {
                setTheme(e.matches ? 'dark' : 'light');
            }
        });
        
        window.matchMedia('(prefers-contrast: more)').addEventListener('change', (e) => {
            if (!localStorage.getItem('ddap-theme')) {
                setTheme(e.matches ? 'high-contrast' : 'light');
            }
        });
    </script>
