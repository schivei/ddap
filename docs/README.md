# DDAP Documentation Website

This directory contains the modern, accessible documentation website for DDAP.

## 🎨 Features

### Design
- **Modern UI**: Inspired by Vercel, Stripe, and Tailwind CSS aesthetics
- **Responsive**: Mobile-first design that works on all screen sizes
- **Fast Loading**: Minimal dependencies, optimized for performance
- **Clean Typography**: Professional font stack with proper hierarchy

### Accessibility (WCAG Compliance)
- ✅ **WCAG AA minimum** (AAA for high contrast mode)
- ✅ **3 Theme System**: Light, Dark, and High Contrast modes
- ✅ **Semantic HTML5**: Proper document structure and landmarks
- ✅ **ARIA Labels**: Screen reader friendly
- ✅ **Keyboard Navigation**: Full keyboard accessibility
- ✅ **Skip Links**: Quick navigation for keyboard users
- ✅ **Focus Indicators**: Clear focus states on all interactive elements
- ✅ **Contrast Ratios**: 4.5:1 for text (AA), 7:1 for high contrast (AAA)
- ✅ **Reduced Motion**: Respects user preferences
- ✅ **Text Resizing**: Supports browser text zoom

## 📁 Files

### `index.html`
The main landing page featuring:
- Hero section with DDAP branding
- "Developer in Control" philosophy
- Feature grid (6 key features)
- Quick start guide with code examples
- Documentation links
- Package overview
- Build information section

### `styles.css`
Complete styling system with:
- 63 CSS custom properties
- 3 theme definitions (light/dark/high-contrast)
- Responsive breakpoints
- Code syntax highlighting
- Print styles
- Accessibility features

### `theme-toggle.js`
3-way theme switcher with:
- Light → Dark → High Contrast → Light cycle
- localStorage persistence
- System preference detection (prefers-color-scheme, prefers-contrast)
- Keyboard accessibility (Space/Enter)
- Screen reader announcements
- Smooth transitions

### `test-themes.html`
Testing page for theme system validation

## 🎨 Theme System

### Available Themes

1. **Light Theme** (Default)
   - Clean, bright interface
   - WCAG AA compliant
   - Best for daytime viewing

2. **Dark Theme**
   - Reduced eye strain in low light
   - WCAG AA compliant
   - Popular for developers

3. **High Contrast Theme**
   - Maximum readability
   - WCAG AAA compliant (7:1 contrast)
   - Accessibility-first design

### Theme Toggle

Click the theme button in the header to cycle through themes:
- 🌙 = Switch to Dark
- ⚡ = Switch to High Contrast  
- ☀️ = Switch to Light

Keyboard users can press Space or Enter to toggle.

### Persistence

The selected theme is saved to localStorage and restored on subsequent visits. The system also respects user preferences:
- `prefers-color-scheme: dark` → Dark theme
- `prefers-contrast: more` → High Contrast theme

## 🔧 Usage

### Local Development

1. Open `index.html` in a web browser
2. Or use a local server:
   ```bash
   # Python 3
   python -m http.server 8000
   
   # Node.js
   npx http-server
   ```
3. Navigate to `http://localhost:8000`

### Testing Themes

Open `test-themes.html` to test the theme system:
- Click the theme toggle button
- Check browser console for theme changes
- Verify all color variables render correctly

### Build Placeholders

The build info section uses placeholders that should be replaced during CI/CD:
- `{{RUN_ID}}` - GitHub Actions run ID
- `{{RUN_NUMBER}}` - Build number
- `{{VERSION}}` - Package version
- `{{BUILD_DATE}}` - Build timestamp
- `{{COMMIT_SHA}}` - Full commit SHA
- `{{COMMIT_SHA_SHORT}}` - Short commit SHA (7 chars)

Example sed replacement in CI/CD:
```bash
sed -i "s/{{RUN_ID}}/$GITHUB_RUN_ID/g" docs/index.html
sed -i "s/{{RUN_NUMBER}}/$GITHUB_RUN_NUMBER/g" docs/index.html
sed -i "s/{{VERSION}}/$VERSION/g" docs/index.html
sed -i "s/{{BUILD_DATE}}/$(date -u +'%Y-%m-%d %H:%M:%S UTC')/g" docs/index.html
sed -i "s/{{COMMIT_SHA}}/$GITHUB_SHA/g" docs/index.html
sed -i "s/{{COMMIT_SHA_SHORT}}/${GITHUB_SHA:0:7}/g" docs/index.html
```

## 🎯 Accessibility Testing

### Automated Tools
- [axe DevTools](https://www.deque.com/axe/devtools/)
- [WAVE](https://wave.webaim.org/)
- [Lighthouse](https://developers.google.com/web/tools/lighthouse)

### Manual Testing
- [ ] Keyboard navigation (Tab, Shift+Tab, Enter, Space)
- [ ] Screen reader (NVDA, JAWS, VoiceOver)
- [ ] Color contrast (all 3 themes)
- [ ] Text resizing (up to 200%)
- [ ] Focus indicators visible
- [ ] Skip link works

### Checklist
- [x] DOCTYPE declaration
- [x] lang attribute on html
- [x] Semantic HTML (header, nav, main, section, footer)
- [x] Heading hierarchy (h1 → h2 → h3)
- [x] Alt text on images
- [x] ARIA labels and landmarks
- [x] Skip to content link
- [x] Focus management
- [x] Keyboard operability
- [x] Color contrast (WCAG AA/AAA)
- [x] Reduced motion support

## 📊 Stats

- **HTML**: 20KB
- **CSS**: 20KB  
- **JavaScript**: 7KB
- **Total**: ~47KB (uncompressed)
- **Dependencies**: 0 external libraries
- **ARIA Labels**: 10
- **Semantic Elements**: 48
- **CSS Variables**: 63
- **Themes**: 3

## 🚀 Performance

- No external dependencies
- Minimal JavaScript (theme toggle only)
- CSS-only animations
- Optimized for fast loading
- Print-friendly styles

## 🤝 Contributing

When updating the documentation website:

1. Maintain WCAG AA compliance minimum
2. Test all 3 themes
3. Verify keyboard navigation
4. Check mobile responsiveness
5. Update this README if adding features

## 📝 License

MIT License - see [../LICENSE](../LICENSE)
