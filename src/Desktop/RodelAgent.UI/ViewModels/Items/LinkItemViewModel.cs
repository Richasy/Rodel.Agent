// Copyright (c) Richasy. All rights reserved.

using Windows.System;

namespace RodelAgent.UI.ViewModels.Items;

/// <summary>
/// 链接项视图模型.
/// </summary>
public sealed partial class LinkItemViewModel : ViewModelBase
{
    private readonly string _url;

    /// <summary>
    /// Initializes a new instance of the <see cref="LinkItemViewModel"/> class.
    /// </summary>
    public LinkItemViewModel(string name, string url)
    {
        Name = name;
        _url = url;
    }

    /// <summary>
    /// 显示名称.
    /// </summary>
    public string Name { get; set; }

    [RelayCommand]
    private async Task ActivateAsync()
        => await Launcher.LaunchUriAsync(new Uri(_url)).AsTask();
}
