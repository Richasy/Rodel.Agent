import { defineConfig } from 'vitepress'

export const shared = defineConfig({
    title: '小幻助理',
  
    lastUpdated: true,
    cleanUrls: true,
    metaChunk: true,

    markdown: {
        codeTransformers: [
          {
            postprocess(code) {
              return code.replace(/\[\!\!code/g, '[!code')
            }
          }
        ]
      },

    head: [
      ['link', { rel: 'icon', type: 'image/svg+xml', href: '/logo-small.svg' }],
      ['link', { rel: 'icon', type: 'image/png', href: '/logo-small.png' }],
      ['meta', { name: 'theme-color', content: '#2095F4' }],
      ['meta', { property: 'og:type', content: 'website' }],
      ['meta', { property: 'og:locale', content: 'zh' }],
      ['meta', { property: 'og:title', content: '小幻助理 | AI Powered App' }],
      ['meta', { property: 'og:site_name', content: '小幻助理' }],
      ['meta', { property: 'og:url', content: 'https://rodel.richasy.com/' }],
    ],

    themeConfig: {
        logo: { src: '/logo-small.svg', width: 24, height: 24 },
    
        socialLinks: [
          { icon: 'github', link: 'https://github.com/Richasy/Rodel.Agent' }
        ],
      }
})  