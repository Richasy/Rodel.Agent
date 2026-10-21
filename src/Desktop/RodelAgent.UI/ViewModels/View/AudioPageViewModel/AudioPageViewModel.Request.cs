// Copyright (c) Richasy. All rights reserved.
// Modified 2026-10-21 by Codex: complete the existing PR correction.

using Richasy.AgentKernel.Models;
using RodelAgent.Interfaces;
using RodelAgent.Models.Common;
using RodelAgent.UI.Toolkits;
using RodelAgent.UI.ViewModels.Core;

namespace RodelAgent.UI.ViewModels.View;

/// <summary>
/// 语音页面视图模型.
/// </summary>
public sealed partial class AudioPageViewModel
{
    [RelayCommand]
    private async Task StartAudioAsync()
    {
        if (string.IsNullOrEmpty(Text))
        {
            return;
        }

        if (IsGenerating)
        {
            CancelAudio();
        }

        IsGenerating = true;
        try
        {
            _audioCts = new CancellationTokenSource();
            var provider = SelectedService!.ProviderType;
            var text = Text;
            var model = SelectedModel?.Data.Id;
            var options = new AudioOptions
            {
                ModelId = SelectedModel?.Id,
                LanguageCode = SelectedLanguage?.Code,
                VoiceId = SelectedVoice?.Data.Id,
                Speed = 1.0,
            };
            var result = await _audioService!.Client!.TextToSpeechAsync(text, options, _audioCts.Token);
            var record = new AudioRecord
            {
                Id = Guid.NewGuid().ToString("N"),
                Text = text,
                Speed = options.Speed,
                Voice = options.VoiceId,
                Model = model,
                Time = DateTimeOffset.Now,
                Provider = provider,
            };

            await this.Get<IStorageService>().AddOrUpdateAudioSessionAsync(record, result.ToArray());
            ReloadHistoryCommand.Execute(default);
            ShowRecord(record);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to generate speech.");
            this.Get<AppViewModel>().ShowTipCommand.Execute((ex.Message, InfoType.Error));
        }
        finally
        {
            IsGenerating = false;
        }
    }

    [RelayCommand]
    private void CancelAudio()
    {
        _audioCts?.Cancel();
        _audioCts = null;
        IsGenerating = false;
    }

    [RelayCommand]
    private void ShowRecord(AudioRecord record)
    {
        Text = record.Text ?? string.Empty;
        AudioPath = AppToolkit.GetAudioPath(record.Id);
        this.Get<AudioWaveViewModel>().LoadFileCommand.Execute(AudioPath);
    }
}
