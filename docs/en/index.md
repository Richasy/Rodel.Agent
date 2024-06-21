---
# https://vitepress.dev/reference/default-theme-home-page
layout: home

hero:
  name: Rodel Agent
  text: Intelligent Desktop App Powered by AI and .NET
  tagline: AI, it should be easy to play.
  actions:
    - theme: brand
      text: Install
      link: /install
    - theme: alt
      text: Getting start
      link: /about
    - theme: alt
      text: Github
      link: https://github.com/Richasy/Rodel.Agent
  image:
    src: /logo-big.png
    alt: Rodel Agent

features:
  - icon: ⛓️
    title: Online & Local
    details: Supports mainstream AI services including Open AI, and supports mainstream open source models through ollama.
  - icon: 🤖
    title: Easily Build Assistants
    details: Combine various services to easily build your own assistant workflow.
  - icon: 🍻
    title: Play Together
    details: Forget about productivity, forget about making money, enjoy life in this AI era, and be happy.
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