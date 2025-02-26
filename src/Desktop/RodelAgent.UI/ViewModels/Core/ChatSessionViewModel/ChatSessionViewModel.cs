﻿// Copyright (c) Richasy. All rights reserved.

using Richasy.AgentKernel;
using Richasy.AgentKernel.Chat;
using Richasy.WinUIKernel.AI.ViewModels;
using Richasy.WinUIKernel.Share.Toolkits;
using RodelAgent.Models.Constants;
using RodelAgent.UI.Models.Constants;
using RodelAgent.UI.Toolkits;
using Windows.ApplicationModel;

namespace RodelAgent.UI.ViewModels.Core;

/// <summary>
/// Chat session view model.
/// </summary>
public sealed partial class ChatSessionViewModel : LayoutPageViewModelBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ChatSessionViewModel"/> class.
    /// </summary>
    public ChatSessionViewModel(ILogger<ChatSessionViewModel> logger)
    {
        _logger = logger;
        IsEnterSend = SettingsToolkit.ReadLocalSetting(SettingNames.ChatServicePageIsEnterSend, true);
        CheckSectionType();
        Messages.CollectionChanged += (_, _) => CheckChatEmpty();
    }

    protected override string GetPageKey()
        => "ChatSession";

    public async Task InitializeAsync(WebView2 view)
    {
        if (_webView is not null)
        {
            _webView.CoreWebView2.Stop();
            _webView.Close();
            _webView = default;
        }

        _webView = view;
        IsWebInitializing = true;
        IsWebInitialized = false;
        CheckChatEmpty();
        try
        {
            await _webView.EnsureCoreWebView2Async();
            _webView.CoreWebView2.WebMessageReceived += OnWebMessageReceived;
#if DEBUG
            _webView.CoreWebView2.Settings.AreDevToolsEnabled = true;
#else
            _webView.CoreWebView2.Settings.AreDevToolsEnabled = false;
#endif
            _webView.CoreWebView2.Settings.AreDefaultContextMenusEnabled = false;
            _webView.CoreWebView2.Settings.IsStatusBarEnabled = false;
            _webView.CoreWebView2.Settings.IsZoomControlEnabled = false;
            _webView.CoreWebView2.Settings.AreHostObjectsAllowed = false;
            _webView.CoreWebView2.Settings.IsWebMessageEnabled = true;
            _webView.CoreWebView2.Settings.IsBuiltInErrorPageEnabled = false;
            _webView.CoreWebView2.Settings.IsScriptEnabled = true;
            _webView.CoreWebView2.Settings.IsSwipeNavigationEnabled = false;
            _webView.CoreWebView2.Settings.AreBrowserAcceleratorKeysEnabled = false;
            _webView.CoreWebView2.Settings.AreDefaultScriptDialogsEnabled = false;
            _webView.CoreWebView2.Settings.IsPinchZoomEnabled = true;

            var renderPath = Path.Combine(Package.Current.InstalledPath, "Web", "chat-render");
            _webView.CoreWebView2.SetVirtualHostNameToFolderMapping("chat.example", renderPath, Microsoft.Web.WebView2.Core.CoreWebView2HostResourceAccessKind.Allow);
            _webView.CoreWebView2.Navigate("http://chat.example/index.html");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to initialize chat session");
            this.Get<AppViewModel>().ShowTipCommand.Execute((ex.Message, InfoType.Error));
            IsWebInitializing = false;
        }
    }

    [RelayCommand]
    private void InitializeWithService(ChatServiceItemViewModel service)
    {
        if (SectionType == AgentSectionType.Service && service.ProviderType == CurrentProvider)
        {
            ReloadAvailableModelsCommand.Execute(default);
            return;
        }

        SectionType = AgentSectionType.Service;
        CurrentProvider = service.ProviderType;
        Title = string.Empty;
        _chatService = this.Get<IChatService>(service.ProviderType.ToString());
        ReloadAvailableModelsCommand.Execute(default);
    }

    [RelayCommand]
    private async Task ReloadAvailableModelsAsync()
    {
        Models.Clear();
        SelectedModel = null;
        var serverModels = _chatService!.GetPredefinedModels().ToList();
        var config = await this.Get<IChatConfigManager>().GetChatConfigAsync(CurrentProvider!.Value);
        var customModels = config?.CustomModels?.ToList() ?? [];
        var models = serverModels.Concat(customModels).ToList().ConvertAll(p => new ChatModelItemViewModel(p));
        models.ForEach(Models.Add);
        var lastSelectedModel = this.Get<ISettingsToolkit>().ReadLocalSetting($"{CurrentProvider}LastSelectedChatModel", string.Empty);
        var model = Models.FirstOrDefault(p => p.Id == lastSelectedModel) ?? Models.FirstOrDefault();
        SelectModelCommand.Execute(model);
    }

    [RelayCommand]
    private async Task SelectModelAsync(ChatModelItemViewModel model)
    {
        RequestCloseFlyout?.Invoke(this, EventArgs.Empty);
        if (SelectedModel == model)
        {
            RequestFocusInput?.Invoke(this, EventArgs.Empty);
            return;
        }

        foreach (var item in Models)
        {
            item.IsSelected = item == model;
        }

        SelectedModel = model;
        this.Get<ISettingsToolkit>().WriteLocalSetting($"{CurrentProvider}LastSelectedChatModel", model.Id);
        if (IsService)
        {
            Subtitle = model.Name;
        }

        var config = await this.Get<IChatConfigManager>().GetServiceConfigAsync(CurrentProvider!.Value, model.Data);
        _chatService!.Initialize(config!);
        RequestFocusInput?.Invoke(this, EventArgs.Empty);
    }

    private void CheckSectionType()
    {
        IsService = SectionType == AgentSectionType.Service;
        IsAgent = SectionType == AgentSectionType.Agent;
        IsGroup = SectionType == AgentSectionType.Group;
    }

    private void CheckChatEmpty()
        => IsChatEmpty = Messages.Count == 0 && !IsGenerating;

    partial void OnSectionTypeChanged(AgentSectionType value)
        => CheckSectionType();

    partial void OnIsGeneratingChanged(bool value)
        => CheckChatEmpty();
}
