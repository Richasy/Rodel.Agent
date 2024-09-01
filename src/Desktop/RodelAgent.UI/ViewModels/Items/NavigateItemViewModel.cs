// Copyright (c) Rodel. All rights reserved.

using RodelAgent.UI.Models.Constants;
using RodelAgent.UI.Toolkits;

namespace RodelAgent.UI.ViewModels.Items;

/// <summary>
/// 导航项视图模型.
/// </summary>
public sealed partial class NavigateItemViewModel : ViewModelBase
{
    [ObservableProperty]
    private bool _isSelected;

    [ObservableProperty]
    private string _name;

    [ObservableProperty]
    private string _tip;

    [ObservableProperty]
    private FluentIcons.Common.Symbol _symbol;

    /// <summary>
    /// Initializes a new instance of the <see cref="NavigateItemViewModel"/> class.
    /// </summary>
    public NavigateItemViewModel(FeatureType type)
    {
        FeatureType = type;
        Name = type switch
        {
            FeatureType.Chat => ResourceToolkit.GetLocalizedString(StringNames.ChatService_Slim),
            FeatureType.RAG => ResourceToolkit.GetLocalizedString(StringNames.RAG_Slim),
            FeatureType.Draw => ResourceToolkit.GetLocalizedString(StringNames.ImageGenerate_Slim),
            FeatureType.Audio => ResourceToolkit.GetLocalizedString(StringNames.VoiceGenerate_Slim),
            FeatureType.Translate => ResourceToolkit.GetLocalizedString(StringNames.TranslateService_Slim),
            FeatureType.Settings => ResourceToolkit.GetLocalizedString(StringNames.Settings),
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
        };

        Tip = type switch
        {
            FeatureType.Chat => ResourceToolkit.GetLocalizedString(StringNames.ChatService_Full),
            FeatureType.RAG => ResourceToolkit.GetLocalizedString(StringNames.RAG_Full),
            FeatureType.Draw => ResourceToolkit.GetLocalizedString(StringNames.ImageGenerate_Full),
            FeatureType.Audio => ResourceToolkit.GetLocalizedString(StringNames.VoiceGenerate_Full),
            FeatureType.Translate => ResourceToolkit.GetLocalizedString(StringNames.TranslateService_Full),
            FeatureType.Settings => ResourceToolkit.GetLocalizedString(StringNames.Settings),
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
        };

        Symbol = type switch
        {
            FeatureType.Chat => FluentIcons.Common.Symbol.Chat,
            FeatureType.RAG => FluentIcons.Common.Symbol.LineHorizontal4Search,
            FeatureType.Draw => FluentIcons.Common.Symbol.Image,
            FeatureType.Audio => FluentIcons.Common.Symbol.Mic,
            FeatureType.Translate => FluentIcons.Common.Symbol.Translate,
            FeatureType.Settings => FluentIcons.Common.Symbol.Settings,
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
        };
    }

    /// <summary>
    /// 功能类型.
    /// </summary>
    public FeatureType FeatureType { get; }

    /// <inheritdoc/>
    public override bool Equals(object? obj) => obj is NavigateItemViewModel model && FeatureType == model.FeatureType;

    /// <inheritdoc/>
    public override int GetHashCode() => HashCode.Combine(FeatureType);
}
