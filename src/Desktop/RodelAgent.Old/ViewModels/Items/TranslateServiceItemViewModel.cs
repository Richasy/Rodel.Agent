// Copyright (c) Richasy. All rights reserved.

using RodelTranslate.Models.Client;
using RodelTranslate.Models.Constants;

namespace RodelAgent.UI.ViewModels.Items;

/// <summary>
/// 翻译服务项视图模型.
/// </summary>
public sealed partial class TranslateServiceItemViewModel : ViewModelBase
{
    [ObservableProperty]
    private string _name;

    [ObservableProperty]
    private ProviderType _providerType;

    [ObservableProperty]
    private bool _isCompleted;

    [ObservableProperty]
    private ClientConfigBase _config;

    [ObservableProperty]
    private bool _isSelected;

    /// <summary>
    /// Initializes a new instance of the <see cref="TranslateServiceItemViewModel"/> class.
    /// </summary>
    public TranslateServiceItemViewModel(
        ProviderType type,
        string name)
    {
        ProviderType = type;
        Name = name;
    }

    /// <summary>
    /// 设置配置.
    /// </summary>
    /// <param name="config">配置.</param>
    public void SetConfig(ClientConfigBase config)
    {
        Config = config;
        CheckCurrentConfig();
    }

    /// <summary>
    /// 检查当前配置是否有效.
    /// </summary>
    public void CheckCurrentConfig()
        => IsCompleted = Config?.IsValid() ?? false;

    /// <inheritdoc/>
    public override bool Equals(object? obj) => obj is TranslateServiceItemViewModel model && ProviderType == model.ProviderType;

    /// <inheritdoc/>
    public override int GetHashCode() => HashCode.Combine(ProviderType);
}
