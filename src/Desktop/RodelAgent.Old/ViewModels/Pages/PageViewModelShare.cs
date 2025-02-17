// Copyright (c) Richasy. All rights reserved.

using RodelAgent.Interfaces;
using RodelAgent.UI.ViewModels.Items;
using System.Text.Json;
using audioClient = RodelAudio.Models.Client;
using audioConstants = RodelAudio.Models.Constants;
using chatClient = RodelChat.Models.Client;
using chatConstants = RodelChat.Models.Constants;
using drawClient = RodelDraw.Models.Client;
using drawConstants = RodelDraw.Models.Constants;
using translateClient = RodelTranslate.Models.Client;
using translateConstants = RodelTranslate.Models.Constants;

namespace RodelAgent.UI.ViewModels.Pages;

internal static class PageViewModelShare
{
    public static async Task<List<ChatServiceItemViewModel>> GetChatServicesAsync(IStorageService storageService)
    {
        var result = new List<ChatServiceItemViewModel>();
        var chatServices = ChatStatics.GetOnlineChatServices();
        foreach (var chatService in chatServices)
        {
            var config = await GetChatConfigFromStorageAsync(chatService.Key, storageService);
            var vm = new ChatServiceItemViewModel(chatService.Key, chatService.Value);
            vm.SetConfig(config);
            result.Add(vm);
        }

        return result;
    }

    public static async Task<List<TranslateServiceItemViewModel>> GetTranslateServicesAsync(IStorageService storageService)
    {
        var result = new List<TranslateServiceItemViewModel>();
        var translateServices = TranslateStatics.GetOnlineTranslateServices();
        foreach (var translateService in translateServices)
        {
            var config = await GetTranslateConfigFromStorageAsync(translateService.Key, storageService);
            var vm = new TranslateServiceItemViewModel(translateService.Key, translateService.Value);
            vm.SetConfig(config);
            result.Add(vm);
        }

        return result;
    }

    public static async Task<List<DrawServiceItemViewModel>> GetDrawServicesAsync(IStorageService storageService)
    {
        var result = new List<DrawServiceItemViewModel>();
        var drawServices = DrawStatics.GetOnlineDrawServices();
        foreach (var drawService in drawServices)
        {
            var config = await GetDrawConfigFromStorageAsync(drawService.Key, storageService);
            var vm = new DrawServiceItemViewModel(drawService.Key, drawService.Value);
            vm.SetConfig(config);
            result.Add(vm);
        }

        return result;
    }

    public static async Task<List<AudioServiceItemViewModel>> GetAudioServicesAsync(IStorageService storageService)
    {
        var result = new List<AudioServiceItemViewModel>();
        var audioServices = AudioStatics.GetOnlineAudioServices();
        foreach (var audioService in audioServices)
        {
            var config = await GetAudioConfigFromStorageAsync(audioService.Key, storageService);
            var vm = new AudioServiceItemViewModel(audioService.Key, audioService.Value);
            vm.SetConfig(config);
            result.Add(vm);
        }

        return result;
    }

    public static async Task InitializeOnlineChatServicesAsync(ObservableCollection<ChatServiceItemViewModel> onlineChatServices, IStorageService storageService)
    {
        if (onlineChatServices.Count > 0)
        {
            return;
        }

        var chatServices = await GetChatServicesAsync(storageService);
        foreach (var chatService in chatServices)
        {
            onlineChatServices.Add(chatService);
        }
    }

    public static async Task InitializeOnlineTranslateServicesAsync(ObservableCollection<TranslateServiceItemViewModel> onlineTranslateServices, IStorageService storageService)
    {
        if (onlineTranslateServices.Count > 0)
        {
            return;
        }

        var translateServices = await GetTranslateServicesAsync(storageService);
        foreach (var translateService in translateServices)
        {
            onlineTranslateServices.Add(translateService);
        }
    }

    public static async Task InitializeOnlineDrawServicesAsync(ObservableCollection<DrawServiceItemViewModel> onlineDrawServices, IStorageService storageService)
    {
        if (onlineDrawServices.Count > 0)
        {
            return;
        }

        var drawServices = await GetDrawServicesAsync(storageService);
        foreach (var drawService in drawServices)
        {
            onlineDrawServices.Add(drawService);
        }
    }

    public static async Task InitializeOnlineAudioServicesAsync(ObservableCollection<AudioServiceItemViewModel> onlineAudioServices, IStorageService storageService)
    {
        if (onlineAudioServices.Count > 0)
        {
            return;
        }

        var drawServices = await GetAudioServicesAsync(storageService);
        foreach (var drawService in drawServices)
        {
            onlineAudioServices.Add(drawService);
        }
    }

    public static async Task SaveOnlineChatServicesAsync(ObservableCollection<ChatServiceItemViewModel> onlineChatServices, IStorageService storageService)
    {
        foreach (var service in onlineChatServices)
        {
            var content = JsonSerializer.Serialize(service.Config, GetChatProviderConfigType(service.ProviderType));
            await storageService.SetChatConfigAsync(service.ProviderType, content);
        }
    }

    public static async Task SaveOnlineTranslateServicesAsync(ObservableCollection<TranslateServiceItemViewModel> onlineTranslateServices, IStorageService storageService)
    {
        foreach (var service in onlineTranslateServices)
        {
            var content = JsonSerializer.Serialize(service.Config, GetTranslateProviderConfigType(service.ProviderType));
            await storageService.SetTranslateConfigAsync(service.ProviderType, content);
        }
    }

    public static async Task SaveOnlineDrawServicesAsync(ObservableCollection<DrawServiceItemViewModel> onlineDrawServices, IStorageService storageService)
    {
        foreach (var service in onlineDrawServices)
        {
            var content = JsonSerializer.Serialize(service.Config, GetDrawProviderConfigType(service.ProviderType));
            await storageService.SetDrawConfigAsync(service.ProviderType, content);
        }
    }

    public static async Task SaveOnlineAudioServicesAsync(ObservableCollection<AudioServiceItemViewModel> onlineAudioServices, IStorageService storageService)
    {
        foreach (var service in onlineAudioServices)
        {
            var content = JsonSerializer.Serialize(service.Config, GetAudioProviderConfigType(service.ProviderType));
            await storageService.SetAudioConfigAsync(service.ProviderType, content);
        }
    }

    private static async Task<chatClient.ClientConfigBase> GetChatConfigFromStorageAsync(chatConstants.ProviderType type, IStorageService storageService)
    {
        var json = await storageService.GetChatConfigAsync<string>(type);
        if (string.IsNullOrEmpty(json))
        {
            return default;
        }

        var config = JsonSerializer.Deserialize(json, GetChatProviderConfigType(type));
        return config as chatClient.ClientConfigBase;
    }

    private static Type GetChatProviderConfigType(chatConstants.ProviderType type)
    {
        var baseType = typeof(chatClient.ClientConfigBase);
        var assembly = baseType.Assembly;
        return assembly.GetType($"{baseType.Namespace}.{type}ClientConfig");
    }

    private static async Task<translateClient.ClientConfigBase> GetTranslateConfigFromStorageAsync(translateConstants.ProviderType type, IStorageService storageService)
    {
        var json = await storageService.GetTranslateConfigAsync<string>(type);
        if (string.IsNullOrEmpty(json))
        {
            return default;
        }

        var config = JsonSerializer.Deserialize(json, GetTranslateProviderConfigType(type));
        return config as translateClient.ClientConfigBase;
    }

    private static Type GetTranslateProviderConfigType(translateConstants.ProviderType type)
    {
        var baseType = typeof(translateClient.ClientConfigBase);
        var assembly = baseType.Assembly;
        return assembly.GetType($"{baseType.Namespace}.{type}ClientConfig");
    }

    private static async Task<drawClient.ClientConfigBase> GetDrawConfigFromStorageAsync(drawConstants.ProviderType type, IStorageService storageService)
    {
        var json = await storageService.GetDrawConfigAsync<string>(type);
        if (string.IsNullOrEmpty(json))
        {
            return default;
        }

        var config = JsonSerializer.Deserialize(json, GetDrawProviderConfigType(type));
        return config as drawClient.ClientConfigBase;
    }

    private static Type GetDrawProviderConfigType(drawConstants.ProviderType type)
    {
        var baseType = typeof(drawClient.ClientConfigBase);
        var assembly = baseType.Assembly;
        return assembly.GetType($"{baseType.Namespace}.{type}ClientConfig");
    }

    private static async Task<audioClient.ClientConfigBase> GetAudioConfigFromStorageAsync(audioConstants.ProviderType type, IStorageService storageService)
    {
        var json = await storageService.GetAudioConfigAsync<string>(type);
        if (string.IsNullOrEmpty(json))
        {
            return default;
        }

        var config = JsonSerializer.Deserialize(json, GetAudioProviderConfigType(type));
        return config as audioClient.ClientConfigBase;
    }

    private static Type GetAudioProviderConfigType(audioConstants.ProviderType type)
    {
        var baseType = typeof(audioClient.ClientConfigBase);
        var assembly = baseType.Assembly;
        return assembly.GetType($"{baseType.Namespace}.{type}ClientConfig");
    }
}
