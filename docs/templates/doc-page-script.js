        // Theme management
        function setTheme(theme) {
            document.documentElement.setAttribute('data-theme', theme);
            localStorage.setItem('ddap-theme', theme);
            document.querySelectorAll('.theme-btn').forEach(btn => {
                btn.classList.toggle('active', btn.getAttribute('data-theme') === theme);
            });
        }
        
        // Initialize theme
        const savedTheme = localStorage.getItem('ddap-theme');
        const systemPrefersDark = window.matchMedia('(prefers-color-scheme: dark)').matches;
        const systemPrefersContrast = window.matchMedia('(prefers-contrast: more)').matches;
        const initialTheme = savedTheme || (systemPrefersContrast ? 'high-contrast' : (systemPrefersDark ? 'dark' : 'light'));
        setTheme(initialTheme);
        
        // Theme button listeners
        document.querySelectorAll('.theme-btn').forEach(btn => {
            btn.addEventListener('click', () => setTheme(btn.getAttribute('data-theme')));
        });
        
        // Listen to system theme changes
        window.matchMedia('(prefers-color-scheme: dark)').addEventListener('change', (e) => {
            if (!localStorage.getItem('ddap-theme')) {
                setTheme(e.matches ? 'dark' : 'light');
            }
        });
        
        // Simple markdown parser
        function parseMarkdown(md) {
            // Split into lines for better processing
            const lines = md.split('\n');
            let html = '';
            let inList = false;
            let inCodeBlock = false;
            let codeBlock = '';
            
            for (let i = 0; i < lines.length; i++) {
                let line = lines[i];
                
                // Handle code blocks
                if (line.trim().startsWith('```')) {
                    if (!inCodeBlock) {
                        inCodeBlock = true;
                        codeBlock = '';
                    } else {
                        html += '<pre><code>' + codeBlock + '</code></pre>';
                        inCodeBlock = false;
                    }
                    continue;
                }
                
                if (inCodeBlock) {
                    codeBlock += line + '\n';
                    continue;
                }
                
                // Handle lists
                if (line.match(/^[\*\-] /) || line.match(/^\d+\. /)) {
                    if (!inList) {
                        html += '<ul>';
                        inList = true;
                    }
                    line = line.replace(/^[\*\-] (.*)$/, '<li>$1</li>');
                    line = line.replace(/^\d+\. (.*)$/, '<li>$1</li>');
                    html += line;
                    continue;
                } else if (inList) {
                    html += '</ul>';
                    inList = false;
                }
                
                // Headers
                if (line.match(/^### /)) {
                    html += line.replace(/^### (.*)$/, '<h3>$1</h3>');
                } else if (line.match(/^## /)) {
                    html += line.replace(/^## (.*)$/, '<h2>$1</h2>');
                } else if (line.match(/^# /)) {
                    html += line.replace(/^# (.*)$/, '<h1>$1</h1>');
                } else if (line.match(/^---$/)) {
                    html += '<hr>';
                } else if (line.match(/^> /)) {
                    html += line.replace(/^> (.*)$/, '<blockquote>$1</blockquote>');
                } else if (line.trim() === '') {
                    html += '</p><p>';
                } else {
                    html += line + ' ';
                }
            }
            
            if (inList) html += '</ul>';
            
            // Wrap in paragraphs
            html = '<p>' + html + '</p>';
            
            // Inline formatting
            html = html.replace(/\*\*(.*?)\*\*/g, '<strong>$1</strong>');
            html = html.replace(/\*([^*]+)\*/g, '<em>$1</em>');
            html = html.replace(/`([^`]+)`/g, '<code>$1</code>');
            html = html.replace(/\[([^\]]+)\]\(([^)]+)\)/g, '<a href="$2">$1</a>');
            
            // Clean up
            html = html.replace(/<p><\/p>/g, '');
            html = html.replace(/<p>(<h[1-6]>)/g, '$1').replace(/(<\/h[1-6]>)<\/p>/g, '$1');
            html = html.replace(/<p>(<pre>)/g, '$1').replace(/(<\/pre>)<\/p>/g, '$1');
            html = html.replace(/<p>(<ul>)/g, '$1').replace(/(<\/ul>)<\/p>/g, '$1');
            html = html.replace(/<p>(<hr>)<\/p>/g, '$1');
            html = html.replace(/<p>(<blockquote>)/g, '$1').replace(/(<\/blockquote>)<\/p>/g, '$1');
            
            return html;
        }
        
        // Load markdown content
        fetch('{{MDFILE}}.md')
            .then(r => r.ok ? r.text() : Promise.reject())
            .then(md => { document.getElementById('content').innerHTML = parseMarkdown(md); })
            .catch(() => { document.getElementById('content').innerHTML = '<h1>Error</h1><p>Could not load document.</p>'; });
    </script>
