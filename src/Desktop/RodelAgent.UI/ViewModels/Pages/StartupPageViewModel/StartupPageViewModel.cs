// Copyright (c) Rodel. All rights reserved.

using Microsoft.Windows.AppLifecycle;
using RodelAgent.Interfaces;
using RodelAgent.UI.Models.Args;
using RodelAgent.UI.Models.Constants;
using RodelAgent.UI.Toolkits;

namespace RodelAgent.UI.ViewModels.Pages;

/// <summary>
/// 启动页面视图模型.
/// </summary>
public sealed partial class StartupPageViewModel : ViewModelBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="StartupPageViewModel"/> class.
    /// </summary>
    public StartupPageViewModel(
        AppViewModel appVM,
        ILogger<StartupPageViewModel> logger,
        IStorageService storageService)
    {
        _appViewModel = appVM;
        _logger = logger;
        _storageService = storageService;
        StepCount = 6;
        CurrentStep = 0;
        CheckStep();
    }

    /// <summary>
    /// 侧页面导航请求.
    /// </summary>
    public void SideNavigate(Type pageType, object parameter = null)
        => SideNavigationRequested?.Invoke(this, new AppNavigationEventArgs(pageType, parameter));

    /// <summary>
    /// 检查其它同名配置是否已完成.
    /// </summary>
    /// <param name="provider">服务商名称.</param>
    /// <param name="type">功能类型.</param>
    /// <returns>是否已完成.</returns>
    public FeatureType? GetOtherConfigCompleted(string provider, FeatureType type)
    {
        if (type == FeatureType.Draw)
        {
            var chat = OnlineChatServices.FirstOrDefault(p => p.ProviderType.ToString() == provider);
            if (chat is not null && chat.IsCompleted)
            {
                return FeatureType.Chat;
            }

            var audio = OnlineAudioServices.FirstOrDefault(p => p.ProviderType.ToString() == provider);
            if (audio is not null && audio.IsCompleted)
            {
                return FeatureType.Audio;
            }
        }
        else if (type == FeatureType.Audio)
        {
            var chat = OnlineChatServices.FirstOrDefault(p => p.ProviderType.ToString() == provider);
            if (chat is not null && chat.IsCompleted)
            {
                return FeatureType.Chat;
            }

            var draw = OnlineDrawServices.FirstOrDefault(p => p.ProviderType.ToString() == provider);
            if (draw is not null && draw.IsCompleted)
            {
                return FeatureType.Draw;
            }
        }
        else if (type == FeatureType.Chat)
        {
            var draw = OnlineDrawServices.FirstOrDefault(p => p.ProviderType.ToString() == provider);
            if (draw is not null && draw.IsCompleted)
            {
                return FeatureType.Draw;
            }

            var audio = OnlineAudioServices.FirstOrDefault(p => p.ProviderType.ToString() == provider);
            if (audio is not null && audio.IsCompleted)
            {
                return FeatureType.Audio;
            }
        }

        return default;
    }

    /// <summary>
    /// 获取其它已完成的配置.
    /// </summary>
    /// <param name="provider">服务商名称.</param>
    /// <param name="type">已完成配置所属的功能.</param>
    /// <returns>配置.</returns>
    public object GetOtherCompletedConfig(string provider, FeatureType type)
    {
        var config = type switch
        {
            FeatureType.Chat => (object)OnlineChatServices.FirstOrDefault(p => p.ProviderType.ToString() == provider).Config,
            FeatureType.Draw => OnlineDrawServices.FirstOrDefault(p => p.ProviderType.ToString() == provider).Config,
            FeatureType.Audio => OnlineAudioServices.FirstOrDefault(p => p.ProviderType.ToString() == provider).Config,
            _ => default
        };

        return config;
    }

    [RelayCommand]
    private void CheckMigration()
    {
        var isMigrating = SettingsToolkit.ReadLocalSetting(SettingNames.IsMigrating, false);
        if (isMigrating)
        {
            SelectFolderCommand.Execute(default);
        }
    }

    [RelayCommand]
    private void Restart()
    {
        try
        {
            SettingsToolkit.WriteLocalSetting(SettingNames.ShouldSkipStartup, true);
            AppInstance.GetCurrent().UnregisterKey();
            _ = AppInstance.Restart(default);
        }
        catch (Exception ex)
        {
            _appViewModel.ShowTip(ex.Message, InfoType.Error);
            _logger.LogError(ex, "Failed to restart the app.");
        }
    }

    [RelayCommand]
    private void GoNext()
    {
        if (CurrentStep < StepCount - 1)
        {
            if (IsOnlineChatStep)
            {
                SaveOnlineChatServicesCommand.Execute(default);
            }
            else if (IsOnlineTranslateStep)
            {
                SaveOnlineTranslateServicesCommand.Execute(default);
            }
            else if (IsOnlineDrawStep)
            {
                SaveOnlineDrawServicesCommand.Execute(default);
            }
            else if (IsOnlineAudioStep)
            {
                SaveOnlineAudioServicesCommand.Execute(default);
            }

            CurrentStep++;
        }
        else
        {
            Restart();
        }
    }

    [RelayCommand]
    private void GoPrev()
        => CurrentStep--;

    [RelayCommand]
    private async Task SelectFolderAsync()
    {
        var isMigrating = SettingsToolkit.ReadLocalSetting(SettingNames.IsMigrating, false);
        var path = string.Empty;

        if (isMigrating)
        {
            var prevFolder = SettingsToolkit.ReadLocalSetting(SettingNames.WorkingDirectory, string.Empty);
            path = prevFolder;
        }

        if (string.IsNullOrEmpty(path))
        {
            var folder = await FileToolkit.PickFolderAsync(_appViewModel.ActivatedWindow);
            if (folder is null)
            {
                return;
            }

            path = folder.Path;
        }

        SelectedFolder = path;
        _storageService.SetWorkingDirectory(path);

        // Check if folder has file name "_secret_.db".
        var secretDbPath = Path.Combine(path, "_secret_.db");
        var isMigrateSuccess = true;
        var needRestart = false;
        if (File.Exists(secretDbPath))
        {
            IsMigrating = true;
            try
            {
                var migration = new Migration.V1.MigrationUtils(path, _storageService);
                await migration.MigrateAsync();
                needRestart = Directory.Exists(Path.Combine(path, "v2_temp"));
                isMigrateSuccess = true;
            }
            catch (Exception ex)
            {
                isMigrateSuccess = false;
                _logger.LogCritical(ex, "Failed to migrate data.");
                await _appViewModel.ShowMessageDialogAsync(ex.Message);
            }

            SettingsToolkit.WriteLocalSetting(SettingNames.IsMigrating, false);
            IsMigrating = false;
        }

        if (needRestart)
        {
            SettingsToolkit.WriteLocalSetting(SettingNames.IsMigrating, true);
            AppInstance.GetCurrent().UnregisterKey();
            _ = AppInstance.Restart(default);
        }
        else if (isMigrateSuccess)
        {
            GoNext();
        }
        else
        {
            SelectedFolder = string.Empty;
        }
    }

    [RelayCommand]
    private async Task InitializeOnlineChatServicesAsync()
    {
        IsOnlineChatInitializing = true;
        await PageViewModelShare.InitializeOnlineChatServicesAsync(OnlineChatServices, _storageService);
        IsOnlineChatInitializing = false;
    }

    [RelayCommand]
    private Task SaveOnlineChatServicesAsync()
        => PageViewModelShare.SaveOnlineChatServicesAsync(OnlineChatServices, _storageService);

    [RelayCommand]
    private async Task InitializeOnlineTranslateServicesAsync()
    {
        IsOnlineTranslateInitializing = true;
        await PageViewModelShare.InitializeOnlineTranslateServicesAsync(OnlineTranslateServices, _storageService);
        IsOnlineTranslateInitializing = false;
    }

    [RelayCommand]
    private Task SaveOnlineTranslateServicesAsync()
        => PageViewModelShare.SaveOnlineTranslateServicesAsync(OnlineTranslateServices, _storageService);

    [RelayCommand]
    private async Task InitializeOnlineDrawServicesAsync()
    {
        IsOnlineDrawInitializing = true;
        await PageViewModelShare.InitializeOnlineDrawServicesAsync(OnlineDrawServices, _storageService);
        foreach (var item in OnlineDrawServices)
        {
            if (!item.IsCompleted)
            {
                var type = GetOtherConfigCompleted(item.ProviderType.ToString(), FeatureType.Draw);
                if (type is not null)
                {
                    var config = GetOtherCompletedConfig(item.ProviderType.ToString(), type.Value);
                    switch (item.ProviderType)
                    {
                        case RodelDraw.Models.Constants.ProviderType.OpenAI:
                            {
                                if (config is RodelChat.Models.Client.OpenAIClientConfig openAIConfig)
                                {
                                    item.SetConfig(new RodelDraw.Models.Client.OpenAIClientConfig
                                    {
                                        Endpoint = openAIConfig.Endpoint,
                                        Key = openAIConfig.Key,
                                        OrganizationId = openAIConfig.OrganizationId,
                                    });
                                }
                            }

                            break;
                        case RodelDraw.Models.Constants.ProviderType.AzureOpenAI:
                            {
                                if (config is RodelChat.Models.Client.AzureOpenAIClientConfig azureOpenAIConfig)
                                {
                                    item.SetConfig(new RodelDraw.Models.Client.AzureOpenAIClientConfig
                                    {
                                        Endpoint = azureOpenAIConfig.Endpoint,
                                        Key = azureOpenAIConfig.Key,
                                        Version = (RodelAgent.Models.Constants.AzureOpenAIVersion)(int)azureOpenAIConfig.Version,
                                    });
                                }
                            }

                            break;
                        case RodelDraw.Models.Constants.ProviderType.QianFan:
                            {
                                if (config is RodelChat.Models.Client.QianFanClientConfig qianfanConfig)
                                {
                                    item.SetConfig(new RodelDraw.Models.Client.QianFanClientConfig
                                    {
                                        Secret = qianfanConfig.Secret,
                                        Key = qianfanConfig.Key,
                                    });
                                }
                            }

                            break;
                        case RodelDraw.Models.Constants.ProviderType.HunYuan:
                            {
                                if (config is RodelChat.Models.Client.HunYuanClientConfig hunyuanConfig)
                                {
                                    item.SetConfig(new RodelDraw.Models.Client.HunYuanClientConfig
                                    {
                                        SecretId = hunyuanConfig.SecretId,
                                        Key = hunyuanConfig.Key,
                                    });
                                }
                            }

                            break;
                        case RodelDraw.Models.Constants.ProviderType.SparkDesk:
                            {
                                if (config is RodelChat.Models.Client.SparkDeskClientConfig sparkDeskConfig)
                                {
                                    item.SetConfig(new RodelDraw.Models.Client.SparkDeskClientConfig
                                    {
                                        Secret = sparkDeskConfig.Secret,
                                        AppId = sparkDeskConfig.AppId,
                                        Key = sparkDeskConfig.Key,
                                    });
                                }
                            }

                            break;
                        default:
                            break;
                    }
                }
            }
        }

        IsOnlineDrawInitializing = false;
    }

    [RelayCommand]
    private Task SaveOnlineDrawServicesAsync()
        => PageViewModelShare.SaveOnlineDrawServicesAsync(OnlineDrawServices, _storageService);

    [RelayCommand]
    private async Task InitializeOnlineAudioServicesAsync()
    {
        IsOnlineAudioInitializing = true;
        await PageViewModelShare.InitializeOnlineAudioServicesAsync(OnlineAudioServices, _storageService);
        foreach (var item in OnlineAudioServices)
        {
            if (!item.IsCompleted)
            {
                var type = GetOtherConfigCompleted(item.ProviderType.ToString(), FeatureType.Audio);
                if (type is not null)
                {
                    var config = GetOtherCompletedConfig(item.ProviderType.ToString(), type.Value);
                    switch (item.ProviderType)
                    {
                        case RodelAudio.Models.Constants.ProviderType.OpenAI:
                            {
                                if (config is RodelChat.Models.Client.OpenAIClientConfig openAIConfig)
                                {
                                    item.SetConfig(new RodelAudio.Models.Client.OpenAIClientConfig
                                    {
                                        Key = openAIConfig.Key,
                                        OrganizationId = openAIConfig.OrganizationId,
                                    });
                                }
                            }

                            break;
                        case RodelAudio.Models.Constants.ProviderType.AzureOpenAI:
                            {
                                if (config is RodelChat.Models.Client.AzureOpenAIClientConfig azureOpenAIConfig)
                                {
                                    item.SetConfig(new RodelAudio.Models.Client.AzureOpenAIClientConfig
                                    {
                                        Endpoint = azureOpenAIConfig.Endpoint,
                                        Key = azureOpenAIConfig.Key,
                                    });
                                }
                            }

                            break;
                        default:
                            break;
                    }
                }
            }
        }

        IsOnlineAudioInitializing = false;
    }

    [RelayCommand]
    private Task SaveOnlineAudioServicesAsync()
        => PageViewModelShare.SaveOnlineAudioServicesAsync(OnlineAudioServices, _storageService);

    private void CheckStep()
    {
        IsWelcomeStep = CurrentStep == 0;
        IsOnlineChatStep = CurrentStep == 1;
        IsOnlineDrawStep = CurrentStep == 2;
        IsOnlineAudioStep = CurrentStep == 3;
        IsOnlineTranslateStep = CurrentStep == 4;
        IsLastStep = CurrentStep == StepCount - 1;
        IsPreviousStepShown = CurrentStep > 0 && !IsLastStep;

        if (IsOnlineChatStep)
        {
            InitializeOnlineChatServicesCommand.Execute(default);
        }
        else if (IsOnlineTranslateStep)
        {
            InitializeOnlineTranslateServicesCommand.Execute(default);
        }
        else if (IsOnlineDrawStep)
        {
            InitializeOnlineDrawServicesCommand.Execute(default);
        }
        else if (IsOnlineAudioStep)
        {
            InitializeOnlineAudioServicesCommand.Execute(default);
        }
    }

    partial void OnCurrentStepChanged(int value)
        => CheckStep();
}
