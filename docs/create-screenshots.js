// Script to create screenshots of all documentation pages in all languages
const pages = [
  'index.html',
  'get-started.html',
  'philosophy.html',
  'database-providers.html',
  'api-providers.html',
  'architecture.html',
  'auto-reload.html',
  'templates.html',
  'advanced.html',
  'troubleshooting.html',
  'extended-types.html',
  'raw-queries.html',
  'client-getting-started.html',
  'client-rest.html',
  'client-graphql.html',
  'client-grpc.html'
];

const languages = [
  { code: 'en', name: 'English' },
  { code: 'pt-br', name: 'Portuguese' },
  { code: 'es', name: 'Spanish' },
  { code: 'fr', name: 'French' },
  { code: 'de', name: 'German' },
  { code: 'ja', name: 'Japanese' },
  { code: 'zh', name: 'Chinese' }
];

console.log('Total screenshots to create:', pages.length * languages.length);
console.log('');

let count = 1;
for (const page of pages) {
  for (const lang of languages) {
    const pageName = page.replace('.html', '');
    const filename = `${String(count).padStart(3, '0')}-${pageName}-${lang.code}.png`;
    console.log(`${count}. ${page} - ${lang.name} -> screenshots/${filename}`);
    count++;
  }
}
