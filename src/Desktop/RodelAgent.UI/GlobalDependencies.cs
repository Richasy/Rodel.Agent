// Copyright (c) Rodel. All rights reserved.

using Microsoft.UI.Dispatching;
using NLog.Extensions.Logging;
using RodelAgent.Context;
using RodelAgent.Interfaces;
using RodelAgent.Statics;
using RodelAgent.UI.Extensions;
using RodelAgent.UI.Toolkits;
using RodelAgent.UI.ViewModels;
using RodelAgent.UI.ViewModels.Components;
using RodelAgent.UI.ViewModels.Pages;
using RodelAudio.Core;
using RodelAudio.Core.Factories;
using RodelAudio.Interfaces.Client;
using RodelAudio.Models.Client;
using RodelChat.Core;
using RodelChat.Core.Factories;
using RodelChat.Interfaces.Client;
using RodelChat.Models.Client;
using RodelDraw.Core;
using RodelDraw.Core.Factories;
using RodelDraw.Interfaces.Client;
using RodelDraw.Models.Client;
using RodelTranslate.Core;
using RodelTranslate.Core.Factories;
using RodelTranslate.Interfaces.Client;
using RodelTranslate.Models.Client;

namespace RodelAgent.UI;

/// <summary>
/// 全局依赖项.
/// </summary>
public static class GlobalDependencies
{
    /// <summary>
    /// 获取服务提供程序.
    /// </summary>
    public static IServiceProvider ServiceProvider { get; private set; }

    /// <summary>
    /// 初始化.
    /// </summary>
    public static void Initialize()
    {
        var dispatcherQueue = DispatcherQueue.GetForCurrentThread();
        var services = new ServiceCollection();
        services.AddLogging(builder =>
        {
            builder.AddNLog();
        });

        var chatProviderFactory = new ChatProviderFactory(new ChatClientConfiguration(), ToolInvoking, ToolInvoked);
        var translateProviderFactory = new TranslateProviderFactory(new TranslateClientConfiguration());
        var drawProviderFactory = new DrawProviderFactory(new DrawClientConfiguration());
        var audioProviderFactory = new AudioProviderFactory(new AudioClientConfiguration());

        services.AddSingleton(dispatcherQueue)
            .AddSingleton<IStringResourceToolkit, StringResourceToolkit>()
            .AddSingleton<IChatProviderFactory>(chatProviderFactory)
            .AddSingleton<IChatParametersFactory, ChatParametersFactory>()
            .AddSingleton<IChatClient, ChatClient>()
            .AddSingleton<ITranslateProviderFactory>(translateProviderFactory)
            .AddSingleton<ITranslateParametersFactory, TranslateParameterFactory>()
            .AddSingleton<ITranslateClient, TranslateClient>()
            .AddSingleton<IDrawProviderFactory>(drawProviderFactory)
            .AddSingleton<IDrawParametersFactory, DrawParametersFactory>()
            .AddSingleton<IDrawClient, DrawClient>()
            .AddSingleton<IAudioProviderFactory>(audioProviderFactory)
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
            .AddSingleton<SettingsPageViewModel>();

        ServiceProvider = services.BuildServiceProvider();
        GlobalStatics.SetServiceProvider(ServiceProvider);
    }

    /// <summary>
    /// 获取指定类型的服务.
    /// </summary>
    /// <typeparam name="T">类型.</typeparam>
    /// <returns>类型实例.</returns>
    public static T Get<T>(this Window window)
        where T : class
        => ServiceProvider.GetRequiredService<T>();

    /// <summary>
    /// 获取指定类型的服务.
    /// </summary>
    /// <typeparam name="T">类型.</typeparam>
    /// <returns>类型实例.</returns>
    public static T Get<T>(this FrameworkElement element)
        where T : class
        => ServiceProvider.GetRequiredService<T>();

    /// <summary>
    /// 获取指定类型的服务.
    /// </summary>
    /// <typeparam name="T">类型.</typeparam>
    /// <returns>类型实例.</returns>
    public static T Get<T>(this Page page)
        where T : class
        => ServiceProvider.GetRequiredService<T>();

    /// <summary>
    /// 获取指定类型的服务.
    /// </summary>
    /// <typeparam name="T">类型.</typeparam>
    /// <returns>类型实例.</returns>
    public static T Get<T>(this ViewModelBase vm)
        where T : class
        => ServiceProvider.GetRequiredService<T>();

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
