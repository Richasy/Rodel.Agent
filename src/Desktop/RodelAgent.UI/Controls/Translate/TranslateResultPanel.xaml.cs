// Copyright (c) Richasy. All rights reserved.

namespace RodelAgent.UI.Controls.Translate;

/// <summary>
/// 翻译结果面板.
/// </summary>
public sealed partial class TranslateResultPanel : TranslatePageControlBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TranslateResultPanel"/> class.
    /// </summary>
    public TranslateResultPanel() => InitializeComponent();

    private void OnTargetLanguageChanged(object sender, SelectionChangedEventArgs e)
    {
        var language = (sender as ComboBox)?.SelectedItem as ViewModels.Items.TranslateLanguageItemViewModel;
        if (language == null || ViewModel.SelectedTargetLanguage == language)
        {
            return;
        }

        ViewModel.SelectedTargetLanguage = language;
    }
}
