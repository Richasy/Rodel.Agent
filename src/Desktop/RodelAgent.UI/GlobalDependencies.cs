// Copyright (c) Richasy. All rights reserved.

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
using RodelAgent.UI.ViewModels.Core;
using RodelAgent.UI.ViewModels.View;
using Serilog;
using System.Diagnostics.CodeAnalysis;
using Windows.Storage;

namespace RodelAgent.UI;

internal static class GlobalDependencies
{
    public static Kernel Kernel { get; private set; }

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

            .AddSingleton<IStringResourceToolkit, ResourceToolkit>()
            .AddSingleton<DbService>()
            .AddSingleton<IStorageService, StorageService>()
            .AddSingleton<AppViewModel>()
            .AddSingleton<NavigationViewModel>()
            .AddSingleton<StartupPageViewModel>()
            .AddSingleton<ChatPageViewModel>()
            .AddSingleton<DrawPageViewModel>()
            .AddSingleton<SpeechPageViewModel>()
            .AddSingleton<SettingsPageViewModel>()
            .AddNotificationViewModel()
            .Build();

        Kernel.InitializeShareKernel();
        Kernel.InitializeAIKernel();
        GlobalStatics.SetKernel(Kernel);
    }

    public static IKernelBuilder AddSingleton<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] T>(this IKernelBuilder builder)
        where T : class
    {
        builder.Services.AddSingleton<T>();
        return builder;
    }

    public static IKernelBuilder AddSingleton<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TService, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TImplementation>(this IKernelBuilder builder)
        where TService : class
        where TImplementation : class, TService
    {
        builder.Services.AddSingleton<TService, TImplementation>();
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
            .AddSingleton<IResourceToolkit, ResourceToolkit>()
            .AddSingleton<IFontToolkit, SharedFontToolkit>();
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
            .AddSingleton<IDrawConfigManager, DrawConfigManager>()
            .AddSingleton<ITranslateConfigManager, TranslateConfigManager>();
        return builder;
    }

    public static IKernelBuilder AddDispatcherQueue(this IKernelBuilder builder)
    {
        var queue = DispatcherQueue.GetForCurrentThread();
        builder.Services.AddSingleton(queue);
        return builder;
    }

    public static T Get<T>(this object ele)
        where T : class
        => Kernel.GetRequiredService<T>();
}
