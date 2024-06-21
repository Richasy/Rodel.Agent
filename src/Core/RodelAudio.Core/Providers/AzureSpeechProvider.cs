// Copyright (c) Rodel. All rights reserved.

using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Speakers.Azure;
using RodelAgent.Interfaces;
using RodelAgent.Statics;
using RodelAudio.Interfaces.Client;
using RodelAudio.Models.Client;
using RodelAudio.Models.Constants;

namespace RodelAudio.Core.Providers;

/// <summary>
/// Azure 语音服务.
/// </summary>
public sealed class AzureSpeechProvider : ProviderBase, IProvider
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AzureOpenAIProvider"/> class.
    /// </summary>
    public AzureSpeechProvider(AzureSpeechClientConfig config)
        : base(config.Key, config.CustomModels)
    {
        Region = config.Region;
    }

    private string Region { get; set; }

    /// <inheritdoc/>
    public PromptExecutionSettings ConvertExecutionSettings(AudioSession sessionData)
    {
        var voices = ServerModels.FirstOrDefault(x => x.Id == sessionData.Model)?.Voices;
        var voice = voices?.FirstOrDefault(x => x.Id == sessionData.Voice);

        return new AzureTextToAudioExecutionSettings
        {
            ModelId = sessionData.Model,
            Speed = (float)(sessionData.Speed ?? 1.0),
            Gender = voice.Gender.ToString(),
            Language = voice.Languages.First(),
            Voice = sessionData.Voice,
        };
    }

    /// <inheritdoc/>
    public Kernel? GetOrCreateKernel(string modelId)
    {
        if (ShouldRecreateKernel(modelId))
        {
            Kernel = Kernel.CreateBuilder()
                .AddAzureTextToSpeech(AccessKey, Region)
                .Build();
        }

        return Kernel;
    }

    /// <summary>
    /// 初始化语音服务.
    /// </summary>
    /// <returns>是否成功.</returns>
    public async Task<bool> InitializeSpeechVoicesAsync()
    {
        try
        {
            var storageService = GlobalStatics.ServiceProvider.GetRequiredService<IStorageService>();
            var localJson = await storageService.RetrieveAzureSpeechVoicesAsync();
            if (string.IsNullOrEmpty(localJson))
            {
                using var client = new HttpClient();
                using var request = new HttpRequestMessage(HttpMethod.Get, new Uri($"https://{Region}.tts.speech.microsoft.com/cognitiveservices/voices/list"));
                request.Headers.Add("Ocp-Apim-Subscription-Key", AccessKey);
                var response = await client.SendAsync(request);
                if (!response.IsSuccessStatusCode)
                {
                    return false;
                }

                var json = await response.Content.ReadAsStringAsync();
                var azureVoices = JsonSerializer.Deserialize<List<AzureVoice>>(json);
                var voices = azureVoices.Select(x => new AudioVoice
                {
                    Id = x.ShortName,
                    DisplayName = x.LocalName,
                    Gender = x.Gender switch
                    {
                        "Male" => VoiceGender.Male,
                        "Female" => VoiceGender.Female,
                        _ => VoiceGender.Neutral,
                    },
                    Languages = new List<string> { x.Locale },
                }).ToList();
                localJson = JsonSerializer.Serialize(voices);
                await storageService.SaveAzureSpeechVoicesAsync(localJson);
            }

            if (!string.IsNullOrEmpty(localJson))
            {
                var voices = JsonSerializer.Deserialize<List<AudioVoice>>(localJson);
                ServerModels = new List<AudioModel>
                {
                    new AudioModel
                    {
                        Id = Region,
                        DisplayName = Region,
                        Voices = voices,
                    },
                };

                return true;
            }
        }
        catch (Exception ex)
        {
            GlobalStatics.ServiceProvider.GetService<ILogger<AzureSpeechProvider>>()?.LogError(ex, "Failed to initialize Azure speech voices.");
        }

        return false;
    }

    internal class AzureVoice
    {
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public string LocalName { get; set; }
        public string ShortName { get; set; }
        public string Gender { get; set; }
        public string Locale { get; set; }
        public string LocaleName { get; set; }
        public string SampleRateHertz { get; set; }
        public string VoiceType { get; set; }
        public string Status { get; set; }
    }
}
