// Copyright (c) Richasy. All rights reserved.
// Modified 2026-10-21 by Codex: complete the existing PR correction.

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
            var provider = SelectedService!.ProviderType;
            var prompt = Prompt;
            var model = SelectedModel?.Data.Id;
            var size = SelectedSize?.Data;
            var options = new DrawOptions
            {
                ModelId = SelectedModel?.Id,
                Width = size?.Width,
                Height = size?.Height,
            };
            var result = await _drawService!.Client!.DrawAsync(prompt, options, _drawCts.Token);
            var record = new DrawRecord
            {
                Id = Guid.NewGuid().ToString("N"),
                Prompt = prompt,
                Size = size,
                Model = model,
                Time = DateTimeOffset.Now,
                Provider = provider,
            };

            await this.Get<IStorageService>().AddOrUpdateDrawSessionAsync(record, result.ToArray());
            ReloadHistoryCommand.Execute(default);
            ShowRecord(record);
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

    [RelayCommand]
    private void ShowRecord(DrawRecord record)
    {
        Prompt = record.Prompt;
        Image = new Uri($"file://{AppToolkit.GetDrawPicturePath(record.Id)}");
        PresenterProvider = record.Provider;
        PresenterTime = record.Time?.ToString("yyyy-MM-dd HH:mm:ss") ?? string.Empty;
        PresenterProportion = (double)(record.Size?.Width ?? 1) / record.Size?.Height ?? 1;
    }
}
