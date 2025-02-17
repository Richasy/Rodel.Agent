// Copyright (c) Richasy. All rights reserved.

using RodelAgent.UI.Toolkits;
using RodelAgent.UI.ViewModels;
using RodelAgent.UI.ViewModels.Items;
using RodelAgent.UI.ViewModels.Pages;
using Windows.ApplicationModel.DataTransfer;

namespace RodelAgent.UI.Controls.Translation;

/// <summary>
/// 翻译会话项控件.
/// </summary>
public sealed partial class TranslateSessionItemControl : TranslateSessionItemControlBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TranslateSessionItemControl"/> class.
    /// </summary>
    public TranslateSessionItemControl() => InitializeComponent();

    /// <inheritdoc/>
    protected override void OnViewModelChanged(TranslateSessionItemViewModel? oldValue, TranslateSessionItemViewModel? newValue)
        => InitSourceAndTarget();

    /// <inheritdoc/>
    protected override void OnControlLoaded()
        => InitSourceAndTarget();

    private void InitSourceAndTarget()
    {
        if (SourceLanguageBlock == null || TargetLanguageBlock == null)
        {
            return;
        }

        var sourceLan = new TranslateLanguageItemViewModel(ViewModel.Data.SourceLanguage);
        var targetLan = new TranslateLanguageItemViewModel(ViewModel.Data.TargetLanguage);
        SourceLanguageBlock.Text = sourceLan.DisplayName;
        TargetLanguageBlock.Text = targetLan.DisplayName;
    }

    private void OnDeleteButtonClick(object sender, RoutedEventArgs e)
    {
        var pageVM = this.Get<TranslateServicePageViewModel>();
        pageVM.DeleteHistoryItemCommand.Execute(ViewModel);
    }

    private void OnCopySourceTextItemClick(object sender, RoutedEventArgs e)
        => CopyTextInternal(ViewModel.Data.InputText);

    private void OnCopyTranslatedTextItemClick(object sender, RoutedEventArgs e)
        => CopyTextInternal(ViewModel.Data.OutputText);

    private void CopyTextInternal(string text)
    {
        var dataPackage = new DataPackage();
        dataPackage.SetText(text);
        Clipboard.SetContent(dataPackage);
        this.Get<AppViewModel>().ShowTipCommand.Execute((ResourceToolkit.GetLocalizedString(Models.Constants.StringNames.Copied), InfoType.Success));
    }
}

/// <summary>
/// <see cref="TranslateSessionItemControl"/> 的基类.
/// </summary>
public abstract class TranslateSessionItemControlBase : LayoutUserControlBase<TranslateSessionItemViewModel>
{
}
