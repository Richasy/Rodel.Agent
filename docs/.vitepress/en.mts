import { defineConfig, type DefaultTheme } from 'vitepress'

export const en = defineConfig({
    lang: 'en',
    title: 'Rodel Agent',
    description: 'Apps that support popular AI services',
    themeConfig: {
      nav: [
        { text: 'Download', link: 'https://www.microsoft.com/store/productId/9NB0NB3MLQTM' },
        { text: 'Services', link: '/en/services' },
        { text: 'Chat', link: '/en/chat-simple' },
        { text: 'Image Generation', link: '/en/image-simple' },
        { text: 'TTS', link: '/en/tts-simple' },
        { text: 'Translation', link: '/en/translate-simple' },
      ],
  
      sidebar: [
        {
          text: 'Overview',
          items: [
            { text: 'About', link: '/en/about' },
            { text: 'Installation', link: '/en/install' },
            { text: 'Supported services', link: '/en/services' },
            { text: 'Work directory & Data sync', link: '/en/work-directory' },
            { text: 'V1 data migration', link: '/en/v1-migration' },
            { text: 'Privacy Policy', link: '/en/privacy' }
          ]
        },
        {
          text: 'Chat',
          items: [
            { text: 'Service config', link: '/en/chat-config' },
            { text: 'Quick chat', link: '/en/chat-simple' },
            { text: 'Agents & Presets', link: '/en/agent-preset' },
            { text: 'Group chat', link: '/en/group-chat'},
            { text: 'Tool invocation', link: '/en/chat-tools' },
            { text: 'Tool development', link: '/en/tool-dev' }
          ]
        },
        {
          text: 'Image generation',
          items: [
            { text: 'Service config', link: '/en/image-config' },
            { text: 'Feature overview', link: '/en/image-simple' }
          ]
        },
        {
          text: 'Text to speech',
          items: [
            { text: 'Service config', link: '/en/tts-config' },
            { text: 'Feature overview', link: '/en/tts-simple' }
          ]
        },
        {
          text: 'Machine translation',
          items: [
            { text: 'Service config', link: '/en/translate-config' },
            { text: 'Feature overview', link: '/en/translate-simple' }
          ]
        }
      ],
        editLink: {
            pattern: 'https://github.com/Richasy/Rodel.Agent/edit/master/docs/:path',
            text: 'Edit this page on GitHub'
          },
      
        footer: {
            message: 'Released under the GPLv3 License',
            copyright: `Copyright © ${new Date().getFullYear()} Richasy`
          },
      }
})