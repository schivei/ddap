# ✅ 库现已在本地托管

**更新：** CDN库现在托管在项目本地。不再需要在防火墙中放行域名！

本文档保留用于历史参考，记录以前需要的CDN域名。

## 所需域名

### 1. cdnjs.cloudflare.com
**用途：** GitHub Markdown CSS  
**资源：** `https://cdnjs.cloudflare.com/ajax/libs/github-markdown-css/5.5.1/github-markdown.min.css`  
**目的：** 文档页面markdown内容的样式

### 2. cdn.jsdelivr.net
**用途：** JavaScript库（marked.js和DOMPurify）  
**资源：**
- `https://cdn.jsdelivr.net/npm/marked/marked.min.js` - JavaScript Markdown解析器
- `https://cdn.jsdelivr.net/npm/dompurify@3.0.8/dist/purify.min.js` - HTML清理（XSS安全防护）  
**目的：** 在文档页面中将markdown文件（.md）渲染为HTML

### 3. img.shields.io（可选）
**用途：** index.html中的徽章  
**资源：**
- `https://img.shields.io/nuget/v/Ddap.Core` - NuGet版本徽章
- `https://img.shields.io/github/license/schivei/ddap` - 许可证徽章
- `https://img.shields.io/github/actions/workflow/status/schivei/ddap/build.yml` - 构建状态徽章  
**目的：** 在首页显示信息徽章（对功能不关键）

## ✅ 已实施解决方案：本地托管

库现在位于 `/docs/lib/` 中：
- **marked.min.js**（39KB）- Markdown解析器
- **purify.min.js**（21KB）- HTML清理器（XSS保护）
- **github-markdown.min.css**（25KB）- markdown样式

**总计：** 约85KB本地托管

### 优势：
- ✅ 离线工作
- ✅ 不需要防火墙放行
- ✅ 更快（无外部请求）
- ✅ 更好的安全控制

## CDN域名（不再需要）

### ~~关键域名（必需）~~：
```
cdnjs.cloudflare.com  ❌ 不再需要
cdn.jsdelivr.net      ❌ 不再需要
```

### 可选域名（仅用于index.html中的徽章）：
```
img.shields.io  ⚠️ 可选（仅视觉徽章）
```

## ~~受影响的文件~~ ✅ 已解决

**更新：** 所有页面现在使用本地库，运行完美！

现在使用本地库运行的文档页面：
- get-started.html
- philosophy.html
- database-providers.html
- api-providers.html
- architecture.html
- auto-reload.html
- advanced.html
- troubleshooting.html
- client-getting-started.html
- client-rest.html
- client-graphql.html
- client-grpc.html
- extended-types.html
- raw-queries.html

**注意：** index.html页面一直正常工作，因为它使用静态HTML。其他页面现在也可以使用本地库正常工作。

## ~~放行后测试~~ ✅ 不再需要

库已在本地托管。只需访问任何文档页面：
- http://localhost:8080/get-started.html
- http://localhost:8080/philosophy.html
- 等等

markdown内容将完美渲染！

## ~~替代方案（本地托管）~~ ✅ 已实施

**完成！** 库已在 `/docs/lib/` 中本地托管：
1. ✅ marked.min.js - Markdown解析器
2. ✅ purify.min.js - HTML清理器
3. ✅ github-markdown.min.css - 样式

所有HTML页面已更新为使用本地引用。
