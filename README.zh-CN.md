<a name="readme-top"></a>

<div align="center">

<img height="100" src="./assets/logo.png">

<h1 align="center">小幻助理</h1>

集聊天、文生图、文本转语音、机器翻译于一身的 Windows 桌面应用，支持目前主流的 AI 服务，提供优秀的桌面 AI 体验。

<a title="从Microsoft获取" href="https://www.microsoft.com/store/apps/9NB0NB3MLQTM?launch=true&amp;mode=full" target="_blank"><picture><source srcset="https://get.microsoft.com/images/zh-CN%20light.svg" media="(prefers-color-scheme: dark)"><source srcset="https://get.microsoft.com/images/zh-CN%20dark.svg" media="(prefers-color-scheme: light), (prefers-color-scheme: no-preference)"><img src="https://get.microsoft.com/images/zh-CN%20dark.svg" width="144"></picture></a>

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

![对话界面](./assets/zh/chat-overview.png)

具体介绍请参考文档网站：[小幻助理](https://agent.richasy.net)

## ⚒️ 项目开发

### 前置需求

|开发工具|Visual Studio 2022|
|-|-|
|基础依赖|.NET 8, Windows App SDK 1.6|
|子仓库|- [Richasy/semantic-kernel](https://github.com/Richasy/semantic-kernel/tree/dev) <br/>- [Richasy/WinUI.Share](https://github.com/Richasy/WinUI.Share)|

### 克隆仓库

运行下面的命令，在克隆主仓库的同时也拉取子仓库。

```shell
git clone --recurse-submodules https://github.com/Richasy/Rodel.Agent.git
```

如果你已经克隆了仓库，但忘记了初始化和更新子模块，可以在仓库目录中运行以下命令来完成这一步：

```shell
git submodule update --init --recursive
```

***拉取完成后，请检查子模块 `Richasy/semantic-kernel` 的分支，确保是 `dev` 分支***

### 构建

#### UI

桌面 UI 项目位于 `src/Desktop/RodelAgent.UI` 目录下。你可以将 `RodelAgent.UI` 设置为启动项目，将环境切换到 x64 进行构建。

#### 控制台

如果你正在开发特定的功能模块，可以在 `src/Console` 目录下找到主要的功能模块的控制台程序，你可以进行快速的功能验证。

对于不同的功能控制台程序，你需要手动在项目目录下创建 `config.json`，在里面填入你的服务配置。

比如对于聊天程序，你需要创建一个匹配 [ChatClientConfigurtation](./src/Core/RodelChat.Models/Client/ChatClientConfiguration.cs) 的 `config.json` 文件，并放置在 `RodelChat.Console` 目录中。

具体的服务配置是可选的，你可以只填写自己想要测试的服务。比如：

```json
{
    "openai": {
        "key": "sk-xxxxxxxxxxxxxxxxxxxxxxx",
    }
}
```

控制台会自动加载本地的 `config.json`，但不会通过 git 上传，以保证密钥不会泄露。

## 🔗 链接

- [小幻助理文档](https://agent.richasy.net)
- [Lobe Chat](https://github.com/lobehub/lobe-chat)
- [Lobe Icons](https://github.com/lobehub/lobe-icons)
- [Spectre.Console](https://spectreconsole.net)
- [Windows App SDK](https://github.com/microsoft/WindowsAppSDK)
- [CommunityToolkit](https://github.com/CommunityToolkit)
- [Semantic Kernel](https://github.com/microsoft/semantic-kernel)

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