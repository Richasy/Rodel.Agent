---
# https://vitepress.dev/reference/default-theme-home-page
layout: home

hero:
  name: 小幻助理
  text: 由 AI 和 .NET 驱动的智能桌面应用
  tagline: AI 嘛，还是要容易玩才行
  actions:
    - theme: brand
      text: 安装应用
      link: /install
    - theme: alt
      text: 使用说明
      link: /about
    - theme: alt
      text: Github
      link: https://github.com/Richasy/Rodel.Agent
  image:
    src: /logo-big.png
    alt: 小幻助理

features:
  - icon: ⛓️
    title: 在线 & 本地
    details: 支持包括 Open AI 在内的主流 AI 服务，通过 Ollama 支持主流开源模型。
  - icon: 🤖
    title: 轻松构建助理
    details: 调用各种服务进行组合，轻松构建属于你的助理工作流。
  - icon: 🍻
    title: 一起玩
    details: 忘掉生产力，忘掉赚钱，在这个 AI 的时代好好享受生活，开心就好。
---
<style>
:root {
  --vp-home-hero-name-color: transparent;
  --vp-home-hero-name-background: -webkit-linear-gradient(-12deg, #2095F4 20%, #FE736E 60%, #FF4AA1);

  --vp-home-hero-image-background-image: linear-gradient(160deg, #2095F4 50%, #FF9EA9 60%, #FE736E 80%);
  --vp-home-hero-image-filter: blur(44px);
}

@media (min-width: 640px) {
  :root {
    --vp-home-hero-image-filter: blur(56px);
  }
}

@media (min-width: 960px) {
  :root {
    --vp-home-hero-image-filter: blur(68px);
  }
}
</style>