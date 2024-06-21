// Copyright (c) Rodel. All rights reserved.

using RodelAgent.UI.ViewModels;
using RodelAgent.UI.ViewModels.Items;
using RodelAgent.UI.ViewModels.Pages;
using RodelTranslate.Models.Client;
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
    public TranslateSessionItemControl()
    {
        InitializeComponent();
        Loaded += OnLoaded;
    }

    /// <inheritdoc/>
    protected override void OnViewModelChanged(DependencyPropertyChangedEventArgs e)
        => InitSourceAndTarget();

    private void OnLoaded(object sender, RoutedEventArgs e)
        => InitSourceAndTarget();

    private void InitSourceAndTarget()
    {
        if (SourceLanguageBlock == null || TargetLanguageBlock == null)
        {
            return;
        }

        var sourceLan = new TranslateLanguageItemViewModel(ViewModel.SourceLanguage);
        var targetLan = new TranslateLanguageItemViewModel(ViewModel.TargetLanguage);
        SourceLanguageBlock.Text = sourceLan.DisplayName;
        TargetLanguageBlock.Text = targetLan.DisplayName;
    }

    private void OnDeleteButtonClick(object sender, RoutedEventArgs e)
    {
        var pageVM = ServiceProvider.GetRequiredService<TranslateServicePageViewModel>();
        pageVM.DeleteHistoryItemCommand.Execute(ViewModel);
    }

    private void OnCopySourceTextItemClick(object sender, RoutedEventArgs e)
        => CopyTextInternal(ViewModel.InputText);

    private void OnCopyTranslatedTextItemClick(object sender, RoutedEventArgs e)
        => CopyTextInternal(ViewModel.OutputText);

    private void CopyTextInternal(string text)
    {
        var dataPackage = new DataPackage();
        dataPackage.SetText(text);
        Clipboard.SetContent(dataPackage);
        ServiceProvider.GetRequiredService<AppViewModel>().ShowTip(Models.Constants.StringNames.Copied, Models.Constants.InfoType.Success);
    }
}

/// <summary>
/// <see cref="TranslateSessionItemControl"/> 的基类.
/// </summary>
public abstract class TranslateSessionItemControlBase : ReactiveUserControl<TranslateSession>
{
}
