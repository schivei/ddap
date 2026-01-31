# DDAP Icons

This directory contains the DDAP project icon in various formats and sizes.

## Icon Design Philosophy

The DDAP icon represents the core philosophy of "Developer in Control":
- **Control Knob/Dial**: Symbolizes the developer's ability to adjust and control every aspect
- **Gradient Colors**: Blue to purple gradient represents the modern, professional nature of the framework
- **Notches Around Dial**: Represent the different configuration options available
- **Indicator Line**: Shows the current setting, emphasizing choice and control
- **Data Flow Elements**: Subtle lines representing the API data flow (REST, GraphQL, gRPC)

## Available Formats

### Vector Format
- `icon.svg` - Scalable Vector Graphics, suitable for all sizes and uses

### Raster Formats (PNG)
The following PNG sizes have been generated from the SVG:
- `icon-32.png` - 32×32 pixels (browser tabs, taskbar)
- `icon-64.png` - 64×64 pixels (medium icons)
- `icon-128.png` - 128×128 pixels (app icons, high-DPI displays) **[NuGet Package Default]**
- `icon-256.png` - 256×256 pixels (large displays, package icons)
- `icon.png` - 128×128 pixels (default/alias for NuGet packages)

## Usage

### In README.md
```markdown
![DDAP Icon](icons/icon.svg)
```

### In Documentation
The icon can be used in:
- Website header/logo
- Documentation pages
- Package metadata
- GitHub repository social preview

### NuGet Package Icon
The icon is configured in `Directory.Build.props`:
```xml
<PackageIcon>icon.png</PackageIcon>
<ItemGroup>
  <None Include="$(MSBuildThisFileDirectory)icon.png" Pack="true" PackagePath="\" />
</ItemGroup>
```

This includes the 128×128 PNG version (`icon.png`) in all NuGet packages.

## Generating PNG Files

PNG files have been generated using Python with cairosvg. To regenerate them:

### Using Python (cairosvg)
```python
import cairosvg

sizes = [32, 64, 128, 256]
for size in sizes:
    cairosvg.svg2png(
        url='icon.svg',
        write_to=f'icon-{size}.png',
        output_width=size,
        output_height=size
    )
```

### Using Inkscape
```bash
inkscape icon.svg --export-filename=icon-16.png --export-width=16 --export-height=16
inkscape icon.svg --export-filename=icon-32.png --export-width=32 --export-height=32
inkscape icon.svg --export-filename=icon-64.png --export-width=64 --export-height=64
inkscape icon.svg --export-filename=icon-128.png --export-width=128 --export-height=128
inkscape icon.svg --export-filename=icon-256.png --export-width=256 --export-height=256
```

### Using ImageMagick
```bash
convert icon.svg -resize 16x16 icon-16.png
convert icon.svg -resize 32x32 icon-32.png
convert icon.svg -resize 64x64 icon-64.png
convert icon.svg -resize 128x128 icon-128.png
convert icon.svg -resize 256x256 icon-256.png
```

### Using rsvg-convert
```bash
rsvg-convert -w 16 -h 16 icon.svg -o icon-16.png
rsvg-convert -w 32 -h 32 icon.svg -o icon-32.png
rsvg-convert -w 64 -h 64 icon.svg -o icon-64.png
rsvg-convert -w 128 -h 128 icon.svg -o icon-128.png
rsvg-convert -w 256 -h 256 icon.svg -o icon-256.png
```

### Online Converters
- [CloudConvert](https://cloudconvert.com/svg-to-png) - SVG to PNG converter
- [Convertio](https://convertio.co/svg-png/) - Online SVG to PNG

## Color Palette

The icon uses the following colors:
- **Primary Blue**: `#2563eb` (Blue 600)
- **Primary Purple**: `#7c3aed` (Violet 600)
- **Light Blue**: `#60a5fa` (Blue 400)
- **Medium Blue**: `#3b82f6` (Blue 500)
- **Dark Background**: `#1e293b` (Slate 800)
- **White**: `#ffffff` (Highlights and accents)

## Icon Specifications

- **Format**: SVG 1.1
- **Canvas Size**: 256×256 pixels
- **Color Mode**: RGB
- **Transparency**: Supported
- **Scalability**: Infinite (vector)

## License

The DDAP icon is part of the DDAP project and is licensed under the MIT License.
