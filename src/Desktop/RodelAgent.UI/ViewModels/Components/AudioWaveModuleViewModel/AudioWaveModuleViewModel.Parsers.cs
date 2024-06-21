// Copyright (c) Rodel. All rights reserved.

using NAudio.Wave;
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

    private void ResetRecording(string sessionId)
    {
        _sessionId = sessionId;
        _samples.Clear();
        Seconds = 0;
        var format = new WaveFormat(SampleRate, 1);
        _waveFileWriter = !string.IsNullOrEmpty(_sessionId)
            ? new WaveFileWriter(GetRecordingFilePath(_sessionId), format)
            : default;

        var waveIn = new WaveInEvent();
        waveIn.WaveFormat = format;
        waveIn.DataAvailable += async (sender, e) =>
        {
            if (!IsRecording)
            {
                _dispatcherQueue.TryEnqueue(() =>
                {
                    IsRecording = true;
                });
            }

            var bytesPerSample = waveIn.WaveFormat.BitsPerSample / 8;
            var sampleCount = e.BytesRecorded / bytesPerSample;
            var floatBuffer = new float[sampleCount];
            for (var index = 0; index < sampleCount; index++)
            {
                // Convert bytes to float
                floatBuffer[index] = BitConverter.ToInt16(e.Buffer, index * bytesPerSample) / 32768f;
            }

            _samples.AddRange(floatBuffer);

            _dispatcherQueue.TryEnqueue(() =>
            {
                Seconds = _samples.Count / (SampleRate * 1.0);
                RedrawWave?.Invoke(this, EventArgs.Empty);
            });

            if (_waveFileWriter != null)
            {
                await WriteFileAsync(e.Buffer, e.BytesRecorded);
            }
        };

        waveIn.RecordingStopped += (sender, e) =>
        {
            _waveIn.Dispose();
            _waveFileWriter?.Dispose();
            _waveIn = null;
            _waveFileWriter = null;

            _dispatcherQueue.TryEnqueue(() =>
            {
                IsRecording = false;
            });
        };

        _waveIn = waveIn;
    }

    private async Task WriteFileAsync(byte[] buffer, int bytesRecorded)
    {
        if (_waveFileWriter == null)
        {
            return;
        }

        await _waveFileWriter.WriteAsync(buffer, 0, bytesRecorded);
    }
}
