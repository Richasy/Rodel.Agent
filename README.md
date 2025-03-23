<a name="readme-top"></a>

<div align="center">

<img height="100" src="./assets/logo.png">

<h1 align="center">Rodel Agent</h1>

A Windows desktop application that integrates chat, text-to-speech, image generation from text, and machine translation. It supports mainstream AI services, MCP server plugins, and is fully AOT compiled, providing an excellent desktop AI experience.

<a href="https://apps.microsoft.com/detail/9NB0NB3MLQTM?launch=true&mode=full" target="_blank">Install from Microsoft Store</a>

English · [简体中文](./README.zh-CN.md)

<!-- SHIELD GROUP -->

[![][github-release-shield]][github-release-link]
[![][github-releasedate-shield]][github-releasedate-link]
[![][github-contributors-shield]][github-contributors-link]
[![][github-forks-shield]][github-forks-link]
[![][github-stars-shield]][github-stars-link]
[![][github-issues-shield]][github-issues-link]
[![][github-license-shield]][github-license-link]

</div>

## ✨ Feature Overview

<img src="./docs/assets/en/chat-overview.png" style="border-radius:4px" alt="对话界面">

Please refer to the document website for detailed introduction: [Rodel Agent](https://agent.richasy.net)

## ⚒️ Project Development

### Prerequisites

|Development Tools|Visual Studio 2022|
|-|-|
|Basic Dependencies|.NET 9, Windows App SDK 1.7|

### Building

#### UI

The desktop UI project is located in the `src/Desktop/RodelAgent.UI` directory. You can set `RodelAgent.UI` as the startup project and switch the environment to **x64** for building.

#### Console

Rodel Agent is attempting to build some derivative command line tools, currently including:

- [Rodel.Commit](./src/CLI/RodelCommit/)

## 🔗 Thanks

- [Microsoft.Extensions.AI](https://learn.microsoft.com/dotnet/ai/ai-extensions)
- [Lobe Chat](https://github.com/lobehub/lobe-chat)
- [Lobe Icons](https://github.com/lobehub/lobe-icons)
- [Spectre.Console](https://spectreconsole.net)
- [Windows App SDK](https://github.com/microsoft/WindowsAppSDK)
- [CommunityToolkit](https://github.com/CommunityToolkit)
- [antd-x](https://github.com/ant-design/x)
- ...

List the specific referenced components in the application's settings page

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