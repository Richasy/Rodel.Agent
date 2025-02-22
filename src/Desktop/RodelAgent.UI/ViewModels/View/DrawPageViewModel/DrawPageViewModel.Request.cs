// Copyright (c) Richasy. All rights reserved.

using Richasy.AgentKernel.Models;
using RodelAgent.Interfaces;
using RodelAgent.Models.Common;
using RodelAgent.UI.Toolkits;
using RodelAgent.UI.ViewModels.Core;

namespace RodelAgent.UI.ViewModels.View;

/// <summary>
/// 绘图页面视图模型.
/// </summary>
public sealed partial class DrawPageViewModel
{
    [RelayCommand]
    private async Task StartDrawAsync()
    {
        if (string.IsNullOrEmpty(Prompt))
        {
            return;
        }

        if (IsDrawing)
        {
            CancelDraw();
        }

        IsDrawing = true;
        try
        {
            _drawCts = new CancellationTokenSource();
            var options = new DrawOptions
            {
                ModelId = SelectedModel?.Id,
                Width = SelectedSize?.Data.Width,
                Height = SelectedSize?.Data.Height,
            };
            var result = await _drawService!.Client!.DrawAsync(Prompt, options, _drawCts.Token);
            var record = new DrawRecord
            {
                Id = Guid.NewGuid().ToString("N"),
                Prompt = Prompt,
                Size = SelectedSize?.Data,
                Model = SelectedModel?.Data.Id,
                Time = DateTimeOffset.Now,
                Provider = SelectedService.ProviderType,
            };

            await this.Get<IStorageService>().AddOrUpdateDrawSessionAsync(record, result.ToArray());
            ReloadHistoryCommand.Execute(default);
            Image = new Uri($"file://{AppToolkit.GetDrawPicturePath(record.Id)}");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to draw picture.");
            this.Get<AppViewModel>().ShowTipCommand.Execute((ex.Message, InfoType.Error));
        }
        finally
        {
            IsDrawing = false;
        }
    }

    [RelayCommand]
    private void CancelDraw()
    {
        _drawCts?.Cancel();
        _drawCts = null;
        IsDrawing = false;
    }
}
