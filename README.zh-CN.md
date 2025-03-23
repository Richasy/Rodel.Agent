<a name="readme-top"></a>

<div align="center">

<img height="100" src="./assets/logo.png">

<h1 align="center">小幻助理</h1>

集对话、文生图、文本转语音、机器翻译于一身的 Windows 桌面应用，支持目前主流的 AI 服务，支持 MCP 服务插件，完全的 AOT 编译，提供优秀的桌面 AI 体验。

<a href="https://apps.microsoft.com/detail/9NB0NB3MLQTM?launch=true&mode=full" target="_blank">从 Microsoft 获取</a>

[English](./README.md) · 简体中文

<!-- SHIELD GROUP -->

[![][github-release-shield]][github-release-link]
[![][github-releasedate-shield]][github-releasedate-link]
[![][github-contributors-shield]][github-contributors-link]
[![][github-forks-shield]][github-forks-link]
[![][github-stars-shield]][github-stars-link]
[![][github-issues-shield]][github-issues-link]
[![][github-license-shield]][github-license-link]

</div>

## ✨ 功能概览

<img src="./docs/assets/zh/chat-overview.png" style="border-radius:4px" alt="对话界面">

具体介绍请参考文档网站：[小幻助理](https://agent.richasy.net)

## ⚒️ 项目开发

### 前置需求

|开发工具|Visual Studio 2022|
|-|-|
|基础依赖|.NET 9, Windows App SDK 1.7|

### 构建

#### UI

桌面 UI 项目位于 `src/Desktop/RodelAgent.UI` 目录下。你可以将 `RodelAgent.UI` 设置为启动项目，将环境切换到 x64 进行构建。

#### 控制台

小幻助理正在尝试构建一些衍生的命令行工具，目前有：

- [Rodel.Commit](./src/CLI/RodelCommit/)

## 🔗 鸣谢

- [Microsoft.Extensions.AI](https://learn.microsoft.com/dotnet/ai/ai-extensions)
- [Lobe Chat](https://github.com/lobehub/lobe-chat)
- [Lobe Icons](https://github.com/lobehub/lobe-icons)
- [Spectre.Console](https://spectreconsole.net)
- [Windows App SDK](https://github.com/microsoft/WindowsAppSDK)
- [CommunityToolkit](https://github.com/CommunityToolkit)
- [antd-x](https://github.com/ant-design/x)
- ...

具体引用的组件在应用的设置页面中列举

<!-- LINK GROUP -->
[github-contributors-link]: https://github.com/Richasy/Rodel.Agent/graphs/contributors
[github-contributors-shield]: https://img.shields.io/github/contributors/Richasy/Rodel.Agent?color=c4f042&labelColor=black&style=flat-square
[github-forks-link]: https://github.com/Richasy/Rodel.Agent/network/members
[github-forks-shield]: https://img.shields.io/github/forks/Richasy/Rodel.Agent?color=8ae8ff&labelColor=black&style=flat-square
[github-issues-link]: https://github.com/Richasy/Rodel.Agent/issues
[github-issues-shield]: https://img.shields.io/github/issues/Richasy/Rodel.Agent?color=ff80eb&labelColor=black&style=flat-square
[github-license-link]: https://github.com/Richasy/Rodel.Agent/blob/main/LICENSE
[github-license-shield]: https://img.shields.io/github/license/Richasy/Rodel.Agent?color=white&labelColor=black&style=flat-square
[github-release-link]: https://github.com/Richasy/Rodel.Agent/releases
[github-release-shield]: https://img.shields.io/github/v/release/Richasy/Rodel.Agent?color=369eff&labelColor=black&logo=github&style=flat-square
[github-releasedate-link]: https://github.com/Richasy/Rodel.Agent/releases
[github-releasedate-shield]: https://img.shields.io/github/release-date/Richasy/Rodel.Agent?labelColor=black&style=flat-square
[github-stars-link]: https://github.com/Richasy/Rodel.Agent/network/stargazers
[github-stars-shield]: https://img.shields.io/github/stars/Richasy/Rodel.Agent?color=ffcb47&labelColor=black&style=flat-square