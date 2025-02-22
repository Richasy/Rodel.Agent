// Copyright (c) Richasy. All rights reserved.

using Richasy.AgentKernel;
using Richasy.WinUIKernel.AI.ViewModels;
using RodelAgent.UI.ViewModels.Core;

namespace RodelAgent.UI.ViewModels.View;

/// <summary>
/// 设置页面视图模型.
/// </summary>
public sealed partial class SettingsPageViewModel
{
    /// <inheritdoc/>
    public override async Task InitializeChatServicesAsync()
    {
        await base.InitializeChatServicesAsync();
        if (ChatServices.Count > 0)
        {
            return;
        }

        foreach (var provider in Enum.GetValues<ChatProviderType>())
        {
            ChatServices.Add(new ChatServiceItemViewModel(provider));
        }
    }

    /// <inheritdoc/>
    public override async Task InitializeAudioServicesAsync()
    {
        await base.InitializeAudioServicesAsync();
        if (AudioServices.Count > 0)
        {
            return;
        }

        foreach (var provider in Enum.GetValues<AudioProviderType>())
        {
            AudioServices.Add(new AudioServiceItemViewModel(provider));
        }
    }

    /// <inheritdoc/>
    public override async Task InitializeDrawServicesAsync()
    {
        await base.InitializeDrawServicesAsync();
        if (DrawServices.Count > 0)
        {
            return;
        }

        foreach (var provider in Enum.GetValues<DrawProviderType>())
        {
            DrawServices.Add(new DrawServiceItemViewModel(provider));
        }
    }

    /// <inheritdoc/>
    public override async Task InitializeTranslateServicesAsync()
    {
        await base.InitializeTranslateServicesAsync();
        if (TranslateServices.Count > 0)
        {
            return;
        }

        foreach (var provider in Enum.GetValues<TranslateProviderType>())
        {
            TranslateServices.Add(new TranslateServiceItemViewModel(provider));
        }
    }

    /// <inheritdoc/>
    protected override async Task SaveChatServicesAsync()
    {
        await base.SaveChatServicesAsync();
        var configManager = this.Get<IChatConfigManager>();
        var dict = ChatServices.Where(p => p.Config != null).ToDictionary(item => item.ProviderType, item => item.Config!);
        await configManager.SaveChatConfigAsync(dict);
        this.Get<AppViewModel>().TryReloadChatServicesCommand.Execute(default);
    }

    /// <inheritdoc/>
    protected override async Task SaveAudioServicesAsync()
    {
        await base.SaveAudioServicesAsync();
        var configManager = this.Get<IAudioConfigManager>();
        var dict = AudioServices.Where(p => p.Config != null).ToDictionary(item => item.ProviderType, item => item.Config!);
        await configManager.SaveAudioConfigAsync(dict);
        this.Get<AppViewModel>().TryReloadAudioServicesCommand.Execute(default);
    }

    /// <inheritdoc/>
    protected override async Task SaveDrawServicesAsync()
    {
        await base.SaveDrawServicesAsync();
        var configManager = this.Get<IDrawConfigManager>();
        var dict = DrawServices.Where(p => p.Config != null).ToDictionary(item => item.ProviderType, item => item.Config!);
        await configManager.SaveDrawConfigAsync(dict);
        this.Get<AppViewModel>().TryReloadDrawServicesCommand.Execute(default);
    }

    /// <inheritdoc/>
    protected override async Task SaveTranslateServicesAsync()
    {
        await base.SaveTranslateServicesAsync();
        var configManager = this.Get<ITranslateConfigManager>();
        var dict = TranslateServices.Where(p => p.Config != null).ToDictionary(item => item.ProviderType, item => item.Config!);
        await configManager.SaveTranslateConfigAsync(dict);
        this.Get<AppViewModel>().TryReloadTranslateServicesCommand.Execute(default);
    }
}
