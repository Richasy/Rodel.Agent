// Copyright (c) Rodel. All rights reserved.

using RodelTranslate.Models.Client;

namespace RodelAgent.UI.ViewModels.Items;

/// <summary>
/// 翻译会话项视图模型.
/// </summary>
public sealed partial class TranslateSessionItemViewModel : ViewModelBase<TranslateSession>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TranslateSessionItemViewModel"/> class.
    /// </summary>
    public TranslateSessionItemViewModel(TranslateSession data)
        : base(data)
    {
    }
}
