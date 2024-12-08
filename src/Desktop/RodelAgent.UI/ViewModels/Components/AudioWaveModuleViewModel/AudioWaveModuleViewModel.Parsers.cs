// Copyright (c) Rodel. All rights reserved.

using RodelAgent.UI.Extensions;
using RodelAgent.UI.Toolkits;

namespace RodelAgent.UI.ViewModels.Components;

/// <summary>
/// 音频波形模块视图模型.
/// </summary>
public sealed partial class AudioWaveModuleViewModel
{
    private static string GetRecordingFilePath(string fileName)
    {
        if (string.IsNullOrEmpty(fileName))
        {
            return null;
        }

        var workDir = SettingsToolkit.ReadLocalSetting(Models.Constants.SettingNames.WorkingDirectory, string.Empty);
        return Path.Combine(workDir, "Recording", fileName + ".wav");
    }

    private async Task ParseFileAsync(string filePath)
    {
        await Task.Run(() =>
        {
            using var audioFileReader = new AudioFileReader(filePath);

            _dispatcherQueue.TryEnqueue(() =>
            {
                Seconds = audioFileReader.TotalTime.TotalSeconds;
            });

            var bytesPerSample = audioFileReader.WaveFormat.BitsPerSample / 8;
            var bytesPerFrame = bytesPerSample * audioFileReader.WaveFormat.Channels;
            var buffer = new byte[bytesPerFrame * audioFileReader.WaveFormat.SampleRate];
            int read;

            var monoData = new List<float>();

            while ((read = audioFileReader.Read(buffer, 0, buffer.Length)) > 0)
            {
                for (var i = 0; i < read; i += bytesPerFrame)
                {
                    float total = 0;
                    for (var channel = 0; channel < audioFileReader.WaveFormat.Channels; channel++)
                    {
                        var sample = BitConverter.ToSingle(buffer, i + (channel * bytesPerSample));
                        total += sample;
                    }

                    monoData.Add(total / audioFileReader.WaveFormat.Channels); // 计算平均值
                }
            }

            _samples = monoData.ToList();
        });

        _dispatcherQueue.TryEnqueue(() =>
        {
            RedrawWave?.Invoke(this, EventArgs.Empty);
        });
    }
}
