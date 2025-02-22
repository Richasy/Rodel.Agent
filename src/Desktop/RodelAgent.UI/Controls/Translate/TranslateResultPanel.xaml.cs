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
        if ((sender as ComboBox)?.SelectedItem is not ViewModels.Items.LanguageItemViewModel language
            || ViewModel.SelectedTargetLanguage == language)
        {
            return;
        }

        ViewModel.SelectedTargetLanguage = language;
        if (!string.IsNullOrEmpty(ViewModel.SourceText) && !string.IsNullOrEmpty(ViewModel.ResultText))
        {
            ViewModel.StartTranslateCommand.Execute(default);
        }
    }
}
