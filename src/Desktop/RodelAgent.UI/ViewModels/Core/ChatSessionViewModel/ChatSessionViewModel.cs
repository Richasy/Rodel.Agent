// Copyright (c) Richasy. All rights reserved.

using Microsoft.Extensions.AI;
using Richasy.AgentKernel;
using Richasy.AgentKernel.Chat;
using Richasy.WinUIKernel.AI.ViewModels;
using Richasy.WinUIKernel.Share.Toolkits;
using RodelAgent.Interfaces;
using RodelAgent.Models.Constants;
using RodelAgent.Models.Feature;
using RodelAgent.UI.Models.Constants;
using RodelAgent.UI.Toolkits;
using RodelAgent.UI.ViewModels.Items;
using RodelAgent.UI.ViewModels.View;
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
    public ChatSessionViewModel(
        IStorageService storageService,
        ILogger<ChatSessionViewModel> logger)
    {
        _storageService = storageService;
        _logger = logger;
        IsEnterSend = SettingsToolkit.ReadLocalSetting(SettingNames.ChatServicePageIsEnterSend, true);
        CheckSectionType();
        Messages.CollectionChanged += (_, _) => CheckChatEmpty();
        History.CollectionChanged += (_, _) => CheckHistoryEmpty();
        IsInstructionVisible = SettingsToolkit.ReadLocalSetting(SettingNames.ChatSessionIsInstructionVisible, true);
        IsOptionsVisible = SettingsToolkit.ReadLocalSetting(SettingNames.ChatSessionIsOptionsVisible, false);
        this.Get<AppViewModel>().RequestReloadChatServices += OnRequestReloadChatServices;
    }

    protected override string GetPageKey()
        => "ChatSession";

    public void InjectFunc(Func<ChatOptions> func, Func<bool>? stream, Func<int>? maxRounds)
    {
        _getCurrentOptions = func;
        _getIsStreamOutput = stream;
        _getMaxRounds = maxRounds;
    }

    public async Task InitializeAsync(WebView2 view)
    {
        if (_webView is not null)
        {
            return;
        }

        _webView = view;
        IsWebInitializing = true;
        IsWebInitialized = false;
        CheckChatEmpty();
        HistoryHeight = SettingsToolkit.ReadLocalSetting(SettingNames.ChatServicePageHistoryHeight, 300d);
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

    public ChatConversation? GetCurrentConversation()
        => _currentConversation;

    [RelayCommand]
    private async Task InitializeWithServiceAsync(ChatServiceItemViewModel service)
    {
        if (SectionType == AgentSectionType.Service && service.ProviderType == CurrentProvider)
        {
            ReloadAvailableModelsCommand.Execute(default);
            return;
        }

        // 不支持生成内容时切换服务.
        if (IsGenerating)
        {
            CancelGenerateCommand.Execute(default);
        }

        SectionType = AgentSectionType.Service;
        CurrentProvider = service.ProviderType;
        Title = string.Empty;
        SetCurrentConversation(null);
        _chatService = this.Get<IChatService>(service.ProviderType.ToString());
        ReloadAvailableModelsCommand.Execute(default);
        ClearMessageCommand.Execute(default);
        await LoadConversationsWithProviderAsync(service.ProviderType);
    }

    [RelayCommand]
    private async Task InitializeWithAgentAsync(ChatAgentItemViewModel agent)
    {
        if (SectionType == AgentSectionType.Agent && agent.Data.Id == CurrentAgent?.Id)
        {
            return;
        }

        // 不支持生成内容时切换服务.
        if (IsGenerating)
        {
            CancelGenerateCommand.Execute(default);
        }

        SectionType = AgentSectionType.Agent;
        CurrentAgent = agent.Data;
        Title = agent.Name;
        SetCurrentConversation(null);
        if (Services.Count == 0)
        {
            await ReloadAvailableServicesAsync();
        }

        var service = Services.FirstOrDefault(p => p.ProviderType == CurrentAgent.Provider);
        service ??= Services[0];
        ChangeService(service);
        ClearMessageCommand.Execute(default);
        await LoadConversationsWithAgentIdAsync(agent.Data.Id);
    }

    [RelayCommand]
    private async Task InitializeWithGroupAsync(ChatGroupItemViewModel group)
    {
        if (SectionType == AgentSectionType.Group && group.Data.Id == CurrentGroup?.Id)
        {
            return;
        }

        // 不支持生成内容时切换服务.
        if (IsGenerating)
        {
            CancelGenerateCommand.Execute(default);
        }

        SectionType = AgentSectionType.Group;
        CurrentGroup = group.Data;
        Title = group.Name;
        Agents.Clear();
        var pageVM = this.Get<ChatPageViewModel>();
        foreach (var agent in pageVM.Agents.Where(p => CurrentGroup.Agents!.Contains(p.Data.Id)).Select(p => p.Data).ToList())
        {
            Agents.Add(new(agent));
        }

        SetCurrentConversation(null);
        ClearMessageCommand.Execute(default);
        await LoadConversationsWithGroupIdAsync(group.Data.Id);
    }

    [RelayCommand]
    private void ChangeService(ChatServiceItemViewModel service)
    {
        if (SelectedService == service)
        {
            return;
        }

        SelectedService = service;
        CurrentProvider = service.ProviderType;
        _chatService = this.Get<IChatService>(CurrentProvider!.Value.ToString());
        ReloadAvailableModelsCommand.Execute(default);
        RequestReloadOptionsUI?.Invoke(this, EventArgs.Empty);
    }

    [RelayCommand]
    private void LoadHistoryItem(ChatHistoryItemViewModel history)
    {
        if (history is null || history.Id == _currentConversation?.Id || IsGenerating)
        {
            return;
        }

        SetCurrentConversation(history.Conversation);
        SetInitialInteropHistoryCommand.Execute(_currentConversation?.History ?? []);
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
        if (IsAgent && models.Any(p => p.Id == CurrentAgent?.Model))
        {
            SelectModelCommand.Execute(models.Find(p => p.Id == CurrentAgent?.Model));
            return;
        }

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
        if (IsService)
        {
            this.Get<ISettingsToolkit>().WriteLocalSetting($"{CurrentProvider}LastSelectedChatModel", model.Id);
        }

        if (!IsGroup)
        {
            Subtitle = model.Name;
        }

        var config = await this.Get<IChatConfigManager>().GetServiceConfigAsync(CurrentProvider!.Value, model.Data);
        _chatService!.Initialize(config);
        RequestFocusInput?.Invoke(this, EventArgs.Empty);
    }

    [RelayCommand]
    private async Task ForceReloadLogoAsync()
    {
        if (IsAgent)
        {
            var agent = CurrentAgent;
            CurrentAgent = null;
            await Task.Delay(200);
            CurrentAgent = agent;
        }
        else if (IsGroup)
        {
            // TODO: Reload group logo.
        }
    }

    private async void OnRequestReloadChatServices(object? sender, EventArgs e)
        => await ReloadAvailableServicesAsync();

    private async Task ReloadAvailableServicesAsync()
    {
        var providers = Enum.GetValues<ChatProviderType>();
        var services = new List<ChatServiceItemViewModel>();
        var chatConfigManager = this.Get<IChatConfigManager>();
        foreach (var p in providers)
        {
            var config = await chatConfigManager.GetChatConfigAsync(p);
            if (config?.IsValid() == true)
            {
                services.Add(new ChatServiceItemViewModel(p));
            }
        }

        Services.Clear();
        services.ForEach(Services.Add);
    }

    private void CheckSectionType()
    {
        IsService = SectionType == AgentSectionType.Service;
        IsAgent = SectionType == AgentSectionType.Agent;
        IsGroup = SectionType == AgentSectionType.Group;
    }

    private void CheckChatEmpty()
        => IsChatEmpty = Messages.Count == 0 && !IsGenerating && !IsWebInitializing;

    private void CheckHistoryEmpty()
        => IsHistoryEmpty = History.Count == 0 && !IsHistoryInitializing;

    partial void OnSectionTypeChanged(AgentSectionType value)
        => CheckSectionType();

    partial void OnIsGeneratingChanged(bool value)
        => CheckChatEmpty();

    partial void OnIsWebInitializingChanged(bool value)
        => CheckChatEmpty();

    partial void OnIsHistoryInitializingChanged(bool value)
        => CheckHistoryEmpty();

    partial void OnHistoryHeightChanged(double value)
        => SettingsToolkit.WriteLocalSetting(SettingNames.ChatServicePageHistoryHeight, value);

    partial void OnIsInstructionVisibleChanged(bool value)
    {
        if (value)
        {
            IsOptionsVisible = false;
        }

        SettingsToolkit.WriteLocalSetting(SettingNames.ChatSessionIsInstructionVisible, value);
    }

    partial void OnIsOptionsVisibleChanged(bool value)
    {
        if (value)
        {
            IsInstructionVisible = false;
        }

        SettingsToolkit.WriteLocalSetting(SettingNames.ChatSessionIsOptionsVisible, value);
    }
}
