// Copyright (c) Richasy. All rights reserved.

using RodelAgent.UI.Toolkits;

namespace RodelAgent.UI.ViewModels.View;

/// <summary>
/// 设置页面视图模型.
/// </summary>
public sealed partial class SettingsPageViewModel
{
    private void InitializeLinks()
    {
        if (Links.Count > 0)
        {
            return;
        }

        Links.Add(new(ResourceToolkit.GetLocalizedString(Models.Constants.StringNames.BiliHomePage), "https://space.bilibili.com/5992670"));
        Links.Add(new(ResourceToolkit.GetLocalizedString(Models.Constants.StringNames.ProjectHomePage), "https://github.com/Richasy/Rodel.Agent"));
        Links.Add(new(ResourceToolkit.GetLocalizedString(Models.Constants.StringNames.ReadDocument), AppToolkit.GetDocumentLink(string.Empty)));
        Links.Add(new(ResourceToolkit.GetLocalizedString(Models.Constants.StringNames.OtherApp), "ms-windows-store://publisher/?name=云之幻"));
    }

    private void InitializeLibraries()
    {
        if (Libraries.Count > 0)
        {
            return;
        }

        Libraries.Add(new("Windows App SDK", "https://github.com/microsoft/WindowsAppSDK"));
        Libraries.Add(new(".NET Community Toolkit", "https://github.com/CommunityToolkit/dotnet"));
        Libraries.Add(new("MVVM Toolkit", "https://github.com/CommunityToolkit/dotnet"));
        Libraries.Add(new("Win2D", "https://github.com/microsoft/Win2D"));
        Libraries.Add(new("ComputeSharp", "https://github.com/Sergio0694/ComputeSharp"));
        Libraries.Add(new("Windows Community Toolkit", "https://github.com/CommunityToolkit/WindowsCommunityToolkit"));
        Libraries.Add(new("FluentUI System Icons", "https://github.com/microsoft/fluentui-system-icons"));
        Libraries.Add(new("CsWin32", "https://github.com/microsoft/cswin32"));
        Libraries.Add(new("Agent Kernel", "https://github.com/Richasy/agent-kernel"));
        Libraries.Add(new("WinUIEx", "https://github.com/dotMorten/WinUIEx"));
        Libraries.Add(new("WinUI Kernel", "https://github.com/Richasy/winui-kernel"));
        Libraries.Add(new("Html Agility Pack", "https://github.com/zzzprojects/html-agility-pack"));
        Libraries.Add(new("FluentIcons", "https://github.com/davidxuang/FluentIcons"));
        Libraries.Add(new("Humanizer", "https://github.com/Humanizr/Humanizer"));
        Libraries.Add(new("Markdig", "https://github.com/xoofx/markdig"));
        Libraries.Add(new("Serilog", "https://github.com/serilog/serilog"));
    }
}
