<div align="center"><a name="readme-top"></a>
<img height="120" src="https://img.picui.cn/free/2025/02/24/67bc7e06c62b3.png">

<h1>Rodel Commit</h1>

Rodel Commit 是一款基于 [agent-kernel](https://github.com/Richasy/agent-kernel) 构建的 CLI 提交工具

<small>🩷感谢 [Lobe Commit](https://github.com/lobehub/lobe-cli-toolbox/packages/lobe-commit) 提供的灵感🩷</small>

[![NuGet Version](https://img.shields.io/nuget/v/Richasy.RodelCommit)](https://www.nuget.org/packages/Richasy.RodelCommit)
[![GitHub License](https://img.shields.io/github/license/Richasy/Rodel.Agent)](https://www.github.com/Richasy/Rodel.Agent)

</div>

## ✨ 特性

- [x] 🤯 支持使用 20 余种国内外主流 AI 服务根据 git diffs 自动生成提交信息
- [x] 🛠️ 流畅的提交信息编辑流程
- [x] 😜 支持添加 CommitType
- [x] 🗺️ 支持多语言提交信息

## 📦 安装

在安装了 .NET 的设备上，请运行下面的命令：

```powershell
dotnet tool install --global Richasy.RodelCommit
```

## 🤯 使用

第一次使用前需要先创建配置文件：

```shell
rodel-commit --config
```

这会打开默认的JSON编辑器，你可以在其中填写你的服务配置.

之后使用 `rodel-commit` 命令生成提交信息：

```shell
git add <files...>
rodel-commit
```