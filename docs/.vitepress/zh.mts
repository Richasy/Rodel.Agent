import { defineConfig, type DefaultTheme } from 'vitepress'

export const zh = defineConfig({
    lang: 'zh-Hans',
    title: '小幻助理',
    description: '支持主流 AI 服务的应用',
    themeConfig: {
        nav: [
          { text: '下载应用', link: 'https://www.microsoft.com/store/productId/9NB0NB3MLQTM' },
          { text: '服务', link: '/services' },
          { text: '聊天', link: '/chat-simple' },
          { text: '文生图', link: '/image-simple' },
          { text: 'TTS', link: '/tts-simple' },
          { text: '翻译', link: '/translate-simple' },
        ],
    
        sidebar: [
          {
            text: '概览',
            items: [
              { text: '关于项目', link: '/about' },
              { text: '安装应用', link: '/install' },
              { text: '支持的服务', link: '/services' },
              { text: '工作目录与数据同步', link: '/work-directory' },
              { text: '常见问题', link: './faq' },
              { text: 'V1 数据迁移', link: '/v1-migration' },
              { text: '隐私策略', link: '/privacy' }
            ]
          },
          {
            text: '聊天',
            items: [
              { text: '服务配置', link: '/chat-config' },
              { text: '简单对话', link: '/chat-simple' },
              { text: '助理与预设', link: '/agent-preset' },
              { text: '群组聊天', link: '/group-chat'},
              { text: '工具调用', link: '/chat-tools' },
              { text: '插件开发', link: '/tool-dev' }
            ]
          },
          {
            text: '文生图',
            items: [
              { text: '服务配置', link: '/image-config' },
              { text: '功能概述', link: '/image-simple' }
            ]
          },
          {
            text: '文本转语音',
            items: [
              { text: '服务配置', link: '/tts-config' },
              { text: '功能概述', link: '/tts-simple' }
            ]
          },
          {
            text: '机器翻译',
            items: [
              { text: '服务配置', link: '/translate-config' },
              { text: '功能概述', link: '/translate-simple' }
            ]
          }
        ],

        editLink: {
            pattern: 'https://github.com/Richasy/Rodel.Agent/edit/main/docs/:path',
            text: '在 GitHub 上编辑此页面'
          },
      
          footer: {
            message: '基于 GPLv3 许可发布',
            copyright: `版权所有 © 2024-${new Date().getFullYear()} 云之幻`
          },
      
          docFooter: {
            prev: '上一页',
            next: '下一页'
          },
      
          outline: {
            label: '页面导航'
          },
      
          lastUpdated: {
            text: '最后更新于',
            formatOptions: {
              dateStyle: 'short',
              timeStyle: 'medium'
            }
          },
          langMenuLabel: '多语言',
          returnToTopLabel: '回到顶部',
          sidebarMenuLabel: '菜单',
          darkModeSwitchLabel: '主题',
          lightModeSwitchTitle: '切换到浅色模式',
          darkModeSwitchTitle: '切换到深色模式'
      }
})