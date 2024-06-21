// Copyright (c) Rodel. All rights reserved.

using RodelAgent.UI.ViewModels.Items;

namespace RodelAgent.UI.Controls.Translation;

/// <summary>
/// 目标文本区域.
/// </summary>
public sealed partial class TargetTextSection : TranslateSessionControlBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TargetTextSection"/> class.
    /// </summary>
    public TargetTextSection() => InitializeComponent();

    private void OnTargetLanguageChanged(object sender, SelectionChangedEventArgs e)
    {
        var language = (sender as ComboBox)?.SelectedItem as TranslateLanguageItemViewModel;
        if (language == null || ViewModel.TargetLanguage == language)
        {
            return;
        }

        ViewModel.TargetLanguage = language;
    }
}
