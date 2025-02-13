// Copyright (c) Rodel. All rights reserved.

using System.Diagnostics.CodeAnalysis;
using Microsoft.UI.Dispatching;
using Richasy.AgentKernel;
using Richasy.WinUIKernel.AI;
using Richasy.WinUIKernel.Share;
using Richasy.WinUIKernel.Share.Toolkits;
using RichasyKernel;
using RodelAgent.Context;
using RodelAgent.Interfaces;
using RodelAgent.Statics;
using RodelAgent.UI.Extensions;
using RodelAgent.UI.Toolkits;
using RodelAgent.UI.ViewModels;
using RodelAgent.UI.ViewModels.Components;
using RodelAgent.UI.ViewModels.Pages;
using RodelAudio.Core;
using RodelAudio.Interfaces.Client;
using RodelChat.Core;
using RodelChat.Interfaces.Client;
using RodelChat.Models.Client;
using RodelDraw.Core;
using RodelDraw.Interfaces.Client;
using RodelTranslate.Core;
using RodelTranslate.Interfaces.Client;
using Serilog;
using Windows.Storage;

namespace RodelAgent.UI;

/// <summary>
/// 全局依赖项.
/// </summary>
internal static class GlobalDependencies
{
    public static Kernel Kernel { get; private set; }

    /// <summary>
    /// 初始化.
    /// </summary>
    public static void Initialize()
    {
        if (Kernel is not null)
        {
            return;
        }

        Kernel = Kernel.CreateBuilder()
            .AddSerilog()
            .AddDispatcherQueue()
            .AddShareToolkits()
            .AddConfigManager()
            .AddXamlRootProvider()

            .AddAliTranslationService()
            .AddAzureTranslationService()
            .AddBaiduTranslationService()
            .AddTencentTranslationService()
            .AddYoudaoTranslationService()
            .AddVolcanoTranslationService()
            .AddGoogleTranslationService()

            .AddAzureAudioService()
            .AddEdgeAudioService()
            .AddAzureOpenAIAudioService()
            .AddOpenAIAudioService()
            .AddWindowsAudioService()

            .AddOpenAIChatService()
            .AddAzureOpenAIChatService()
            .AddAzureAIChatService()
            .AddXAIChatService()
            .AddZhiPuChatService()
            .AddLingYiChatService()
            .AddAnthropicChatService()
            .AddMoonshotChatService()
            .AddGeminiChatService()
            .AddDeepSeekChatService()
            .AddQwenChatService()
            .AddErnieChatService()
            .AddHunyuanChatService()
            .AddSparkChatService()
            .AddDoubaoChatService()
            .AddSiliconFlowChatService()
            .AddOpenRouterChatService()
            .AddTogetherAIChatService()
            .AddGroqChatService()
            .AddOllamaChatService()
            .AddMistralChatService()
            .AddPerplexityChatService()

            .AddAzureOpenAIDrawService()
            .AddOpenAIDrawService()
            .AddErnieDrawService()
            .AddHunyuanDrawService()
            .AddSparkDrawService()

            .AddSingleton<IStringResourceToolkit, ResourceToolkit>()
            .AddSingleton<IChatParametersFactory, ChatParametersFactory>()
            .AddSingleton<IChatClient, ChatClient>()
            .AddSingleton<ITranslateParametersFactory, TranslateParameterFactory>()
            .AddSingleton<ITranslateClient, TranslateClient>()
            .AddSingleton<IDrawParametersFactory, DrawParametersFactory>()
            .AddSingleton<IDrawClient, DrawClient>()
            .AddSingleton<IAudioParametersFactory, AudioParametersFactory>()
            .AddSingleton<IAudioClient, AudioClient>()
            .AddSingleton<DbService>()
            .AddSingleton<IStorageService, StorageService>()
            .AddSingleton<AppViewModel>()
            .AddSingleton<NavigationViewModel>()
            .AddSingleton<AudioWaveModuleViewModel>()
            .AddSingleton<StartupPageViewModel>()
            .AddSingleton<ChatPresetModuleViewModel>()
            .AddSingleton<GroupPresetModuleViewModel>()
            .AddSingleton<ChatServicePageViewModel>()
            .AddSingleton<DrawSessionViewModel>()
            .AddSingleton<DrawServicePageViewModel>()
            .AddSingleton<AudioSessionViewModel>()
            .AddSingleton<AudioServicePageViewModel>()
            .AddSingleton<TranslateSessionViewModel>()
            .AddSingleton<TranslateServicePageViewModel>()
            .AddSingleton<PromptTestPageViewModel>()
            .AddSingleton<SettingsPageViewModel>()
            .Build();

        Kernel.InitializeShareKernel();
        Kernel.InitializeAIKernel();
        GlobalStatics.SetKernel(Kernel);
    }

    public static T Get<T>(this object ele)
        where T : class
        => Kernel.GetRequiredService<T>();

    public static IKernelBuilder AddSingleton<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] T>(this IKernelBuilder builder)
        where T : class
    {
        builder.Services.AddSingleton<T>();
        return builder;
    }

    public static IKernelBuilder AddSingleton<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TInterface, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TImplementation>(this IKernelBuilder builder)
        where TInterface : class
        where TImplementation : class, TInterface
    {
        builder.Services.AddSingleton<TInterface, TImplementation>();
        return builder;
    }

    public static IKernelBuilder AddDispatcherQueue(this IKernelBuilder builder)
    {
        var queue = DispatcherQueue.GetForCurrentThread();
        builder.Services.AddSingleton(queue);
        return builder;
    }

    public static IKernelBuilder AddSerilog(this IKernelBuilder builder)
    {
        var loggerPath = Path.Combine(ApplicationData.Current.LocalFolder.Path, "Logger");
        if (!Directory.Exists(loggerPath))
        {
            Directory.CreateDirectory(loggerPath);
        }

        // Create a logger with current date.
        Log.Logger = new LoggerConfiguration()
            .WriteTo.File(Path.Combine(loggerPath, $"log-{DateTimeOffset.Now:yyyy-MM-dd}.txt"))
            .CreateLogger();

        builder.Services.AddLogging(b => b.AddSerilog(dispose: true));
        return builder;
    }

    public static IKernelBuilder AddNotificationViewModel(this IKernelBuilder builder)
    {
        builder.Services.AddSingleton<INotificationViewModel, NotificationViewModel>();
        return builder;
    }

    public static IKernelBuilder AddShareToolkits(this IKernelBuilder builder)
    {
        builder.Services.AddSingleton<IAppToolkit, AppToolkit>()
            .AddSingleton<ISettingsToolkit, SettingsToolkit>()
            .AddSingleton<IFileToolkit, SharedFileToolkit>()
            .AddSingleton<IResourceToolkit, ResourceToolkit>();
        return builder;
    }

    public static IKernelBuilder AddXamlRootProvider(this IKernelBuilder builder)
    {
        builder.Services.AddSingleton<IXamlRootProvider, XamlRootProvider>();
        return builder;
    }

    public static IKernelBuilder AddConfigManager(this IKernelBuilder builder)
    {
        builder.Services
            .AddSingleton<IAudioConfigManager, AudioConfigManager>()
            .AddSingleton<IChatConfigManager, ChatConfigManager>()
            .AddSingleton<ITranslateConfigManager, TranslateConfigManager>()
            .AddSingleton<IDrawConfigManager, DrawConfigManager>();
        return builder;
    }

    private static void ToolInvoking(ToolInvokingEventArgs args)
    {
        var chatPageVM = ServiceProvider.GetService<ChatServicePageViewModel>();
        chatPageVM.CurrentSession?.ToolInvokingHandleCommand.Execute(args);
    }

    private static void ToolInvoked(ToolInvokedEventArgs args)
    {
        var chatPageVM = ServiceProvider.GetService<ChatServicePageViewModel>();
        chatPageVM.CurrentSession?.ToolInvokedHandleCommand.Execute(args);
    }
}
