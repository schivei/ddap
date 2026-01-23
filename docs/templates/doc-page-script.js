        // Initialize Mermaid
        if (typeof mermaid !== 'undefined') {
            mermaid.initialize({ 
                startOnLoad: false,
                theme: 'default',
                securityLevel: 'loose'
            });
        }
        
        // Theme management
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
                // localStorage might not be available
                console.warn('Could not save theme preference:', e);
            }
            document.querySelectorAll('.theme-btn').forEach(btn => {
                btn.classList.toggle('active', btn.getAttribute('data-theme') === theme);
            });
        }
        
        // Initialize theme
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
        
        // Theme button listeners
        document.querySelectorAll('.theme-btn').forEach(btn => {
            btn.addEventListener('click', () => setTheme(btn.getAttribute('data-theme')));
        });
        
        // Listen to system theme changes
        window.matchMedia('(prefers-color-scheme: dark)').addEventListener('change', (e) => {
            try {
                if (!localStorage.getItem('ddap-theme')) {
                    setTheme(e.matches ? 'dark' : 'light');
                }
            } catch (err) {
                // localStorage might not be available
            }
        });
        
        // Listen to system contrast changes
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
        
        // Simple syntax highlighter
        function highlightCode(code, language) {
            // Escape HTML first
            code = code.replace(/&/g, '&amp;').replace(/</g, '&lt;').replace(/>/g, '&gt;');
            
            // Special handling for mermaid - don't apply syntax highlighting
            if (language && language.toLowerCase() === 'mermaid') {
                return code;
            }
            
            if (!language) return code;
            
            // Create a safe version to work with
            const parts = [];
            let current = code;
            
            // Extract strings first and replace with placeholders
            const stringMatches = [];
            current = current.replace(/("(?:[^"\\]|\\.)*"|'(?:[^'\\]|\\.)*')/g, (match, p1) => {
                const index = stringMatches.length;
                stringMatches.push('<span class="hl-string">' + p1 + '</span>');
                return '___STRING_' + index + '___';
            });
            
            // Extract and highlight comments
            const commentMatches = [];
            if (['csharp', 'javascript', 'typescript', 'java'].includes(language.toLowerCase())) {
                current = current.replace(/(\/\/.*$)/gm, (match, p1) => {
                    const index = commentMatches.length;
                    commentMatches.push('<span class="hl-comment">' + p1 + '</span>');
                    return '___COMMENT_' + index + '___';
                });
                current = current.replace(/(\/\*[\s\S]*?\*\/)/g, (match, p1) => {
                    const index = commentMatches.length;
                    commentMatches.push('<span class="hl-comment">' + p1 + '</span>');
                    return '___COMMENT_' + index + '___';
                });
            }
            
            if (language.toLowerCase() === 'python') {
                current = current.replace(/(#.*$)/gm, (match, p1) => {
                    const index = commentMatches.length;
                    commentMatches.push('<span class="hl-comment">' + p1 + '</span>');
                    return '___COMMENT_' + index + '___';
                });
            }
            
            // Keywords for different languages
            const keywords = {
                csharp: ['public', 'private', 'protected', 'internal', 'static', 'readonly', 'const', 'class', 'interface', 'struct', 'enum', 'namespace', 'using', 'var', 'void', 'string', 'int', 'bool', 'double', 'float', 'long', 'decimal', 'if', 'else', 'for', 'foreach', 'while', 'do', 'switch', 'case', 'break', 'continue', 'return', 'new', 'this', 'base', 'null', 'true', 'false', 'async', 'await', 'Task', 'IEnumerable', 'List', 'Dictionary', 'override', 'virtual', 'abstract', 'sealed', 'partial', 'get', 'set', 'value'],
                javascript: ['const', 'let', 'var', 'function', 'return', 'if', 'else', 'for', 'while', 'do', 'switch', 'case', 'break', 'continue', 'async', 'await', 'class', 'extends', 'import', 'export', 'from', 'default', 'new', 'this', 'null', 'undefined', 'true', 'false', 'typeof', 'instanceof'],
                typescript: ['const', 'let', 'var', 'function', 'return', 'if', 'else', 'for', 'while', 'do', 'switch', 'case', 'break', 'continue', 'async', 'await', 'class', 'extends', 'import', 'export', 'from', 'default', 'new', 'this', 'null', 'undefined', 'true', 'false', 'typeof', 'instanceof', 'interface', 'type', 'enum', 'public', 'private', 'protected', 'readonly'],
                python: ['def', 'class', 'if', 'elif', 'else', 'for', 'while', 'return', 'import', 'from', 'as', 'try', 'except', 'finally', 'with', 'lambda', 'None', 'True', 'False', 'and', 'or', 'not', 'in', 'is', 'async', 'await'],
                java: ['public', 'private', 'protected', 'static', 'final', 'class', 'interface', 'extends', 'implements', 'import', 'package', 'void', 'int', 'String', 'boolean', 'double', 'float', 'long', 'if', 'else', 'for', 'while', 'do', 'switch', 'case', 'break', 'continue', 'return', 'new', 'this', 'null', 'true', 'false'],
                sql: ['SELECT', 'FROM', 'WHERE', 'INSERT', 'UPDATE', 'DELETE', 'CREATE', 'ALTER', 'DROP', 'TABLE', 'INDEX', 'VIEW', 'JOIN', 'LEFT', 'RIGHT', 'INNER', 'OUTER', 'ON', 'AND', 'OR', 'NOT', 'NULL', 'PRIMARY', 'KEY', 'FOREIGN', 'REFERENCES'],
                bash: ['if', 'then', 'else', 'elif', 'fi', 'for', 'while', 'do', 'done', 'case', 'esac', 'function', 'return', 'exit', 'echo', 'cd', 'ls', 'grep', 'sed', 'awk', 'export'],
                sh: ['if', 'then', 'else', 'elif', 'fi', 'for', 'while', 'do', 'done', 'case', 'esac', 'function', 'return', 'exit', 'echo', 'cd', 'ls', 'grep', 'sed', 'awk', 'export']
            };
            
            const langKeywords = keywords[language.toLowerCase()] || [];
            
            // Highlight keywords
            langKeywords.forEach(keyword => {
                // Escape special regex characters in the keyword
                const escapedKeyword = keyword.replace(/[.*+?^${}()|[\]\\]/g, '\\$&');
                const regex = new RegExp('\\b(' + escapedKeyword + ')\\b', 'g');
                current = current.replace(regex, '<span class="hl-keyword">$1</span>');
            });
            
            // Highlight numbers
            current = current.replace(/\b(\d+\.?\d*)\b/g, '<span class="hl-number">$1</span>');
            
            // Restore comments
            commentMatches.forEach((comment, index) => {
                current = current.replace('___COMMENT_' + index + '___', comment);
            });
            
            // Restore strings
            stringMatches.forEach((str, index) => {
                current = current.replace('___STRING_' + index + '___', str);
            });
            
            return current;
        }
        
        // Simple markdown parser
        function parseMarkdown(md) {
            // Split into lines for better processing
            const lines = md.split('\n');
            let html = '';
            let inList = false;
            let inCodeBlock = false;
            let codeBlock = '';
            let codeLanguage = '';
            
            for (let i = 0; i < lines.length; i++) {
                let line = lines[i];
                
                // Handle code blocks
                if (line.trim().startsWith('```')) {
                    if (!inCodeBlock) {
                        inCodeBlock = true;
                        codeBlock = '';
                        // Extract language from ```language
                        codeLanguage = line.trim().substring(3).trim();
                    } else {
                        // Check if it's a mermaid diagram
                        if (codeLanguage && codeLanguage.toLowerCase() === 'mermaid') {
                            html += '<div class="mermaid">' + codeBlock.replace(/&/g, '&amp;').replace(/</g, '&lt;').replace(/>/g, '&gt;') + '</div>';
                        } else {
                            // Apply syntax highlighting based on language
                            const highlightedCode = highlightCode(codeBlock, codeLanguage);
                            html += '<pre><code class="language-' + (codeLanguage || 'plaintext') + '">' + highlightedCode + '</code></pre>';
                        }
                        inCodeBlock = false;
                        codeLanguage = '';
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
            .then(md => { 
                document.getElementById('content').innerHTML = parseMarkdown(md);
                // Render mermaid diagrams after content is loaded
                if (typeof mermaid !== 'undefined') {
                    mermaid.run({
                        querySelector: '.mermaid'
                    }).catch(err => console.error('Mermaid rendering error:', err));
                }
            })
            .catch(() => { document.getElementById('content').innerHTML = '<h1>Error</h1><p>Could not load document.</p>'; });
    </script>
