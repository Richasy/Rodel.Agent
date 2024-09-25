<a name="readme-top"></a>

<div align="center">

<img height="100" src="./assets/logo.png">

<h1 align="center">Rodel Agent</h1>

A Windows desktop application that integrates chat, text-to-image, text-to-speech, and machine translation, supports the current mainstream AI services, and offers an excellent desktop AI experience.

<a title="Get from Microsoft" href="https://www.microsoft.com/store/apps/9NB0NB3MLQTM?launch=true&amp;mode=full" target="_blank"><picture><source srcset="https://get.microsoft.com/images/en-US%20light.svg" media="(prefers-color-scheme: dark)"><source srcset="https://get.microsoft.com/images/en-US%20dark.svg" media="(prefers-color-scheme: light), (prefers-color-scheme: no-preference)"><img src="https://get.microsoft.com/images/en-US%20dark.svg" width="144"></picture></a>

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

![chat interface](./assets/en/chat-overview.png)

Please refer to the document website for detailed introduction: [Rodel Agent](https://agent.richasy.net)

## ⚒️ Project Development

### Prerequisites

|Development Tools|Visual Studio 2022|
|-|-|
|Basic Dependencies|.NET 8, Windows App SDK 1.6|
|Submodules|- [Richasy/semantic-kernel](https://github.com/Richasy/semantic-kernel/tree/dev) <br/> - [Richasy/WinUI.Share](https://github.com/Richasy/WinUI.Share)|

### Cloning the Repository

Run the following command to clone the main repository and pull the submodules simultaneously.

```shell
git clone --recurse-submodules https://github.com/Richasy/Rodel.Agent.git
```

If you have already cloned the repository but forgot to initialize and update the submodules, you can run the following command in the repository directory to complete this step:

```shell
git submodule update --init --recursive
```

***After pulling, please check the branch of the submodule `Richasy/semantic-kernel` to ensure it is the `dev` branch.***

### Building

#### UI

The desktop UI project is located in the `src/Desktop/RodelAgent.UI` directory. You can set `RodelAgent.UI` as the startup project and switch the environment to x64 for building.

#### Console

If you are developing specific functional modules, you can find the main console programs for these modules in the `src/Console` directory, where you can quickly verify functionalities.

For different functional console programs, you need to manually create a `config.json` file in the project directory and fill in your service configuration.

For example, for the chat program, you need to create a `config.json` file matching the [ChatClientConfiguration](./src/Core/RodelChat.Models/Client/ChatClientConfiguration.cs) and place it in the `RodelChat.Console` directory.

The specific service configuration is optional, and you can fill in only the services you want to test. For example:

```json
{
    "openai": {
        "key": "sk-xxxxxxxxxxxxxxxxxxxxxxx"
    }
}
```

The console will automatically load the local `config.json` but will not upload it via git to ensure that the key is not leaked.

## 🔗 Links

- [Rodel Agent Documentation](https://agent.richasy.net)
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