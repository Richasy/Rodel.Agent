// Copyright (c) Richasy. All rights reserved.

using Richasy.AgentKernel.Models;
using RodelAgent.UI.ViewModels.Core;

namespace RodelAgent.UI.ViewModels.View;

public sealed partial class TranslatePageViewModel
{
    [RelayCommand]
    private async Task StartTranslateAsync()
    {
        if (string.IsNullOrEmpty(SourceText) || IsExceedLimit)
        {
            return;
        }

        if (IsTranslating)
        {
            CancelTranslate();
        }

        IsTranslating = true;
        try
        {
            _cancellationTokenSource = new CancellationTokenSource();
            ResultText = string.Empty;
            var options = new TranslateOptions
            {
                SourceLanguage = SelectedSourceLanguage!.Code,
                TargetLanguage = SelectedTargetLanguage!.Code,
            };
            var result = await _translateService!.Client!.TranslateTextAsync(SourceText, options, _cancellationTokenSource.Token);
            ResultText = result.Result;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to translate text.");
            this.Get<AppViewModel>().ShowTipCommand.Execute((ex.Message, InfoType.Error));
        }
        finally
        {
            IsTranslating = false;
        }
    }

    [RelayCommand]
    private void CancelTranslate()
    {
        _cancellationTokenSource?.Cancel();
        _cancellationTokenSource = null;
        IsTranslating = false;
    }
}
